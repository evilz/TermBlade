using System.Diagnostics.CodeAnalysis;
using System.Text;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.RenderTree;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TermBlade.Core.Ansi;
using TermBlade.Core.Layout;
using TermBlade.Core.Rendering;
using TermBlade.Razor.Hosting;
using RazorRenderer = Microsoft.AspNetCore.Components.RenderTree.Renderer;

#pragma warning disable BL0006

namespace TermBlade.Docs.Wasm.Terminal.Previews;

/// <summary>
/// Renders documentation component previews by mounting the same Razor demo pages used by
/// the terminal gallery and serializing the resulting terminal frame to ANSI for xterm.js.
/// </summary>
public static class ComponentPreviewService
{
  private const string DemoTypeSuffix = "Demo";

  /// <summary>
  /// Renders the named gallery component preview as ANSI terminal output.
  /// </summary>
  /// <param name="component">The gallery component name, such as <c>Text</c> or <c>Table</c>.</param>
  /// <returns>ANSI output for the terminal frame produced by the Razor demo page.</returns>
  public static string RenderPreview(string component)
  {
    ArgumentException.ThrowIfNullOrWhiteSpace(component);

    var demoType = ResolveDemoType(component);
    var provider = CreateServiceProvider();
    var renderer = provider.GetRequiredService<PreviewComponentRenderer>();
    var app = provider.GetRequiredService<TermBladeAppContext>();

    try
    {
      renderer.MountComponentAsync(demoType).GetAwaiter().GetResult();
      return RenderTerminalFrame(app.Renderer);
    }
    finally
    {
      renderer.Dispose();
      app.Dispose();
      provider.Dispose();
    }
  }

  private static Type ResolveDemoType(string component)
  {
    var demoTypeName = $"TermBlade.Docs.Wasm.Terminal.Previews.Demos.{component}{DemoTypeSuffix}";
    return typeof(ComponentPreviewService).Assembly.GetType(demoTypeName) ?? typeof(UnknownPreviewDemo);
  }

  private static ServiceProvider CreateServiceProvider()
  {
    var services = new ServiceCollection();
    services.AddLogging();
    services.AddOptions<TermBladeRazorOptions>().Configure(options =>
    {
      options.Testing = true;
      options.BackgroundColor = "#0d1117";
    });
    services.AddSingleton<IComponentActivator, PreviewComponentActivator>();
    services.AddSingleton<TermBladeAppContext>();
    services.AddSingleton<PreviewComponentRenderer>();
    return services.BuildServiceProvider();
  }

  private static string RenderTerminalFrame(CliRenderer renderer)
  {
    var buffer = new RenderBuffer(renderer.TerminalWidth, renderer.TerminalHeight);
    buffer.Clear(Rgba.FromCss("#0d1117"));

    FlexLayout.Calculate(renderer.Root.LayoutNode, renderer.TerminalWidth, renderer.TerminalHeight);
    renderer.Root.Render(buffer, deltaTime: 0);

    return ToAnsi(buffer);
  }

  private static string ToAnsi(RenderBuffer buffer)
  {
    var sb = new StringBuilder(buffer.Width * buffer.Height * 4);
    Rgba? lastFg = null;
    Rgba? lastBg = null;
    TextAttributes? lastAttrs = null;

    sb.Append("\x1b[2J\x1b[H");
    for (var y = 0; y < buffer.Height; y++)
    {
      for (var x = 0; x < buffer.Width; x++)
      {
        var cell = buffer.GetCell(x, y) ?? default;
        if (lastAttrs is null || cell.Attributes != lastAttrs.Value)
        {
          sb.Append("\x1b[0m");
          AnsiCodes.WriteAttributes(new StringWriter(sb), cell.Attributes);
          lastFg = null;
          lastBg = null;
          lastAttrs = cell.Attributes;
        }

        if (lastFg is null || cell.Fg != lastFg.Value)
        {
          AnsiCodes.WriteFgColor(new StringWriter(sb), cell.Fg);
          lastFg = cell.Fg;
        }

        if (lastBg is null || cell.Bg != lastBg.Value)
        {
          AnsiCodes.WriteBgColor(new StringWriter(sb), cell.Bg);
          lastBg = cell.Bg;
        }

        sb.Append(cell.Codepoint == 0 ? ' ' : char.ConvertFromUtf32(cell.Codepoint));
      }

      if (y < buffer.Height - 1)
        sb.Append("\r\n");
    }

    sb.Append("\x1b[0m");
    return sb.ToString();
  }

  private sealed class UnknownPreviewDemo : ComponentBase
  {
    protected override void BuildRenderTree(Microsoft.AspNetCore.Components.Rendering.RenderTreeBuilder builder)
    {
      builder.OpenComponent<TermBlade.Razor.Components.Text>(0);
      builder.AddAttribute(1, "Content", "Unknown preview");
      builder.AddAttribute(2, "Fg", "#f7768e");
      builder.AddAttribute(3, "Width", "24");
      builder.AddAttribute(4, "Height", "1");
      builder.CloseComponent();
    }
  }

  private sealed class PreviewComponentActivator(IServiceProvider services) : IComponentActivator
  {
    public IComponent CreateInstance(Type componentType)
      => (IComponent)ActivatorUtilities.CreateInstance(services, componentType);
  }

  private sealed class SerializedDispatcher : Dispatcher
  {
    private readonly object _sync = new();
    private readonly AsyncLocal<bool> _isExecuting = new();
    private Task _tail = Task.CompletedTask;

    public override bool CheckAccess() => _isExecuting.Value;

    public override Task InvokeAsync(Action workItem)
    {
      ArgumentNullException.ThrowIfNull(workItem);
      return InvokeAsync(() =>
      {
        workItem();
        return Task.CompletedTask;
      });
    }

    public override Task InvokeAsync(Func<Task> workItem)
    {
      ArgumentNullException.ThrowIfNull(workItem);

      if (CheckAccess())
        return workItem();

      lock (_sync)
      {
        var state = (_isExecuting, workItem);
        var task = _tail.ContinueWith(static (_, state) => ExecuteAsync(((AsyncLocal<bool>, Func<Task>))state!), state, CancellationToken.None, TaskContinuationOptions.None, TaskScheduler.Default).Unwrap();
        _tail = task.ContinueWith(static _ => { }, CancellationToken.None, TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Default);
        return task;
      }
    }

    public override Task<TResult> InvokeAsync<TResult>(Func<TResult> workItem)
    {
      ArgumentNullException.ThrowIfNull(workItem);
      return InvokeAsync(() => Task.FromResult(workItem()));
    }

    public override Task<TResult> InvokeAsync<TResult>(Func<Task<TResult>> workItem)
    {
      ArgumentNullException.ThrowIfNull(workItem);

      if (CheckAccess())
        return workItem();

      lock (_sync)
      {
        var state = (_isExecuting, workItem);
        var task = _tail.ContinueWith(static (_, state) => ExecuteAsync(((AsyncLocal<bool>, Func<Task<TResult>>))state!), state, CancellationToken.None, TaskContinuationOptions.None, TaskScheduler.Default).Unwrap();
        _tail = task.ContinueWith(static _ => { }, CancellationToken.None, TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Default);
        return task;
      }
    }

    private static async Task ExecuteAsync((AsyncLocal<bool> Flag, Func<Task> WorkItem) state)
    {
      state.Flag.Value = true;
      try
      {
        await state.WorkItem().ConfigureAwait(false);
      }
      finally
      {
        state.Flag.Value = false;
      }
    }

    private static async Task<TResult> ExecuteAsync<TResult>((AsyncLocal<bool> Flag, Func<Task<TResult>> WorkItem) state)
    {
      state.Flag.Value = true;
      try
      {
        return await state.WorkItem().ConfigureAwait(false);
      }
      finally
      {
        state.Flag.Value = false;
      }
    }
  }

  private sealed class PreviewComponentRenderer(IServiceProvider services, ILoggerFactory loggerFactory) : RazorRenderer(services, loggerFactory)
  {
    private readonly Dispatcher _dispatcher = new SerializedDispatcher();

    public override Dispatcher Dispatcher => _dispatcher;

    public Task MountComponentAsync([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type componentType)
    {
      ArgumentNullException.ThrowIfNull(componentType);
      return Dispatcher.InvokeAsync(async () =>
      {
        var component = InstantiateComponent(componentType);
        var componentId = AssignRootComponentId(component);
        await RenderRootComponentAsync(componentId).ConfigureAwait(false);
      });
    }

    protected override Task UpdateDisplayAsync(in RenderBatch renderBatch) => Task.CompletedTask;

    protected override void HandleException(Exception exception)
    {
      System.Runtime.ExceptionServices.ExceptionDispatchInfo.Capture(exception).Throw();
    }
  }
}
