namespace PowWeb.ChromeApi.DDebugger.Structs;

record Location(
	string ScriptId,
	int LineNumber,
	int? ColumnNumber
)
{
	public override string ToString() => $"(script:{ScriptId} line:{LineNumber} col:{ColStr})";

	private string ColStr => ColumnNumber switch
	{
		not null => $"{ColumnNumber}",
		null => "_"
	};
}