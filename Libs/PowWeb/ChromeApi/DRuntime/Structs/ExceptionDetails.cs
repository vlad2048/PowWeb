namespace PowWeb.ChromeApi.DRuntime.Structs;

public record ExceptionDetails(
	int ExceptionId,
	string Text,
	int LineNumber,
	int ColumnNumber,
	string? ScriptId,
	string? Url,
	StackTrace? StackTrace,
	RemoteObject? Exception,
	int? ExecutionContextId,
	object? ExceptionMetaData
);