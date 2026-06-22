using TermBlade.Core.Ansi;
using TermBlade.Core.Renderables;
using TermBlade.Core.Rendering;

namespace TermBlade.Razor.Components;

/// <summary>
/// Renders formatted JSON text.
/// </summary>
public sealed class JsonText : RenderableComponentBase<JsonTextRenderable>
{
  [Parameter] public string Json { get; set; } = string.Empty;

  protected override JsonTextRenderable CreateRenderable(CliRenderer renderer) => new(renderer);

  protected override void ApplyParameters(JsonTextRenderable renderable) => renderable.SetJson(Json);
}
