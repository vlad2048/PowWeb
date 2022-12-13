using System.Reactive;
using System.Reactive.Linq;
using PowRxVar;
using PuppeteerSharp;

namespace PowWeb._2_Actions._5_Click.Logic;

static class DOMContentLoadedWaitingLogic
{
	public static IDisposable WaitDOMContentLoaded(
		this WebInst www,
		out ManualResetEventSlim slim,
		TimeSpan? domContentLoadedTimout
	)
	{
		var d = new Disp();
		slim = new ManualResetEventSlim().D(d);
		var slim_ = slim;
		if (!domContentLoadedTimout.HasValue)
		{
			slim.Set();
			return d;
		}
		var page = www.GetPage();

		page.WhenLoaded()
			.Subscribe(_ =>
			{
				slim_.Set();
			}).D(d);

		return d;
	}

	private static IObservable<Unit> WhenLoaded(this Page page) =>
		Obs.FromEventPattern(e => page.DOMContentLoaded += e, e => page.DOMContentLoaded -= e).ToUnit();
}