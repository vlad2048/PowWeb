namespace PowWeb.ChromeApi.DTracing.Structs;

enum RecordMode
{
	RecordUntilFull,
	RecordContinuously,
	RecordAsMuchAsPossible,
	EchoToConsole
}

record TraceConfig
{
	public RecordMode? RecordMode { get; set; }
	public string[]? IncludedCategories { get; set; }
	public string[]? ExcludedCategories { get; set; }
}
