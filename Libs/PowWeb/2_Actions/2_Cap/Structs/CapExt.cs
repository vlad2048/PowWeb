namespace PowWeb._2_Actions._2_Cap.Structs;

public static class CapExt
{
	private const string AttrId = "id";
	private const string AttrClass = "class";

	/*public static Bitmap GetScreenshotBmp(this Cap cap)
	{
		var bytes = Convert.FromBase64String(cap.Screenshot);
		//using
		var ms = new MemoryStream(bytes);
		var img = Image.FromStream(ms);
		if (img is not Bitmap bmp) throw new InvalidOperationException();
		return bmp;
	}*/

	public static string? GetAttr(this CapNode node, string key)
	{
		var attr = node.Attrs.FirstOrDefault(e => e.Key == key);
		return attr switch
		{
			not null => attr.Val,
			null => null
		};
	}

	public static string? GetId(this CapNode node) => node.GetAttr(AttrId);
	public static string? GetClass(this CapNode node) => node.GetAttr(AttrClass);
}