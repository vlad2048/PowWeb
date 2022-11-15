using PowBasics.Geom;
using PowWeb.ChromeApi.Utils.Extensions;

namespace PowWeb._2_Actions._2_Cap.Logic._1_Converting.Utils;

static class ArrExt
{
	// ************
	// * string[] *
	// ************
	public static string Get(this string[] strs, int strIdx)
		=> (strIdx != -1) switch
		{
			true => strs[strIdx],
			false => string.Empty
		};

	public static string[] LookupStrings(this int[] strIndices, string[] strs) => strIndices.SelectToArray(e => strs[e]);

	public static string ReadStr(this string[] strs, int[] strIndices, int nodeIdx) => strs.Get(strIndices[nodeIdx]);

	public static string? ReadStrOpt(this string[] strs, int[]? strIndices, int nodeIdx) => strIndices switch
	{
		not null => strs.Get(strIndices[nodeIdx]),
		null => null
	};



	// ***********
	// * other[] *
	// ***********
	public static R? ToROpt(this double[]? arr)
		=> arr switch
		{
			not null when arr.Length == 4 => arr.ToR(),
			_ => null,
		};

	public static R ToR(this double[] arr)
	{
		var x = (int)arr[0];
		var y = (int)arr[1];
		var width = (int)arr[2];
		var height = (int)arr[3];
		var isValid = width > 0 && height > 0;

		return isValid switch
		{
			true => new R(x, y, width, height),
			false => R.Empty
		};
	}


	// ***********
	// * Parsing *
	// ***********
	public static T? Parse<T>(this string? str, Func<string, T> parseFun) where T : struct
		=> str switch
		{
			null => null,
			not null => parseFun(str)
		};



	// ********
	// * Misc *
	// ********
	public static V? GetOpt<K, V>(this Dictionary<K, V> map, K key) where K : notnull where V : class
		=> map.TryGetValue(key, out var val) switch
		{
			true => val,
			false => null
		};
}