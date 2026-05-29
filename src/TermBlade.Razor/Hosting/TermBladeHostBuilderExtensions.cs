using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace TermBlade.Razor.Hosting;

public static class TermBladeHostBuilderExtensions
{
  extension(IHostBuilder hostBuilder)
  {
    public IHostBuilder UseTermBladeRazor<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TComponent>()
        where TComponent : IComponent
    {
      hostBuilder.ConfigureServices(services =>
      {
        services.AddTermBladeRazor();
        services.AddHostedService<TermBladeHostedService<TComponent>>();
      });

      return hostBuilder;
    }
  }
}
