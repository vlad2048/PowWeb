using PowWeb.ChromeApi.DRuntime.Structs;

namespace PowWeb.ChromeApi.DDebugger.Structs;

public record CallFrame(
	string CallFrameId,
	string FunctionName,
	Location? FunctionLocation,
	Location Location,
	string Url,
	Scope[] ScopeChain,
	RemoteObject This,
	RemoteObject ReturnValue,
	bool CanBeRestarted
);