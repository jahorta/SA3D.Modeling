using System;

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
