using PowWeb.ChromeApi.DDebugger.Enums;
using PowWeb.ChromeApi.DDebugger.Structs;
using PowWeb.ChromeApi.Utils;
using PuppeteerSharp;

// ReSharper disable ClassNeverInstantiated.Local

namespace PowWeb.ChromeApi.DDebugger;

static class DebuggerApi
{
	private record Debugger_Enable_Ret(string DebuggerId);
	public static string Debugger_Enable(this CDPSession client) => client.Send<Debugger_Enable_Ret>("Debugger.enable").DebuggerId;
	public static void Debugger_Disable(this CDPSession client) => client.Send("Debugger.disable");

	private record Debugger_SetBreakpointOnFunctionCall_Ret(string BreakpointId);
	public static string Debugger_SetBreakpointOnFunctionCall(this CDPSession client, string objectId, string? condition = null) =>
		client.Send<Debugger_SetBreakpointOnFunctionCall_Ret>(
			"Debugger.setBreakpointOnFunctionCall",
			new
			{
				ObjectId = objectId,
				Condition = condition
			}
		).BreakpointId;



	public static void Debugger_Pause(this CDPSession client) => client.Send("Debugger.pause");
	public static void Debugger_Resume(this CDPSession client, bool terminateOnResume = false) => client.Send("Debugger.resume", new { TerminateOnResume = terminateOnResume });
	public static void Debugger_SetBreakpointsActive(this CDPSession client, bool active) => client.Send("Debugger.setBreakpointsActive", new { Active = active });
	public static void Debugger_SetPauseOnExceptions(this CDPSession client, ExceptionPauseState state) => client.Send("Debugger.setPauseOnExceptions", new { State = $"{state}".ToLowerInvariant() });
	public static void Debugger_SetSkipAllPauses(this CDPSession client, bool skip) => client.Send("Debugger.setSkipAllPauses", new { Skip = skip });

	public record Debugger_SetBreakpoint_Ret(string BreakpointId, Location ActualLocation);
	public static Debugger_SetBreakpoint_Ret SetBreakpoint(this CDPSession client, Location location, string? condition = null) => client.Send<Debugger_SetBreakpoint_Ret>("Debugger.setBreakpoint", new { Location = location, Condition = condition });

	public record Debugger_GetScriptSource_Ret(string ScriptSource, string? Bytecode);
	public static Debugger_GetScriptSource_Ret Debugger_GetScriptSource(this CDPSession client, string scriptId) => client.Send<Debugger_GetScriptSource_Ret>("Debugger.getScriptSource", new { ScriptId = scriptId });
	
	private record Debugger_SetInstrumentationBreakpoint_Ret(string BreakpointId);
	public static string Debugger_SetInstrumentationBreakpoint(this CDPSession client, Instrumentation instrumentation) => client.Send<Debugger_SetInstrumentationBreakpoint_Ret>("Debugger.setInstrumentationBreakpoint", new { Instrumentation = $"{instrumentation}".CamelCase() }).BreakpointId;
}