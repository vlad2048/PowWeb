using PowWeb.ChromeApi.DDebugger.Structs;
using PowWeb.ChromeApi.Utils.Attributes;

namespace PowWeb.ChromeApi.DDebugger.Events;

[ChromeEvent("Debugger.paused")]
record PausedEvent(
	CallFrame[] CallFrames,
	string Reason,
	object? Data,
	string[]? HitBreakpoints,
	DRuntime.Structs.StackTrace? AsyncStackTrace,
	DRuntime.Structs.StackTraceId? AsyncStackTraceId
);

[ChromeEvent("Debugger.scriptParsed")]
record ScriptParsedEvent(
	string ScriptId,
	string Url,
	int StartLine,
	int StartColumn,
	int EndLine,
	int EndColumn,
	int ExecutionContextId,
	string Hash,
	object? ExecutionContextAuxData,
	bool? IsLiveEdit,
	string? SourceMapUrl,
	bool? HasSourceURL,
	bool? IsModule,
	int? Length,
	DRuntime.Structs.StackTrace? StackTrace,
	int? CodeOffset,
	string? ScriptLanguage,
	DebugSymbols? DebugSymbols,
	string? EmbedderName
);