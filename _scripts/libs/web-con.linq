<Query Kind="Program">
  <Reference>C:\Dev_Nuget\Libs\PowWeb\Libs\PowWeb\bin\Debug\net6.0-windows\PowWeb.dll</Reference>
  <Namespace>LINQPad.Controls</Namespace>
  <Namespace>System.Drawing</Namespace>
  <Namespace>PowWeb._1_Init._3_Logging</Namespace>
  <Namespace>PowWeb._1_Init._3_Logging.Structs</Namespace>
</Query>

void Main()
{
	WebCon.Init();
	WebCon.CallDump();
	void L(string str, Color col) => WebCon.Log(str, col);	
	L(@"First
Line again cont
", Light);
	L("sdfs", Blue);
}

public static readonly Color Default = Color.FromArgb(0x818181);
public static readonly Color Light = Color.FromArgb(0xFFFFFF);
public static readonly Color Dark = Color.FromArgb(0x494949);
public static readonly Color Mute = Color.DarkSlateGray;

public static readonly Color No = Color.FromArgb(0xFF431F);
public static readonly Color Yes = Color.FromArgb(0x7BFC42);

public static readonly Color Blue = Color.FromArgb(0x658AF8);
public static readonly Color BlueLight = Color.FromArgb(0x87C0F8);
public static readonly Color BlueDark = Color.FromArgb(0x1D75F3);

public static readonly Color Warn = Color.FromArgb(0xFDEA13);
public static readonly Color Special = Color.FromArgb(0x915FC0);

public static readonly Color FromLinqPad = Color.FromArgb(0xE650D2);




public class LinqPadLogger : IPowWebLogger
{
	public LinqPadLogger()
	{
		WebCon.Init();
	}
	
	public void CallDump() => WebCon.CallDump();
	
	public void Log(Txt txt) => WebCon.Log(txt.Str, txt.Col);
	
	public void Log(string str) => Log(new Txt(str, FromLinqPad));
}



public class WebConOpt
{
	private WebConOpt() {}
	public static WebConOpt Build(Action<WebConOpt>? optFun) { var opt = new WebConOpt(); optFun?.Invoke(opt); return opt; }
}

public static class WebCon
{
	private record Txt(string Str, Color Col) {
		public Span MakeSpan() {
			var span = new Span(Str);
			if (!colMap.TryGetValue(Col, out var colStr))
			{
				var colName = $"--col-{colMap.Count}";
				var rgbStr = $"#{Col.R:X2}{Col.G:X2}{Col.B:X2}";
				Util.HtmlHead.AddStyles($@":root {{ {colName}: {rgbStr}; }}");
				colStr = colMap[Col] = $"var({colName});";
			}
			span.Styles["color"] = colStr;
			return span;
		}
	}
	private static DumpContainer dc = null!;
	private static Dictionary<Color, string> colMap = null!;
	private static List<List<Txt>> data = null!;
	
	public static void Init(Action<WebConOpt>? optFun = null)
	{
		var opt = WebConOpt.Build(optFun);
		data = new List<List<Txt>> { new List<Txt>() };
		dc = new DumpContainer();
		colMap = new Dictionary<Color, string>();
	}
	
	public static void CallDump() => dc.Dump();
	
	public static void Log(string str, Color col)
	{
		var parts = str.Split(Environment.NewLine);
		for (var i = 0; i < parts.Length; i++)
		{
			var part = parts[i];
			if (part.Length > 0) LogSpanToData(new Txt(part, col));
			if (i < parts.Length - 1) LogNewlineToData();
		}
		UpdateDC();
	}
	
	private static void LogSpanToData(Txt txt) => data.Last().Add(txt);	
	private static void LogNewlineToData() => data.Add(new List<Txt>());
	
	private static void UpdateDC()
	{
		var rootDiv = MakeRootDiv();
		dc.UpdateContent(rootDiv);
	}
	
	private static Div MakeRootDiv()
	{
		var div = new Div(data.Select(MakeLineDiv));
		div.Styles["font-family"] = "Consolas";
		div.Styles["font-weight"] = "bold";
		div.Styles["font-size"] = "12px";
		div.Styles["background-color"] = "#000935";
		div.Styles["color"] = "#0FFC7E";
		div.Styles["padding"] = "5px";
		return div;
	}

	private static Div MakeLineDiv(List<Txt> line)
	{
		var div = new Div(line.Select(e => e.MakeSpan()));
		div.Styles["display"] = "flex";
		return div;
	}
}