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
  public void DestroyingNestedRenderablesCleansUpWhenBlurHandlerThrows()
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
    textarea.On("blurred", _ => throw new InvalidOperationException("blur handler failed"));
    framebuffer.On("destroyed", _ => framebufferDestroyed++);
    textarea.Focus();

    Assert.Throws<InvalidOperationException>(() => parent.Destroy());
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
  public void FocusChangeRollsBackWhenPreviousBlurHandlerThrows()
  {
    using var renderer = CreateTestingRenderer();
    var previous = new ProbeRenderable(renderer) { Focusable = true };
    var next = new ProbeRenderable(renderer) { Focusable = true };
    renderer.Root.Add(previous);
    renderer.Root.Add(next);
    previous.Focus();

    bool throwOnBlur = true;
    previous.On("blurred", _ =>
    {
      if (throwOnBlur)
        throw new InvalidOperationException("blur handler failed");
    });

    Assert.Throws<InvalidOperationException>(() => next.Focus());

    Assert.False(previous.Focused);
    Assert.False(next.Focused);
    Assert.Null(renderer.CurrentFocus);

    throwOnBlur = false;
    next.Focus();

    Assert.True(next.Focused);
    Assert.Same(next, renderer.CurrentFocus);
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

  [Fact]
  public void ReparentingMovesChildAndReattachesRendererOwnership()
  {
    using var renderer = CreateTestingRenderer();
    var oldParent = new ProbeRenderable(renderer);
    var newParent = new ProbeRenderable(renderer);
    var child = new ProbeRenderable(renderer) { Focusable = true };
    renderer.Root.Add(oldParent);
    renderer.Root.Add(newParent);
    oldParent.Add(child);

    int blurCount = 0;
    child.On("blurred", _ => blurCount++);
    child.Focus();
    newParent.Add(child);

    Assert.Same(newParent, child.Parent);
    Assert.DoesNotContain(child, oldParent.GetChildren());
    Assert.Contains(child, newParent.GetChildren());
    Assert.Null(renderer.CurrentFocus);
    Assert.False(child.Focused);
    Assert.Equal(1, blurCount);

    child.Focus();
    Assert.Same(child, renderer.CurrentFocus);
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
