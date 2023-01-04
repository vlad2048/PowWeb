using PowWeb.ChromeApi.DRuntime.Structs;

namespace PowWeb.ChromeApi.DDebugger.Structs;

public record Scope(
	string Type,
	RemoteObject Object,
	string Name,
	Location StartLocation,
	Location EndLocation
);