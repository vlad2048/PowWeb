using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using PowBasics.Geom;
using PowRxVar;
using PowWeb._2_Actions._5_Click.Events;
using PuppeteerSharp;

// ReSharper disable CheckNamespace
namespace PowWeb;

public record PageScrollNfo(
	Sz TotalSz,
	R ViewR
);

/*public record ScrollNfo(
	Pt ScrollPrev,
	Pt ScrollNext
)
{
	public bool ScrollWasNeeded => ScrollNext != ScrollPrev;
}*/

public static class Scroll_Ext
{
	private const int EdgeThresholdBeforeScrolling = 50;

	public static void ScrollIntoView(this WebInst www, Pt pt)
	{
		var nfo = www.GetScrollNfo();
		var scrollNext = new Pt(
			GetScrollPos(
				nfo.ViewR.X, nfo.ViewR.X + nfo.ViewR.Width,
				nfo.TotalSz.Width,
				pt.X
			),
			GetScrollPos(
				nfo.ViewR.Y, nfo.ViewR.Y + nfo.ViewR.Height,
				nfo.TotalSz.Height,
				pt.Y
			)
		);
		www.SetScroll(scrollNext);
	}

	public static IDisposable ScrollIntoViewAndRestore(this WebInst www, Pt pt, bool restore) => www.ScrollIntoViewAndRestore(pt, null, restore);

	internal static IDisposable ScrollIntoViewAndRestore(this WebInst www, Pt pt, IClickEvtObs? evtObs, bool restore)
	{
		var d = new Disp();

		var unscrollCancelled = false;
		evtObs?.WhenForgetAboutReversingTheScroll.Subscribe(_ => unscrollCancelled = true).D(d);

		var nfo = www.GetScrollNfo();
		var scrollPrev = nfo.ViewR.Pos;
		var scrollNext = new Pt(
			GetScrollPos(
				nfo.ViewR.X, nfo.ViewR.X + nfo.ViewR.Width,
				nfo.TotalSz.Width,
				pt.X
			),
			GetScrollPos(
				nfo.ViewR.Y, nfo.ViewR.Y + nfo.ViewR.Height,
				nfo.TotalSz.Height,
				pt.Y
			)
		);
		www.SetScroll(scrollNext);

		var scrollNeeded = scrollNext != scrollPrev;

		Disposable.Create(() =>
		{
			if (restore && scrollNeeded && !unscrollCancelled)
			{
				www.SetScroll(scrollPrev);
			}
		}).D(d);

		return d;
	}

	private static int GetScrollPos(int view0, int view1, int total, int target)
	{
		var viewDim = view1 - view0;
		var scrollMax = Math.Max(0, total - viewDim);
		var threshold = Math.Min(EdgeThresholdBeforeScrolling, viewDim / 4);
		return (target >= view0 + threshold && target <= view1 - threshold) switch
		{
			true => view0,
			false => Cap(target - viewDim / 2, scrollMax)
		};
	}

	private static int Cap(int val, int max) => Math.Max(0, Math.Min(max, val));


	public static PageScrollNfo GetScrollNfo(this WebInst www)
	{
		var page = www.GetPage();
		return new PageScrollNfo(
			new Sz(
				page.Read("scrollWidth"),
				page.Read("scrollHeight")
			),
			new R(
				new Pt(
					page.Read("scrollLeft"),
					page.Read("scrollTop")
				),
				new Sz(
					page.Read("clientWidth"),
					page.Read("clientHeight")
				)
			)
		);
	}

	public static Pt GetScroll(this WebInst www)
	{
		var page = www.GetPage();
		return new Pt(page.Read("scrollLeft"), page.Read("scrollTop"));
	}
	
	public static void SetScroll(this WebInst www, Pt pt)
	{
		try
		{
			var page = www.GetPage();
			page.Write("scrollLeft", pt.X);
			page.Write("scrollTop", pt.Y);
		}
		catch (Exception)
		{
			// ignored
		}
	}

	public static Sz GetViewport(this WebInst www)
	{
		var page = www.GetPage();
		var viewport = page.Viewport;
		return new Sz(viewport.Width, viewport.Height);
	}

	public static void SetViewport(this WebInst www, Sz sz)
	{
		var page = www.GetPage();
		page.SetViewportAsync(new ViewPortOptions
		{
			Width = sz.Width,
			Height = sz.Height,
		}).Wait();
	}


	private static int Read(this Page page, string propName) =>
		page.EvaluateExpressionAsync($"document.documentElement.{propName}").Result.ToObject<int>();

	private static void Write(this Page page, string propName, int propVal) =>
		page.EvaluateExpressionAsync($"document.documentElement.{propName} = {propVal}").Wait();
}