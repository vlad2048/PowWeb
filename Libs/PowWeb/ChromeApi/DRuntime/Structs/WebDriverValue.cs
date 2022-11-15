namespace PowWeb.ChromeApi.DRuntime.Structs;

record WebDriverValue(
	string Type,
	object? Value,
	string? ObjectId
);