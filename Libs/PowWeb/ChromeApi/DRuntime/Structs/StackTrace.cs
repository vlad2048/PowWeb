namespace PowWeb.ChromeApi.DRuntime.Structs;

public record StackTrace(
	string? Description,
	CallFrame[] CallFrames,
	StackTrace? Parent,
	StackTraceId? ParentId
);