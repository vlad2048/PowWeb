using PowBasics.Geom;
using PowWeb._1_Init._1_OptStructs.Enums;
using PowWeb._1_Init._1_OptStructs.Interfaces;
using PowWeb._1_Init._3_Logging;
using PowWeb._1_Init._3_Logging.Loggers;

namespace PowWeb._1_Init._1_OptStructs;

public class WebOpt : IInitLogging
{
	/// <summary>
	/// Defines how we want to open the browser
	/// <para/>
	/// Default is OpenMode.ConnectOrCreate
	/// </summary>
	public OpenMode OpenMode { get; set; } = OpenMode.ConnectOrCreate;

	/// <summary>
	/// Specify where to store: downloaded chrome, user profiles and extensions.
	/// <para/>
	/// Default is C:\ProgramData\PowWeb\
	/// </summary>
	public string? StorageFolder { get; set; }

	/// <summary>
	/// Path to a Chromium or Chrome executable to run instead of bundled Chromium
	/// </summary>
	public string? ChromeExe { get; set; } = @"C:\Program Files\Google\Chrome\Application\chrome.exe";

	/// <summary>
	/// User profile to use (stored in the [StorageFolder]\Profiles)
	/// <para/>
	/// Default is magic
	/// </summary>
	public string Profile { get; set; } = "magic";

	/// <summary>
	/// If true, it will delete the user profile folder before starting
	/// <para/>
	/// Default is false
	/// </summary>
	public bool DeleteProfile { get; set; }

	/// <summary>
	/// Location of the browser window on the desktop when not running in headless mode
	/// </summary>
	public Pt WinLoc { get; set; } = new(-940, 0);
	
	/// <summary>
	/// Run in headless mode
	/// <para/>
	/// Default is false
	/// </summary>
	public bool Headless { get; set; }

	/// <summary>
	/// Open DevTools
	/// <para/>
	/// Default is false
	/// </summary>
	public bool DevTools { get; set; }

	/// <summary>
	/// Install and use the AdBlockPlus extension
	/// <para/>
	/// Default is EnabledCloseTabOnStartup
	/// </summary>
	public AdBlockMode AdBlockMode { get; set; } = AdBlockMode.Disabled; //.EnabledCloseTabOnStartup;

	/// <summary>
	/// Debug port
	/// <para/>
	/// Default is 2122
	/// </summary>
	public int DebugPort { get; set; } = 2122;

	/// <summary>
	/// Setup how logging is done
	/// <para/>
	/// Default is to output to the Console
	/// </summary>
	public IPowWebLogger Logger { get; set; } = new ConLogger();

	/// <summary>
	/// true:
	///   - keep track of the current url
	///   - GetPage() returns the page pointing to the current url
	///   - consider other tabs as popups and close them
	/// false:
	///   - does not keep track of the current url
	///   - GetPage() returns the active tab
	///   - does not automatically close other tabs
	/// use true when you want to let the user play with the browser
	/// <para/>
	/// Default is true
	/// </summary>
	public bool DisallowMultipleTabs { get; set; } = true;

	internal static WebOpt Build(Action<WebOpt>? action)
	{
		var opt = new WebOpt();
		action?.Invoke(opt);
		return opt;
	}
}