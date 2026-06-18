using TermBlade.FileManager;

namespace TermBlade.FileManager.Components;

internal readonly record struct SidebarGroup(string Title, SidebarEntryKind Kind, IReadOnlyList<SidebarEntry> Entries, bool HideTitle = false);

internal static class FileManagerSidebarModel
{
  public const string HeaderText = "Favorites";

  /// <summary>
  /// Build groups.
  /// </summary>
  /// <param name="entries">The entries value.</param>
  public static IReadOnlyList<SidebarGroup> BuildGroups(IReadOnlyList<SidebarEntry> entries)
  {
    var groups = new List<SidebarGroup>();
    AddGroup(groups, HeaderText, SidebarEntryKind.Home, entries, hideTitle: true);
    AddGroup(groups, "Pinned", SidebarEntryKind.Pinned, entries);
    AddGroup(groups, "Disks", SidebarEntryKind.Disk, entries);
    return groups;
  }

  private static void AddGroup(
      List<SidebarGroup> groups,
      string title,
      SidebarEntryKind kind,
      IReadOnlyList<SidebarEntry> entries,
      bool hideTitle = false)
  {
    var groupEntries = entries.Where(entry => entry.Kind == kind).ToList();
    if (groupEntries.Count > 0)
      groups.Add(new SidebarGroup(title, kind, groupEntries, hideTitle));
  }
}
