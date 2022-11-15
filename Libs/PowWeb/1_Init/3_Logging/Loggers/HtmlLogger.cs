using System.Text;
using PowWeb._1_Init._3_Logging.Structs;

namespace PowWeb._1_Init._3_Logging.Loggers;

public class HtmlLogger : IPowWebLogger
{
	private readonly MemoryLogger memLogger = new();
	private readonly Dictionary<Color, string> colMap = new();

	public HtmlLogger(string htmlFile)
	{
		memLogger.WhenColorAdded.Subscribe(col =>
		{
			var colName = $"--col-{colMap.Count}";
			colMap[col] = colName;
		});

		memLogger.WhenChanged.Subscribe(data =>
		{
			WriteHtmlAndCss(data, htmlFile);
		});
	}

	public void Log(Txt txt) => memLogger.Log(txt);

	private void WriteHtmlAndCss(LogData data, string htmlFile)
	{
		var folder = Path.GetDirectoryName(htmlFile)!;
		var baseName = Path.GetFileNameWithoutExtension(htmlFile);
		var cssFile = Path.Combine(folder, $"{baseName}.css");
		File.WriteAllText(htmlFile, BuildHtmlFile(data, baseName));
		File.WriteAllText(cssFile, BuildCssFile());
	}

	private string BuildHtmlFile(LogData data, string baseFile)
	{
		var sb = new StringBuilder();

		sb.Append($@"
<!DOCTYPE html>
<html lang='en'>
  <head>
    <meta charset='UTF-8'/>
    <title>Nice</title>
    <link rel='stylesheet' href='{baseFile}.css'/>
  </head>
  <body>
");

		foreach (var line in data.Data)
		{
			sb.AppendLine("    <div>");
			foreach (var span in line)
			{
				var (str, col) = span;
				str = str.Replace(" ", "&nbsp;");
				var colName = colMap[col];
				sb.AppendLine($"      <span style='color:var({colName})'>{str}</span>");
			}
			sb.AppendLine("    </div>");
		}

		sb.Append(@"
  </body>
</html>
");

		return sb.ToString();
	}

	private string BuildCssFile()
	{
		var sb = new StringBuilder();

		sb.Append(@"html, body {
  margin: 0;
  width: 100%;
  height: 100%;
}

html {
  padding: 0;
}

body {
  padding: 5px;
  font-family: Consolas;
  font-weight: bold;
  font-size: 16px;
}

* {
  box-sizing: border-box;
}

:root {
  --col-background: #000935;
");

		foreach (var (col, colName) in colMap)
			sb.AppendLine($"{colName}: #{col.R:X2}{col.G:X2}{col.B:X2};");

		sb.Append(@"
}

body {
  background-color: var(--col-background);
}
");

		return sb.ToString();
	}
}