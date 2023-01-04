namespace PowWeb.ChromeApi.DDebugger.Structs;

public record BreakLocation(
	string ScriptId,
	int LineNumber,
	int? ColumnNumber,
	string? Type
);