using PowWeb._2_Actions._2_Cap.Logic._1_Converting.Utils;
using PowWeb.ChromeApi.Utils.Extensions;

namespace PowWeb._2_Actions._2_Cap.Logic._1_Converting.Structs;

record AttrNfo(string Name, string? Value)
{
	public static AttrNfo[] Decode(int[][]? attributes, int nodeIdx, string[] strs)
	{
		if (attributes == null) return Array.Empty<AttrNfo>();

		var flatArr = attributes[nodeIdx].SelectToArray(strs.Get);

		if (flatArr.Length % 2 != 0) throw new ArgumentException();
		return Enumerable.Range(0, flatArr.Length / 2).SelectToArray(i => new AttrNfo(flatArr[i * 2 + 0], flatArr[i * 2 + 1]));
	}
}