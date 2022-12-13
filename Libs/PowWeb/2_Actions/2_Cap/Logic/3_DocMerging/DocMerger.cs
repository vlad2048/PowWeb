using PowTrees.Algorithms;
using PowWeb._2_Actions._2_Cap.Logic._3_DocMerging.Utils;
using PowWeb._2_Actions._2_Cap.Structs;
using PowWeb.ChromeApi.Utils.Extensions;

namespace PowWeb._2_Actions._2_Cap.Logic._3_DocMerging;

/*
 * TODO:
 *
 * It turns out that DomSnaps[].DocumentUrl exactly match the page.Frame[].Url
 * So we can use the page.Frame hierarchy to massively restrict the search space
 * when looking for anchor points
 *
 */
static class DocMerger
{
	public static Cap Merge(this Cap[] caps, string currentUrl, bool enable)
		=> enable switch
		{
			true => Merge(caps, currentUrl),
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
	private static Cap Merge(Cap[] caps, string currentUrl)
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
}