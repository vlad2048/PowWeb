using PowBasics.Geom;

namespace PowWeb._2_Actions._2_Cap.Logic._1_Converting.Structs;

record LayNfo(
	int Index,
	string[] Styles,
	R Bounds,
	string Text,
	bool StackingContexts,
	int? PaintOrders,
	R? OffsetRects,
	R? ScrollRects,
	R? ClientRects,
	string? BlendedBackgroundColors,
	double? TextColorOpacities
);