using System.Reactive.Linq;
using System.Reactive.Subjects;
using PowRxVar;
using PowWeb._1_Init;
using PowWeb._1_Init._1_OptStructs;
using PowWeb._1_Init._4_Exec;
using PowWeb._1_Init._4_Exec.Events;
using PowWeb._1_Init._4_Exec.Structs;
using PowWeb._1_Init._4_Exec.Structs.Enums;

namespace PowWeb;

public interface IWebState
{
	CodeLoc CurCodeLoc { get; set; }
}

public class Web : IDisposable, IWebState
{
	private static readonly TimeSpan InvalidatePause = TimeSpan.FromSeconds(5);

	private readonly Disp d = new();
	public void Dispose() => d.Dispose();

	private readonly WebOpt opt;
	private readonly SerialDisp<WebInst> serDInst;
	private readonly ISubject<ExecEvt> whenExecEvt = new Subject<ExecEvt>();

	internal WebInst WebInst => serDInst.Value ??= CreateNewWebInst();
	private void InvalidateInst()
	{
		serDInst.Value = null;
		Thread.Sleep(InvalidatePause);
	}

	private WebInst CreateNewWebInst()
	{
		var inst = WebGetLogic.Get(opt, this);
		inst.WhenFinished.Subscribe(_ =>
		{
			InvalidateInst();
		}).D(inst.D);
		return inst;
	}


	private Web(WebOpt opt)
	{
		this.opt = opt;
		serDInst = new SerialDisp<WebInst>().D(d);
	}

	// IWebState
	// ---------
	public CodeLoc CurCodeLoc { get; set; } = CodeLoc.UserCode;


	// Get
	// ---
	public static Web Get(Action<WebOpt>? optFun = null) => new(WebOpt.Build(optFun));


	// Exec
	// ----
	public IObservable<ExecEvt> WhenExecEvt => whenExecEvt.AsObservable();

	public async Task<T> Exec<T>(ExecOpt? execOpt, Func<WebInst, Disp, Task<T>> execFun)
	{
		execOpt ??= new ExecOpt();

		whenExecEvt.OnNext(new ExecEvt(ExecEvtType.Start, execOpt.Name, null));

		for (var tryIdx = 0; tryIdx < execOpt.MaxRetryCount; tryIdx++)
		{
			if (CurCodeLoc != CodeLoc.UserCode) throw new ArgumentException("Impossible");

			try
			{
				using var execD = new Disp();
				var res = await execFun(WebInst, execD);
				whenExecEvt.OnNext(new ExecEvt(ExecEvtType.End, execOpt.Name, null));
				return res;
			}
			catch (Exception ex)
			{
				var err = new ExecErr(CurCodeLoc, ErrTypeIdentifier.Identify(ex), ex, tryIdx);
				var errNfo = new ExecErrNfo(err, execOpt);
				whenExecEvt.OnNext(new ExecEvt(ExecEvtType.Error, execOpt.Name, errNfo));

				if (errNfo.WillRetry)
				{
					InvalidateInst();
					CurCodeLoc = CodeLoc.UserCode;
				}
				else
				{
					throw;
				}
			}
		}

		throw new InvalidOperationException("PowWeb -> We should never get here in the Exec loop");
	}
}