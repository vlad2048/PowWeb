// ReSharper disable CheckNamespace

using System.Reactive;
using PowRxVar;
using PowWeb._1_Init._4_Exec;

namespace PowWeb;

public static class ExecOverloadsExt
{
	// Exec variants (non async)
	// -------------
	public static T Exec<T>(this Web web, ExecOpt? execOpt, Func<WebInst, Disp, T> execFun) => web.Exec(execOpt, (www, _d) => Task.FromResult(execFun(www, _d))).Result;
	public static T Exec<T>(this Web web, ExecOpt? execOpt, Func<WebInst, T> execFun) => web.Exec(execOpt, (www, _) => execFun(www));
	public static void Exec(this Web web, ExecOpt? execOpt, Action<WebInst> execFun) => web.Exec(execOpt, www => { execFun(www); return Unit.Default; });
	public static void Exec(this Web web, ExecOpt? execOpt, Action<WebInst, Disp> execFun) => web.Exec(execOpt, (www, execD) => { execFun(www, execD); return Unit.Default; });


	// Exec variants (async)
	// -------------
	public static Task<T> Exec<T>(this Web web, ExecOpt? execOpt, Func<WebInst, Task<T>> execFun) => web.Exec(execOpt, async (www, _) => await execFun(www));
	public static Task Exec(this Web web, ExecOpt? execOpt, Func<WebInst, Task> execFun) => web.Exec(execOpt, async www => { await execFun(www); return Unit.Default; });
	public static Task Exec(this Web web, ExecOpt? execOpt, Func<WebInst, Disp, Task> execFun) => web.Exec(execOpt, async (www, execD) => { await execFun(www, execD); return Unit.Default; });
}