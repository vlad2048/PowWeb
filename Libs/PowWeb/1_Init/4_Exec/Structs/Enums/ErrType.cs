using PuppeteerSharp;

namespace PowWeb._1_Init._4_Exec.Structs.Enums;

public enum ErrType
{
	Other,
	TargetClosed,
	Timeout,
	NetErrAborted
}

static class ErrTypeIdentifier
{
	public static ErrType Identify(Exception ex)
	{
		if (ex is not AggregateException aggEx) return ErrType.Other;
		if (aggEx.InnerExceptions.Count != 1) return ErrType.Other;
		var aggExInner = aggEx.InnerExceptions[0];
		if (aggExInner is not NavigationException navigationEx) return ErrType.Other;
		return navigationEx.InnerException switch
		{
			TargetClosedException => ErrType.TargetClosed,
			TimeoutException => ErrType.Timeout,
			NavigationException navEx when navEx.Message.StartsWith("net::ERR_ABORTED") => ErrType.NetErrAborted,
			_ => ErrType.Other
		};
	}
}