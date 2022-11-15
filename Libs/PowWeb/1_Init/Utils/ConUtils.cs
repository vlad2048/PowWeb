using System.Runtime.InteropServices;
using PowWeb._1_Init._1_OptStructs;
using PowWeb._1_Init._3_Logging.Structs;

namespace PowWeb._1_Init.Utils;

static class ConUtils
{
	private static Color? colPrev;

	public static void Write(Txt t)
	{
		if (t.Col != colPrev)
		{
			SetColor(t.Col);
			colPrev = t.Col;
		}
		Console.Write(t.Str);
	}



	private static void SetColor(Color c)
	{
		InitColor();
		Console.Write($"{EscChar}[38;2;{c.R};{c.G};{c.B}m");
	}

	
	

	private static bool isColorInit;
	private static void InitColor() { if (isColorInit) return; isColorInit = true; EnableVirtualTerminalProcessing(); }
	
	private const uint ENABLE_VIRTUAL_TERMINAL_PROCESSING = 0x04;
	private const char EscChar = (char)0x1B;

	private static void EnableVirtualTerminalProcessing()
	{
		var hOut = GetStdHandle(unchecked((uint)(int)StdHandle.STD_OUTPUT_HANDLE));
		if (hOut == new IntPtr(-1))
			return;
		var res = GetConsoleMode(hOut, out var mode);
		if (!res)
			return;
		mode |= ENABLE_VIRTUAL_TERMINAL_PROCESSING;
		SetConsoleMode(hOut, mode);
	}


	private const string LibraryNameKernel32 = "kernel32";

	[DllImport(LibraryNameKernel32, ExactSpelling = true)]
	private static extern IntPtr GetStdHandle(uint nStdHandle);

	[DllImport(LibraryNameKernel32, ExactSpelling = true)]
	private static extern bool GetConsoleMode(IntPtr hConsoleHandle, out uint lpMode);

	[DllImport(LibraryNameKernel32, ExactSpelling = true)]
	private static extern bool SetConsoleMode(IntPtr hConsoleHandle, uint dwMode);


	private enum StdHandle
	{
		/*
		/// <summary>
		///     The standard input device. Initially, this is the console input buffer, CONIN$.
		/// </summary>
		STD_INPUT_HANDLE = unchecked((int) (uint) -10),
		*/

		/// <summary>
		///     The standard output device. Initially, this is the active console screen buffer, CONOUT$.
		/// </summary>
		STD_OUTPUT_HANDLE = unchecked((int) (uint) -11),

		/*
		/// <summary>
		///     The standard error device. Initially, this is the active console screen buffer, CONOUT$.
		/// </summary>
		STD_ERROR_HANDLE = unchecked((int) (uint) -12)
		*/
	}
}