namespace PowWeb.ChromeApi.DFetch.Structs;

record RequestPattern(
	string? UrlPattern,
	//DNetwork.Enums.ResourceType? ResourceType,
	string? ResourceType,
	string? RequestStage
);