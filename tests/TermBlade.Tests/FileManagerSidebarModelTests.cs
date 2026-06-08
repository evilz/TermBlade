using TermBlade.FileManager;
using TermBlade.FileManager.Components;

namespace TermBlade.Tests;

public class FileManagerSidebarModelTests
{
  [Fact]
  public void BuildGroups_UsesFavoritesHeaderAndOmitsHomeHeading()
  {
    var groups = FileManagerSidebarModel.BuildGroups(
        [
            new SidebarEntry("Home", "/home/user", SidebarEntryKind.Home),
            new SidebarEntry("Downloads", "/home/user/Downloads", SidebarEntryKind.Home),
            new SidebarEntry("TermBlade", "/repo", SidebarEntryKind.Pinned),
            new SidebarEntry("C", "C:\\", SidebarEntryKind.Disk)
        ]);

    Assert.Equal("Favorites", FileManagerSidebarModel.HeaderText);
    Assert.Equal("Favorites", groups[0].Title);
    Assert.True(groups[0].HideTitle);
    Assert.Equal(["Home", "Downloads"], groups[0].Entries.Select(entry => entry.Label));
    Assert.Equal("Pinned", groups[1].Title);
    Assert.Equal("Disks", groups[2].Title);
  }
}
