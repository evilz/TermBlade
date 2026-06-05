namespace TermBlade.Gallery.Components;

public readonly record struct GalleryMenuWindow(int StartIndex, int Count)
{
  public static GalleryMenuWindow Create(int totalCount, int selectedIndex, int capacity)
  {
    if (totalCount <= 0 || capacity <= 0)
      return new GalleryMenuWindow(0, 0);

    var visibleCount = Math.Min(totalCount, capacity);
    var clampedSelectedIndex = Math.Clamp(selectedIndex, 0, totalCount - 1);
    var startIndex = Math.Clamp(clampedSelectedIndex - visibleCount + 1, 0, totalCount - visibleCount);

    return new GalleryMenuWindow(startIndex, visibleCount);
  }
}
