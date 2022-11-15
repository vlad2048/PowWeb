namespace PowWeb.ChromeApi.DDomSnapshot.Structs;

record TextBoxSnapshot(
	int[] LayoutIndex,
	double[][] Bounds,
	int[] Start,
	int[] Length
);