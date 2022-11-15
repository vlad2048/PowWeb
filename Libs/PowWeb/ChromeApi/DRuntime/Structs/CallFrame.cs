namespace PowWeb.ChromeApi.DRuntime.Structs;

record CallFrame(
	string FunctionName,
	string ScriptId,
	string Url,
	int LineNumber,
	int ColumnNumber
);