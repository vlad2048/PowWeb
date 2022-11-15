using PowWeb._1_Init._1_OptStructs;
using PowWeb._1_Init._1_OptStructs.Enums;
using PowWeb._1_Init._2_OptExts;
using PowWeb._1_Init.Logic;
using PowWeb._1_Init.Utils;
using PuppeteerSharp;

namespace PowWeb._1_Init;

static class WebGetLogic
{
	public static WebInst Get(WebOpt opt, IWebState state)
	{
		var inst = opt.OpenMode switch
		{
			OpenMode.Create => Create(opt, state),
			OpenMode.Connect => Connect(opt, state),
			OpenMode.ConnectOrCreate => ProcessExt.IsUp() switch
			{
				true => Connect(opt, state),
				false => Create(opt, state)
			},
			_ => throw new ArgumentException()
		};
		opt.LogLine(" -> Init done", Cols.Light);
		return inst;
	}


	private static WebInst Create(WebOpt opt, IWebState state)
	{
		opt.LogTitle("Web.Get", "Create");
		opt.KillIfUp();
		using var fetcher = new BrowserFetcher(new BrowserFetcherOptions {
			Path = opt.DownloadFolder(),
			CustomFileDownload = (srcUrl, dstFile) => {
				opt.DownloadChromiumIfNotCached(srcUrl, dstFile);
				return Task.CompletedTask;
			}
		});

		opt.LogOp("Download ... ");
		fetcher.DownloadAsync().Wait();
		opt.LogOpDone();

		opt.EmptyProfileIfNeeded();

		var args = new List<string>
		{
			$@"--user-data-dir=""{opt.ProfileFolder()}""",
			$@"--disable-web-security" ,
			$@"--disable-features=IsolateOrigins,site-per-process",
			$@"--window-position={opt.WinLoc.X},{opt.WinLoc.Y}",
			$@"--remote-debugging-port={opt.DebugPort}",
			$@"--disable-site-isolation-trials",	// otherwise Puppeteer cannot listen to requests made in iFrames when not running in headless mode
		};

		if (opt.AdBlockMode != AdBlockMode.Disabled) {
			args.Add($@"--disable-extensions-except=""{opt.GetAdblockPlusExtensionFolder()}""");
			args.Add($@"--load-extension=""{opt.GetAdblockPlusExtensionFolder()}""");
		}

		opt.LogOp("Launch ... ");
		var browser = Puppeteer.LaunchAsync(
			new LaunchOptions {
				Headless = opt.Headless,
				Devtools = opt.DevTools,
				ExecutablePath = opt.ChromeExe,
				Args = args.ToArray(),
			}
		).Result;
		opt.LogOpDone();

		//TargetLogger.Log(browser, opt, false);
		//AdblockInitialTabClosingLogic.Close(browser, opt);

		if (opt.AdBlockMode == AdBlockMode.EnabledCloseTabOnStartup)
		{
			using var adblockWaiter = new AdblockPlusWaiter(browser, opt);
			adblockWaiter.Wait();
		}

		return new WebInst(browser, opt, state);
	}

	private static WebInst Connect(WebOpt opt, IWebState state)
	{
		opt.LogTitle("Web.Get", "Connect");

		if (!ProcessExt.IsUp())
			throw new InvalidOperationException("Could not find an instance of Puppeteer to connect to");

		opt.LogOp("Connect ... ");
		var browser = Puppeteer.ConnectAsync(
			new ConnectOptions {
				BrowserURL = $"ws://127.0.0.1:{opt.DebugPort}",
			}
		).Result;
		opt.LogOpDone();

		opt.BringToTop();

		return new WebInst(browser, opt, state);
	}
}