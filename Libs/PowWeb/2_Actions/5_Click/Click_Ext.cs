using PowBasics.Geom;
using PowRxVar;
using PowWeb._1_Init._4_Exec.Structs.Enums;
using PowWeb._2_Actions._5_Click.Events;
using PowWeb._2_Actions._5_Click.Logic;

// ReSharper disable CheckNamespace
namespace PowWeb;

public class ClickOpt
{
	public CancellationToken CancelToken { get; set; } = CancellationToken.None;
	public bool HighlightClick { get; set; } = true;
	public bool RestoreScrollingPosAfter { get; set; }
	public TimeSpan? TimeToWaitForDodgyTabsAfterClick { get; set; } = TimeSpan.FromMilliseconds(1000);
	public TimeSpan? DOMContentLoadedTimeout { get; set; } = TimeSpan.FromMilliseconds(1000);

	private ClickOpt()
	{
	}

	internal static ClickOpt Build(Action<ClickOpt>? optFun)
	{
		var opt = new ClickOpt();
		optFun?.Invoke(opt);
		return opt;
	}
}

public static class Click_Ext
{
	public static void Click(this WebInst www, N nod, Action<ClickOpt>? optFun = null) => www.Click(nod.V.Bounds, optFun);

	public static void Click(this WebInst www, R nodR, Action<ClickOpt>? optFun = null)
	{
		if (nodR == R.Empty) return;
		www.Click(nodR.Center, optFun);
	}

	public static void Click(this WebInst www, Pt clickPt, Action<ClickOpt>? optFun = null)
	{
		www.SigStart(CodeLoc.Click);

		var opt = ClickOpt.Build(optFun);
		var page = www.GetPage();
		using var d = new Disp();
		var (evtSig, evtObs) = ClickEvents.Make().D(d);

		www.EnforceSingleTab(out var slimTab, evtObs, evtSig, opt.TimeToWaitForDodgyTabsAfterClick).D(d);
		www.WaitDOMContentLoaded(out var slimDomLoaded, opt.DOMContentLoadedTimeout).D(d);

		using (var _ = www.ScrollIntoViewAndRestore(clickPt, opt.RestoreScrollingPosAfter))
		{
			clickPt -= www.GetScroll();
			if (opt.HighlightClick)
				www.Blink(clickPt, bo =>
				{
					bo.CancelToken = opt.CancelToken;
				});
			page.Mouse.ClickAsync(clickPt.X, clickPt.Y).Wait();
			evtSig.SignalClickDone();
		}

		slimTab.Wait(opt.TimeToWaitForDodgyTabsAfterClick ?? TimeSpan.Zero, opt.CancelToken);
		slimDomLoaded.Wait(opt.DOMContentLoadedTimeout ?? TimeSpan.Zero, opt.CancelToken);


		www.SigEnd();
	}
}















/*
using PowBasics.Geom;
using PowRxVar;
using PowWeb._1_Init._4_Exec.Structs.Enums;
using PowWeb._2_Actions._5_Click.Utils;
using PuppeteerSharp;

// ReSharper disable CheckNamespace

namespace PowWeb;

public class ClickOpt
{
	public TimeSpan? DebugDelayBeforeClosingExtraTabs { get; set; }
	public bool WaitForDOMContentLoadedEvent { get; set; }
	public TimeSpan DOMContentLoadedEventTimeout { get; set; } = TimeSpan.FromMilliseconds(1000);
	public TimeSpan SneakyTabChangeDetectorTimeout { get; set; } = TimeSpan.FromMilliseconds(1000);

	private ClickOpt()
	{
	}

	internal static ClickOpt Build(Action<ClickOpt>? optFun)
	{
		var opt = new ClickOpt();
		optFun?.Invoke(opt);
		return opt;
	}
}

public static class Click_Ext
{
	public static void Click(this WebInst www, N nod, Action<ClickOpt>? optFun = null) => www.Click(nod.V.Bounds, optFun);

	public static void Click(this WebInst www, R nodR, Action<ClickOpt>? optFun = null)
	{
		if (nodR == R.Empty) return;
		www.Click(nodR.Center, optFun);
	}

	public static void Click(this WebInst www, Pt clickPt, Action<ClickOpt>? optFun = null)
	{
		www.SigStart(CodeLoc.Click);

		var opt = ClickOpt.Build(optFun);
		var page = www.GetPage();
		using var d = new Disp();

		ISneakyTabChangeDetector detector = www.Opt.DisallowMultipleTabs switch
		{
			true => new SneakyTabChangeDetector(page, www, opt.SneakyTabChangeDetectorTimeout).D(d),
			false => new DummySneakyTabChangeDetector().D(d),
		};

		var needScroll = ScrollIFN(page, ref clickPt);
		page.Mouse.ClickAsync(clickPt.X, clickPt.Y).Wait();

		if (opt.WaitForDOMContentLoadedEvent)
		{
			var eventAwaiter = new EventAwaiter();
			page.DOMContentLoaded += eventAwaiter.Listen;
			eventAwaiter.Wait(opt.DOMContentLoadedEventTimeout);
			page.DOMContentLoaded -= eventAwaiter.Listen;
		}

		if (www.Opt.DisallowMultipleTabs)
		{
			detector.SigGotoDone();
			var changeNfo = detector.WaitAndGetInfo();
			if (opt.DebugDelayBeforeClosingExtraTabs.HasValue)
				Thread.Sleep(opt.DebugDelayBeforeClosingExtraTabs.Value);
			changeNfo.CloseExtraPages();

			if (changeNfo.HasUrlChanged)
			{
				www.CurrentUrl = changeNfo.Page.Url;
				needScroll = false;
			}
		}

		UnscrollIFN(page, needScroll);

		www.SigEnd();
	}


	private static bool ScrollIFN(Page page, ref Pt clickPt)
	{
		var needScroll = clickPt.X >= page.Viewport.Width || clickPt.Y >= page.Viewport.Height;
		if (needScroll)
		{
			page.SetScroll(clickPt);
			var scrollPt = page.GetScroll();
			clickPt -= scrollPt;
		}
		return needScroll;
	}

	private static void UnscrollIFN(Page page, bool needScroll)
	{
		if (needScroll)
			page.SetScroll(Pt.Empty);
	}

	private static Pt GetScroll(this Page page)
	{
		var x = page.EvaluateExpressionAsync("document.documentElement.scrollLeft").Result.ToObject<int>();
		var y = page.EvaluateExpressionAsync("document.documentElement.scrollTop").Result.ToObject<int>();
		return new Pt(x, y);
	}
	
	private static void SetScroll(this Page page, Pt pt)
	{
		try
		{
			page.EvaluateExpressionAsync($"document.documentElement.scrollLeft = {pt.X}").Wait();
			page.EvaluateExpressionAsync($"document.documentElement.scrollTop= {pt.Y}").Wait();
		}
		catch (Exception)
		{
		}
	}
}
*/