namespace PowWeb.ChromeApi.DRuntime.Structs;

record PropertyPreview(
	string Name,
	string Type,
	string? Value,
	RemoteObject? ValuePreview,
	string Subtype
);