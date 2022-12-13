using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using PowRxVar;
using PowWeb._1_Init._1_OptStructs.Interfaces;
using PowWeb._1_Init._2_OptExts;
using PowWeb._1_Init._4_Exec.Structs;
using PowWeb._1_Init.Utils;
using PowWeb._2_Actions._2_Cap.Logic._3_DocMerging.Utils;
using PowWeb._2_Actions._5_Click.Events;
using PowWeb.ChromeApi.Utils.Extensions;
using PuppeteerSharp;

namespace PowWeb._2_Actions._5_Click.Logic;


static class SingleTabEnforceLogic
{
	public static IDisposable EnforceSingleTab(
		this WebInst www,
		out ManualResetEventSlim slim,
		IClickEvtObs evtObs,
		IClickEvtSig evtSig,
		TimeSpan? timeoutOpt
	)
	{
		var d = new Disp();
		slim = new ManualResetEventSlim().D(d);
		var slim_ = slim;
		if (!timeoutOpt.HasValue)
		{
			slim.Set();
			return d;
		}

		var page = www.GetPage();
		var timeout = timeoutOpt.Value;
		var browser = page.Browser;
		var initialUrl = page.Url;
		var pages = browser.GetPages(null);
		var extraTabCountBefore = pages.Count(e => e != page);
		if (extraTabCountBefore == pages.Length) throw new FatalException("Cannot find our page");

		evtObs.WhenClickDone
			.Delay(timeout)
			.Subscribe(_ =>
			{
				pages = browser.GetPages(null);
				page = LevenshteinDistance.FindClosest(initialUrl, pages, e => e.Url);
				var extraPagesAfter = pages.WhereToArray(e => e != page);

				var needsBringToFront = page.IsVisible();

				var nfo = new SneakyTabChangeNfo(
					InitialUrl: initialUrl,
					ExtraTabCountBefore: extraTabCountBefore,
					Page: page,
					ExtraPagesAfter: extraPagesAfter,
					NeedsBringToFront: needsBringToFront
				);

				nfo.Log(www);

				nfo.CloseExtraPages();

				if (nfo.HasUrlChanged)
				{
					evtSig.SignalForgetAboutReversingTheScroll();
					www.CurrentUrl = initialUrl;
				}

				slim_.Set();

			}).D(d);

		return d;
	}





	private record SneakyTabChangeNfo(
		string InitialUrl,
		int ExtraTabCountBefore,
		Page Page,
		Page[] ExtraPagesAfter,
		bool NeedsBringToFront
	)
	{
		public bool HasUrlChanged => Page.Url != InitialUrl;

		public void CloseExtraPages() => ExtraPagesAfter.ClosePages();

		public void Log(IInitLogging logger)
		{
			logger.Log("hasUrlChanged:", Cols.Default);
			if (HasUrlChanged)
				logger.Log("yes", Cols.Yes);
			else
				logger.Log("no", Cols.No);
			logger.Log($" extraTabsBefore:{ExtraTabCountBefore} extraTabsAfter:{ExtraPagesAfter.Length}", Cols.Default);
			logger.Log(" needsBringToFront:", Cols.Default);
			if (NeedsBringToFront)
				logger.Log("yes", Cols.Yes);
			else
				logger.Log("no", Cols.No);
			logger.LogNewLine();
			if (HasUrlChanged)
			{
				logger.LogLine($"url prev: '{InitialUrl}'", Cols.Default);
				logger.LogLine($"url next: '{Page.Url}'", Cols.Default);
			}
		}
	}
}