using TermBlade.Gallery.Components;

namespace TermBlade.Tests;

public class GalleryLayoutTests
{
  [Theory]
  [InlineData(1, 1)]
  [InlineData(2, 1)]
  [InlineData(24, 22)]
  [InlineData(60, 58)]
  public void GalleryViewportLayout_MenuCapacityUsesFullConsoleHeightMinusBorderPadding(int terminalHeight, int expectedCapacity)
  {
    var layout = GalleryViewportLayout.Create(terminalHeight);

    Assert.Equal(expectedCapacity, layout.MenuCapacity);
  }

  [Fact]
  public void GalleryViewportLayout_ContentAreaFillsRemainingConsoleHeight()
  {
    var layout = GalleryViewportLayout.Create(terminalHeight: 24);

    Assert.Equal(19, layout.ContentPanelHeight);
  }

  [Theory]
  [InlineData(GalleryContentTab.Preview, 1, GalleryContentTab.Source)]
  [InlineData(GalleryContentTab.Source, 1, GalleryContentTab.Preview)]
  [InlineData(GalleryContentTab.Preview, -1, GalleryContentTab.Source)]
  [InlineData(GalleryContentTab.Source, -1, GalleryContentTab.Preview)]
  public void GalleryContentTabNavigation_WrapsBetweenPreviewAndSource(
      GalleryContentTab current,
      int delta,
      GalleryContentTab expected)
  {
    Assert.Equal(expected, GalleryContentTabNavigation.Move(current, delta));
  }
}
