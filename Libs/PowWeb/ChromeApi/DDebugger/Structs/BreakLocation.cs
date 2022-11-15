namespace PowWeb.ChromeApi.DDebugger.Structs;

record BreakLocation(
	string ScriptId,
	int LineNumber,
	int? ColumnNumber,
	string? Type
);