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

		ISneakyTabChangeDetector detector = www.Opt.DisablePageTracking switch
		{
			false => new SneakyTabChangeDetector(page, www, opt.SneakyTabChangeDetectorTimeout).D(d),
			true => new DummySneakyTabChangeDetector().D(d),
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

		if (!www.Opt.DisablePageTracking)
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