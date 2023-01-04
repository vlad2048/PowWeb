using System.Reactive.Concurrency;
using System.Reactive.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PowWeb.ChromeApi.Utils.Attributes;
using PuppeteerSharp;

namespace PowWeb.ChromeApi.Utils;

static class SendExt
{
	public static R Send<R>(this CDPSession client, string cmd, object args) => client.SendAsync<R>(cmd, args).Result;
	public static R Send<R>(this CDPSession client, string cmd) => client.SendAsync<R>(cmd).Result;
	public static void Send(this CDPSession client, string cmd, object args) => client.SendAsync(cmd, args).Wait();
	public static void Send(this CDPSession client, string cmd) => client.SendAsync(cmd).Wait();
}

public static class WhenEventExt
{
	private static readonly Dictionary<CDPSession, IObservable<MessageEventArgs>> obsMap = new();
	private static readonly Lazy<IScheduler> scheduler = new(() => new TaskPoolScheduler(new TaskFactory()));
	private static IScheduler Scheduler => scheduler.Value;

	public static IObservable<T> WhenEvent<T>(this CDPSession client)
	{
		var evtName = ChromeEventAttribute.GetName<T>();
		return obsMap.GetOrCreate(
				client,
				() => Observable.FromEventPattern<MessageEventArgs>(
						e => client.MessageReceived += e,
						e => client.MessageReceived -= e
					)
					.Select(e => e.EventArgs)
			)
			.Where(e => e.MessageID == evtName)
			.Select(e => e.MessageData.Into<T>())
			.ObserveOn(Scheduler);
	}

	private static T Into<T>(this JToken token)
	{
		var str = JsonConvert.SerializeObject(token, Formatting.Indented);
		var val = JsonConvert.DeserializeObject<T>(str);
		return val!;
	}


	private static TValue GetOrCreate<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key, Func<TValue> createFun)
		where TKey : notnull
	{
		if (!dict.TryGetValue(key, out var val))
			val = dict[key] = createFun();
		return val;
	}
}