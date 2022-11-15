using System.Reactive.Linq;
using DynamicData;
using PowRxVar;
using PowWeb._Internal.ChromeDevApi.DFetch;
using PowWeb._Internal.ChromeDevApi.DFetch.Events;
using PowWeb._Internal.ChromeDevApi.Utils;
using PowWeb._Internal.ChromeDevApi.Utils.Extensions;
using PuppeteerSharp;

namespace PowWeb.Utils;

class TargetNfo : IDisposable
{
	private readonly Disp d = new();
	public void Dispose() => d.Dispose();

	public Target Target { get; }
	public CDPSession Client { get; }
	public string Id => Target.TargetId;

	public IObservable<(TargetNfo, RequestPausedEvent)> WhenRequest { get; }

	public TargetNfo(Target target)
	{
		Target = target;
		Client = Target.CreateCDPSessionAsync().Result;
		/*var patterns = new RequestPattern[]
		{
			new(null, "Media", "Response")
		};
		Client.Fetch_Enable(patterns);*/

		Client.Fetch_Enable();

		WhenRequest = Client
			.WhenEvent<RequestPausedEvent>()
			.Do(evt =>
			{
				Task.Run(() =>
				{
					Client.Fetch_ContinueRequest(evt.RequestId);
				});
			})
			.Select(evt => (this, evt));
	}
}

class TargetTracker : IDisposable
{
	private readonly Disp d = new();
	public void Dispose() => d.Dispose();

	public IObservable<IChangeSet<TargetNfo, string>> Targets { get; }

	public TargetTracker(Browser browser)
	{
		var targetsSource = new SourceCache<TargetNfo, string>(e => e.Id).D(d);
		Targets = targetsSource
			.Connect()
			.DisposeMany();

		browser.WhenTargetCreated().Subscribe(e =>
		{
			var targetNfo = new TargetNfo(e.Target);
			targetsSource.AddOrUpdate(targetNfo);
		}).D(d);

		browser.WhenTargetDestroyed().Subscribe(e =>
		{
			targetsSource.RemoveKey(e.Target.TargetId);
		}).D(d);

		var targetsToAdd = browser.Targets()
			.SelectToArray(e => new TargetNfo(e));
		targetsSource.AddOrUpdate(targetsToAdd);
	}
}


static class TargetTrackerUtils
{
	public static IObservable<TargetChangedArgs> WhenTargetCreated(this Browser browser) => Observable.FromEventPattern<TargetChangedArgs>(e => browser.TargetCreated += e, e => browser.TargetCreated -= e).Select(e => e.EventArgs);
	public static IObservable<TargetChangedArgs> WhenTargetDestroyed(this Browser browser) => Observable.FromEventPattern<TargetChangedArgs>(e => browser.TargetDestroyed += e, e => browser.TargetDestroyed -= e).Select(e => e.EventArgs);
}