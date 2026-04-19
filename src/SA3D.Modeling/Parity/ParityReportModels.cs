using System.Collections.Generic;

namespace SA3D.Modeling.Parity
{
	public sealed class ParityReport
	{
		public string Schema { get; set; } = "parity_report_v1";
		public string Fixture { get; set; } = string.Empty;
		public Dictionary<string, object> Reference { get; set; } = [];
		public Dictionary<string, object> Metrics { get; set; } = [];
		public List<Dictionary<string, object>> Diagnostics { get; set; } = [];
		public Dictionary<string, object> Comparison { get; set; } = [];
		public List<SliceIOPair> SliceIOPairs { get; set; } = [];
	}

	public sealed class SliceIOPair
	{
		public int Slice { get; set; }
		public string SliceSchema { get; set; } = string.Empty;
		public string Stage { get; set; } = string.Empty;
		public string RunId { get; set; } = string.Empty;
		public string FixtureId { get; set; } = string.Empty;
		public int OperationCount { get; set; }
		public int DiagnosticCount { get; set; }
		public object Inputs { get; set; } = new { };
		public object Outputs { get; set; } = new { };
		public List<Dictionary<string, object>> Diagnostics { get; set; } = [];
	}
}
