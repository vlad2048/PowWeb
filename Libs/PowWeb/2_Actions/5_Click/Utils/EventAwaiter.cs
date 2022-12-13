/*
namespace PowWeb._2_Actions._5_Click.Utils;

class EventAwaiter
{
	private bool isDisposed;
	private readonly ManualResetEventSlim slim = new();

	public void Listen(object? sender, EventArgs eventArgs) => slim.Set();

	public bool Wait(TimeSpan timeout)
	{
		if (isDisposed)
			throw new ObjectDisposedException(nameof(EventAwaiter));
		var isOk = slim.Wait(timeout);
		isDisposed = true;
		return isOk;
	}
}
*/