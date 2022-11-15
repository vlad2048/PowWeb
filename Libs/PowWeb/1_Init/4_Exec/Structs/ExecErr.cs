using PowWeb._1_Init._4_Exec.Structs.Enums;

namespace PowWeb._1_Init._4_Exec.Structs;

public record ExecErr(
	CodeLoc Loc,
	ErrType Type,
	Exception Ex,
	int TryIdx
)
{
	public override string ToString() => $"[@{Loc} of type {Type} try:{TryIdx}]: {Ex}";
}

public class ExecErrNfo
{
	private readonly ExecOpt opt;

	public ExecErr Err { get; }
	public bool Retriable => Err.Ex is not FatalException && opt.DecideIfRetry(Err);
	public bool WillRetry => Retriable && Err.TryIdx < opt.MaxRetryCount - 1;

	public override string ToString() => $"Retriable:{Retriable} WillRetry:{WillRetry} Err:{Err}";

	public ExecErrNfo(
		ExecErr err,
		ExecOpt opt
	)
	{
		this.opt = opt;
		Err = err;
	}
}