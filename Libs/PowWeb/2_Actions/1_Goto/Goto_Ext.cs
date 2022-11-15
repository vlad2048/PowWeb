﻿using PowWeb._1_Init._4_Exec.Structs;
using PowWeb._1_Init._4_Exec.Structs.Enums;
using PowWeb._1_Init.Utils;
using PuppeteerSharp;
// ReSharper disable CheckNamespace

namespace PowWeb;

public static class Goto_Ext
{
	public static void Goto(this WebInst www, string url)
	{
		www.SigStart(CodeLoc.Goto);

		var page = www.GetPage();
		page.GoToAsync(url, WaitUntilNavigation.Networkidle2).Wait();
		var areUrlsTheSame = UrlUtils.AreUrlsTheSame(page.Url, url);
		if (!areUrlsTheSame) throw new FatalException($"Failed to goto url: '{url}'");

		www.CurrentUrl = url;

		www.SigEnd();
	}
}