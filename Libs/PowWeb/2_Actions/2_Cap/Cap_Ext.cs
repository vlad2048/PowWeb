using PowWeb._1_Init._4_Exec.Structs;
using PowWeb._1_Init._4_Exec.Structs.Enums;
using PowWeb._2_Actions._2_Cap.Logic._1_Converting;
using PowWeb._2_Actions._2_Cap.Logic._2_Simplifying;
using PowWeb._2_Actions._2_Cap.Logic._3_DocMerging;
using PowWeb._2_Actions._2_Cap.Structs;
using PowWeb.ChromeApi.DDomSnapshot;
// ReSharper disable CheckNamespace

namespace PowWeb;

public class CapOpt
{
	public bool Deep { get; set; }
	public string? LogFolder { get; set; }
	private CapOpt()
	{
	}
	internal static CapOpt Build(Action<CapOpt>? optFun)
	{
		var opt = new CapOpt();
		optFun?.Invoke(opt);
		return opt;
	}
}

public static class Cap_Ext
{
	public static Cap Cap(this WebInst www, Action<CapOpt>? optFun = null)
	{
		www.SigStart(CodeLoc.Cap);

		var opt = CapOpt.Build(optFun);
		var page = www.GetPage();
		var domSnap = page.Client.DomSnapshot_CaptureSnapshot(
			computedStyles: null,	// usage: new [] { "background-color" },
			includePaintOrder: false,
			includeDOMRects: true,
			includeBlendedBackgroundColors: false,
			includeTextColorOpacities: false
		);
		if (domSnap.Documents.Length == 0) throw new FatalException("Cap returned 0 docs");

		var res = domSnap
			.CheckAssumptions()
			.Convert()
			.Simplify()
			.Merge(www.CurrentUrl, opt.Deep, opt.LogFolder);

		www.SigEnd();
		return res;
	}
}