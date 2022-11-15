namespace PowWeb.ChromeApi.DDomSnapshot.Structs;

record DocumentSnapshot(
	int DocumentURL,
	int Title,
	int BaseURL,
	int ContentLanguage,
	int EncodingName,
	int PublicId,
	int SystemId,
	int FrameId,
	NodeTreeSnapshot Nodes,
	LayoutTreeSnapshot Layout,
	TextBoxSnapshot TextBoxes,
	int? ScrollOffsetX,
	int? ScrollOffsetY,
	int? ContentWidth,
	int? ContentHeight
);