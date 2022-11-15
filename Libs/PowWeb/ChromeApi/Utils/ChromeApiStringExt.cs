namespace PowWeb.ChromeApi.Utils;

static class ChromeApiStringExt
{
	public static string? CamelCase(this string? str) => str switch
	{
		null => null,
		not null => (str == string.Empty) switch
		{
			true => str,
			false => $"{str[0]}".ToLowerInvariant() + str[1..]
		}
	};
}