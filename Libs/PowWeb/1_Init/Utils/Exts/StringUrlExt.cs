namespace PowWeb._1_Init.Utils.Exts;

static class StringUrlExt
{
	public static string FmtUrlSimple(this string url)
	{
		var res = Path.GetDirectoryName(url) ?? string.Empty;
		return (res == string.Empty) switch {
			false => res,
			true => url
		};
	}
}