namespace PowWeb.ChromeApi.DFetch.Structs;

record AuthChallenge(
	string? Source,
	string Origin,
	string Scheme,
	string Realm
);