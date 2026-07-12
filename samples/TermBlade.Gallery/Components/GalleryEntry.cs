namespace TermBlade.Gallery.Components;

/// <summary>
/// Describes a single gallery entry (component demo).
/// </summary>
public sealed record GalleryEntry(
    string Name,
    string Description,
    string SourceFile,
    string RazorCode)
{
  /// <summary>
  /// Gets the Razor component type used to render this entry's preview.
  /// The gallery convention maps an entry named <c>Box</c> to <c>BoxDemo</c>.
  /// </summary>
  public Type ComponentType
    => typeof(GalleryEntry).Assembly.GetType(
        $"TermBlade.Gallery.Components.Demos.{Name}Demo",
        throwOnError: true)!;
}
