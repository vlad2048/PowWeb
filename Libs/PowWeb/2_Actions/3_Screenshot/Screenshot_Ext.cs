using PowWeb._1_Init._4_Exec.Structs;
using PowWeb._1_Init._4_Exec.Structs.Enums;
using PowWeb._2_Actions._3_Screenshot.Logic;
using PowWeb._2_Actions._3_Screenshot.Utils;
using PuppeteerSharp;
// ReSharper disable CheckNamespace

namespace PowWeb;

public enum ScreenshotMethod
{
	None,
	Builtin,
	ScrollAndAssemble
}

public static class Screenshot_Ext
{
	public static string Screenshot(this WebInst www, ScreenshotMethod method)
	{
		www.SigStart(CodeLoc.Screenshot);

		var res = method switch
		{
			ScreenshotMethod.None =>
				ScreenshotConsts.MissingImage,

			ScreenshotMethod.Builtin =>
				www.GetPage().ScreenshotBase64Async(new ScreenshotOptions
						{
							FullPage = true,
							Type = ScreenshotType.Jpeg,
							Quality = 50,
						}
					).Result
					.ThrowIfInvalid(() => new FatalException("Failed to take a screenshot")),

			ScreenshotMethod.ScrollAndAssemble =>
				ScreenshotAssembler.ScrollAndAssemble(www.GetPage()),

			_ => throw new FatalException("Invalid ScreenshotMethod")
		};

		www.SigEnd();
		return res;
	}

	public static Bitmap ToBmp(this string screenshot)
	{
		var bytes = Convert.FromBase64String(screenshot);
		//using
		var ms = new MemoryStream(bytes);
		var img = Image.FromStream(ms);
		if (img is not Bitmap bmp) throw new InvalidOperationException();
		return bmp;
	}


	private static string ThrowIfInvalid(this string str, Func<Exception> exFun)
		=> string.IsNullOrEmpty(str) switch
		{
			false => str,
			true => throw exFun()
		};
}