namespace PowWeb.ChromeApi.DDomSnapshot.Structs;

record LayoutTreeSnapshot(
	int[] NodeIndex,
	int[][] Styles,
	double[][] Bounds,
	int[] Text,
	RareBooleanData StackingContexts,
	int[]? PaintOrders,
	double[][]? OffsetRects,
	double[][]? ScrollRects,
	double[][]? ClientRects,
	int[]? BlendedBackgroundColors,
	double[]? TextColorOpacities
);