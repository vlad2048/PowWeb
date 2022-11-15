using System.Diagnostics;
using Newtonsoft.Json.Linq;
using PowWeb._1_Init._1_OptStructs;
using PowWeb._1_Init._2_OptExts;
using PowWeb._1_Init._4_Exec.Structs;
using PowWeb.ChromeApi.Utils.Extensions;
using PuppeteerSharp;

namespace PowWeb._1_Init.Utils;

static class GetPageUtils
{
	private static readonly string[] ignoreUrls =
	{
		"adblockplus",
		"chrome-extension",
	};

	public static string GetUrlOnStartup(this Browser browser, WebOpt opt)
	{
		var pages = browser.GetPages(opt);
		if (pages.Length == 0) throw new FatalException("No page on startup");
		var visiblePage = pages.FirstOrDefault(e => e.IsVisible());
		if (visiblePage != null)
		{
			var others = pages.WhereToArray(e => e != visiblePage);
			others.ClosePages();
			return visiblePage.Url;
		}
		else
		{
			var page = pages[0];
			page.BringToFrontAsync();
			var others = pages.WhereToArray(e => e != page);
			others.ClosePages();
			return page.Url;
		}
	}

	public static (Page[], int) GetPagesAndAdCnt(this Browser browser, WebOpt? opt)
	{
		var watch = Stopwatch.StartNew();
		var pages = browser.PagesAsync().Result;
		var pagesAdblock = pages.WhereToArray(e => ignoreUrls.Any(f => e.Url.Contains(f)));
		pages = pages.WhereToArray(e => !pagesAdblock.Contains(e));
		var time = watch.Elapsed;
		opt?.LogLine($"GetPages({pages.Length}) adpages:{pagesAdblock.Length} time:{(int)time.TotalMilliseconds}ms", Cols.Perf);
		pagesAdblock.ClosePages();
		return (pages, pagesAdblock.Length);
	}

	public static Page[] GetPages(this Browser browser, WebOpt? opt) => browser.GetPagesAndAdCnt(opt).Item1;


	public static Page GetPage(Browser browser, string currentUrl, WebOpt opt)
	{
		var watch = Stopwatch.StartNew();
		var (pages, pageAdblockCnt) = browser.GetPagesAndAdCnt(null);
		var time = (int)watch.Elapsed.TotalMilliseconds;

		if (opt.DisablePageTracking)
		{
			var visiblePage = pages.FirstOrDefault(e => e.IsVisible());
			if (visiblePage == null)
				visiblePage = pages[0];
			return visiblePage;
		}

		var page = pages.FirstOrDefault(e => UrlUtils.AreUrlsTheSame(e.Url, currentUrl));
		var isVisible = page != null && page.IsVisible();
		var needBringToFront = page != null && !isVisible;

		LogGetPageInfo(opt, pages.Length, pageAdblockCnt, page != null, isVisible, time);

		if (page == null)
			throw new FatalException($"Could not find page for CurrentUrl:'{currentUrl}'");

		if (needBringToFront)
			page.BringToFrontAsync().Wait();

		return page;
	}

	public static bool IsVisible(this Page page)
	{
		try
		{
			return (page.EvaluateExpressionAsync("document.visibilityState").Result as JValue)!.Value as string == "visible";
		}
		catch (Exception ex)
		{
			return true;
		}
	}

	public static void ClosePages(this IEnumerable<Page> pages)
	{
		foreach (var page in pages)
			page.CloseAsync().Wait();
	}


	private static void LogGetPageInfo(WebOpt opt, int pageCnt, int pageAdblockCnt, bool pageFound, bool isVisible, int time)
	{
		return;
		opt.Log($"[GetPage] pages:{pageCnt} pagesAdblock:{pageAdblockCnt} found:", Cols.Default);
		if (pageFound)
		{
			opt.Log("yes", Cols.Yes);
			opt.Log(" visible:", Cols.Default);
			if (isVisible)
			{
				opt.Log("yes", Cols.Yes);
			}
			else
			{
				opt.Log("no", Cols.No);
			}
		}
		else
		{
			opt.Log("no", Cols.No);
		}

		opt.LogLine($"  time:{time}ms", Cols.Perf);
	}
}