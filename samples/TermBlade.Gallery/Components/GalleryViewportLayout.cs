namespace TermBlade.Gallery.Components;

public readonly record struct GalleryViewportLayout(int MenuCapacity, int ContentPanelHeight)
{
  private const int MenuVerticalChrome = 2;
  private const int ContentHeaderRows = 5;

  public static GalleryViewportLayout Create(int terminalHeight)
  {
    var height = Math.Max(1, terminalHeight);
    return new GalleryViewportLayout(
        Math.Max(1, height - MenuVerticalChrome),
        Math.Max(1, height - ContentHeaderRows));
  }
}

public enum GalleryContentTab
{
  Preview = 0,
  Source = 1
}

public static class GalleryContentTabNavigation
{
  private const int TabCount = 2;

  public static GalleryContentTab Move(GalleryContentTab current, int delta)
  {
    var next = ((int)current + delta) % TabCount;
    if (next < 0)
      next += TabCount;

    return (GalleryContentTab)next;
  }
}
