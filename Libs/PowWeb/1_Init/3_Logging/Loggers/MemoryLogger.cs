using System.Reactive.Linq;
using System.Reactive.Subjects;
using PowWeb._1_Init._3_Logging.Structs;
using PowWeb.ChromeApi.Utils.Extensions;

namespace PowWeb._1_Init._3_Logging.Loggers;

public record LogData(Txt[][] Data)
{
	public static LogData FromList(List<List<Txt>> list) => new(list.SelectToArray(e => e.ToArray()));
}

public class MemoryLogger : IPowWebLogger
{
	private readonly List<List<Txt>> data = new() { new List<Txt>() };
	private readonly HashSet<Color> knownColors = new();
	private readonly ISubject<LogData> whenChanged = new Subject<LogData>();
	private readonly ISubject<Color> whenColorAdded = new Subject<Color>();
	
	public LogData Data => LogData.FromList(data);
	public IObservable<LogData> WhenChanged => whenChanged.AsObservable();
	public IObservable<Color> WhenColorAdded => whenColorAdded.AsObservable();

	public void Log(Txt txt)
	{
		var (str, col) = txt;
		if (!knownColors.Contains(col))
		{
			knownColors.Add(col);
			whenColorAdded.OnNext(col);
		}
		var parts = str.Split(Environment.NewLine);
		for (var i = 0; i < parts.Length; i++)
		{
			var part = parts[i];
			if (part.Length > 0) AddSpan(new Txt(part, col));
			if (i < parts.Length - 1) AddNewline();
		}
		whenChanged.OnNext(Data);
	}

	private void AddSpan(Txt txt) => data.Last().Add(txt);
	private void AddNewline() => data.Add(new List<Txt>());
}