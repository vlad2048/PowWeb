namespace PowWeb.ChromeApi.DNetwork.Structs;

record Request(
	string Url,
	string? UrlFragment,
	string Method,
	object Headers,
	string? PostData,
	bool? HasPostData,
	PostDataEntry[]? PostDataEntry,
	string? MixedContentType,
	string InitialPriority,
	string ReferrerPolicy,
	bool? IsLinkPreload,
	TrustTokenParams? TrustTokenParams,
	bool? IsSameSite
);