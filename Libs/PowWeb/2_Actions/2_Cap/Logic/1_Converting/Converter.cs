using PowBasics.Geom;
using PowWeb._2_Actions._2_Cap.Logic._1_Converting.Structs;
using PowWeb._2_Actions._2_Cap.Logic._1_Converting.Utils;
using PowWeb._2_Actions._2_Cap.Structs;
using PowWeb._2_Actions._2_Cap.Structs.Enums;
using PowWeb.ChromeApi.DDomSnapshot;
using PowWeb.ChromeApi.DDomSnapshot.Structs;
using PowWeb.ChromeApi.Utils.Extensions;

namespace PowWeb._2_Actions._2_Cap.Logic._1_Converting;

static class Converter
{
	public static Cap[] Convert(this DomSnapshotApi.DomSnapshot_CaptureSnapshot_Ret domSnap) =>
		domSnap.Documents.SelectToArray(domDoc => ConvertDoc(domDoc, domSnap.Strings));

	private static Cap ConvertDoc(DocumentSnapshot domDoc, string[] strs)
	{
		var nods = NodDecoder.Decode(domDoc.Nodes, strs);
		var lays = LayDecoder.Decode(domDoc.Layout, strs);
		var tupMap = nods
			.ToDictionary(
				n => n.Index,
				n => (
					Nod: Mix2CapNod(n, lays.GetOpt(n.Index)),
					n.ParentIndex
				)
			);

		foreach (var (_, tup) in tupMap)
		{
			var (nod, parentIndex) = tup;
			if (parentIndex == -1) continue;

			tupMap[parentIndex].Nod.AddChild(nod);
		}

		var root = tupMap.Values.Single(e => e.ParentIndex == -1).Nod;
		return new Cap(
			strs.Get(domDoc.DocumentURL),
			root
		);
	}

	// @formatter:off
	private static N Mix2CapNod(NodNfo nod, LayNfo? lay) => Nod.Make(new CapNode(
		// Nods
		Index:			nod.Index,
		NodeType:		(CapNodeType)nod.NodeType,
		Name:			nod.Name,
		Attrs:			nod.Attrs.SelectToArray(f => new CapAttr(f.Name, f.Value)),
		IsClickable:	nod.IsClickable,
		// Lays?
		Text:			(lay?.Text ?? string.Empty).Trim(),
		Bounds:			lay?.Bounds ?? R.Empty
	));
	// @formatter:on
}
