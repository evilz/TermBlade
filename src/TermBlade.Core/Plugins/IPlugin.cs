namespace TermBlade.Core.Plugins
{
  /// <summary>
  /// Represents a plugin managed by <see cref="PluginRegistry"/>.
  /// </summary>
  /// <remarks>
  /// The registry owns the plugin's registration entry, not the plugin instance.
  /// A failed <see cref="Register(PluginRegistry)"/> callback removes that entry,
  /// but the plugin must undo its own external side effects. A failed
  /// <see cref="Unregister(PluginRegistry)"/> callback leaves the entry registered,
  /// so cleanup callbacks should be safe to retry.
  /// </remarks>
  public interface IPlugin
  {
    /// <summary>Gets the plugin's stable registry name.</summary>
    string Name { get; }

    /// <summary>Initializes this plugin after its registry entry becomes visible.</summary>
    /// <param name="registry">The registry that owns the registration entry.</param>
    void Register(PluginRegistry registry);

    /// <summary>Cleans up this plugin before its registry entry is removed.</summary>
    /// <param name="registry">The registry that owns the registration entry.</param>
    void Unregister(PluginRegistry registry);
  }
}
