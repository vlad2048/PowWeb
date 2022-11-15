using PowWeb._1_Init._3_Logging.Structs;
using PowWeb._1_Init.Utils;

namespace PowWeb._1_Init._3_Logging.Loggers;

public class ConLogger : IPowWebLogger
{
	public void Log(Txt txt) => ConUtils.Write(txt);
}