using SA3D.Modeling.File;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace SA3D.Modeling.Parity
{
	internal enum ParityParseAdapter
	{
		Auto,
		ModelFile,
		AnimationFile,
	}

	internal sealed class ParityCaptureSession : IDisposable
	{
		private static readonly AsyncLocal<ParityCaptureSession?> _current = new();
		private readonly Dictionary<int, List<ParityIORecord>> _records = [];
		private readonly Dictionary<int, int> _droppedCounts = [];
		private readonly HashSet<int> _requestedSlices;

		public static ParityCaptureSession? Current => _current.Value;

		public int MaxPairsPerSlice { get; }

		private ParityCaptureSession(ParityCaptureOptions options)
		{
			MaxPairsPerSlice = Math.Max(1, options.MaxPairsPerSlice);
			_requestedSlices = options.RequestedSlices?.Count > 0
				? [.. options.RequestedSlices]
				: BuildDefaultSliceSet(options);
		}

		public static ParityCaptureSession Begin(ParityCaptureOptions options)
		{
			ParityCaptureSession session = new(options);
			_current.Value = session;
			return session;
		}

		public bool IsSliceEnabled(int slice)
		{
			return _requestedSlices.Contains(slice);
		}

		public void Record(int slice, string operation, object inputs, object outputs)
		{
			if(!IsSliceEnabled(slice))
			{
				return;
			}

			if(!_records.TryGetValue(slice, out List<ParityIORecord>? sliceRecords))
			{
				sliceRecords = [];
				_records[slice] = sliceRecords;
			}

			if(sliceRecords.Count >= MaxPairsPerSlice)
			{
				_droppedCounts[slice] = _droppedCounts.GetValueOrDefault(slice) + 1;
				return;
			}

			sliceRecords.Add(new(operation, inputs, outputs));
		}

		public IReadOnlyDictionary<int, IReadOnlyList<ParityIORecord>> GetRecords()
		{
			return _records.ToDictionary(x => x.Key, x => (IReadOnlyList<ParityIORecord>)x.Value);
		}

		public int GetDroppedCount(int slice)
		{
			return _droppedCounts.GetValueOrDefault(slice);
		}

		public void Dispose()
		{
			if(_current.Value == this)
			{
				_current.Value = null;
			}
		}

		private static HashSet<int> BuildDefaultSliceSet(ParityCaptureOptions options)
		{
			HashSet<int> result = [];
			if(options.EmitSlice1)
			{
				result.Add(1);
			}

			if(options.EmitSlice2)
			{
				result.Add(2);
			}

			return result;
		}
	}

	internal readonly record struct ParityIORecord(string Operation, object Inputs, object Outputs);

	internal static class ParityCaptureHooks
	{
		public static void RecordPrimitiveRead(string type, uint offset, uint imageBase, object valueHint)
		{
			ParityCaptureSession? session = ParityCaptureSession.Current;
			session?.Record(1, "primitive_op", new
			{
				op = "read",
				type,
				offset,
				image_base = imageBase,
			}, new
			{
				value_hint = valueHint,
			});
		}

		public static void RecordBamsConversion(string mode, object input, object output)
		{
			ParityCaptureSession? session = ParityCaptureSession.Current;
			session?.Record(1, "bams_checkpoint", new
			{
				mode,
				input,
			}, new
			{
				output,
			});
		}

		public static void RecordLutOp(string action, string category, uint? address = null)
		{
			ParityCaptureSession? session = ParityCaptureSession.Current;
			session?.Record(1, "lut_op", new
			{
				action,
				category,
				address,
			}, new { });
		}

		public static void RecordNJBlockScan(IReadOnlyList<NJBlockInfo> blocks)
		{
			ParityCaptureSession? session = ParityCaptureSession.Current;
			session?.Record(2, "nj_blocks", new
			{
				blocks = blocks.Select(x => new
				{
					offset = x.Offset,
					header = x.Header,
					size = x.Size,
					selected_role = x.Role,
				}),
			}, new
			{
				block_count = blocks.Count,
			});
		}
	}
}
