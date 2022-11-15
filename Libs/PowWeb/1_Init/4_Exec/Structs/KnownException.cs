using PowWeb._1_Init._4_Exec.Structs.Enums;

namespace PowWeb._1_Init._4_Exec.Structs;

class KnownException : Exception
{
	public ErrType ErrType { get; }

	public KnownException(ErrType errType)
	{
		ErrType = errType;
	}
}