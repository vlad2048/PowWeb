using System.Text.Json;
using PowTrees.Algorithms;
using PowWeb._2_Actions._2_Cap.Logic._3_DocMerging.Utils;
using PowWeb._2_Actions._2_Cap.Structs;
using PowWeb.ChromeApi.Utils.Extensions;

namespace PowWeb._2_Actions._2_Cap.Logic._3_DocMerging;

static class DocMerger
{
	public static Cap Merge(this Cap[] caps, string currentUrl, bool enable, string? logFolder)
		=> enable switch
		{
			true => Merge(caps, currentUrl, logFolder),
			false => SelectSingleCap(caps, currentUrl)
		};

	private static Cap SelectSingleCap(Cap[] caps, string currentUrl)
		=> caps
			.OrderBy(cap => LevenshteinDistance.Calculate(cap.Url, currentUrl))
			.First();


	// ***********
	// * Structs *
	// ***********
	private record CapAnchor(
		N Nod,
		string Src
	)
	{
		public override string ToString() => $@"Anchor: [{Nod.V.Index}] ""{Src}""";
	}


	// ****************************
	// * Anchoring Caps Algorithm *
	// ****************************
	private static Cap Merge(Cap[] caps, string currentUrl, string? logFolder)
	{
		var rootCap = SelectSingleCap(caps, currentUrl);
		var ancs = (
			from cap in caps
			from nod in cap.Root
			where nod.V.Name == "IFRAME"
			select new CapAnchor(nod, nod.V.GetAttr("src") ?? string.Empty)
		).ToList();



		var matches = caps
			.Where(cap => cap != rootCap)
			.Select(cap =>
			{
				var matchingAnc = ancs.MinBy(anc => LevenshteinDistance.Calculate(anc.Src, cap.Url));
				if (matchingAnc != null)
				{
					ancs.Remove(matchingAnc);
					return (cap, matchingAnc);
				}
				else
				{
					return ((Cap?)null, (CapAnchor?)null);
				}
			})
			.Where(t => t != (null, null))
			.SelectToArray(t => (t.Item1!, t.Item2!));


		foreach (var match in matches)
		{
			var (cap, anc) = match;
			
			var idxStart = anc.Nod.GoUpToRoot().Max(e => e.V.Index) + 1;
			var reindexedRoot = cap.Root.Map(e => e with { Index = e.Index + idxStart });
			anc.Nod.AddChild(reindexedRoot);
		}

		return rootCap;
	}


	/*
	private record MatchResult(
		List<(Cap, CapAnchor)> Matches,
		Cap[] UnmatchedCaps,
		CapAnchor[] UnmatchedCapAnchors
	)
	{
		public override string ToString() => $"Matches:{Matches.Count}  (Unmatched  Caps:{UnmatchedCaps.Length}  CapAnchors:{UnmatchedCapAnchors.Length})";
	}


	// ****************************
	// * Anchoring Caps Algorithm *
	// ****************************
	private static Cap Merge(Cap[] caps, string currentUrl, string? logFolder)
	{
		var rootCap = SelectSingleCap(caps, currentUrl);
		var capAnchors = (
			from cap in caps
			from nod in cap.Root
			where nod.V.Name == "IFRAME"
			select new CapAnchor(nod, nod.V.GetAttr("src") ?? string.Empty)
		).ToArray();

		var res = MatchWithAll(
			capAnchors,
			caps.WhereToArray(cap => cap != rootCap)
		);

		LogProcess(logFolder, new MergeNfo(caps, capAnchors, res, currentUrl, rootCap));

		foreach (var match in res.Matches)
		{
			var (root, anchor) = match;
			
			var idxStart = anchor.Nod.GoUpToRoot().Max(e => e.V.Index) + 1;
			var reindexedRoot = root.Root.Map(e => e with { Index = e.Index + idxStart });
			anchor.Nod.AddChild(reindexedRoot);
		}

		return rootCap;
	}

	private record MergeNfo(
		Cap[] Caps,
		CapAnchor[] CapAnchors,
		MatchResult Res,
		string CurrentUrl,
		Cap RootCap
	)
	{
		public MergeNfoSer ToNfoSer() => new(
			Caps.SelectToArray(e => e.Url),
			CapAnchors.SelectToArray(e => e.Src),
			Res.Matches.SelectToArray(e => new CapAndAnc(e.Item1.Url, e.Item2.Src)),
			Res.UnmatchedCaps.SelectToArray(e => e.Url),
			Res.UnmatchedCapAnchors.SelectToArray(e => e.Src),
			CurrentUrl,
			RootCap.Url
		);
	}

	private record CapAndAnc(string Cap, string Anc);
	private record MergeNfoSer(
		string[] Caps,
		string[] CapAnchors,
		CapAndAnc[] Matches,
		string[] UnmatchedCaps,
		string[] UnmatchedCapAnchors,
		string CurrentUrl,
		string RootCap
	);

	private static void LogProcess(string? logFolder, MergeNfo nfo)
	{
		if (logFolder == null) return;

		var fileLog = Path.Combine(logFolder, "DocMerger-Log.txt");
		var fileData = Path.Combine(logFolder, "DocMerger-Data.json");

		File.WriteAllText(fileData, JsonSerializer.Serialize(nfo.ToNfoSer(), new JsonSerializerOptions{WriteIndented = true}));

		using var fs = File.OpenWrite(fileLog);
		using var sw = new StreamWriter(fs);

		void Log(string s) => sw.WriteLine($"  {s}");
		void LogTitle(string s) { sw.WriteLine(s); sw.WriteLine(new string('=', s.Length)); }

		void LogArr<T>(string name, IEnumerable<T> source, Func<T, string> fmtFun)
		{
			var arr = source.ToArray();
			LogTitle($"{name}: {arr.Length}");
			for (var idx = 0; idx < arr.Length; idx++)
			{
				var elt = arr[idx];
				Log($"[{idx:D2}] - '{fmtFun(elt)}'");
			}
			Log("");
		}

		LogArr("Caps", nfo.Caps, e => e.Url.Quote());
		LogArr("Anchors", nfo.CapAnchors, e => e.Src.Quote());

		Log("");
		LogTitle($"=> Result   Matches:{nfo.Res.Matches.Count}   UnmatchedCaps:{nfo.Res.UnmatchedCaps.Length}   UnmatchedAnchors:{nfo.Res.UnmatchedCapAnchors.Length}");
		Log("");

		LogArr("Matches", nfo.Res.Matches, e => $"cap:'{e.Item1.Url}' <-> anchor:'{e.Item2.Src}'");
		LogArr("UnmatchedCaps", nfo.Res.UnmatchedCaps, e => e.Url.Quote());
		LogArr("UnmatchedAnchrs", nfo.Res.UnmatchedCapAnchors, e => e.Src.Quote());

	}

	private static string Quote(this string s) => $"'{s}'";




	private static MatchResult MatchWithAll(CapAnchor[] anchors, Cap[] caps)
	{
		Func<string, string>[] mapFuncs =
		{
			MergeUrlUtils.Id,
			MergeUrlUtils.RemoveUrlParams,
			MergeUrlUtils.GetDomain,
			MergeUrlUtils.True,
		};

		var matches = new MatchResult[mapFuncs.Length];

		var matchPrev = new MatchResult(new List<(Cap, CapAnchor)>(), caps, anchors);

		for (var i = 0; i < mapFuncs.Length; i++)
		{
			var mapFun = mapFuncs[i];
			var matchNext = MatchWith(matchPrev, MakeCmp(mapFun));
			matches[i] = matchNext;
			matchPrev = matchNext;
		}

		return matchPrev;





		static Func<string, string, bool> MakeCmp(Func<string, string> mapFun) => (a, b) => string.Compare(mapFun(a), mapFun(b), StringComparison.OrdinalIgnoreCase) == 0;


		static MatchResult MatchWith(MatchResult srcData, Func<string, string, bool> matchFun)
		{
			var dstData = MatchAnchorsToRoots(srcData.UnmatchedCaps, srcData.UnmatchedCapAnchors, matchFun);
			dstData.Matches.AddRange(srcData.Matches);
			return dstData;
		}


		static MatchResult MatchAnchorsToRoots(Cap[] rootsArr, CapAnchor[] anchorsArr, Func<string, string, bool> matchFun)
		{
			bool IsMatch(string? anchorName, string rootName)
			{
				if (anchorName == null) return string.IsNullOrWhiteSpace(rootName);
				return matchFun(
					MergeUrlUtils.PreProcess(anchorName),
					MergeUrlUtils.PreProcess(rootName)
				);
			}

			var anchors = anchorsArr.ToList();
			var roots = rootsArr.ToList();
			var matches = new List<(Cap, CapAnchor)>();
			var unmatchedAnchors = new List<CapAnchor>();
			foreach (var anchor in anchors)
			{
				var matchedRoot = roots.FirstOrDefault(root => IsMatch(anchor.Src, root.Url));
				if (matchedRoot != null)
				{
					matches.Add((matchedRoot, anchor));
					roots.Remove(matchedRoot);
				}
				else
				{
					unmatchedAnchors.Add(anchor);
				}
			}
			return new MatchResult(
				matches,
				roots.ToArray(),
				unmatchedAnchors.ToArray()
			);
		}
	}*/

}