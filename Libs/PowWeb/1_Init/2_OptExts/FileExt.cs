using System.IO.Compression;
using PowWeb._1_Init._1_OptStructs;
using PowWeb._1_Init._1_OptStructs.Enums;
using PowWeb._1_Init.Utils;
using RestSharp;

namespace PowWeb._1_Init._2_OptExts;

static class FileExt
{
	// C:\ProgramData\PowWeb\
	private static string RootFolder(this WebOpt opt) => opt.StorageFolder ?? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), WebConstsPrivate.FolderRoot);
	// C:\ProgramData\PowWeb\[folder]
	private static string Folder(this WebOpt opt, string folder) => Path.Combine(opt.RootFolder(), folder).EnsureFolder();

	// C:\ProgramData\PowWeb\Profiles
	private static string ProfilesFolder(this WebOpt opt) => opt.Folder(WebConstsPrivate.FolderProfiles);
	// C:\ProgramData\PowWeb\Extensions
	private static string ExtensionsFolder(this WebOpt opt) => opt.Folder(WebConstsPrivate.FolderExtensions);

	// C:\ProgramData\PowWeb\Download
	public static string DownloadFolder(this WebOpt opt) => opt.Folder(WebConstsPrivate.FolderDownload);

	// C:\ProgramData\PowWeb\DownloadCache
	public static string DownloadCacheFolder(this WebOpt opt) => opt.Folder(WebConstsPrivate.FolderDownloadCache);

	public static void DownloadChromiumIfNotCached(this WebOpt opt, string srcUrl, string dstFile)
	{
		var fileCache = Path.Combine(opt.DownloadCacheFolder(), Path.GetFileName(dstFile));
		if (!File.Exists(fileCache))
		{
			using var client = new RestClient(srcUrl);
			using var downloadStream = client.DownloadStream(new RestRequest());
			using var writer = File.OpenWrite(fileCache);
			downloadStream!.CopyTo(writer);
		}
		File.Copy(fileCache, dstFile, true);
	}

	public static void DownloadChromium(string srcUrl, string dstFile)
	{
	}

	// C:\ProgramData\PowWeb\Profiles\[profile]
	public static string ProfileFolder(this WebOpt opt) => Path.Combine(opt.ProfilesFolder(), opt.Profile).EnsureFolder();

	public static void EmptyProfileIfNeeded(this WebOpt opt)
	{
		if (!opt.DeleteProfile) return;
		opt.LogOp($"Emptying profile {opt.Profile} ... ");
		opt.ProfileFolder().EmptyFolder();
		opt.LogOpDone();
	}

	// C:\ProgramData\PowWeb\Extensions\AdblockPlus
	public static string GetAdblockPlusExtensionFolder(this WebOpt opt)
	{
		if (opt.AdBlockMode == AdBlockMode.Disabled) throw new InvalidOperationException();
		var dstFolder = Path.Combine(opt.ExtensionsFolder(), WebConstsPrivate.SubFolderAdblockPlus).EnsureFolder();
		if (Directory.GetFiles(dstFolder).Any() || Directory.GetDirectories(dstFolder).Any()) return dstFolder;

		var srcFolder = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly()!.Location)!;
		var srcFile = Path.Combine(srcFolder, WebConstsPrivate.AdblockPlusZipFile);
		ZipFile.ExtractToDirectory(srcFile, dstFolder);
		return dstFolder;
	}

	private static string EnsureFolder(this string folder)
	{
		if (!Directory.Exists(folder))
			Directory.CreateDirectory(folder);
		return folder;
	}

	private static string EmptyFolder(this string folder)
	{
		var files = Directory.GetFiles(folder.EnsureFolder());
		var subdirs = Directory.GetDirectories(folder);
		foreach (var file in files) File.Delete(file);
		foreach (var subdir in subdirs) Directory.Delete(subdir, true);
		return folder;
	}
}
