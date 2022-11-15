using PowWeb.ChromeApi.DDomSnapshot.Structs;
using PowWeb.ChromeApi.Utils;
using PuppeteerSharp;

namespace PowWeb.ChromeApi.DDomSnapshot;

static class DomSnapshotApi
{
	public record DomSnapshot_CaptureSnapshot_Ret(
		DocumentSnapshot[] Documents,
		string[] Strings
	);
	public static DomSnapshot_CaptureSnapshot_Ret DomSnapshot_CaptureSnapshot(
		this CDPSession client,
		string[]? computedStyles = null,
		bool? includePaintOrder = null,
		bool? includeDOMRects = null,
		bool? includeBlendedBackgroundColors = null,
		bool? includeTextColorOpacities = null
	) =>
		client.Send<DomSnapshot_CaptureSnapshot_Ret>(
			"DOMSnapshot.captureSnapshot",
			new
			{
				ComputedStyles = computedStyles ?? Array.Empty<string>(),
				IncludePaintOrder = includePaintOrder,
				IncludeDOMRects = includeDOMRects,
				IncludeBlendedBackgroundColors = includeBlendedBackgroundColors,
				IncludeTextColorOpacities = includeTextColorOpacities
			}
		);
}