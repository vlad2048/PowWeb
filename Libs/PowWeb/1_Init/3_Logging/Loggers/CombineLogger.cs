using PowWeb._1_Init._3_Logging.Structs;

namespace PowWeb._1_Init._3_Logging.Loggers;

public class CombineLogger : IPowWebLogger
{
	private readonly IPowWebLogger[] loggers;

	public CombineLogger(params IPowWebLogger[] loggers)
	{
		this.loggers = loggers;
	}

	public void Log(Txt txt)
	{
		foreach (var logger in loggers)
			logger.Log(txt);
	}
}