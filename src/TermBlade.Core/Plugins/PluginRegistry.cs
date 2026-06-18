using System;
using System.Collections.Generic;

namespace TermBlade.Core.Plugins
{
  /// <summary>Manages plugin registration lifecycle.</summary>
  public sealed class PluginRegistry
  {
    private readonly Dictionary<string, IPlugin> _plugins =
        new(StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// Register.
    /// </summary>
    /// <param name="plugin">The plugin value.</param>
    public void Register(IPlugin plugin)
    {
      if (_plugins.ContainsKey(plugin.Name))
        throw new InvalidOperationException($"Plugin '{plugin.Name}' is already registered.");
      _plugins[plugin.Name] = plugin;
      plugin.Register(this);
    }

    /// <summary>
    /// Unregister.
    /// </summary>
    /// <param name="name">The name value.</param>
    public void Unregister(string name)
    {
      if (_plugins.TryGetValue(name, out var plugin))
      {
        plugin.Unregister(this);
        _plugins.Remove(name);
      }
    }

    /// <summary>
    /// Gets the get.
    /// </summary>
    public IPlugin? Get(string name) => _plugins.TryGetValue(name, out var p) ? p : null;
    /// <summary>
    /// Gets the has.
    /// </summary>
    public bool Has(string name) => _plugins.ContainsKey(name);
    /// <summary>
    /// Gets the all.
    /// </summary>
    public IEnumerable<IPlugin> All => _plugins.Values;
  }
}
