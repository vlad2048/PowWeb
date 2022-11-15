<Query Kind="Program">
  <Reference>C:\Dev_Nuget\Libs\PowWeb\Libs\PowWeb\bin\Debug\net6.0-windows\PowWeb.dll</Reference>
  <Namespace>PowTrees.Algorithms</Namespace>
  <Namespace>PowWeb</Namespace>
  <Namespace>PowWeb._1_Init._1_OptStructs</Namespace>
  <Namespace>PowWeb._1_Init._3_Logging</Namespace>
  <Namespace>PowWeb._1_Init._3_Logging.Loggers</Namespace>
  <Namespace>PowWeb._1_Init._3_Logging.Structs</Namespace>
  <Namespace>PowWeb._2_Actions._2_Cap.Ser</Namespace>
</Query>

#load "..\libs\web-con"

const string OutFolder = @"C:\temp\PowWebTests";
string Mk(string name) => Path.Combine(OutFolder, name);

const string UrlBatch = "https://motherless.com/u/Myhumantoilet?t=v";
const string UrlVideoPage = "https://motherless.com/C7EC50F";
const string UrlVideoPageIframe = "https://hpjav.tv/168216/sun-008";
const string UrlVideoLink = "https://cdn5-videos.motherlessmedia.com/videos/C7EC50F.mp4";


void Main()
{
	//Test_Goto();
	Test_Cap();
	//Test_Screenshot();
	//Test_WhenRequest();
}

void OptFun(WebOpt opt)
{
	opt.Logger = new CombineLogger(
		new LinqPadLogger(),
		new HtmlLogger(@"C:\temp\html-nice\output.html")
	);
}




void Test_Goto()
{
	var web = Web.Get(OptFun);
	web.Exec(www => www.Goto(UrlBatch));
}

void Test_Cap()
{
	var web = Web.Get(OptFun);
	web.Exec(www =>
	{
		www.Goto(UrlVideoPageIframe);

		p_start("Cap");         				// 162ms
		var cap = www.Cap();
		p_end();
		//PowWebJsonUtils.Save(Mk("cap.json"), cap);

		p_start("DeepCap");     				// 393ms
		var deepCap = www.Cap(opt =>
		{
			opt.Deep = true;
			opt.LogFolder = @"C:\temp\PowWebTests";
		});
		p_end();
		//PowWebJsonUtils.Save(Mk("deepCap.json"), deepCap);
	});
}

void Test_Screenshot()
{
	var web = Web.Get(OptFun);
	web.Exec(www =>
	{
		www.Goto(UrlBatch);

		p_start("Screen.None");					//  14ms
		var scrNone = www.Screenshot(ScreenshotMethod.None).ToBmp();
		p_end();
		scrNone.Save(Mk("scr-none.jpg"));

		p_start("Screen.Builtin");				// 746ms
		var scrBuiltin = www.Screenshot(ScreenshotMethod.Builtin).ToBmp();
		p_end();
		scrBuiltin.Save(Mk("scr-builtin.jpg"));

		p_start("Screen.ScrollAndAssemble");	// 3765ms
		var scrScroll = www.Screenshot(ScreenshotMethod.ScrollAndAssemble).ToBmp();
		p_end();
		scrScroll.Save(Mk("scr-scroll.jpg"));
	});
}

void Test_WhenRequest()
{
	var web = Web.Get(OptFun);
	web.Exec(www =>
	{
		www.WhenRequest().Subscribe(req => $"[REQ - {req.ResourceType}] '{req.Url}'".Dump());
		www.Goto(UrlBatch);
	});
}





string? profCat;
Stopwatch profWatch = null!;
public void p_start(string cat)
{
	if (profCat != null) throw new ArgumentException();
	profCat = cat;
	profWatch = Stopwatch.StartNew();
}
public void p_end()
{
	if (profCat == null) throw new ArgumentException();
	var msg = $"[{profCat}]: {profWatch.ElapsedMilliseconds}ms";
	profCat = null;
	msg.Dump();
}