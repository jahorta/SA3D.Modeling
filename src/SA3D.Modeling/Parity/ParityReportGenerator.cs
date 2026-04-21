using SA3D.Modeling.File;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace SA3D.Modeling.Parity
{
	/// <summary>
	/// Generates parity report artifacts for staged C++-port validation.
	/// </summary>
	public static class ParityReportGenerator
	{
		private static readonly JsonSerializerOptions _serializerOptions = new()
		{
			WriteIndented = true,
			PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
		};

		/// <summary>
		/// Create a parity report from binary data.
		/// </summary>
		public static ParityReport CreateFromBytes(
			byte[] data,
			string fixtureId,
			string runId,
			ParityCaptureOptions? options = null)
		{
			options ??= ParityCaptureOptions.Disabled;

			ParityReport report = new()
			{
				Fixture = fixtureId,
				Reference = new()
				{
					["run_id"] = runId,
					["generator"] = "SA3D.Modeling.ParityReportGenerator",
					["capture_enabled"] = options.EnableCapture,
				}
			};

			if(!options.EnableCapture)
			{
				return report;
			}

			using ParityCaptureSession session = ParityCaptureSession.Begin(options);
			RunParserEntry(data, options, report);
			BuildSlices(report, fixtureId, runId, session);

			report.Metrics["slice_count"] = report.SliceIOPairs.Count;
			report.Metrics["max_pairs_per_slice"] = session.MaxPairsPerSlice;
			return report;
		}

		/// <summary>
		/// Write a report as deterministic UTF-8 JSON.
		/// </summary>
		public static void WriteToFile(ParityReport report, string filepath)
		{
			string json = JsonSerializer.Serialize(report, _serializerOptions);
			System.IO.File.WriteAllText(filepath, json, new UTF8Encoding(false));
		}

		private static void RunParserEntry(byte[] data, ParityCaptureOptions options, ParityReport report)
		{
			try
			{
				switch(options.ParseAdapter)
				{
					case ParityParseAdapter.ModelFile:
						ModelFile.ReadFromBytes(data, options.Address);
						break;
					case ParityParseAdapter.AnimationFile:
						AnimationFile.ReadFromBytes(data, options.Address, options.AnimationNodeCount, options.AnimationShortRot);
						break;
					default:
						if(ModelFile.CheckIsModelFile(data, options.Address))
						{
							ModelFile.ReadFromBytes(data, options.Address);
						}
						else if(AnimationFile.CheckIsAnimationFile(data, options.Address))
						{
							AnimationFile.ReadFromBytes(data, options.Address, options.AnimationNodeCount, options.AnimationShortRot);
						}
						else
						{
							report.Diagnostics.Add(new()
							{
								["code"] = "parity_no_adapter_match",
								["severity"] = "warning",
								["stage"] = "entry",
								["message"] = "Input bytes did not match ModelFile or AnimationFile adapter checks",
							});
						}
						break;
				}
			}
			catch(Exception ex)
			{
				report.Diagnostics.Add(new()
				{
					["code"] = "parity_capture_exception",
					["severity"] = "error",
					["stage"] = "entry",
					["message"] = ex.Message,
					["exception_type"] = ex.GetType().FullName ?? ex.GetType().Name,
				});
			}
		}

		private static void BuildSlices(ParityReport report, string fixtureId, string runId, ParityCaptureSession session)
		{
			IReadOnlyDictionary<int, IReadOnlyList<ParityIORecord>> recordsBySlice = session.GetRecords();
			foreach((int slice, IReadOnlyList<ParityIORecord> records) in recordsBySlice.OrderBy(x => x.Key))
			{
				switch(slice)
				{
					case 1:
						report.SliceIOPairs.Add(BuildSlice1(records, fixtureId, runId, session.GetDroppedCount(slice)));
						break;
					case 2:
						report.SliceIOPairs.Add(BuildSlice2(records, fixtureId, runId, session.GetDroppedCount(slice)));
						break;
				}
			}
		}

		private static SliceIOPair BuildSlice1(IReadOnlyList<ParityIORecord> records, string fixtureId, string runId, int droppedCount)
		{
			List<object> primitiveOps = records
				.Where(x => x.Operation == "primitive_op")
				.Select(x =>
				{
					Dictionary<string, object?> input = JsonSerializer.Deserialize<Dictionary<string, object?>>(JsonSerializer.Serialize(x.Inputs))!;
					Dictionary<string, object?> output = JsonSerializer.Deserialize<Dictionary<string, object?>>(JsonSerializer.Serialize(x.Outputs))!;
					input["value_hint"] = output["value_hint"];
					return (object)input;
				})
				.ToList();

			List<object> bams = records
				.Where(x => x.Operation == "bams_checkpoint")
				.Select(x =>
				{
					Dictionary<string, object?> input = JsonSerializer.Deserialize<Dictionary<string, object?>>(JsonSerializer.Serialize(x.Inputs))!;
					Dictionary<string, object?> output = JsonSerializer.Deserialize<Dictionary<string, object?>>(JsonSerializer.Serialize(x.Outputs))!;
					input["output"] = output["output"];
					return (object)input;
				})
				.ToList();

			List<object> lutOps = records
				.Where(x => x.Operation == "lut_op")
				.Select(x => (object)x.Inputs)
				.ToList();

			object inputs = new
			{
				endianness = "mixed",
				primitive_ops = primitiveOps,
				bams_checkpoints = bams,
				lut_ops = lutOps,
			};

			object outputs = new
			{
				primitive_hash = ComputeHash(primitiveOps),
				bams_hash = ComputeHash(bams),
				lut_summary = new
				{
					adds = lutOps.Count,
					hits = 0,
					misses = 0,
				},
				truncated_pairs = droppedCount,
			};

			return new()
			{
				Slice = 1,
				SliceSchema = "sa3d_slice_1_primitives_v1",
				Stage = "slice_1_primitives",
				RunId = runId,
				FixtureId = fixtureId,
				OperationCount = records.Count,
				DiagnosticCount = 0,
				Inputs = inputs,
				Outputs = outputs,
			};
		}

		private static SliceIOPair BuildSlice2(IReadOnlyList<ParityIORecord> records, string fixtureId, string runId, int droppedCount)
		{
			ParityIORecord? blockRecord = records.LastOrDefault(x => x.Operation == "nj_blocks");
			List<object> blocks = [];
			if(blockRecord.Operation != null)
			{
				Dictionary<string, JsonElement> input = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(JsonSerializer.Serialize(blockRecord.Inputs))!;
				if(input.TryGetValue("blocks", out JsonElement blockElement) && blockElement.ValueKind == JsonValueKind.Array)
				{
					foreach(JsonElement item in blockElement.EnumerateArray())
					{
						blocks.Add(JsonSerializer.Deserialize<Dictionary<string, object?>>(item.GetRawText())!);
					}
				}
			}

			object inputs = new
			{
				blocks,
				meta_blocks = Array.Empty<object>(),
			};

			object outputs = new
			{
				block_count = blocks.Count,
				recognized_meta_count = 0,
				unknown_meta_count = 0,
				selection_hash = ComputeHash(blocks),
				truncated_pairs = droppedCount,
			};

			return new()
			{
				Slice = 2,
				SliceSchema = "sa3d_slice_2_blockmap_meta_v1",
				Stage = "slice_2_blockmap_meta",
				RunId = runId,
				FixtureId = fixtureId,
				OperationCount = records.Count,
				DiagnosticCount = 0,
				Inputs = inputs,
				Outputs = outputs,
			};
		}

		private static string ComputeHash(object value)
		{
			byte[] bytes = JsonSerializer.SerializeToUtf8Bytes(value, _serializerOptions);
			byte[] hash = SHA256.HashData(bytes);
			return Convert.ToHexString(hash).ToLowerInvariant();
		}
	}
}
