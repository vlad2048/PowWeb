using PowWeb._1_Init._3_Logging.Structs;
using PowWeb._1_Init.Utils;
using L = PowWeb._1_Init._1_OptStructs.Interfaces.IInitLogging;

namespace PowWeb._1_Init._2_OptExts;

static class LoggingExt
{
	private static void Log(this L log, Txt t) => log.Logger.Log(t);


	public static void LogTitle(this L log, string title, string? txtOpt = null)
	{
		const char c = '*';
		var pad = new string(c, title.Length + txtOpt.GetLng(3) + 4);

		log.LogLine(pad, Cols.Mute);
		log.Log($"{c} ", Cols.Mute);

		log.Log(title, Cols.Light);
		if (txtOpt != null)
			log.Log($" ({txtOpt})", Cols.Dark);
		log.Log($" {c}", Cols.Mute);
		log.LogNewLine();

		log.LogLine(pad, Cols.Mute);
	}

	public static void LogOpLine(this L log, string str) => log.Log($"  {str}", Cols.Dark);
	public static void LogOp(this L log, string str) => log.Log($"  {str} ... ", Cols.Dark);
	public static void LogOpDone(this L log) => log.LogLine("done", Cols.Default);

	public static void LogWarnLine(this L log, string str) => log.Log($"  {str}", Cols.Warn);


	public static void Log(this L log, string s, Color c) => log.Log(new Txt(s, c));
	public static void LogLine(this L log, string s, Color c) => log.LogLine(new Txt(s, c));

	public static void LogNewLine(this L log) => log.LogLine(string.Empty, Cols.Default);


	private static void LogLine(this L log, Txt t) => log.Log(t with { Str = t.Str + Environment.NewLine });


	private static int GetLng(this string? s, int plusIf) => s switch
	{
		null => 0,
		not null => s.Length + plusIf
	};
}