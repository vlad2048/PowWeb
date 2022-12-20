using PowBasics.Geom;
using PowRxVar;
using PowWeb.ChromeApi.DDom.Structs;
using PuppeteerSharp;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using PowWeb.ChromeApi.DOverlay;
using PowWeb._1_Init._4_Exec.Structs.Enums;
using PowWeb._2_Actions._2_Cap.Structs;
using PowWeb.ChromeApi.DDom;

// ReSharper disable CheckNamespace
namespace PowWeb;

public class BlinkOpt
{
	public CancellationToken CancelToken { get; set; } = CancellationToken.None;
	public TimeSpan Freq { get; set; } = TimeSpan.FromMilliseconds(100);
	public TimeSpan TotalTime { get; set; } = TimeSpan.FromMilliseconds(1000);

	private BlinkOpt() { }

	internal static BlinkOpt Build(Action<BlinkOpt>? optFun)
	{
		var opt = new BlinkOpt();
		optFun?.Invoke(opt);
		return opt;
	}
}


public static class Blink_Ext
{
	private static readonly RGBA colOuterFill = new(91, 118, 255, 0.5);
	private static readonly RGBA colOuterStroke = new(8, 8, 8, 1.0);
	private static readonly RGBA colInnerFill = new(255, 66, 63, 1.0);
	private static readonly RGBA colInnerStroke = new(81, 0, 8, 0.9);
	private const int OuterSz = 100;
	private const int InnerSz = 30;


	public static void Blink(this WebInst www, N nod, Action<BlinkOpt>? optFun = null) => www.Blink(nod.V, optFun);
	public static void Blink(this WebInst www, CapNode node, Action<BlinkOpt>? optFun = null) => www.Blink(node.Bounds, optFun);
	public static void Blink(this WebInst www, R r, Action<BlinkOpt>? optFun = null) => www.Blink(r.Center, optFun);
	public static void Blink(this WebInst www, Pt origPt, Action<BlinkOpt>? optFun = null)
	{
		//www.SigStart(CodeLoc.Blink);

		var opt = BlinkOpt.Build(optFun);
		var page = www.GetPage();
		var client = page.Client;
		var scroll = www.GetScroll();
		var pt = origPt - scroll;
		var outerR = R.FromCenterAndSize(pt, new Sz(OuterSz, OuterSz));
		var innerR = R.FromCenterAndSize(pt, new Sz(InnerSz, InnerSz));
		
		using var d = new Disp();
		client.Dom_Enable();
		client.Overlay_Enable();
		Disposable.Create(() =>
		{
			client.Overlay_Disable();
			client.Dom_Disable();
		}).D(d);
		
		void ShowOuter() => client.ShowR(outerR, colOuterFill, colOuterStroke);
		void ShowInner() => client.ShowR(innerR, colInnerFill, colInnerStroke);
		void HideBoth() => client.Overlay_HideHighlight();
		
		var slim = new SemaphoreSlim(0).D(d);
		var startTime = DateTime.Now;
		
		Observable.Interval(opt.Freq)
			.Subscribe(i => WrapSafe(() =>
			{
				if (DateTime.Now - startTime >= opt.TotalTime)
				{
					slim.Release();
					return;
				}
				switch (i % 4)
				{
					case 0:
						ShowOuter();
						break;
						
					case 1:
						ShowInner();
						break;
						
					case 3:
						HideBoth();
						break;
				}
			})).D(d);
		
		slim.Wait(opt.CancelToken);

		WrapSafe(() =>
		{
			HideBoth();
		});

		//www.SigEnd();
	}

	private static void WrapSafe(Action action)
	{
		try
		{
			action();
		}
		catch (Exception ex)
		{
			// Protocol error (Overlay.highlightRect): Overlay must be enabled before a tool can be shown
			if (IsEx<MessageException>(ex) && ex.HResult == -2146233088)
				return;
			throw;
		}
	}

	private static bool IsEx<T>(Exception ex) => ex switch
	{
		T => true,
		AggregateException e when e.InnerExceptions.Any(f => f is T) => true,
		_ => false
	};
	
	private static void ShowR(this CDPSession client, R r, RGBA colFill, RGBA colStroke) =>
		client.Overlay_HighlightRect(r.X, r.Y, r.Width, r.Height, colFill, colStroke);
}