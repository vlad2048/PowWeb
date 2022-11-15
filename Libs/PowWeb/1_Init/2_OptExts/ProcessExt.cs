using System.ComponentModel;
using System.Diagnostics;
using System.Management;
using PowWeb._1_Init._1_OptStructs;

namespace PowWeb._1_Init._2_OptExts;

static class ProcessExt
{
	public static void KillIfUp(this WebOpt opt)
	{
		opt.LogOp("IsUp");
		if (IsUp())
		{
			opt.LogOp("yes->kill");
			Kill();
			opt.LogOpDone();
		}
		else
		{
			opt.LogOpLine("no");
		}
	}

	public static bool IsUp() => GetChromeProcesses().Any();

	private static void Kill()
	{
		var procs = GetChromeProcesses();
		var hasKilled = false;
		foreach (var proc in procs)
		{
			try
			{
				proc.Kill();
				hasKilled = true;
			}
			catch (Win32Exception ex) when ((uint)ex.ErrorCode == 0x80004005)
			{
				// Intentionally empty - no security access to the process.
			}
			catch (InvalidOperationException)
			{
				// Intentionally empty - the process exited before getting details.
			}
		}
		if (hasKilled) Thread.Sleep(TimeSpan.FromSeconds(1));
	}

	private static Process[] GetChromeProcesses()
	{
		static string? GetCommandLine(Process process)
		{
			using var searcher = new ManagementObjectSearcher("SELECT CommandLine FROM Win32_Process WHERE ProcessId = " + process.Id);
			using ManagementObjectCollection objects = searcher.Get();
			return objects.Cast<ManagementBaseObject>().SingleOrDefault()?["CommandLine"]?.ToString();
		}


		return
			Process.GetProcessesByName("Chrome")
				.Where(proc =>
				{
					var cmd = GetCommandLine(proc);
					//var cmp = $@"--user-data-dir=""{RenderConsts.UserDataFolder}""";
					var cmp = "--disable-web-security";
					return cmd != null && cmd.Contains(cmp);
				})
				.ToArray();
	}
}