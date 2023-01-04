using PowWeb.ChromeApi.DDebugger.Structs;
using PowWeb.ChromeApi.Utils.Attributes;

namespace PowWeb.ChromeApi.DDebugger.Events;

[ChromeEvent("Debugger.paused")]
public record PausedEvent(
	CallFrame[] CallFrames,
	string Reason,
	object? Data,
	string[]? HitBreakpoints,
	DRuntime.Structs.StackTrace? AsyncStackTrace,
	DRuntime.Structs.StackTraceId? AsyncStackTraceId
);