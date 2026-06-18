using TermBlade.Core.Ansi;
using TermBlade.Core.Renderables;
using TermBlade.FileManager;

namespace TermBlade.FileManager.Components;

internal static class FileManagerViewSegments
{
  /// <summary>
  /// Sidebar header.
  /// </summary>
  public static IReadOnlyList<SegmentedTextSegment> SidebarHeader =>
  [
      new("\udb80\udc4b ", FileManagerTheme.Accent),
      new(FileManagerSidebarModel.HeaderText, FileManagerTheme.Accent, Attributes: TextAttributes.Bold)
  ];

  /// <summary>
  /// Search.
  /// </summary>
  public static IReadOnlyList<SegmentedTextSegment> Search =>
  [
      new(" ", FileManagerTheme.Green),
      new("(/) Type something", FileManagerTheme.Muted)
  ];

  /// <summary>
  /// Sidebar group.
  /// </summary>
  /// <param name="title">The title value.</param>
  public static IReadOnlyList<SegmentedTextSegment> SidebarGroup(string title) =>
  [
      new(GetSidebarGroupIcon(title), FileManagerTheme.Accent),
      new(title, FileManagerTheme.Accent),
      new(" ─────────", FileManagerTheme.FrameBorder)
  ];

  private static string GetSidebarGroupIcon(string title)
    => title switch
    {
      "Pinned" => " ",
      "Disks" => "󰋊 ",
      _ => string.Empty
    };

  /// <summary>
  /// Sidebar entry.
  /// </summary>
  /// <param name="entry">The entry value.</param>
  /// <param name="selected">The selected value.</param>
  public static IReadOnlyList<SegmentedTextSegment> SidebarEntry(SidebarEntry entry, bool selected)
  {
    var icon = entry.Kind switch
    {
      SidebarEntryKind.Home => entry.Label switch
      {
        "Home" => " ",
        "Downloads" => "󰇚 ",
        "Documents" => "󰈙 ",
        "Pictures" => " ",
        "Music" => " ",
        _ => "󰉋 "
      },
      SidebarEntryKind.Disk => "  ",
      _ => "  "
    };
    var label = entry.Kind == SidebarEntryKind.Disk ? $"  {entry.Label}:" : $"{icon}{entry.Label}";
    return [new(selected ? "❯ " : "  ", FileManagerTheme.ActiveBorder), new(label, selected ? FileManagerTheme.Text : FileManagerTheme.Muted)];
  }

  /// <summary>
  /// Entry.
  /// </summary>
  /// <param name="entry">The entry value.</param>
  /// <param name="selected">The selected value.</param>
  public static IReadOnlyList<SegmentedTextSegment> Entry(FileManagerEntry entry, bool selected, bool compact = false)
  {
    var icon = entry.IsDirectory ? "󰉋 " : "󰈔 ";
    var prefix = GetEntryPrefix(selected, compact);
    return
    [
        new(prefix, FileManagerTheme.ActiveBorder),
        new(icon, entry.IsDirectory ? FileManagerTheme.Green : FileManagerTheme.Muted),
        new(entry.Name, selected ? FileManagerTheme.Text : FileManagerTheme.Muted)
    ];
  }

  private static string GetEntryPrefix(bool selected, bool compact)
  {
    if (compact)
      return string.Empty;

    return selected ? "❯ " : "  ";
  }

  /// <summary>
  /// Status.
  /// </summary>
  /// <param name="content">The content value.</param>
  public static IReadOnlyList<SegmentedTextSegment> Status(string content)
  {
    var color = content.StartsWith('✖') ? FileManagerTheme.Muted : FileManagerTheme.Text;
    return [new(content, color)];
  }

  /// <summary>
  /// Command.
  /// </summary>
  /// <param name="state">The state value.</param>
  public static IReadOnlyList<SegmentedTextSegment> Command(FileManagerState state)
  {
    if (state.Focus == FileManagerFocus.Prompt)
      return [new(state.PromptMode == PromptMode.Shell ? ": " : "> ", FileManagerTheme.Accent), new(state.PromptText, FileManagerTheme.Text)];

    return [new(" ↑↓/kj navigate  ↵/l open  h parent  s/p/m focus  : shell  > spf  f preview  ctrl+c/x/v/d ops  q quit", FileManagerTheme.Muted)];
  }
}
