using System;
using Xunit;
using TermBlade.Core.Plugins;

namespace TermBlade.Tests
{
  public class PluginRegistryTests
  {
    private sealed class DemoPlugin : IPlugin
    {
      public string Name { get; }
      public bool Registered { get; private set; }
      public bool Unregistered { get; private set; }
      public Action<PluginRegistry>? RegisterAction { get; set; }
      public Action<PluginRegistry>? UnregisterAction { get; set; }

      public DemoPlugin(string name = "demo") => Name = name;

      public void Register(PluginRegistry r)
      {
        Registered = true;
        RegisterAction?.Invoke(r);
      }

      public void Unregister(PluginRegistry r)
      {
        Unregistered = true;
        UnregisterAction?.Invoke(r);
      }
    }

    [Fact]
    public void Register_CallsPluginRegister()
    {
      var reg = new PluginRegistry();
      var p = new DemoPlugin();
      reg.Register(p);
      Assert.True(p.Registered);
    }

    [Fact]
    public void Has_AfterRegister_True()
    {
      var reg = new PluginRegistry();
      reg.Register(new DemoPlugin());
      Assert.True(reg.Has("demo"));
    }

    [Fact]
    public void Get_ReturnsPlugin()
    {
      var reg = new PluginRegistry();
      var p = new DemoPlugin();
      reg.Register(p);
      Assert.Same(p, reg.Get("demo"));
    }

    [Fact]
    public void Register_Duplicate_Throws()
    {
      var reg = new PluginRegistry();
      reg.Register(new DemoPlugin());
      Assert.Throws<System.InvalidOperationException>(() => reg.Register(new DemoPlugin()));
    }

    [Fact]
    public void Unregister_CallsPluginUnregister()
    {
      var reg = new PluginRegistry();
      var p = new DemoPlugin();
      reg.Register(p);
      reg.Unregister("demo");
      Assert.True(p.Unregistered);
    }

    [Fact]
    public void Unregister_Has_False()
    {
      var reg = new PluginRegistry();
      reg.Register(new DemoPlugin());
      reg.Unregister("demo");
      Assert.False(reg.Has("demo"));
    }

    [Fact]
    public void All_ReturnsRegisteredPlugins()
    {
      var reg = new PluginRegistry();
      reg.Register(new DemoPlugin());
      Assert.Single(reg.All);
    }

    [Fact]
    public void Register_WhenCallbackThrows_RollsBackAndCanBeRetried()
    {
      var reg = new PluginRegistry();
      bool failFirstAttempt = true;
      var plugin = new DemoPlugin("retry")
      {
        RegisterAction = _ =>
        {
          if (!failFirstAttempt) return;
          failFirstAttempt = false;
          throw new InvalidOperationException("initialization failed");
        }
      };

      Assert.Throws<InvalidOperationException>(() => reg.Register(plugin));
      Assert.False(reg.Has("retry"));
      Assert.Null(reg.Get("retry"));
      Assert.Empty(reg.All);

      reg.Register(plugin);

      Assert.Same(plugin, reg.Get("retry"));
      Assert.Contains(plugin, reg.All);
    }

    [Fact]
    public void RegisterCallback_CanObserveItselfAndRegisterAnotherPlugin()
    {
      var reg = new PluginRegistry();
      var nested = new DemoPlugin("nested");
      DemoPlugin? outer = null;
      outer = new DemoPlugin("outer")
      {
        RegisterAction = registry =>
        {
          Assert.True(registry.Has("outer"));
          Assert.Same(outer, registry.Get("outer"));
          Assert.Contains(outer!, registry.All);
          Assert.Throws<InvalidOperationException>(() => registry.Register(new DemoPlugin("OUTER")));
          registry.Register(nested);
        }
      };

      reg.Register(outer!);

      Assert.Same(outer, reg.Get("outer"));
      Assert.Same(nested, reg.Get("nested"));
      Assert.Equal(2, new System.Collections.Generic.List<IPlugin>(reg.All).Count);
    }

    [Fact]
    public void Unregister_WhenCallbackThrows_KeepsEntryAndCanBeRetried()
    {
      var reg = new PluginRegistry();
      bool failFirstAttempt = true;
      var plugin = new DemoPlugin("cleanup")
      {
        UnregisterAction = registry =>
        {
          Assert.True(registry.Has("cleanup"));
          if (!failFirstAttempt) return;
          failFirstAttempt = false;
          throw new InvalidOperationException("cleanup failed");
        }
      };
      reg.Register(plugin);

      Assert.Throws<InvalidOperationException>(() => reg.Unregister("cleanup"));
      Assert.True(reg.Has("cleanup"));
      Assert.Same(plugin, reg.Get("cleanup"));
      Assert.Contains(plugin, reg.All);

      reg.Unregister("cleanup");

      Assert.False(reg.Has("cleanup"));
      Assert.Null(reg.Get("cleanup"));
    }

    [Fact]
    public void UnregisterCallback_CannotRecursivelyUnregisterSamePlugin()
    {
      var reg = new PluginRegistry();
      var plugin = new DemoPlugin("recursive");
      plugin.UnregisterAction = registry => registry.Unregister("recursive");
      reg.Register(plugin);

      Assert.Throws<InvalidOperationException>(() => reg.Unregister("recursive"));
      Assert.Same(plugin, reg.Get("recursive"));

      plugin.UnregisterAction = null;
      reg.Unregister("recursive");
      Assert.False(reg.Has("recursive"));
    }

    [Fact]
    public void Registry_RejectsNullAndBlankNames()
    {
      var reg = new PluginRegistry();

      Assert.Throws<ArgumentNullException>(() => reg.Register(null!));
      Assert.Throws<ArgumentException>(() => reg.Register(new DemoPlugin(" ")));
      var emptyNameException = Assert.Throws<ArgumentException>(() => reg.Get(""));
      Assert.Contains("empty", emptyNameException.Message.ToLowerInvariant());
      Assert.Throws<ArgumentException>(() => reg.Get(" "));
      Assert.Throws<ArgumentException>(() => reg.Has(" "));
      Assert.Throws<ArgumentException>(() => reg.Unregister(" "));
    }
  }
}
