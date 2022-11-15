public static (Video[], TCap, LinkResolveStrat) Op_GetVideosOnPage(
  this Ctx ctx,
  Action<WebInst> gotoPageFun,
  Maybe<LinkResolveStrat> mayImgLinkResolveStrat
) =>
  web.Exec((www, d) =>
  {
    var imgListener = new Stream_ImageListener(ctx).D(d);
    gotoPageFun(www);
    var imgNetLinks = imgListener.DisposeAndGetNetLinks();
    var tcap = www.Cap().Detect(PageModelNames.PaginatedVideoList);

    var videos = Proc.ParseVideos(tcap);

    var imgLinkResolveStrat = mayImgLinkResolveStrat.IsSome(out var stratVal) switch
    {
      true => stratVal!,
      false => videos.Any() switch
      {
        true => ImgLinkFixer.FindResolveStrat(new DomLink(LinkType.Image, videos[0].ImgUrl), imgNetLinks, new RestClient()),
        false => LinkResolveStrat.MakeFailed("No videos found")
      }
    };

    var (fixedVideos, imgLinkResolveStratUpdate) = ImgLinkFixer.FixWithResolveStrat(videos, imgNetLinks, imgLinkResolveStrat, ctx.LinkForwarder);

    return (fixedVideos, tcap, imgLinkResolveStratUpdate);
  });
