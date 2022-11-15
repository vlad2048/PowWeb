using PowWeb._1_Init._1_OptStructs.Interfaces;
using PowWeb._1_Init._4_Exec;
using PowWeb._1_Init._4_Exec.Structs;

namespace PowWeb._1_Init._2_OptExts;

static class LoggingErrExt
{
	private static readonly Color colLoc = Color.FromArgb(0xFC594A);
	private static readonly Color colDeco = Color.FromArgb(0x3d5875);
	private static readonly Color colType = Color.FromArgb(0xFEB68E);
	private static readonly Color colRetry = Color.FromArgb(0x7cad53);
	private static readonly Color colOutOfTries = Color.FromArgb(0xed574c);
	private static readonly Color colFatal = Color.FromArgb(0xde52d7);
	private static readonly Color colExType = Color.FromArgb(0xdb1aa8);
	private static readonly Color colMsg = Color.FromArgb(0xd55de8);
	private static readonly Color colStack = Color.FromArgb(0x2f80d6);
	private static Color colTries(RetryState state) => state == RetryState.RanOutOfTries ? Color.FromArgb(0xFE1818) : Color.FromArgb(0xFEF74E);


	private enum RetryState
	{
		Retrying,
		RanOutOfTries,
		FatalError
	}

	private record Nfo(IInitLogging log, ExecErr err, int tryIdx, ExecOpt opt)
	{
		public RetryState RetryState => opt.DecideIfRetry(err) switch
		{
			true => (tryIdx < opt.MaxRetryCount - 1) switch
			{
				true => RetryState.Retrying,
				false => RetryState.RanOutOfTries,
			},
			false => RetryState.FatalError,
		};
	}


	public static void LogErr(this IInitLogging log, ExecErr err, int tryIdx, ExecOpt opt)
	{
		var nfo = new Nfo(log, err, tryIdx, opt);
		Write_AdvancedInfo(nfo);
		Write_ExceptionTypeMessage(nfo);
		Write_RetryResult(nfo);
		Write_StackTrace(nfo);
	}



	private static void Write_AdvancedInfo(Nfo nfo)
	{
		var (log, err, tryIdx, opt) = nfo;
		log.LogNewLine();
		log.Log("[@", colDeco);
		log.Log($"{err.Loc}", colLoc);
		log.Log(" of type ", colDeco);
		log.Log($"{err.Type}", colType);
		log.Log("]     ", colDeco);
		log.LogLine($"tries:{tryIdx + 1}/{opt.MaxRetryCount}", colTries(nfo.RetryState));
	}

	private static void Write_ExceptionTypeMessage(Nfo nfo)
	{
		var (log, err, tryIdx, opt) = nfo;
		log.LogNewLine();
		log.Log($"[{err.Ex.GetType().Name}] - ", colExType);
		log.LogLine($"{err.Ex.Message}", colMsg);
	}

	private static void Write_RetryResult(Nfo nfo)
	{
		var (log, err, tryIdx, opt) = nfo;
		log.LogNewLine();
		switch (nfo.RetryState)
		{
			case RetryState.Retrying:
				log.Log(" => Retrying", colRetry);
				break;

			case RetryState.RanOutOfTries:
				log.Log(" => Ran out of tries", colOutOfTries);
				break;

			case RetryState.FatalError:
				log.Log(" => Fatal (cannot retry)", colFatal);
				break;

			default:
				throw new ArgumentException();
		}
	}

	private static void Write_StackTrace(Nfo nfo)
	{
		var (log, err, tryIdx, opt) = nfo;

		log.LogNewLine();
		var stackLines = (err.Ex.StackTrace ?? string.Empty).SplitInLines()
			.Where(e => e.Contains(":line "))
			.Select(e =>
			{
				var idx = e.IndexOf(@"C:\", StringComparison.Ordinal);
				return (idx != -1) switch
				{
					true => e[idx..],
					false => e
				};
			})
			.ToArray();

		foreach (var stackLine in stackLines)
			log.LogLine($"    {stackLine}", colStack);


		log.LogNewLine();
	}


	private static List<string> SplitInLines(this string? str) => str == null ? new List<string>() : str.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries).ToList();
}