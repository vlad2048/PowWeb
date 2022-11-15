using PowWeb._2_Actions._2_Cap.Logic._1_Converting.Maps;
using PowWeb._2_Actions._2_Cap.Logic._1_Converting.Structs;
using PowWeb._2_Actions._2_Cap.Logic._1_Converting.Utils;
using PowWeb.ChromeApi.DDomSnapshot.Enums;
using PowWeb.ChromeApi.DDomSnapshot.Structs;

namespace PowWeb._2_Actions._2_Cap.Logic._1_Converting;

static class NodDecoder
{
	public static NodNfo[] Decode(NodeTreeSnapshot nodes, string[] strs)
	{
		var cnt = nodes.NodeType.Length;
		var arr = new NodNfo[cnt];

		// @formatter:off
		var shadowRootTypeMap		= new RareStringMap(nodes.ShadowRootType, strs);
		var textValueMap			= new RareStringMap(nodes.TextValue, strs);
		var inputValueMap			= new RareStringMap(nodes.InputValue, strs);
		var pseudoTypeMap			= new RareStringMap(nodes.PseudoType, strs);
		var pseudoIdentifierMap		= new RareStringMap(nodes.PseudoIdentifier, strs);
		var currentSourceURLMap		= new RareStringMap(nodes.CurrentSourceURL, strs);
		var originlURLMap			= new RareStringMap(nodes.OriginURL, strs);

		var inputCheckedMap			= new RareBooleanMap(nodes.InputChecked);
		var optionSelectedMap		= new RareBooleanMap(nodes.OptionSelected);
		var isClickableMap			= new RareBooleanMap(nodes.IsClickable);

		var contentDocumentIndexMap	= new RareIntegerMap(nodes.ContentDocumentIndex);

		for (var i = 0; i < cnt; i++) {
			arr[i] = new NodNfo(
				Index:					i,
				ParentIndex: 			nodes.ParentIndex[i],
				NodeType:				(DomNodeType)nodes.NodeType[i],
				ShadowRootType:			shadowRootTypeMap[i].Parse(Enum.Parse<DomShadowRootType>),
				Name:					strs.ReadStr(nodes.NodeName, i),
				Value:					strs.ReadStr(nodes.NodeValue, i),
				BackendNodeId:			nodes.BackendNodeId.GetAt(i),
				Attrs:					AttrNfo.Decode(nodes.Attributes, i, strs),
				TextValue:				textValueMap[i],
				InputValue:				inputValueMap[i],
				InputChecked:			inputCheckedMap[i],
				OptionSelected:			optionSelectedMap[i],
				ContentDocumentIndex:	contentDocumentIndexMap[i],
				PseudoType:				pseudoTypeMap[i],
				PseudoIdentifier:		pseudoIdentifierMap[i],
				IsClickable:			isClickableMap[i],
				CurrentSourceURL:		currentSourceURLMap[i],
				OriginURL:				originlURLMap[i]
			);
		}
		// @formatter:on

		return arr;
	}

	private static int GetAt(this int[]? arr, int idx) => arr switch
	{
		not null => arr[idx],
		null => -1
	};
}
