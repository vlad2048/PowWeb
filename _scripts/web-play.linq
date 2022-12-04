<Query Kind="Program">
  <Reference>C:\Dev_Nuget\Libs\PowWeb\Libs\PowWeb\bin\Debug\net7.0-windows\PowWeb.dll</Reference>
  <NuGetReference>PowBasics</NuGetReference>
  <Namespace>DynamicData</Namespace>
  <Namespace>LINQPad.Controls</Namespace>
  <Namespace>Newtonsoft.Json.Linq</Namespace>
  <Namespace>PowBasics.Geom</Namespace>
  <Namespace>PowBasics.StringsExt</Namespace>
  <Namespace>PowTrees.Algorithms</Namespace>
  <Namespace>PowWeb</Namespace>
  <Namespace>PowWeb._1_Init._1_OptStructs.Enums</Namespace>
  <Namespace>PowWeb._1_Init._3_Logging.Loggers</Namespace>
  <Namespace>PowWeb.ChromeApi.DDom</Namespace>
  <Namespace>PowWeb.ChromeApi.DDomSnapshot</Namespace>
  <Namespace>PowWeb.ChromeApi.DOverlay</Namespace>
  <Namespace>PowWeb.ChromeApi.Utils</Namespace>
  <Namespace>PuppeteerSharp</Namespace>
</Query>

#load ".\libs\web-con"
public static Web web = null!;
public static DumpContainer logDC = null!;
public static LinqPadLogger logger = null!;

void Main()
{
	var url = "https://motherless.com/u/Myhumantoilet?t=v";

	logger = new LinqPadLogger();
	
	web = Web.Get(opt =>
	{
		opt.OpenMode = OpenMode.ConnectOrCreate;
		opt.DeleteProfile = false;
		opt.AdBlockMode = AdBlockMode.Disabled;
		opt.Logger = logger;
	});
	
	web.Exec(null, www =>
	{
		www.Goto(url);
		var page = www.GetPage();
		var client = page.Client;
		
		client.Dom_Enable();
		client.Overlay_Enable();
		page.SetScroll(new Pt(0, 150));
		client.Overlay_HighlightRect(120, 70, 250, 130, new(245, 102, 66, 1.0), new(66, 245, 129, 1.0));
		
	});
	
	/*Util.HorizontalRun(true,
		Btn("Log", Lg),
		Btn("Go", www =>
		{
			www.Goto(url);
			//Lg(www);
		}),
		Btn("Click", www =>
		{
			www.Click(pt, opt =>
			{
				opt.DebugDelayBeforeClosingExtraTabs = TimeSpan.FromSeconds(5);
			});
			//Lg(www);
		}),
		Btn("Fix", www =>
		{
			Fix(www);
		})
	).Dump();*/
	
	logger.CallDump();
	
	logDC = new DumpContainer().Dump();
}

static class PageExt
{
	public static void SetScroll(this Page page, Pt pt)
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

/*Button Btn(string name, Action<WebInst> action) => new Button(name, b =>
{
	b.Styles["background-color"] = "#FEA5A0";
	web.Exec(action);
	b.Styles["background-color"] = "#EFEFEF";
});

void Fix(WebInst www)
{
	var pages = www.Browser.PagesAsync().Result;
	var page = pages.FirstOrDefault(e => e.Url == web.CurrentUrl);
	if (page == null)
	{
		logger.Log("Could not find page\n");
		return;
	}
	var pagesToClose = pages.Where(e => e != page).ToArray();
	if (pagesToClose.Length > 0)
	{
		logger.Log($"Closing {pagesToClose.Length} pages ... ");
		foreach (var p in pagesToClose)
			p.CloseAsync().Wait();
		logger.Log("Done\n");
	}
}


void Jump(WebInst www)
{
	var pages = www.Browser.PagesAsync().Result;
	var page = pages.Single(e => e.Url == "about:blank");
	page.BringToFrontAsync().Wait();
}


void Lg(WebInst www)
{
	var browser = www.Browser;
	var pages = browser.PagesAsync().Result;
	var visPage = pages.GetVisiblePage();
	logDC.UpdateContent(
		Util.VerticalRun(
			pages
				.Select(e => new
				{
					Visible = e == visPage ? "active" : "",
					Url = e.Url.FmtUrl(),
					Target = e.Target.TargetId,
					Frames = Utils.GetFrameTreeDiv(e),
				}),
			browser.Targets().Select(e =>
			{
				var tp = e.PageAsync().Result;
				var (tpIdx, tpUrl) = tp switch
				{
					null => (-1, "_"),
					not null => (pages.IndexOf(tp), tp.Url)
				};
				return new
				{
					Target = e.TargetId,
					Type = e.Type,
					Url = e.Url.FmtUrl(),
					PageIdx = tpIdx,
					PageUrl = tpUrl.FmtUrl(),
				};
			})
		)
	);
}


public static class Utils
{
	public static Page? GetVisiblePage(this Page[] pages)
	{
		var visPages = pages.Where(IsPageVisible).ToArray();
		return visPages.Length switch
		{
			0 => null,
			1 => visPages[0],
			_ => throw new ArgumentException()
		};
	}
	
	private static bool IsPageVisible(Page page)
	{
		var stateJValue = page.EvaluateExpressionAsync("document.visibilityState").Result as JValue;
		var state = stateJValue!.Value as string;
		return state == "visible";
	}
	
	public static Div GetFrameTreeDiv(Page page)
	{
		var tree = GetFrameTree(page).Map(e => e.Url.Truncate(32));
		var str = tree.LogToString();
		str = $"{tree.Count()} frame\n{str}";
		var span = new Span(str);
		var div = new Div(span);
		div.Styles["font-family"] = "Consolas";
		div.Styles["font-weight"] = "bold";
		div.Styles["font-size"] = "12px";
		div.Styles["background-color"] = "#000935";
		div.Styles["color"] = "#0FFC7E";
		div.Styles["padding"] = "5px";
		div.Styles["white-space"] = "no-wrap";
		div.Styles["overflow-x"] = "auto";
		return div;
	}
	private static TNod<Frame> GetFrameTree(Page page)
	{
		var root = Nod.Make(page.MainFrame);

		void AddChildren(TNod<Frame> nod)
		{
			foreach (var childFrame in nod.V.ChildFrames)
			{
				var childNod = Nod.Make(childFrame);
				nod.AddChild(childNod);
				AddChildren(childNod);
			}
		}

		AddChildren(root);

		var actCnt = page.Frames.Length;
		var expCnt = root.Count();
		if (expCnt != actCnt)
		{
			// this can happen but it should only be temporary (short)
			Console.WriteLine($"WRONG FRAME NUMBER (tree:{expCnt}   list:{actCnt})");
		}

		return root;
	}
}



public static class Ext
{
	public static U[] SelectToArray<T, U>(this IEnumerable<T> source, Func<T, U> fun) => source.Select(fun).ToArray();
	public static string Quote(this string s) => $"'{s}'";
	
	public static void Exec(this Web web, Action<WebInst> execFun) => web.Exec(null, execFun);
	
	public static string FmtUrl(this string? url) => string.IsNullOrWhiteSpace(url) switch
	{
		true => "_",
		false => url.Truncate(32)
	};
}*/