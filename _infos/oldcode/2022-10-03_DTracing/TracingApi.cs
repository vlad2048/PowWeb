using PowWeb.ChromeApi.DTracing.Structs;
using PowWeb.ChromeApi.Utils;
using PuppeteerSharp;

namespace PowWeb.ChromeApi.DTracing;

static class TracingApi
{
	public record Tracing_GetCategories_Ret(string[] Categories);
	public static Tracing_GetCategories_Ret Tracing_GetCategories(this CDPSession client) => client.Send<Tracing_GetCategories_Ret>("Tracing.getCategories");

	public static void Tracing_Start(this CDPSession client, TraceConfig traceConfig) => client.Send("Tracing.start", new
	{
		TraceConfig = traceConfig
	});

	public static void Tracing_End(this CDPSession client) => client.Send("Tracing.end");
}