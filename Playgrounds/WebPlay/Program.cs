using PowWeb;

namespace WebPlay;

static class Program
{
	public static void Main()
	{
		var web = Web.Get(opt =>
		{
		
		});
	
		var url_1 = "https://motherless.com/u/Myhumantoilet?t=v";
		var url_2 = "https://motherless.com/C7EC50F";
	
		var globIdx = 0;
	
		web.Exec(null, www =>
		{
			var idx = globIdx++;
			Log($"[{idx}] start(url_1)");
			www.Goto(url_1);
			Log($"[{idx}] done(url_1)");
		});	
		Log("url_1 finished");

		Log("Press a key ...");
		Console.ReadKey();

		web.Exec(null, www =>
		{
			var idx = globIdx++;
			Log($"[{idx}] start(url_2)");
			www.Goto(url_2);
			Log($"[{idx}] done(url_2)");
		});
		Log("url_2 finished");
	}

	private static void Log(string str) => Console.WriteLine(str);
}











/*
using System.Text;
using Newtonsoft.Json;
using PowWeb;
using PowWeb._Internal.ChromeDevApi.DDebugger;
using PowWeb._Internal.ChromeDevApi.DDebugger.Events;
using PowWeb._Internal.ChromeDevApi.DFetch;
using PowWeb._Internal.ChromeDevApi.DFetch.Events;
using PowWeb._Internal.ChromeDevApi.DFetch.Structs;
using PowWeb._Internal.ChromeDevApi.Utils;
using PowWeb._Internal.Logic;
using PowWeb.Functionality.Web_Capture_Utils;
using PowWeb.Structs;
using PuppeteerSharp;
using RestSharp;

namespace WebPlay;

static class Program
{
	private static bool IsUrlWelcome(this string str) => Path.GetDirectoryName(str) == @"https:\welcome.adblockplus.org\en";
	private static bool IsUrlExtension(this string str) => Path.GetDirectoryName(str) == @"chrome-extension:\cfhdojbkjhnklbpkdaibdccddilifddb";

	public static async Task Main()
	{
		await GetVideo();
		//AntiDebugFetch();

		//AntiDebug();

		Log("Press a key to exit");
		Console.ReadKey();
	}

	private static async Task GetVideo()
	{
		var videoFile = @"C:\Dev\WebExtractor\_infos\script-hacking\video.mp4";
		var client = new RestClient();
		var headers = new Dictionary<string, string>
		{
			{ "Accept", "* /*" } !!,
			{ "Accept-Encoding", "identity;q=1, *;q=0" },
			{ "Range", "bytes=0-10000000" },
			{ "Referer", "https://javhub.net/play/DStPWSIsaSWQFu4AjdISfE0Khybf7M76evMbpBZ4T5U/bnmc-003-piss-queen-instructor-amber-uta" },
			{ "User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/105.0.0.0 Safari/537.36" },
			{ "sec-ch-ua", "\"Google Chrome\";v=\"105\", \"Not)A;Brand\";v=\"8\", \"Chromium\";v=\"105\"" },
			{ "sec-ch-ua-mobile", "?0\"" },
			{ "sec-ch-ua-platform", "\"Windows\"" },
		};
		var url = "https://p48.anyhentai.com/PDpQbVQNc39gc2k=.mp4?st=VI5s7ukkicHXNSHD4UShZw&e=1663929153";
		var req = new RestRequest(url);
		req.AddOrUpdateHeaders(headers);

		var res = await client.ExecuteAsync(req);
		var bytes = res.RawBytes;
		if (bytes != null)
		{
			await File.WriteAllBytesAsync(videoFile, bytes);
		}
	}

	private static void AntiDebugFetch()
	{
		var url = "https://javhub.net/play/DStPWSIsaSWQFu4AjdISfE0Khybf7M76evMbpBZ4T5U/bnmc-003-piss-queen-instructor-amber-uta";
		var web = Web.Get(opt => { opt.DeleteProfile = true; });
		var idx = 0;
		web.WhenFetchInterceptRequest().Subscribe(script =>
		{
			var scriptIdx = idx++;
			var lines = script.Body.SplitInLines();
			for (var i = 1; i < lines.Length; i++)
				lines[i] = "         " + lines[i];
			var body = string.Join(Environment.NewLine, lines.Take(15));
			Log($"Script [{scriptIdx}]");
			//Log($"  index: {script.Index}");
			Log($"  url  : {script.Url}");
			//Log($"  body : {body.Length}");
			//Log($"  dbg  : {body}");
			Log(" ");
		});
		WaitKey("Goto");
		//web.Goto(url);
		web.Page.GoToAsync(url);


		WaitKey("Exit");
	}

	private static void AntiDebug()
	{
		var url = "https://javhub.net/play/DStPWSIsaSWQFu4AjdISfE0Khybf7M76evMbpBZ4T5U/bnmc-003-piss-queen-instructor-amber-uta";
		//var url = "https://scat.gold/2020/06/02/liglee-liglee-poo-fetish/";
		var web = Web.Get(opt =>
		{
			opt.DeleteProfile = true;
		});

		var idx = 0;

		web.WhenPuppeteerScriptIntercepted()
			.Subscribe(script =>
			{
				var scriptIdx = idx++;
				var lines = script.Body.SplitInLines();
				for (var i = 1; i < lines.Length; i++)
					lines[i] = "         " + lines[i];
				var body = string.Join(Environment.NewLine, lines);
				Log($"Script [{scriptIdx}]");
				Log($"  index: {script.Index}");
				Log($"  url  : {script.Url}");
				Log($"  body : {body.Length}");
				Log($"  dbg  : {body.Contains("debugger", StringComparison.InvariantCultureIgnoreCase)}");
				Log(" ");
			});

		WaitKey("Goto");
		web.Goto(url);
		WaitKey("Exit");
	}

	private static string[] SplitInLines(this string? str) => str == null ? Array.Empty<string>() : str.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries).ToArray();

	private static void WaitKey(string msg)
	{
		Log($"Wait for a keypress: {msg}");
		Console.ReadKey();
	}

	private static void CheckMotherless()
	{
		var url = "https://motherless.com/u/Myhumantoilet?t=v";
		var web = Web.Get(opt =>
		{
			opt.AdBlockMode = AdBlockMode.Disabled;
			opt.DeleteProfile = true;
		});
		web.Goto(url);
		var browser = web.Browser;
	
		var pages = browser.PagesAsync();
		var targets = browser.Targets();

		var cap = web.Capture(opt =>
		{
			opt.ScreenCapType = ScreenCapType.None;
			opt.EnableFrameTreeMerging = true;
		});

		var root = cap.Root;
		var grps = root.GroupBy(e => e.V.Index).Where(e => e.Count() > 1).ToArray();

		var abc = 123;
	}


	private static void CheckLocalFramesTargets()
	{
		var web = Web.Get(opt =>
		{
			opt.AdBlockMode = AdBlockMode.Disabled;
			opt.DeleteProfile = true;
		});
		var browser = web.Browser;
	
		var pages = browser.PagesAsync();
		var targets = browser.Targets();

		var cap = web.Capture(opt => opt.ScreenCapType = ScreenCapType.None);

		var abc = 123;
	}

	private static void TryFetch()
	{
		var url = "https://javcl.com/";
		var web = Web.Get(opt =>
		{
			opt.DeleteProfile = true;
			opt.AdBlockMode = AdBlockMode.Disabled;
		});

		Log("READY");
		while (true)
		{
			var key = Console.ReadKey().Key;
			switch (key)
			{
				case ConsoleKey.L:
					web.LogPagesAndTargets();
					break;

				case ConsoleKey.T:
					TargetLogger.Log(web.Browser, WebOpt.Build(null), true);
					break;

				case ConsoleKey.G:
				{
					web.Goto(url);
					break;
				}
			}
		}


		return;

		var browser = web.Browser;
		var page = browser.PagesAsync().Result[0];
		var client = page.Client;

		string GetNfo(TargetInfo nfo) => nfo switch
		{
			null => "(null)",
			not null => $"[{nfo.Type}/{nfo.Url}]"
		};

		//browser.TargetCreated += (o, e) =>
		//{
		//	Log($"[TargetCreated]: {e.Target.Type} / {e.Target.Url}  {GetNfo(e.TargetInfo)}");
		//};
		//browser.TargetDestroyed += (o, e) =>
		//{
		//	Log($"[TargetDestroyed]: {e.Target.Type} / {e.Target.Url}  {GetNfo(e.TargetInfo)}");
		//};
		//browser.TargetChanged += (o, e) =>
		//{
		//	Log($"[TargetChanged]: {e.Target.Type} / {e.Target.Url}  {GetNfo(e.TargetInfo)}");
		//};

		var patterns = new RequestPattern[]
		{
			new RequestPattern(null, null, "Response")
		};

		var str = JsonConvert.SerializeObject(patterns, Formatting.Indented);

		client.Fetch_Enable(patterns);

		client.WhenEvent<RequestPausedEvent>()
			.Subscribe(evt =>
			{
				var req = evt.Request;

				if (evt.ResponseErrorReason != null || evt.ResponseStatusCode != null)
				{
					var resBody = client.Fetch_GetResponseBody(evt.RequestId);
					//Log($"Intercept lng:{resBody.Length} for '{req.Url}'");
				}

				client.Fetch_ContinueRequest(evt.RequestId);
			});

		Log("Press a key to start");
		Console.ReadKey();

		page.GoToAsync(url).Wait();
	}

	private static void TryDebugger()
	{
		var url = "https://javcl.com/";
		var breakFun = "document.createElement";

		url = "file:///C:/Dev/WebExtractor/_infos/pageshtml/basic/index.html";
		breakFun = "console.log";

		var web = Web.Get(opt =>
		{
			opt.DevTools = true;
			opt.AdBlockMode = AdBlockMode.Disabled;
		});
		var page = web.Browser.PagesAsync().Result[0];
		var client = page.Client;
		client.Debugger_Enable();

		Log("Press a key to start");
		Console.ReadKey(true);

		var breakId = web.BreakOnFunction(breakFun);
		Log($"BreakId: {breakId}");

		web.WhenEvent<PausedEvent>()
			.Subscribe(evt =>
			{
				var frames = evt.CallFrames;
				Log("Pause Event");
				Log($"Call stack ({frames.Length})");
				for (var i = 0; i < frames.Length; i++)
				{
					var frame = frames[i];
					var sb = new StringBuilder($"  [{i}]: ");
					sb.Append(frame.FunctionName);
					sb.Append(" @ ");
					sb.Append($"{frame.Location}");
					if (frame.FunctionLocation != null)
						sb.Append($" @Fun {frame.FunctionLocation}");
					sb.Append($" in {frame.This.ClassName} ({frame.This.Type}/{frame.This.SubType})");
					var str = sb.ToString();
					Log(str);
				}
				Log("");
			});

		var scriptIds = new List<string>();

		web.WhenEvent<ScriptParsedEvent>()
			.Subscribe(evt =>
			{
				Log($"ScriptParsed Event -> ScriptId={evt.ScriptId}");
				var scriptId = evt.ScriptId;
				scriptIds.Add(scriptId);

				var srcRec = client.Debugger_GetScriptSource(scriptId);
				var src = srcRec.ScriptSource;
				Log(src);

				Log("");
			});

		page.GoToAsync(url).Wait();

		Log("Press a key to get scripts");
		Console.ReadKey();

		foreach (var scriptId in scriptIds)
		{
			Log($"ScriptId:{scriptId}");
			var srcRec = client.Debugger_GetScriptSource(scriptId);
			var src = srcRec.ScriptSource;
			Log(src);
			Log("");
		}
	}

	private static void Log(string str) => Console.WriteLine(str);
}
*/