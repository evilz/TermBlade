using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace TermBlade.Razor.Hosting;

/// <summary>
/// Represents term blade razor service collection extensions.
/// </summary>
public static class TermBladeRazorServiceCollectionExtensions
{
  extension(IServiceProvider services)
  {
    /// <summary>
    /// Has term blade razor services.
    /// </summary>
    public bool HasTermBladeRazorServices =>
        services.GetService<IServiceProviderIsService>() is { } serviceChecker &&
        serviceChecker.IsService(typeof(TermBladeAppContext)) &&
        serviceChecker.IsService(typeof(NoopComponentRenderer));
  }

  extension(IServiceCollection services)
  {
    /// <summary>
    /// Add term blade razor.
    /// </summary>
    public IServiceCollection AddTermBladeRazor()
    {
      services.AddOptions<TermBladeRazorOptions>();
      services.TryAddSingleton<IComponentActivator, TermBladeComponentActivator>();
      services.TryAddSingleton<TermBladeAppContext>();
      services.TryAddSingleton<NoopComponentRenderer>();
      return services;
    }
  }
}
