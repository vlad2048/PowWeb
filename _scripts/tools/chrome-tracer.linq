<Query Kind="Program">
  <Reference>C:\Dev_Nuget\Libs\PowWeb\Libs\PowWeb\bin\Debug\net6.0-windows\PowWeb.dll</Reference>
  <Namespace>PowWeb</Namespace>
  <Namespace>PuppeteerSharp</Namespace>
</Query>

#load "..\libs\web-con"

void Main()
{
	const string url = "https://hpjav.tv/168216/sun-008";
	const string cat = "net";
	var web = Web.Get(opt =>
	{
		opt.Logger = new LinqPadLogger();
	});
	web.Exec(async www =>
	{
		var page = www.Page;
		await page.Tracing.StartAsync(new TracingOptions
		{
			Categories = new List<string>{ cat }
		});
		
		www.Goto(url);
		
		var res = await page.Tracing.StopAsync();
		
		res.Dump();
	});
}

