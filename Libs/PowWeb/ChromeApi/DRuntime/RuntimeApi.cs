using PowWeb.ChromeApi.DRuntime.Structs;
using PowWeb.ChromeApi.Utils;
using PuppeteerSharp;

namespace PowWeb.ChromeApi.DRuntime;

static class RuntimeApi
{
	public record Runtime_Evaluate_Ret(RemoteObject Result, ExceptionDetails? ExceptionDetails);
	public static Runtime_Evaluate_Ret Runtime_Evaluate(
		this CDPSession client,
		string expression
	) =>
		client.Send<Runtime_Evaluate_Ret>("Runtime.evaluate", new
		{
			Expression = expression
		});
}