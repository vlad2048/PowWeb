using System.Text.Json;
using PowBasics.Geom.Serializers;
using PowTrees.Serializer;
using PowWeb._2_Actions._2_Cap.Structs;

namespace PowWeb._2_Actions._2_Cap.Ser;

public class PowWebJsonUtils
{
	public static JsonSerializerOptions JsonOpt { get; } = new()
	{
		WriteIndented = true
	};
	static PowWebJsonUtils()
	{
		JsonOpt.Converters.Add(new TNodSerializer<CapNode>());
		JsonOpt.Converters.Add(new PtSerializer());
		JsonOpt.Converters.Add(new RSerializer());
		JsonOpt.Converters.Add(new SzSerializer());
	}

	public static void Save<T>(string file, T obj)
	{
		var str = JsonSerializer.Serialize(obj, JsonOpt);
		File.WriteAllText(file, str);
	}

	public static T Load<T>(string file)
	{
		var str = File.ReadAllText(file);
		return LoadFromString<T>(str);
	}

	public static T LoadFromString<T>(string str) => JsonSerializer.Deserialize<T>(str, JsonOpt)!;
}