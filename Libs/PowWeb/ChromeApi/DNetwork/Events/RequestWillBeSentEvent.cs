using PowWeb.ChromeApi.DNetwork.Enums;
using PowWeb.ChromeApi.DNetwork.Structs;
using PowWeb.ChromeApi.Utils.Attributes;

namespace PowWeb.ChromeApi.DNetwork.Events;

[ChromeEvent("Network.requestWillBeSentEvent")]
record RequestWillBeSentEvent(
	string RequestId,
	string LoaderId,
	string DocumentUrl,
	Request Request,
	int Timestamp,
	int WallTime,
	Initiator Initiator,
	bool RedirectHasExtraInfo,
	Response? RedirectResponse,
	ResourceType? Type,
	string? FrameId,
	bool? HasUserGesture
);