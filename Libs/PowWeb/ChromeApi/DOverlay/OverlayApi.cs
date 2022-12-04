using PowWeb.ChromeApi.DOverlay.Structs;
using PowWeb.ChromeApi.Utils;
using PuppeteerSharp;
using RGBA = PowWeb.ChromeApi.DDom.Structs.RGBA;

namespace PowWeb.ChromeApi.DOverlay;

static class OverlayApi
{
	public static void Overlay_Disable(this CDPSession client) => client.Send("Overlay.disable");
	public static void Overlay_Enable(this CDPSession client) => client.Send("Overlay.enable");

	public static void Overlay_HideHighlight(this CDPSession client) => client.Send("Overlay.hideHighlight");
	
	public static void Overlay_HighlightNode(
		this CDPSession client,
		HighlightConfig highlightConfig,
		int? nodeId,
		int? backendNodeId,
		string? remoteObjectId,
		string? selector
	)
		=> client.Send("Overlay.highlightNode", new
		{
			HighlightConfig = highlightConfig,
			NodeId = nodeId,
			BackendNodeId = backendNodeId,
			RemoteObjectId = remoteObjectId,
			Selector = selector
		});

	public static void Overlay_HighlightRect(
		this CDPSession client,
		int x,
		int y,
		int width, 
		int height,
		RGBA? color,
		RGBA? outlineColor
	)
		=> client.Send("Overlay.highlightRect", new
		{
			X = x,
			Y = y,
			Width = width,
			Height = height,
			Color = color,
			OutlineColor = outlineColor,
		});
}