using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;

namespace TermBlade.Razor.Hosting;

internal sealed class TermBladeComponentActivator(IServiceProvider services) : IComponentActivator
{
  public IComponent CreateInstance(Type componentType)
      => (IComponent)ActivatorUtilities.CreateInstance(services, componentType);
}
