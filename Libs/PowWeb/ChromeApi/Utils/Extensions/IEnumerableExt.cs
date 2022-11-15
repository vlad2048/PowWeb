namespace PowWeb.ChromeApi.Utils.Extensions;

static class IEnumerableExt
{
	public static U[] SelectToArray<T, U>(this IEnumerable<T> source, Func<T, U> mapFun) => source.Select(mapFun).ToArray();
	public static T[] WhereToArray<T>(this IEnumerable<T> source, Func<T, bool> predicate) => source.Where(predicate).ToArray();
}