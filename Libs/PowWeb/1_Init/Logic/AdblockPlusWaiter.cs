using System.Reactive.Linq;
using System.Reactive.Subjects;
using PowRxVar;
using PowWeb._1_Init._1_OptStructs;
using PowWeb._1_Init._2_OptExts;
using PowWeb._1_Init.Utils;
using PowWeb._1_Init.Utils.Exts;
using PuppeteerSharp;

namespace PowWeb._1_Init.Logic;

/*
 * UrlWelcome   = @"https:\welcome.adblockplus.org\en"
 * UrlExtension = @"chrome-extension:\cfhdojbkjhnklbpkdaibdccddilifddb"
 *
 * Steps:
 * - Wait for Page[UrlWelcome]
 * - Close Page[UrlWelcome]
 * - Wait for CloseEvent Page[UrlWelcome]
 *
 * - Wait for Page [UrlExtension]   It might never come, but if it does then:
 * - Close Page[UrlExtension]
 * - Wait for CloseEvent Page[UrlExtension]
 */

class AdblockPlusWaiter : IDisposable
{
	private readonly Disp d = new();
	public void Dispose() => d.Dispose();

	private const string UrlWelcome = @"https:\welcome.adblockplus.org\en";
	private const string UrlExtension = @"chrome-extension:\cfhdojbkjhnklbpkdaibdccddilifddb";
	private static readonly TimeSpan timeout = TimeSpan.FromSeconds(7);
	private static readonly TimeSpan pageExtensionTimeout = TimeSpan.FromMilliseconds(5000);
	private static readonly TimeSpan pollInterval = TimeSpan.FromMilliseconds(500);
	private static bool IsAdblockRunning(Browser browser) => browser.Targets().Any(e => e.Url.FmtUrlSimple() == UrlExtension);

	private enum State
	{
		WaitingForPageWelcome,
		WaitingForPageExtension,
	}

	private enum Transition
	{
		PageWelcomeClosed,
		PageExtensionClosed,
	}

	private readonly WebOpt opt;
	private readonly ManualResetEventSlim slim = new();
	private readonly ISubject<Transition> whenTransition = new Subject<Transition>();
	private IObservable<Transition> WhenTransition => whenTransition.AsObservable();
	private string finishStr = "(none)";

	public AdblockPlusWaiter(Browser browser, WebOpt opt)
	{
		this.opt = opt;
		if (!IsAdblockRunning(browser))
		{
			opt.LogOpLine("AdblockPlus not running");
			return;
		}
		opt.LogOpLine("AdblockPlus running");

		browser.TargetDestroyed += HandlerTargetDestroyed;
		var isFinished = false;
		void Finish(string str)
		{
			isFinished = true;
			finishStr = str;
			browser.TargetDestroyed -= HandlerTargetDestroyed;
			slim.Set();
		}

		var state = Var.Make(State.WaitingForPageWelcome).D(d);

		//state.Subscribe(_state => opt.Log($"STATE <- {_state}")).D(d);

		WhenTransition
			.Where(e => e == Transition.PageWelcomeClosed)
			.Subscribe(_ =>
			{
				if (state.V != State.WaitingForPageWelcome) throw new ArgumentException();
				state.V = State.WaitingForPageExtension;
			}).D(d);

		WhenTransition
			.Where(e => e == Transition.PageExtensionClosed)
			.Subscribe(_ =>
			{
				Finish("Done (closed 2nd page too)");
			}).D(d);

		state
			.Where(e => e == State.WaitingForPageExtension)
			.Subscribe(_ =>
			{
				Observable.Timer(pageExtensionTimeout).Subscribe(_ =>
				{
					Finish("Done (2nd page didn't come)");
				});
			}).D(d);

		Observable.Timer(timeout)
			.Subscribe(_ =>
			{
				Finish("Timeout");
			}).D(d);

		var whenPagesReady = new Subject<Page[]>();
		IObservable<Page[]> WhenPagesReady = whenPagesReady.AsObservable();

		Observable.Interval(pollInterval)
			.Subscribe(_ =>
			{
				var pages = browser.GetPages(opt);
				whenPagesReady.OnNext(pages);
			}).D(d);

		WhenPagesReady
			.Subscribe(pages =>
			{
				if (isFinished) return;
				var urlToClose = state.V switch
				{
					State.WaitingForPageWelcome => UrlWelcome,
					State.WaitingForPageExtension => UrlExtension,
					_ => throw new ArgumentException()
				};

				var targets = browser.Targets();

				void Close(string url)
				{
					var page = pages.FirstOrDefault(page => page.Url.FmtUrlSimple() == url);
					var target = targets.FirstOrDefault(target => target.Url.FmtUrlSimple() == url);
					if (page != null && target != null)
					{
						//opt.Log($"(trying to close '{url}' -> YES)");
						page.CloseAsync();
					}
					else
					{
						//opt.Log($"(trying to close '{url}' -> NO)");
					}
				}

				Close(urlToClose);
			}).D(d);
	}


	public void Wait()
	{
		slim.Wait();
		opt.LogOpLine($"AdblockPlus tab -> {finishStr}");
	}

	private void HandlerTargetDestroyed(object? o, TargetChangedArgs e)
	{
		var target = e.Target;
		var url = target.Url.FmtUrlSimple();
		if (url == UrlWelcome) whenTransition.OnNext(Transition.PageWelcomeClosed);
		if (url == UrlExtension) whenTransition.OnNext(Transition.PageExtensionClosed);
	}

}