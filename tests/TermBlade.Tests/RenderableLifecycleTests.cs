using TermBlade.Core.Ansi;
using TermBlade.Core.Input;
using TermBlade.Core.Renderables;
using TermBlade.Core.Rendering;
using Xunit;

namespace TermBlade.Tests;

public class RenderableLifecycleTests
{
  [Fact]
  public void RemovingFocusedNodeClearsFocusAndStopsKeyDispatch()
  {
    using var renderer = CreateTestingRenderer();
    var renderable = new ProbeRenderable(renderer) { Focusable = true };
    renderer.Root.Add(renderable);
    renderable.Focus();

    renderer.Root.Remove(renderable.Id);

    Assert.Null(renderer.CurrentFocus);
    Assert.False(renderable.Focused);
    renderer.DispatchKey(new KeyEvent { Name = "a", Key = "a", Char = 'a' });
    Assert.Equal(0, renderable.KeyEventCount);
  }

  [Fact]
  public void DestroyingNestedRenderablesRepeatedlyEmitsOnceAndReleasesOwnedBuffers()
  {
    using var renderer = CreateTestingRenderer();
    var parent = new ProbeRenderable(renderer);
    var textarea = new TextareaRenderable(renderer);
    var framebuffer = new FrameBufferRenderable(renderer, 2, 2);
    parent.Add(textarea);
    parent.Add(framebuffer);
    renderer.Root.Add(parent);

    int parentDestroyed = 0;
    int textareaDestroyed = 0;
    int framebufferDestroyed = 0;
    parent.On("destroyed", _ => parentDestroyed++);
    textarea.On("destroyed", _ => textareaDestroyed++);
    framebuffer.On("destroyed", _ => framebufferDestroyed++);
    textarea.Focus();

    parent.Destroy();
    parent.Destroy();

    Assert.Null(renderer.CurrentFocus);
    Assert.Null(parent.Parent);
    Assert.Null(textarea.Parent);
    Assert.Null(framebuffer.Parent);
    Assert.Equal(1, parentDestroyed);
    Assert.Equal(1, textareaDestroyed);
    Assert.Equal(1, framebufferDestroyed);
    Assert.Throws<ObjectDisposedException>(() => _ = textarea.Value);
    Assert.Throws<ObjectDisposedException>(() => framebuffer.Clear(Rgba.FromInts(0, 0, 0)));
  }

  [Fact]
  public async Task RequestRenderRacingWithDisposeDoesNotSetClosedSignal()
  {
    using var renderer = CreateTestingRenderer();
    using var start = new ManualResetEventSlim();
    var requests = new Task[8];

    for (int i = 0; i < requests.Length; i++)
    {
      requests[i] = Task.Run(() =>
      {
        start.Wait();
        for (int request = 0; request < 10_000; request++)
          renderer.RequestRender();
      });
    }

    start.Set();
    renderer.Destroy();

    await Task.WhenAll(requests);
    renderer.RequestRender();
    Assert.All(requests, task => Assert.True(task.IsCompletedSuccessfully));
  }

  [Fact]
  public void FocusChangeStopsWhenBlurHandlerDisposesRenderer()
  {
    using var renderer = CreateTestingRenderer();
    var previous = new ProbeRenderable(renderer) { Focusable = true };
    var next = new ProbeRenderable(renderer) { Focusable = true };
    renderer.Root.Add(previous);
    renderer.Root.Add(next);
    previous.Focus();
    previous.On("blurred", _ => renderer.Destroy());

    next.Focus();

    Assert.Null(renderer.CurrentFocus);
    Assert.False(previous.Focused);
    Assert.False(next.Focused);
  }

  private static CliRenderer CreateTestingRenderer()
    => new(new CliRendererConfig { Testing = true });

  private sealed class ProbeRenderable : Renderable
  {
    public int KeyEventCount { get; private set; }

    public ProbeRenderable(CliRenderer renderer) : base(renderer) { }

    public override void HandleKey(KeyEvent key)
      => KeyEventCount++;

    protected override void RenderSelf(RenderBuffer buffer, double deltaTime) { }
  }
}
