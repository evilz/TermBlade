namespace TermBlade.Core.Plugins
{
  /// <summary>
  /// Represents iplugin.
  /// </summary>
  public interface IPlugin
  {
    string Name { get; }
    void Register(PluginRegistry registry);
    void Unregister(PluginRegistry registry);
  }
}
