public static BatchRunNfo Extract_BatchRun(
  this Ctx ctx,
  string batchUrl,
  LinkResolveStrat batchImgLinkResolveStrat,
  int? maxPageCnt
)
{
  var videos = new List<Video>();

  var (videosToAdd, tcap, _)

  var (pageVideos, tcap, _) = ctx.Op_GetVideosOnPage(www => www.GotoUrl(batchUrl), May.Some(batchImgLinkResolveStrat));
  videos.AddRange(pageVideos);
  var (mayJumper, _) = tcap.Op_GetPageJumper();
  if (mayJumper.IsNone(out var jumper)) throw new ArgumentException("Could not find the IPageJumper");

  var lastPageIdx = jumper.PageKnowledge.GetLastPageIdx(maxPageCnt);
  for (var pageIdx = 1; pageIdx <= lastPageIdx; pageIdx++)
  {
    var (morePageVideos, _, _) = ctx.Op_GetVideosOnPage(() => jumper.GotoPage(ctx.Web, pageIdx), May.Some(batchImgLinkResolveStrat));
    videos.AddRange(morePageVideos);
  }

  return new BatchRunNfo(
    videos.ToArray(),
    lastPageIdx + 1
  );
}
