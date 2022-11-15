using PowWeb.ChromeApi.DFetch.Structs;
using PowWeb.ChromeApi.Utils.Attributes;

namespace PowWeb.ChromeApi.DFetch.Events;

[ChromeEvent("Fetch.authRequired")]
record AuthRequiredEvent(
	string RequestId,
	DNetwork.Structs.Request Request,
	string FrameId,
	DNetwork.Enums.ResourceType ResourceType,
	AuthChallenge AuthChallenge
);