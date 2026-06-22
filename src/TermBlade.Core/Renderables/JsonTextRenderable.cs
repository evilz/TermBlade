using System.Text.Json;
using TermBlade.Core.Ansi;
using TermBlade.Core.Input;
using TermBlade.Core.Layout;
using TermBlade.Core.Rendering;

namespace TermBlade.Core.Renderables;

/// <summary>
/// Renders JSON text with stable indentation.
/// </summary>
public class JsonTextRenderable : CodeRenderable
{
  /// <summary>
  /// Initializes a new instance of the <see cref="JsonTextRenderable"/> class.
  /// </summary>
  /// <param name="renderer">The renderer that owns this renderable.</param>
  public JsonTextRenderable(CliRenderer? renderer) : base(renderer)
  {
  }

  /// <summary>
  /// Sets JSON content, formatting valid JSON when possible.
  /// </summary>
  /// <param name="json">The JSON content to display.</param>
  public void SetJson(string json)
  {
    try
    {
      var value = JsonSerializer.Deserialize<JsonElement>(json);
      Content = JsonSerializer.Serialize(value, new JsonSerializerOptions { WriteIndented = true });
    }
    catch (JsonException)
    {
      Content = json;
    }
  }
}
