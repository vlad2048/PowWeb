using PowWeb.ChromeApi.DDomSnapshot.Enums;

namespace PowWeb._2_Actions._2_Cap.Logic._1_Converting.Structs;

record NodNfo(
	int Index,
	int ParentIndex,
	DomNodeType NodeType,
	DomShadowRootType? ShadowRootType,
	string Name,
	string Value,
	int? BackendNodeId,
	AttrNfo[] Attrs,
	string? TextValue,
	string? InputValue,
	bool? InputChecked,
	bool? OptionSelected,
	int? ContentDocumentIndex,
	string? PseudoType,
	string? PseudoIdentifier,
	bool? IsClickable,
	string? CurrentSourceURL,
	string? OriginURL
);