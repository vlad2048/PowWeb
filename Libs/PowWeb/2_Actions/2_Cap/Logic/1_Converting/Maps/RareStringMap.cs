using PowWeb._2_Actions._2_Cap.Logic._1_Converting.Utils;
using PowWeb.ChromeApi.DDomSnapshot.Structs;

namespace PowWeb._2_Actions._2_Cap.Logic._1_Converting.Maps;

class RareStringMap
{
	private readonly Dictionary<int, int> map;
	private readonly string[] strs;

	public RareStringMap(RareStringData? data, string[] strs)
	{
		this.strs = strs;
		map = data switch {
			not null => data.Index.Zip(data.Value)
				.Select(t => (nodeIdx: t.First, strIdx: t.Second))
				.ToDictionary(
					t => t.nodeIdx,
					t => t.strIdx
				),
			null => new Dictionary<int, int>(),
		};
	}

	public string? this[int nodeIdx] =>
		map.TryGetValue(nodeIdx, out var strIndex) switch
		{
			true => strs.Get(strIndex),
			false => null
		};
}