using PowTrees.Algorithms;
using PowWeb._2_Actions._2_Cap.Structs;
using PowWeb._2_Actions._2_Cap.Structs.Enums;
using PowWeb.ChromeApi.Utils.Extensions;

namespace PowWeb._2_Actions._2_Cap.Logic._2_Simplifying;

static class Simplifier
{
	private const bool EnableCaptureBodyOnly = true;
	private const bool EnableRemoveNodesWithNoLayout = true;
	private const bool EnableRemoveLeafTextNodesWithNoText = true;
	private static readonly string[] NodeNamesToRemove = { "#comment" };

	public static Cap[] Simplify(this Cap[] docs)
		=> docs.SelectToArray(SimplifyDoc);

	private static Cap SimplifyDoc(Cap doc)
		=> doc with
		{
			Root = FilterRoot(doc.Root)
		};


	private static N FilterRoot(N root)
		=> root
			.CaptureBodyOnly()
			.RemoveNodesWithNoLayout()
			.RemoveLeafTextNodesWithNoText()
			.RemoveUselessNodeNames();



	private static N CaptureBodyOnly(this N root)
	{
		if (!EnableCaptureBodyOnly) return root;
		var bodyN = root.FirstOrDefault(e => e.V.Name == "BODY");
		return bodyN switch
		{
			not null => bodyN,
			null => root
		};
	}

	private static N RemoveNodesWithNoLayout(this N root)
	{
		if (!EnableRemoveNodesWithNoLayout) return root;
		return root
			.Filter(
				n => !n.Bounds.IsDegenerate,
				filterOpt => filterOpt.AlwaysKeepRoot = true
			)
			.Single();
	}

	private static N RemoveLeafTextNodesWithNoText(this N root)
	{
		if (!EnableRemoveLeafTextNodesWithNoText) return root;
		static bool DoWeKeepThisNode(N nod)
		{
			if (nod.Children.Any()) return true;
			if (nod.V.NodeType != CapNodeType.Text) return true;
			if (nod.V.Text != string.Empty) return true;
			return false;
		}

		return root
			.FilterN(
				DoWeKeepThisNode,
				filterOpt => filterOpt.AlwaysKeepRoot = true
			)
			.Single();
	}

	private static N RemoveUselessNodeNames(this N root) =>
		root
			.Filter(
				n => !NodeNamesToRemove.Contains(n.Name),
				filterOpt => filterOpt.AlwaysKeepRoot = true
			)
			.Single();
}