using System.Reactive.Linq;
using PowWeb._1_Init._4_Exec.Structs.Enums;
using PuppeteerSharp;
// ReSharper disable CheckNamespace

namespace PowWeb;

public static class WhenRequest_Ext
{
	public static IObservable<Request> WhenRequestOfType(this WebInst www, ResourceType resourceType) =>
		www.WhenRequest()
			.Where(e => e.ResourceType == resourceType);
	
	public static IObservable<Request> WhenRequest(this WebInst www)
	{
		www.SigStart(CodeLoc.WhenRequest);

		var page = www.GetPage();
		var res = Observable.FromEventPattern<RequestEventArgs>(e => page.Request += e, e => page.Request -= e)
			.Select(e => e.EventArgs.Request);

		www.SigEnd();
		return res;
	}
}