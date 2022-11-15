using PowWeb.ChromeApi.DFetch.Structs;
using PowWeb.ChromeApi.Utils.Attributes;

namespace PowWeb.ChromeApi.DFetch.Events;

[ChromeEvent("Fetch.requestPaused")]
record RequestPausedEvent(
	string RequestId,
	DNetwork.Structs.Request Request,
	string FrameId,
	DNetwork.Enums.ResourceType ResourceType,
	string? ResponseErrorReason,
	int? ResponseStatusCode,
	string? ResponseStatusText,
	HeaderEntry[]? ResponseHeaders,
	string? NetworkId
);