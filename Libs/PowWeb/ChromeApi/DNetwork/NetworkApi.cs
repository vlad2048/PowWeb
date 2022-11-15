using PowWeb.ChromeApi.Utils;
using PuppeteerSharp;

namespace PowWeb.ChromeApi.DNetwork;

static class NetworkApi
{
	public static void Network_Disable(this CDPSession client) => client.Send("Network.disable");
	public static void Network_Enable(
		this CDPSession client,
		int? maxTotalBufferSize = null,
		int? maxResourceBufferSize = null,
		int? maxPostDataSize = null
	)
		=> client.Send("Network.enable", new
		{
			MaxTotalBufferSize = maxTotalBufferSize,
			MaxResourceBufferSize = maxResourceBufferSize,
			MaxPostDataSize = maxPostDataSize
		});

	public static void Network_SetRequestInterception(
		this CDPSession client
	)
		=> client.Send("Network.setRequestInterception", new
		{
			Patterns = Array.Empty<bool>()
		});
}