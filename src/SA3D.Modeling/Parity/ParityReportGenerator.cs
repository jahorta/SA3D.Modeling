using SA3D.Common.IO;
using SA3D.Modeling.File;
using SA3D.Modeling.Structs;
using System;
using System.Collections.Generic;
using System.IO;
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

			if(options.EmitSlice1)
			{
				report.SliceIOPairs.Add(CreateSlice1(data, fixtureId, runId));
			}

			if(options.EmitSlice2)
			{
				report.SliceIOPairs.Add(CreateSlice2(data, fixtureId, runId));
			}

			report.Metrics["slice_count"] = report.SliceIOPairs.Count;
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

		private static SliceIOPair CreateSlice1(byte[] data, string fixtureId, string runId)
		{
			List<Dictionary<string, object>> primitiveOps = [];
			using EndianStackReader reader = new(data);

			if(data.Length >= 4)
			{
				primitiveOps.Add(new()
				{
					["op"] = "read",
					["type"] = "uint",
					["offset"] = 0,
					["image_base"] = reader.ImageBase,
					["value_hint"] = reader.ReadUInt(0),
				});
			}

			if(data.Length >= 8)
			{
				primitiveOps.Add(new()
				{
					["op"] = "read",
					["type"] = "float",
					["offset"] = 4,
					["image_base"] = reader.ImageBase,
					["value_hint"] = reader.ReadFloat(4),
				});
			}

			List<Dictionary<string, object>> bams =
			[
				new() { ["input"] = 0, ["mode"] = "bams_to_rad", ["output"] = BAMSFHelper.BAMSFToRad(0) },
				new() { ["input"] = 32767, ["mode"] = "bams_to_rad", ["output"] = BAMSFHelper.BAMSFToRad(32767) },
				new() { ["input"] = 65535f / 2f, ["mode"] = "deg_to_bams", ["output"] = BAMSFHelper.DegToBAMSF(180f) },
			];

			object inputs = new
			{
				endianness = "mixed",
				primitive_ops = primitiveOps,
				bams_checkpoints = bams,
				lut_ops = Array.Empty<object>(),
			};

			object outputs = new
			{
				primitive_hash = ComputeHash(primitiveOps),
				bams_hash = ComputeHash(bams),
				lut_summary = new
				{
					adds = 0,
					hits = 0,
					misses = 0,
				}
			};

			return new()
			{
				Slice = 1,
				SliceSchema = "sa3d_slice_1_primitives_v1",
				Stage = "slice_1_primitives",
				RunId = runId,
				FixtureId = fixtureId,
				OperationCount = primitiveOps.Count + bams.Count,
				DiagnosticCount = 0,
				Inputs = inputs,
				Outputs = outputs,
			};
		}

		private static SliceIOPair CreateSlice2(byte[] data, string fixtureId, string runId)
		{
			IReadOnlyList<NJBlockInfo> blocks = NJDebugInfo.ReadBlocks(data, 0);
			object inputs = new
			{
				blocks = blocks.Select(x => new
				{
					offset = x.Offset,
					header = x.Header,
					size = x.Size,
					selected_role = x.Role,
				}),
				meta_blocks = Array.Empty<object>(),
			};

			int recognizedMeta = 0;
			int unknownMeta = 0;

			object outputs = new
			{
				block_count = blocks.Count,
				recognized_meta_count = recognizedMeta,
				unknown_meta_count = unknownMeta,
				selection_hash = ComputeHash(blocks),
			};

			return new()
			{
				Slice = 2,
				SliceSchema = "sa3d_slice_2_blockmap_meta_v1",
				Stage = "slice_2_blockmap_meta",
				RunId = runId,
				FixtureId = fixtureId,
				OperationCount = blocks.Count,
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
