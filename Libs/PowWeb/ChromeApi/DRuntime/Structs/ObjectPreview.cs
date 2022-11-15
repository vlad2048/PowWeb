namespace PowWeb.ChromeApi.DRuntime.Structs;

record ObjectPreview(
	string Type,
	string Subtype,
	string Description,
	bool Overflow,
	PropertyPreview[] Properties,
	EntryPreview[] Entries
);