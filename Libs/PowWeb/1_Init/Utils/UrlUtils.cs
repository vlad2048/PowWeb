using StringComparison = System.StringComparison;

namespace PowWeb._1_Init.Utils;

public static class UrlUtils
{
	public static bool AreUrlsTheSame(string u1, string u2)
	{
		var s1 = u1.NormalizeUrl();
		var s2 = u2.NormalizeUrl();
		return string.Compare(s1, s2, StringComparison.InvariantCultureIgnoreCase) == 0;
	}

	private static string NormalizeUrl(this string url)
	{
		url = url.Trim();
		if (url.Length == 0) return url;
		if (url[^1] == '/') url = url[..^1];
		return url;
	}
}
