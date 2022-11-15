using System.Text;
using PowWeb.ChromeApi.DFetch.Structs;
using PowWeb.ChromeApi.Utils;
using PuppeteerSharp;

// ReSharper disable ClassNeverInstantiated.Local

namespace PowWeb.ChromeApi.DFetch;

static class FetchApi
{
	public static void Fetch_Disable(this CDPSession client) => client.Send("Fetch.disable");
	public static void Fetch_Enable(
		this CDPSession client,
		RequestPattern[]? patterns = null,
		bool? handleAuthRequests = null
	)
		=> client.Send("Fetch.enable", new
		{
			Patterns = patterns,
			HandleAuthRequests = handleAuthRequests
		});
	public static void Fetch_ContinueRequest(
		this CDPSession client,
		string requestId,
		string? url = null,
		string? method = null,
		string? postData = null,
		HeaderEntry[]? headers = null,
		bool? interceptResponse = null
	)
		=> client.Send("Fetch.continueRequest", new
		{
			RequestId = requestId,
			Url = url,
			Method = method,
			PostData = postData,
			Headers = headers,
			InterceptResponse = interceptResponse
		});

	public static void Fetch_FullfillRequest(
		this CDPSession client,
		string requestId,
		int responseCode,
		HeaderEntry[]? responseHeaders,
		string? binaryResponseHeaders,
		string? body,
		string? responsePhrase
	)
		=> client.Send("Fetch.fullfillRequest", new
		{
			RequestId = requestId,
			ResponseCode = responseCode,
			ResponseHeaders = responseHeaders,
			BinaryResponseHeaders = binaryResponseHeaders,
			Body = body,
			ResponsePhrase = responsePhrase,
		});

	private record Fetch_GetResponseBody_Ret(string Body, bool Base64Encoded);
	public static string Fetch_GetResponseBody(
		this CDPSession client,
		string requestId
	)
	{
		var res = client.Send<Fetch_GetResponseBody_Ret>("Fetch.getResponseBody", new { RequestId = requestId });
		if (res.Base64Encoded)
		{
			var bytes = Convert.FromBase64String(res.Body);
			var str = Encoding.UTF8.GetString(bytes);
			return str;
		}
		else
		{
			return res.Body;
		}
	}
}