using PowBasics.Geom;
using PowWeb._2_Actions._2_Cap.Structs.Enums;

namespace PowWeb._2_Actions._2_Cap.Structs;

public record CapNode(
	int Index,
	int? BackendNodeId,
	CapNodeType NodeType,
	string Name,
	CapAttr[] Attrs,
	bool? IsClickable,
	string Text,
	R Bounds
);