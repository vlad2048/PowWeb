namespace PowWeb.ChromeApi.Utils.Attributes;

class ChromeEventAttribute : Attribute
{
	public string Name { get; }
	public ChromeEventAttribute(string name)
	{
		Name = name;
	}

	public static string GetName<T>()
	{
		var evtType = typeof(T);
		var attr = evtType.CustomAttributes.FirstOrDefault(e => e.AttributeType == typeof(ChromeEventAttribute));
		if (attr == null || attr.ConstructorArguments.Count != 1) throw new ArgumentException("Invalid evt (1)");
		if (attr.ConstructorArguments.First().Value is not string cmdName) throw new ArgumentException("Invalid evt (2)");
		return cmdName;
	}
}