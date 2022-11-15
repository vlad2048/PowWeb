using System.Diagnostics;
using PowWeb._1_Init._3_Logging.Structs;

namespace PowWeb._1_Init._3_Logging.Loggers;

public class DebugOutputLogger : IPowWebLogger
{
	public void Log(Txt txt) => Debug.WriteLine(txt.Str);
}