using System.Web;

namespace PowWeb._2_Actions._2_Cap.Logic._3_DocMerging.Utils;

static class MergeUrlUtils
{
	public static string PreProcess(string s) => HttpUtility.UrlDecode(s).Trim();

	public static string Id(string s) => s;
	public static string True(string s) => string.Empty;

	public static string RemoveUrlParams(string s)
	{
		int Idx(char c)
		{
			var idx = s.IndexOf(c);
			return idx switch
			{
				-1 => int.MaxValue,
				_ => idx
			};
		}

		var idx = Min(Idx('?'), Idx('&'), Idx('#'));
		if (idx != int.MaxValue) s = s[..idx];
		return Path.GetFileNameWithoutExtension(s);
	}

	public static string GetDomain(string url)
	{
		var idxStart = url.IndexOf("//", StringComparison.Ordinal);
		if (idxStart == -1) return url;
		var idxEnd = url.IndexOf("/", idxStart + 2, StringComparison.Ordinal);
		if (idxEnd == -1) return url;
		return url[..idxEnd];
	}

	private static int Min(params int[] source) => source.Min();
}