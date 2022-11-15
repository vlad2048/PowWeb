namespace PowWeb.ChromeApi.DNetwork.Structs;

record TrustTokenParams(
	string Type,
	string RefreshPolicy,
	string[]? Issuers
);