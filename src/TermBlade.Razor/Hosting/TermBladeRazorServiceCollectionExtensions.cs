using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace TermBlade.Razor.Hosting;

public static class TermBladeRazorServiceCollectionExtensions
{
    extension(IServiceProvider services)
    {
        public bool HasTermBladeRazorServices =>
            services.GetService<IServiceProviderIsService>() is { } serviceChecker &&
            serviceChecker.IsService(typeof(TermBladeAppContext)) &&
            serviceChecker.IsService(typeof(NoopComponentRenderer));
    }

    extension(IServiceCollection services)
    {
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
