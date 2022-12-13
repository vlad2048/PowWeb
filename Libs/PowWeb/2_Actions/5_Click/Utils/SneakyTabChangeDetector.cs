/*
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using PowRxVar;
using PowWeb._1_Init._1_OptStructs.Interfaces;
using PowWeb._1_Init._2_OptExts;
using PowWeb._1_Init._4_Exec.Structs;
using PowWeb._1_Init.Utils;
using PowWeb._2_Actions._2_Cap.Logic._3_DocMerging.Utils;
using PowWeb.ChromeApi.Utils.Extensions;
using PuppeteerSharp;

namespace PowWeb._2_Actions._5_Click.Utils;

record SneakyTabChangeNfo(
	string InitialUrl,
	int ExtraTabCountBefore,
	Page Page,
	Page[] ExtraPagesAfter,
	bool NeedsBringToFront
)
{
	public bool HasUrlChanged => Page.Url != InitialUrl;

	public void CloseExtraPages() => ExtraPagesAfter.ClosePages();
}


interface ISneakyTabChangeDetector : IDisposable
{
	void SigGotoDone();
	SneakyTabChangeNfo WaitAndGetInfo();
}

class DummySneakyTabChangeDetector : ISneakyTabChangeDetector
{
	private readonly Disp d = new();
	public void Dispose() => d.Dispose();

	public void SigGotoDone() { }
	public SneakyTabChangeNfo WaitAndGetInfo() => throw new NotImplementedException();
}

class SneakyTabChangeDetector : ISneakyTabChangeDetector
{
	private readonly Disp d = new();
	public void Dispose() => d.Dispose();

	private readonly ManualResetEventSlim reset;
	private readonly IInitLogging logger;
	private readonly Browser browser;
	private readonly string initialUrl;
	private readonly int extraTabCountBefore;
	private readonly ISubject<Unit> whenGotoDone = new AsyncSubject<Unit>();
	private IObservable<Unit> WhenGotoDone => whenGotoDone.AsObservable();

	public void SigGotoDone() { whenGotoDone.OnNext(Unit.Default); whenGotoDone.OnCompleted(); }

	public SneakyTabChangeDetector(Page page, IInitLogging logger, TimeSpan timeout)
	{
		initialUrl = page.Url;
		this.logger = logger;
		browser = page.Browser;
		var pages = browser.GetPages(null);
		extraTabCountBefore = pages.Count(e => e != page);
		if (extraTabCountBefore == pages.Length) throw new FatalException("Cannot find our page");
		reset = new ManualResetEventSlim().D(d);

		//logger.Log("SneakyTab ... ", Cols.Default);

		WhenGotoDone
			//.Do(_ => logger.Log("GotoDone ... ", Cols.Default))
			.Delay(timeout)
			//.Do(_ => logger.Log("Timeout -> ", Cols.Default))
			.Subscribe(_ => reset.Set()).D(d);
	}

	public SneakyTabChangeNfo WaitAndGetInfo()
	{
		reset.Wait();

		var pages = browser.GetPages(null);
		var page = LevenshteinDistance.FindClosest(initialUrl, pages, e => e.Url);
		var extraPagesAfter = pages.WhereToArray(e => e != page);

		var needsBringToFront = page.IsVisible();

		var nfo = new SneakyTabChangeNfo(
			InitialUrl: initialUrl,
			ExtraTabCountBefore: extraTabCountBefore,
			Page: page,
			ExtraPagesAfter: extraPagesAfter,
			NeedsBringToFront: needsBringToFront
		);

		logger.Log("hasUrlChanged:", Cols.Default);
		if (nfo.HasUrlChanged)
			logger.Log("yes", Cols.Yes);
		else
			logger.Log("no", Cols.No);
		logger.Log($" extraTabsBefore:{nfo.ExtraTabCountBefore} extraTabsAfter:{nfo.ExtraPagesAfter.Length}", Cols.Default);
		logger.Log(" needsBringToFront:", Cols.Default);
		if (nfo.NeedsBringToFront);
			logger.Log("yes", Cols.Yes);
		else
			logger.Log("no", Cols.No);
		logger.LogNewLine();
		if (nfo.HasUrlChanged)
		{
			logger.LogLine($"url prev: '{nfo.InitialUrl}'", Cols.Default);
			logger.LogLine($"url next: '{nfo.Page.Url}'", Cols.Default);
		}

		return nfo
	}
}
*/