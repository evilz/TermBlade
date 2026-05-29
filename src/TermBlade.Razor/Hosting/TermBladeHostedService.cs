using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Hosting;

namespace TermBlade.Razor.Hosting;

internal sealed class TermBladeHostedService<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TComponent>(
    TermBladeAppContext app,
    NoopComponentRenderer renderer,
    IHostApplicationLifetime lifetime) : BackgroundService where TComponent : IComponent
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var registration = stoppingToken.Register(app.Renderer.Destroy);

        try
        {
            await renderer.MountComponentAsync<TComponent>().ConfigureAwait(false);
            await Task.Run(app.Renderer.Start).ConfigureAwait(false);
        }
        finally
        {
            app.Renderer.Destroy();
            lifetime.StopApplication();
        }
    }
}
