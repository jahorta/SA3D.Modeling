using System.Collections.Generic;

namespace SA3D.Modeling.Parity
{
	/// <summary>
	/// Options for parity capture generation.
	/// </summary>
	public sealed class ParityCaptureOptions
	{
		/// <summary>
		/// Whether slice capture is enabled.
		/// </summary>
		public bool EnableCapture { get; set; }

		/// <summary>
		/// Emit slice 1 primitive/LUT/BAMS outputs.
		/// </summary>
		public bool EmitSlice1 { get; set; } = true;

		/// <summary>
		/// Emit slice 2 block-map/metadata-shell outputs.
		/// </summary>
		public bool EmitSlice2 { get; set; } = true;

		/// <summary>
		/// Explicit requested slices. When null/empty, legacy slice flags are used.
		/// </summary>
		public IReadOnlyCollection<int>? RequestedSlices { get; set; }

		/// <summary>
		/// Maximum IO pairs to keep per slice.
		/// </summary>
		public int MaxPairsPerSlice { get; set; } = 128;

		/// <summary>
		/// Adapter entrypoint that should parse the input bytes.
		/// </summary>
		internal ParityParseAdapter ParseAdapter { get; set; } = ParityParseAdapter.Auto;

		/// <summary>
		/// Address offset for parser entry.
		/// </summary>
		public uint Address { get; set; }

		/// <summary>
		/// Optional node-count fallback for animation parsing.
		/// </summary>
		public uint? AnimationNodeCount { get; set; }

		/// <summary>
		/// Short-rotation fallback for animation parsing.
		/// </summary>
		public bool AnimationShortRot { get; set; }

		/// <summary>
		/// Creates default options with capture disabled.
		/// </summary>
		public static ParityCaptureOptions Disabled => new();

		/// <summary>
		/// Creates options with slice capture enabled.
		/// </summary>
		public static ParityCaptureOptions Enabled(bool emitSlice1 = true, bool emitSlice2 = true)
		{
			return new()
			{
				EnableCapture = true,
				EmitSlice1 = emitSlice1,
				EmitSlice2 = emitSlice2,
			};
		}
	}
}
