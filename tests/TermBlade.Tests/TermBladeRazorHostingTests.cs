using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TermBlade.Razor.Hosting;

namespace TermBlade.Tests;

public class TermBladeRazorHostingTests
{
  [Fact]
  public async Task HostedService_ShutsDownPromptly_WhenHostStops()
  {
    using var host = Host.CreateDefaultBuilder()
        .UseTermBladeRazor<TestComponent>()
        .ConfigureServices(services =>
        {
          services.Configure<TermBladeRazorOptions>(options => options.Testing = true);
        })
        .Build();

    await host.StartAsync();

    var stopTask = host.StopAsync();
    await stopTask.WaitAsync(TimeSpan.FromSeconds(2));
  }

  [Fact]
  public void UseTermBladeRazor_RegistersRequiredServices()
  {
    using var host = Host.CreateDefaultBuilder()
        .UseTermBladeRazor<TestComponent>()
        .Build();

    var services = host.Services;

    Assert.True(services.HasTermBladeRazorServices);
    Assert.Contains(services.GetServices<IHostedService>(), service => service.GetType().Name.Contains("TermBladeHostedService"));
  }

  [Fact]
  public void HasTermBladeRazorServices_DoesNotConstructRegisteredServices()
  {
    using var services = new ServiceCollection()
        .AddTermBladeRazor()
        .BuildServiceProvider();

    Assert.True(services.HasTermBladeRazorServices);
  }

  private sealed class TestComponent : IComponent
  {
    private RenderHandle _renderHandle;

    public void Attach(RenderHandle renderHandle) => _renderHandle = renderHandle;

    public Task SetParametersAsync(ParameterView parameters)
    {
      _renderHandle.Render(_ => { });
      return Task.CompletedTask;
    }
  }
}
