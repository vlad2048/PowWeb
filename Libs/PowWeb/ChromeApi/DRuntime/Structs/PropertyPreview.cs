namespace PowWeb.ChromeApi.DRuntime.Structs;

public record PropertyPreview(
	string Name,
	string Type,
	string? Value,
	RemoteObject? ValuePreview,
	string Subtype
);