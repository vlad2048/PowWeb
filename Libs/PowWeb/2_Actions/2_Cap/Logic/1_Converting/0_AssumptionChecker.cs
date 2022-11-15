using PowWeb.ChromeApi.DDomSnapshot;
using PowWeb.ChromeApi.DDomSnapshot.Structs;

namespace PowWeb._2_Actions._2_Cap.Logic._1_Converting;

static class AssumptionChecker
{
	public static DomSnapshotApi.DomSnapshot_CaptureSnapshot_Ret CheckAssumptions(this DomSnapshotApi.DomSnapshot_CaptureSnapshot_Ret domSnap)
	{
		foreach (var domDoc in domSnap.Documents)
			CheckAssumptions(domDoc, domSnap.Strings);
		return domSnap;
	}

	private static void CheckAssumptions(DocumentSnapshot doc, string[] strArr)
	{
		var nodes = doc.Nodes;
		var strCnt = strArr.Length;

		void Check(bool cond)
		{
			if (!cond)
				throw new InvalidOperationException();
		}

		void CheckIsStrIndices(int[] arr) => Check(arr != null! && arr.All(e => e == -1 || (e >= 0 && e < strCnt)));

		Check(nodes.NodeType != null!);
		CheckIsStrIndices(nodes.NodeName);
		CheckIsStrIndices(nodes.NodeValue);
		foreach (var arr in nodes.Attributes)
			CheckIsStrIndices(arr);
	}
}