using System.Reactive;
using System.Reactive.Linq;
using PowRxVar;
using PowWeb._1_Init._1_OptStructs;
using PowWeb._1_Init._1_OptStructs.Interfaces;
using PowWeb._1_Init._3_Logging;
using PowWeb._1_Init._4_Exec.Structs;
using PowWeb._1_Init._4_Exec.Structs.Enums;
using PowWeb._1_Init.Utils;
using PuppeteerSharp;

namespace PowWeb;

public class WebInst : IInitLogging, IDisposable
{
	internal Disp D { get; } = new();
	internal bool IsDisposed { get; private set; }
	public void Dispose() { if (IsDisposed) return; IsDisposed = true; D.Dispose(); }

	private readonly IWebState webState;
	//private readonly Page page;

	internal WebOpt Opt { get; }

	public IPowWebLogger Logger => Opt.Logger;

	public Browser Browser { get; }
	public string CurrentUrl { get; internal set; }

	public IObservable<Unit> WhenClosed { get; }
	public IObservable<Unit> WhenDisconnected { get; }
	public IObservable<TargetChangedArgs> WhenTargetCreated { get; }
	public IObservable<TargetChangedArgs> WhenTargetChanged { get; }
	public IObservable<TargetChangedArgs> WhenTargetDestroyed { get; }

	public IObservable<Unit> WhenFinished { get; }

	public Page GetPage() => GetPageUtils.GetPage(Browser, CurrentUrl, Opt);


	internal WebInst(Browser browser, WebOpt opt, IWebState webState)
	{
		Browser = browser.D(D);
		Opt = opt;
		this.webState = webState;
		CurrentUrl = browser.GetUrlOnStartup(Opt);

		WhenClosed = Observable.FromEventPattern(e => browser.Closed += e, e => browser.Closed -= e).ToUnit();
		WhenDisconnected = Observable.FromEventPattern(e => browser.Disconnected += e, e => browser.Disconnected -= e).ToUnit();
		WhenTargetCreated = Observable.FromEventPattern<TargetChangedArgs>(e => browser.TargetCreated += e, e => browser.TargetCreated -= e).Select(e => e.EventArgs);
		WhenTargetChanged = Observable.FromEventPattern<TargetChangedArgs>(e => browser.TargetChanged += e, e => browser.TargetChanged -= e).Select(e => e.EventArgs);
		WhenTargetDestroyed = Observable.FromEventPattern<TargetChangedArgs>(e => browser.TargetDestroyed += e, e => browser.TargetDestroyed -= e).Select(e => e.EventArgs);

		WhenFinished = Observable.Merge(WhenClosed, WhenDisconnected).Take(1);
	}

	private CodeLoc? curLoc;
	internal void SigStart(CodeLoc loc)
	{
		if (curLoc != null) throw new FatalException("PowWeb illegal function reentry");
		if (webState.CurCodeLoc != CodeLoc.UserCode) throw new FatalException("PowWeb illegal function reentry (not in usercode at the start of op)");
		if (loc == CodeLoc.UserCode) throw new FatalException("PowWeb illegal cannot signal entry to usercode");

		curLoc = loc;
		SetState(s => s.CurCodeLoc = loc);
	}

	internal void SigEnd()
	{
		if (curLoc == null) throw new FatalException("PowWeb illegal function reentry (at the end)");
		curLoc = null;
		SetState(s => s.CurCodeLoc = CodeLoc.UserCode);
	}

	private void SetState(Action<IWebState> stateFun)
	{
		if (IsDisposed) return;
		stateFun(webState);
	}
}