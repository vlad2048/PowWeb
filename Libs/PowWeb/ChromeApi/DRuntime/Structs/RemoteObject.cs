namespace PowWeb.ChromeApi.DRuntime.Structs;

record RemoteObject(
	string Type,
	string SubType,
	string ClassName,
	object Value,
	string UnserializableValue,
	string Description,
	WebDriverValue WebDriverValue,
	string ObjectId,
	ObjectPreview Preview,
	CustomPreview CustomPreview
);