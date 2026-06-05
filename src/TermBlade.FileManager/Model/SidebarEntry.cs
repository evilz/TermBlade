namespace TermBlade.FileManager;

internal enum SidebarEntryKind
{
  Home,
  Pinned,
  Disk
}

internal readonly record struct SidebarEntry(string Label, string Path, SidebarEntryKind Kind);
