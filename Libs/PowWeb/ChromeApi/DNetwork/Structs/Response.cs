namespace PowWeb.ChromeApi.DNetwork.Structs;

record Response(
	string Url,
	int Status,
	string StatusText,
	Dictionary<string, string> Headers,
	string? HeadersText,
	string MimeType,
	Dictionary<string, string>? RequestHeaders,
	string? RequestHeadersText,
	bool ConnectionReused,
	int ConnectionId,
	string? RemoteIPAddress,
	int? RemotePort,
	bool? FromDiskCache,
	bool? FromServiceWorker,
	bool? FromPrefetchCache,
	int EncodedDataLength,
	ResourceTiming Timing,
	// ...
	string? Protocol
	// ...
);