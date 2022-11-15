using System.Drawing.Imaging;
using PowBasics.Geom;
using PuppeteerSharp;
using PuppeteerSharp.Media;

namespace PowWeb._2_Actions._3_Screenshot.Logic;

static class ScreenshotAssembler
{
	public static string ScrollAndAssemble(Page page)
	{
		var mover = new Mover(page);

		int scrollWidth;
		int scrollHeight;
		int clientWidth;
		int clientHeight;
		int yPageCount;
		int xPageCount;


		void UpdateVars()
		{
			scrollWidth = mover.ScrollWidth;
			scrollHeight = mover.ScrollHeight;
			clientWidth = mover.ClientWidth;
			clientHeight = mover.ClientHeight;
			yPageCount = FillingDiv(scrollHeight, clientHeight);
			xPageCount = FillingDiv(scrollWidth, clientWidth);
		}

		UpdateVars();

		var cpyNfos = new List<CpyNfo>();

		for (var yPage = 0; yPage < yPageCount; yPage++)
		{
			var dstY = yPage * clientHeight;
			var overY = Math.Max(0, dstY + clientHeight - scrollHeight);
			var srcY = dstY;
			var srcHeight = clientHeight - overY;

			mover.ScrollTop = dstY;

			for (var xPage = 0; xPage < xPageCount; xPage++)
			{
				var dstX = xPage * clientWidth;
				var overX = Math.Max(0, dstX + clientWidth - scrollWidth);
				var srcX = dstX;
				var srcWidth = clientWidth - overX;

				mover.ScrollLeft = dstX;

				var srcR = new R(srcX, srcY, srcWidth, srcHeight);
				var areaBmp = mover.TakeScreenshot(srcR);

				cpyNfos.Add(new CpyNfo(areaBmp, dstX, dstY));

				UpdateVars();
			}

			mover.ScrollLeft = 0;
			mover.ScrollTop = 0;
		}

		var bmp = new Bitmap(scrollWidth, scrollHeight);
		var gfx = Graphics.FromImage(bmp);
		foreach (var cpyNfo in cpyNfos)
		{
			gfx.DrawImage(cpyNfo.Bmp, cpyNfo.X, cpyNfo.Y);
			cpyNfo.Bmp.Dispose();
		}

		var str = Bmp2Base64(bmp);
		return str;
	}

	private static string Bmp2Base64(Bitmap bmp)
	{
		using var ms = new MemoryStream();
		bmp.Save(ms, ImageFormat.Jpeg);
		var byteImage = ms.ToArray();
		var str = Convert.ToBase64String(byteImage);
		bmp.Dispose();
		return str;
	}

	private record CpyNfo(Bitmap Bmp, int X, int Y);

	private class Mover
	{
		private readonly Page page;

		public int ScrollWidth => page.EvaluateExpressionAsync("document.documentElement.scrollWidth").Result.ToObject<int>();
		public int ClientWidth => page.EvaluateExpressionAsync("document.documentElement.clientWidth").Result.ToObject<int>();
		public int ScrollLeft
		{
			get => page.EvaluateExpressionAsync("document.documentElement.scrollLeft").Result.ToObject<int>();
			set => page.EvaluateExpressionAsync($"document.documentElement.scrollLeft = {value}").Wait();
		}

		public int ScrollHeight => page.EvaluateExpressionAsync("document.documentElement.scrollHeight").Result.ToObject<int>();
		public int ClientHeight => page.EvaluateExpressionAsync("document.documentElement.clientHeight").Result.ToObject<int>();
		public int ScrollTop
		{
			get => page.EvaluateExpressionAsync("document.documentElement.scrollTop").Result.ToObject<int>();
			set => page.EvaluateExpressionAsync($"document.documentElement.scrollTop = {value}").Wait();
		}

		public Mover(Page page)
		{
			this.page = page;
		}

		public Bitmap TakeScreenshot(R clipR)
		{
			var stream = page.ScreenshotStreamAsync(new ScreenshotOptions
			{
				Type = ScreenshotType.Png,
				FullPage = false,
				Clip = new Clip
				{
					X = clipR.X,
					Y = clipR.Y,
					Width = clipR.Width,
					Height = clipR.Height
				}
			}).Result;
			var bmp = new Bitmap(stream);
			return bmp;
		}
	}


	private static int FillingDiv(int a, int b)
	{
		if (b == 0) throw new ArgumentException();
		if (a < 0)
			return 0;
		var res = (a - 1) / b + 1;
		return res;
	}
}