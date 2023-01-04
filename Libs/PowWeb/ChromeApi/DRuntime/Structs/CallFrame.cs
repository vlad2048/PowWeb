namespace PowWeb.ChromeApi.DRuntime.Structs;

public record CallFrame(
	string FunctionName,
	string ScriptId,
	string Url,
	int LineNumber,
	int ColumnNumber
);