using PowWeb._2_Actions._2_Cap.Logic._1_Converting.Maps;
using PowWeb._2_Actions._2_Cap.Logic._1_Converting.Structs;
using PowWeb._2_Actions._2_Cap.Logic._1_Converting.Utils;
using PowWeb.ChromeApi.DDomSnapshot.Structs;

namespace PowWeb._2_Actions._2_Cap.Logic._1_Converting;

static class LayDecoder
{
	public static Dictionary<int, LayNfo> Decode(LayoutTreeSnapshot lays, string[] strs)
	{
		var cnt = lays.NodeIndex.Length;
		var arr = new LayNfo[cnt];

		// @formatter:off
		var stackingContexts = new RareBooleanMap(lays.StackingContexts);

		for (var i = 0; i < cnt; i++)
		{
			arr[i] = new LayNfo(
				Index:						lays.NodeIndex[i],
				Styles:						lays.Styles[i].LookupStrings(strs),
				Bounds:						lays.Bounds[i].ToR(),
				Text:						strs.Get(lays.Text[i]),
				StackingContexts:			stackingContexts[i],
				PaintOrders:				lays.PaintOrders?[i],
				OffsetRects:				lays.OffsetRects?[i].ToROpt(),
				ScrollRects:				lays.ScrollRects?[i].ToROpt(),
				ClientRects:				lays.ClientRects?[i].ToROpt(),
				BlendedBackgroundColors:	strs.ReadStrOpt(lays.BlendedBackgroundColors, i),
				TextColorOpacities:			lays.TextColorOpacities?[i]
			);
		}

		// @formatter:on

		var normalizedArr = arr
			.GroupBy(e => e.Index)
			.Select(e => e
				.OrderByDescending(f => f.Text.Length)
				.ThenByDescending(f => f.Bounds.Width * f.Bounds.Height)
				.Take(1)
			)
			.SelectMany(e => e)
			.ToArray();

		return normalizedArr
			.ToDictionary(
				e => e.Index,
				e => e
			);
	}
}