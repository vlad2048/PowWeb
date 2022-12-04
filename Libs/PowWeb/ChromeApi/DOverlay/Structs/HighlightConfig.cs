using RGBA = PowWeb.ChromeApi.DDom.Structs.RGBA;

namespace PowWeb.ChromeApi.DOverlay.Structs;

record GridHighlightConfig;
record FlexContainerHighlightConfig;
record FlexItemHighlightConfig;
record ContainerQueryContainerHighlightConfig;

record HighlightConfig(
	bool? ShowInfo,
	bool? ShowStyles,
	bool? ShowRulers,
	bool? ShowAccessibilityInfo,
	bool? ShowExtensionLines,
	RGBA? ContentColor,
	RGBA? paddingColor,
	RGBA? borderColor,
	RGBA? marginColor,
	RGBA? eventTargetColor,
	RGBA? shapeColor,
	RGBA? shapeMarginColor,
	RGBA? cssGridColor,

	// rgb, hsl, hwb, hex
	string? ColorFormat,

	GridHighlightConfig? GridHighlightConfig,
	FlexContainerHighlightConfig? FlexContainerHighlightConfig,
	FlexItemHighlightConfig? FlexItemHighlightConfig,

	// aa, aaa, apca
	string? ContrastAlgorithm,

	ContainerQueryContainerHighlightConfig? containerQueryContainerHighlightConfig
);