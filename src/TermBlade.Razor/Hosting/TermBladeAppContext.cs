using Microsoft.Extensions.Options;
using TermBlade.Core.Events;
using TermBlade.Core.Rendering;

namespace TermBlade.Razor.Hosting;

/// <summary>
/// Represents term blade razor options.
/// </summary>
public sealed class TermBladeRazorOptions
{
  /// <summary>
  /// Gets or sets the exit on ctrl c.
  /// </summary>
  public bool ExitOnCtrlC { get; set; } = true;
  /// <summary>
  /// Gets or sets the target fps.
  /// </summary>
  public int TargetFps { get; set; } = 30;
  /// <summary>
  /// Gets or sets the testing.
  /// </summary>
  public bool Testing { get; set; }
  /// <summary>
  /// Gets or sets the testing terminal width.
  /// </summary>
  public int? TestingWidth { get; set; }
  /// <summary>
  /// Gets or sets the testing terminal height.
  /// </summary>
  public int? TestingHeight { get; set; }
  /// <summary>
  /// Gets or sets the background color.
  /// </summary>
  public string? BackgroundColor { get; set; }
}

/// <summary>
/// Represents term blade app context.
/// </summary>
public sealed class TermBladeAppContext : IDisposable
{
  /// <summary>
  /// Gets the renderer.
  /// </summary>
  public CliRenderer Renderer { get; }
  /// <summary>
  /// Gets the key events.
  /// </summary>
  public EventEmitter KeyEvents => Renderer.KeyInput;

  /// <summary>
  /// Term blade app context.
  /// </summary>
  /// <param name="options">The options value.</param>
  public TermBladeAppContext(IOptions<TermBladeRazorOptions> options)
  {
    var settings = options.Value;
    Renderer = new CliRenderer(new CliRendererConfig
    {
      ExitOnCtrlC = settings.ExitOnCtrlC,
      TargetFps = settings.TargetFps,
      Testing = settings.Testing,
      TestingWidth = settings.TestingWidth,
      TestingHeight = settings.TestingHeight,
      BackgroundColor = settings.BackgroundColor
    });
  }

  /// <summary>
  /// Gets the request render.
  /// </summary>
  public void RequestRender() => Renderer.RequestRender();

  /// <summary>
  /// Gets the dispose.
  /// </summary>
  public void Dispose() => Renderer.Destroy();
}
