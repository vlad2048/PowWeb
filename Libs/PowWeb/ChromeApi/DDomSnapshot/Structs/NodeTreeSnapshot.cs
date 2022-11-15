namespace PowWeb.ChromeApi.DDomSnapshot.Structs;

record NodeTreeSnapshot(
	int[] ParentIndex,
	int[] NodeType,
	RareStringData? ShadowRootType,
	int[] NodeName,
	int[] NodeValue,
	int[]? BackendNodeId,
	int[][] Attributes,
	RareStringData? TextValue,
	RareStringData? InputValue,
	RareBooleanData? InputChecked,
	RareBooleanData? OptionSelected,
	RareIntegerData? ContentDocumentIndex,
	RareStringData? PseudoType,
	RareStringData? PseudoIdentifier,
	RareBooleanData? IsClickable,
	RareStringData? CurrentSourceURL,
	RareStringData? OriginURL
);

