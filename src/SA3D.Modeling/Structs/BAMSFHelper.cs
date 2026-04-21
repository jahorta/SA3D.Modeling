using System;

using SA3D.Modeling.Parity;

namespace SA3D.Modeling.Structs
{
	/// <summary>
	/// Math helper for converting to and from BAMSF rotations (360° = 0xFFFF, as opposed ot 0x10000)
	/// </summary>
	public static class BAMSFHelper
	{
		/// <summary>
		/// BAMSF to Degree ratio
		/// </summary>
		public static readonly float BAMSF2Deg = 0xFFFF / 360f;

		/// <summary>
		/// BAMSF to Radians ratio
		/// </summary>
		public static readonly float BAMSF2Rad = 0xFFFF / float.Tau;

		/// <summary>
		/// Converts an angle from BAMSF to radians.
		/// </summary>
		/// <param name="BAMS"></param>
		/// <returns></returns>
		public static float BAMSFToRad(int BAMS)
		{
			float result = BAMS / BAMSF2Rad;
			ParityCaptureHooks.RecordBamsConversion("bams_to_rad", BAMS, result);
			return result;
		}

		/// <summary>
		/// Converts an angle from radians to BAMSF.
		/// </summary>
		/// <param name="rad"></param>
		/// <returns></returns>
		public static int RadToBAMSF(float rad)
		{
			int result = (int)Math.Round(rad * BAMSF2Rad);
			ParityCaptureHooks.RecordBamsConversion("rad_to_bams", rad, result);
			return result;
		}

		/// <summary>
		/// Converts an angle from BAMSF to Degrees.
		/// </summary>
		public static float BAMSFToDeg(int BAMS)
		{
			float result = BAMS / BAMSF2Deg;
			ParityCaptureHooks.RecordBamsConversion("bams_to_deg", BAMS, result);
			return result;
		}

		/// <summary>
		/// Converts an angle from degrees to BAMSF.
		/// </summary>
		public static int DegToBAMSF(float deg)
		{
			int result = (int)Math.Round(deg * BAMSF2Deg);
			ParityCaptureHooks.RecordBamsConversion("deg_to_bams", deg, result);
			return result;
		}

	}
}
