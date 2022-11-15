namespace PowWeb.ChromeApi.DNetwork.Structs;

record Initiator(
	string Type,
	DRuntime.Structs.StackTrace? Stack,
	string? Url,
	int? LineNumber,
	int? ColumnNumber,
	string? RequestId
);