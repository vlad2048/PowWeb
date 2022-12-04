using PowWeb.ChromeApi.Utils;
using PuppeteerSharp;

namespace PowWeb.ChromeApi.DDom;

static class DomApi
{
	public static void Dom_Disable(this CDPSession client) => client.Send("DOM.disable");
	public static void Dom_Enable(
		this CDPSession client,
		// none, all
		string? includeWhitespace = null
	)
		=> client.Send("DOM.enable", new
		{
			IncludeWhitespace = includeWhitespace
		});

}