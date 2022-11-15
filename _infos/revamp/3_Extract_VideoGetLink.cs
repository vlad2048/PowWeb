private static readonly WebExecOpt opt = new(
  retryOn: err => err.Type is WebErr.BrowserClosed
  retryCount: 3,
);

private static Maybe<ResolvedLink> GetResolvedLink(
  this Ctx ctx,
  string videoPageUrl
) =>
  ctx.Web.Exec(www =>
  {
    // *********
    // * Setup *
    // *********
    // Setup the result variable
    Maybe<ResolvedLink>? resolvedLink = null;
    void Finish() { if (resolvedLink == null) resolvedLink = May.None<ResolvedLink>(); }
    bool IsFinished() => resolvedLink != null;

    // Listen to NetLinks
    using var d = new Disp();
    var (subjLink, whenLink) = RxEventMaker.Make<ILink>().D(d);
    void CheckLink(ILink link) => subjLink.OnNext(link);
    ctx.Web.Page.WhenRequest(ResourceType.Media).Select(MakeLink).Subscribe(CheckLink).D(d);

    // Check both incoming NetLinks and DomLinks and finish when we manage to resolve one
    whenLink
      .Select(VidLinkFixer.FixLink)
      .WhereSome()
      .Subscribe(e => resolvedLink = May.Some(e)).D(d);

    // Give up if it's been too long
    Observable.Timer(TimeoutWaitTime).Subscribe(_ => Finish()).D(d);


    // *************
    // * Goto Page *
    // *************
    www.GotoUrl(videoPageUrl);


    // **********************************************************
    // * Keep clicking on the video hoping it'll start playing  *
    // * and we'll see it either in the DOM or network requests *
    // **********************************************************
    do
    {
      // Look for DomLinks
      var deepCap = ctx.Gen_DeepCap();
      var mayDomLink = Proc.FindVideoSrc(deepCap);
      if (mayDomLink.IsSome(out var domLink)) CheckLink(domLink);

      // Click in the right places
      var tcap = ctx.Gen_TCap(PageModelNames.VideoDetails);
      var mayVidNod = Proc.FindVidNod(tcap);
      if (mayVidNod.IsNone(out var vidNod)) throw new ArgumentException("Failed to find the VidNod");
      var nodsToClick = Proc.FindNodsToClickToPlayVid(vidNod);
      foreach (var nod in nodsToClick) www.Click(nod);

      // Sleep a bit before trying again
      if (IsFinished()) break;
      Sleep((int)ClickPostWaitTime.TotalMilliseconds);
    } while (!IsFinished());

    return resolvedLink!;
  });

