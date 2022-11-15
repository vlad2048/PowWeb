<Query Kind="Program">
  <Reference>C:\Dev_Nuget\Libs\PowWeb\Libs\PowWeb\bin\Debug\net6.0-windows\PowWeb.dll</Reference>
  <Reference>C:\Dev\WebExtractor\Libs\SnapLib\bin\Debug\net6.0-windows\SnapLib.dll</Reference>
  <Namespace>PowWeb._2_Actions._2_Cap.Ser</Namespace>
  <Namespace>PowWeb._2_Actions._2_Cap.Structs</Namespace>
  <Namespace>SnapLib.Structs</Namespace>
</Query>

void Main()
{
	//var folderIn = @"C:\Dev\WebExtractor\_infos\olddata\2022-10-03_snaps\main\PaginatedVideoList";
	//var folderOut = @"C:\Dev\WebExtractor\_data\snaps\PaginatedVideoList";
	
	var t = TCap.FromFile(@"C:\Dev\WebExtractor\_data\snaps\main\fullscatmovies.json");
	return;
	
	var folderIn = @"C:\Dev\WebExtractor\_infos\olddata\2022-10-03_snaps\main\VideoDetails";
	var folderOut = @"C:\Dev\WebExtractor\_data\snaps\VideoDetails";
	
	var files = Directory.GetFiles(folderIn);
	foreach (var file in files)
	{
		var fileSrc = file;
		var fileDst = Path.Combine(folderOut, Path.GetFileName(file));
		fileSrc.Dump();
		fileDst.Dump();
		ConvertFile(fileSrc, fileDst);
	}
}

void ConvertFile(string fileSrc, string fileDst)
{
	var t = PowWebJsonUtils.Load<pTCap>(fileSrc);
	var tNext = new TCap(
		new Cap(
			t.Cap.Url,
			t.Cap.Root
		),
		t.Cap.Screenshot,
		t.PageModelName,
		MapDict(t.UserTags),
		MapDict(t.DetectedTags)
	);
	PowWebJsonUtils.Save(fileDst, tNext);
}

record pTCap(
	pCap Cap,
	string PageModelName,
	Dictionary<int, string> UserTags,
	Dictionary<int, string> DetectedTags
);

record pCap(
	string Url,
	string Screenshot,
	TNod<CapNode> Root
);

TagSet MapDict(Dictionary<int, string> dict) =>
	TagSet.FromDict(
		dict
			.GroupBy(kv => kv.Value)
			.ToDictionary(
				grp => grp.Key,
				grp => grp.Select(e => e.Key).ToArray()
			)
	);