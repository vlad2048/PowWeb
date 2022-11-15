using PowWeb._1_Init._4_Exec.Structs;

namespace PowWeb._1_Init._4_Exec.Events;

public enum ExecEvtType
{
	Start,
	End,
	Error
}

public class ExecEvt
{
	public ExecEvtType Type { get; }
	public string Name { get; }
	public ExecErrNfo? Err { get; }

	public ExecEvt(ExecEvtType type, string name, ExecErrNfo? err)
	{
		Type = type;
		Name = name;
		Err = err;
	}
}