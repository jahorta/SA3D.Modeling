using System.Collections.Generic;

namespace SA3D.Modeling.Parity
{
	/// <summary>
	/// Parity report
	/// </summary>
	public sealed class ParityReport
	{
		/// <summary>
		/// Schema for the parity report
		/// </summary>
		public string Schema { get; set; } = "parity_report_v1";

		/// <summary>
		/// Fixtures to use for the parity report
		/// </summary>
		public string Fixture { get; set; } = string.Empty;

		/// <summary>
		/// References for the parity report
		/// </summary>
		public Dictionary<string, object> Reference { get; set; } = [];

		/// <summary>
		/// Metrics from the parity report
		/// </summary>
		public Dictionary<string, object> Metrics { get; set; } = [];

		/// <summary>
		/// Diagnostics
		/// </summary>
		public List<Dictionary<string, object>> Diagnostics { get; set; } = [];

		/// <summary>
		/// Comparison of the two paths
		/// </summary>
		public Dictionary<string, object> Comparison { get; set; } = [];

		/// <summary>
		/// IO pairs for each slice
		/// </summary>
		public List<SliceIOPair> SliceIOPairs { get; set; } = [];
	}

	/// <summary>
	/// IO pairs for each slice
	/// </summary>
	public sealed class SliceIOPair
	{
		/// <summary>
		/// Slice number
		/// </summary>
		public int Slice { get; set; }

		/// <summary>
		/// Schema for this slice
		/// </summary>
		public string SliceSchema { get; set; } = string.Empty;

		/// <summary>
		/// Stage for this slice
		/// </summary>
		public string Stage { get; set; } = string.Empty;

		/// <summary>
		/// RunID for this slice
		/// </summary>
		public string RunId { get; set; } = string.Empty;

		/// <summary>
		/// Fixture used for this slice comparison
		/// </summary>
		public string FixtureId { get; set; } = string.Empty;

		/// <summary>
		/// Operations performed in this slice
		/// </summary>
		public int OperationCount { get; set; }

		/// <summary>
		/// Diagnostics generated in this slice
		/// </summary>
		public int DiagnosticCount { get; set; }

		/// <summary>
		/// Inputs for this slice
		/// </summary>
		public object Inputs { get; set; } = new { };

		/// <summary>
		/// Outputs for this slice
		/// </summary>
		public object Outputs { get; set; } = new { };

		/// <summary>
		/// List of diagnostics for this slice
		/// </summary>
		public List<Dictionary<string, object>> Diagnostics { get; set; } = [];
	}
}
