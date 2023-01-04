namespace PowWeb.ChromeApi.DRuntime.Structs;

public record WebDriverValue(
	string Type,
	object? Value,
	string? ObjectId
);