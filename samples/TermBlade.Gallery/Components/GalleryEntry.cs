namespace TermBlade.Gallery.Components;

/// <summary>
/// Describes a single gallery entry (component demo).
/// </summary>
public sealed record GalleryEntry(
    string Name,
    string Description,
    string SourceFile,
    string RazorCode);
