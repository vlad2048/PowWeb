<Query Kind="Program">
  <Reference>C:\Dev_Nuget\Libs\PowWeb\Libs\PowWeb\bin\Debug\net6.0-windows\PowWeb.dll</Reference>
  <Namespace>PowWeb._2_Actions._2_Cap.Ser</Namespace>
  <Namespace>PowWeb._2_Actions._2_Cap.Logic._3_DocMerging.Utils</Namespace>
</Query>

void Main()
{
	var file = @"C:\temp\PowWebTests\DocMerger-Data.json";
	var nfo = PowWebJsonUtils.Load<MergeNfoSer>(file);
	//CheckDistToCur(nfo);
	CheckMatching(nfo.Caps, nfo.CapAnchors);
	return;
	LevenshteinDistance.Calculate(
		"vlad",
		"vladvlad"
	).Dump();
}

void CheckMatching(string[] caps, string[] ancsArr)
{
	string Prc(string s) => s.Replace("/", "").Replace(@"\", "").Replace(":", "");
	//caps = caps.Select(Prc).ToArray();
	//ancsArr = ancsArr.Select(Prc).ToArray();
	
	var ancs = ancsArr.ToList();
	var matches = new List<(string, string, int)>();
	foreach (var cap in caps.Skip(1))
	{
		//var ancsDbg = ancs.OrderBy(anc => LevenshteinDistance.Calculate(anc, cap)).Select(anc => (cap, anc, LevenshteinDistance.Calculate(anc, cap))).ToArray();
		//ancsDbg.Dump();
		
		var matchingAnc = ancs.OrderBy(anc => LevenshteinDistance.Calculate(anc, cap)).FirstOrDefault();
		int dist;
		if (matchingAnc == null)
		{
			matchingAnc = "_";
			dist = -1;
		}
		else
		{
			ancs.Remove(matchingAnc);
			dist = LevenshteinDistance.Calculate(matchingAnc, cap);
		}
		matches.Add((cap, matchingAnc, dist));
	}
	matches.Dump();
}

void CheckDistToCur(MergeNfoSer nfo)
{
	nfo.Caps
	.OrderBy(cap => LevenshteinDistance.Calculate(cap, nfo.CurrentUrl))
	.Select(cap => new
	{
		Cap = cap,
		Dist = LevenshteinDistance.Calculate(cap, nfo.CurrentUrl)
	})
	.Dump();
	nfo.CurrentUrl.Dump();
}

record CapAndAnc(string Cap, string Anc);
record MergeNfoSer(
	string[] Caps,
	string[] CapAnchors,
	CapAndAnc[] Matches,
	string[] UnmatchedCaps,
	string[] UnmatchedCapAnchors,
	string CurrentUrl,
	string RootCap
);