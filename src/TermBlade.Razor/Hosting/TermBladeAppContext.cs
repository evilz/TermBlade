using Microsoft.Extensions.Options;
using TermBlade.Core.Events;
using TermBlade.Core.Rendering;

namespace TermBlade.Razor.Hosting;

public sealed class TermBladeRazorOptions
{
    public bool ExitOnCtrlC { get; set; } = true;
    public int TargetFps { get; set; } = 30;
    public bool Testing { get; set; }
    public string? BackgroundColor { get; set; }
}

public sealed class TermBladeAppContext : IDisposable
{
    public CliRenderer Renderer { get; }
    public EventEmitter KeyEvents => Renderer.KeyInput;

    public TermBladeAppContext(IOptions<TermBladeRazorOptions> options)
    {
        var settings = options.Value;
        Renderer = new CliRenderer(new CliRendererConfig
        {
            ExitOnCtrlC = settings.ExitOnCtrlC,
            TargetFps = settings.TargetFps,
            Testing = settings.Testing,
            BackgroundColor = settings.BackgroundColor
        });
    }

    public void RequestRender() => Renderer.RequestRender();

    public void Dispose() => Renderer.Destroy();
}
