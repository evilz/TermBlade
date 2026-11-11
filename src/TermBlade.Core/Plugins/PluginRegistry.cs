using System;
using System.Collections.Generic;

namespace TermBlade.Core.Plugins
{
  /// <summary>Manages plugin registration lifecycle.</summary>
  public sealed class PluginRegistry
  {
    private readonly Dictionary<string, IPlugin> _plugins =
        new(StringComparer.OrdinalIgnoreCase);
    private readonly HashSet<string> _registering = new(StringComparer.OrdinalIgnoreCase);
    private readonly HashSet<string> _unregistering = new(StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// Registers a plugin and makes it visible to its registration callback.
    /// </summary>
    /// <param name="plugin">The plugin value.</param>
    /// <remarks>
    /// If the callback throws, this registry removes the plugin entry. It cannot undo
    /// external side effects performed by the callback. The registry owns membership,
    /// but does not own or dispose the plugin instance.
    /// </remarks>
    /// <exception cref="ArgumentNullException">The plugin is null.</exception>
    /// <exception cref="ArgumentException">The plugin name is null, empty, or whitespace.</exception>
    /// <exception cref="InvalidOperationException">A plugin with this name is already registered.</exception>
    public void Register(IPlugin plugin)
    {
      ArgumentNullException.ThrowIfNull(plugin);
      var name = ValidateName(plugin.Name, nameof(plugin));
      if (!_plugins.TryAdd(name, plugin))
        throw new InvalidOperationException($"Plugin '{name}' is already registered.");

      _registering.Add(name);
      try
      {
        plugin.Register(this);
      }
      catch
      {
        if (_plugins.TryGetValue(name, out var registered) && ReferenceEquals(registered, plugin))
          _plugins.Remove(name);
        throw;
      }
      finally
      {
        _registering.Remove(name);
      }
    }

    /// <summary>
    /// Unregisters a plugin after its cleanup callback succeeds.
    /// </summary>
    /// <param name="name">The name value.</param>
    /// <remarks>
    /// The plugin remains registered while its callback runs. If the callback throws,
    /// the entry stays in the registry so cleanup can be retried. This registry does
    /// not dispose the plugin instance.
    /// </remarks>
    /// <exception cref="ArgumentException">The name is null, empty, or whitespace.</exception>
    /// <exception cref="InvalidOperationException">The plugin is currently registering or unregistering.</exception>
    public void Unregister(string name)
    {
      name = ValidateName(name, nameof(name));
      if (_registering.Contains(name))
        throw new InvalidOperationException($"Plugin '{name}' is still registering.");
      if (_plugins.TryGetValue(name, out var plugin))
      {
        if (!_unregistering.Add(name))
          throw new InvalidOperationException($"Plugin '{name}' is already unregistering.");

        try
        {
          plugin.Unregister(this);
          if (_plugins.TryGetValue(name, out var registered) && ReferenceEquals(registered, plugin))
            _plugins.Remove(name);
        }
        finally
        {
          _unregistering.Remove(name);
        }
      }
    }

    /// <summary>
    /// Gets a registered plugin by name.
    /// </summary>
    /// <param name="name">The plugin name.</param>
    /// <exception cref="ArgumentException">The name is null, empty, or whitespace.</exception>
    public IPlugin? Get(string name)
    {
      name = ValidateName(name, nameof(name));
      return _plugins.TryGetValue(name, out var plugin) ? plugin : null;
    }

    /// <summary>
    /// Checks whether a plugin is registered.
    /// </summary>
    /// <param name="name">The plugin name.</param>
    /// <exception cref="ArgumentException">The name is null, empty, or whitespace.</exception>
    public bool Has(string name)
    {
      name = ValidateName(name, nameof(name));
      return _plugins.ContainsKey(name);
    }

    /// <summary>
    /// Gets the currently registered plugins.
    /// </summary>
    public IEnumerable<IPlugin> All => _plugins.Values;

    private static string ValidateName(string? name, string paramName)
    {
      if (string.IsNullOrWhiteSpace(name))
        throw new ArgumentException("Plugin name must not be null, empty, or whitespace.", paramName);
      return name;
    }
  }
}
