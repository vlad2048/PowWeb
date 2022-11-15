using PowWeb.ChromeApi.DDomSnapshot.Structs;

namespace PowWeb._2_Actions._2_Cap.Logic._1_Converting.Maps;

class RareBooleanMap
{
	private readonly HashSet<int> set;

	public RareBooleanMap(RareBooleanData? data)
	{
		set = data switch {
			not null => data.Index.ToHashSet(),
			null => new HashSet<int>()
		};
	}

	public bool this[int nodeIdx] => set.Contains(nodeIdx);
}