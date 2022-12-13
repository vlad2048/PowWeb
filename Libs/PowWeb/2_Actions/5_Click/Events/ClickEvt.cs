using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using PowRxVar;

namespace PowWeb._2_Actions._5_Click.Events;

interface IClickEvtObs
{
	IObservable<Unit> WhenClickDone { get; }
	IObservable<Unit> WhenForgetAboutReversingTheScroll { get; }
}

interface IClickEvtSig
{
	void SignalClickDone();
	void SignalForgetAboutReversingTheScroll();
}

class ClickEvents : IClickEvtSig, IClickEvtObs, IDisposable
{
	private readonly Disp d = new();
	public void Dispose() => d.Dispose();

	private readonly ISubject<Unit> whenClickDone;
	public IObservable<Unit> WhenClickDone => whenClickDone.AsObservable();
	public void SignalClickDone()
	{
		whenClickDone.OnNext(Unit.Default);
		whenClickDone.OnCompleted();
	}

	private readonly ISubject<Unit> whenForgetAboutReversingTheScroll;
	public IObservable<Unit> WhenForgetAboutReversingTheScroll => whenForgetAboutReversingTheScroll.AsObservable();
	public void SignalForgetAboutReversingTheScroll()
	{
		whenForgetAboutReversingTheScroll.OnNext(Unit.Default);
		whenForgetAboutReversingTheScroll.OnCompleted();
	}




	private ClickEvents()
	{
		whenClickDone = new AsyncSubject<Unit>().D(d);
		whenForgetAboutReversingTheScroll = new AsyncSubject<Unit>().D(d);
	}

	public static (IClickEvtSig, IClickEvtObs, IDisposable) Make()
	{
		var evt = new ClickEvents();
		return (evt, evt, evt);
	}
}