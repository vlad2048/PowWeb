using System.Diagnostics;
using System.Management;
using System.Runtime.InteropServices;
using PowWeb._1_Init._1_OptStructs;

namespace PowWeb._1_Init._2_OptExts;

static class BringToTopExt
{
	public static void BringToTop(this WebOpt opt)
	{
		static string GetProcArgs(int procId)
		{
			try
			{
				using var searcher = new ManagementObjectSearcher("SELECT CommandLine FROM Win32_Process WHERE ProcessId = " + procId);
				using var objects = searcher.Get();
				var result = objects.Cast<ManagementBaseObject>().SingleOrDefault();
				return result?["CommandLine"]?.ToString() ?? "";
			}
			catch
			{
				return string.Empty;
			}
		}

		[DllImport("USER32.DLL")]
		static extern bool SetForegroundWindow(IntPtr hWnd);

		var profileFolder = opt.ProfileFolder();

		//var watch = Stopwatch.StartNew();
		var chromeProcs = Process.GetProcessesByName("chrome");
		//var t1 = watch.Elapsed;
		//watch = Stopwatch.StartNew();
		var proc = chromeProcs
			.FirstOrDefault(e =>
			{
				var args = GetProcArgs(e.Id);
				var isPowWeb = args.Contains($@"--user-data-dir=""{profileFolder}""");
				var isMain = !args.Contains("--type=");
				return isPowWeb && isMain;
			});
		//var t2 = watch.Elapsed;
		if (proc == null)
		{
			opt.LogWarnLine("Process not found");
			return;
		}

		//Log($"Process found: {proc.Id}   time1:{t1.TotalMilliseconds:F3}ms   time2:{t2.TotalMilliseconds:F3}ms");
		SetForegroundWindow(proc.MainWindowHandle);
	}
}