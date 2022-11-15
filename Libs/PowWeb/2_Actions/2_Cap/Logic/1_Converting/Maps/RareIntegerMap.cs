using PowWeb.ChromeApi.DDomSnapshot.Structs;

namespace PowWeb._2_Actions._2_Cap.Logic._1_Converting.Maps;

class RareIntegerMap
{
	private readonly Dictionary<int, int> map;

	public RareIntegerMap(RareIntegerData? data)
	{
		map = data switch {
			not null => data.Index.Zip(data.Value)
				.Select(t => (nodeIdx: t.First, value: t.Second))
				.ToDictionary(
					t => t.nodeIdx,
					t => t.value
				),
			null => new Dictionary<int, int>()
		};
	}

	public int? this[int nodeIdx] =>
		map.TryGetValue(nodeIdx, out var value) switch
		{
			true => value,
			false => null
		};
}