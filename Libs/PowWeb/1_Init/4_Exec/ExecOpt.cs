using PowWeb._1_Init._4_Exec.Structs;
using PowWeb._1_Init._4_Exec.Structs.Enums;

namespace PowWeb._1_Init._4_Exec;

public class ExecOpt
{
	public string Name { get; set; } = "(unnamed)";
	public int MaxRetryCount { get; set; } = 2;
	public Func<ExecErr, bool> DecideIfRetry { get; set; } = err => err.Loc != CodeLoc.UserCode || err.Ex is ArgumentException;
}