using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace TermBlade.Razor.Hosting;

/// <summary>
/// Represents term blade host builder extensions.
/// </summary>
public static class TermBladeHostBuilderExtensions
{
  extension(IHostBuilder hostBuilder)
  {
    /// <summary>
    /// Dynamically accessed members.
    /// </summary>
    /// <param name="TComponent>(">The TComponent>( value.</param>
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
