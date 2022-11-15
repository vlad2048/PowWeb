namespace PowWeb.ChromeApi.DRuntime.Structs;

record StackTrace(
	string? Description,
	CallFrame[] CallFrames,
	StackTrace? Parent,
	StackTraceId? ParentId
);