using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;

namespace TermBlade.Razor.Hosting;

internal sealed class TermBladeComponentActivator(IServiceProvider services) : IComponentActivator
{
  /// <summary>
  /// Create instance.
  /// </summary>
  /// <param name="componentType">The componentType value.</param>
  public IComponent CreateInstance(Type componentType)
      => (IComponent)ActivatorUtilities.CreateInstance(services, componentType);
}
