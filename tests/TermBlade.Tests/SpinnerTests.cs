using TermBlade.Core.Renderables;

namespace TermBlade.Tests;

public class SpinnerTests
{
  [Fact]
  public void DefaultIsSpinning_IsTrue()
  {
    var spinner = new SpinnerRenderable(null);
    Assert.True(spinner.IsSpinning);
  }

  [Fact]
  public void DefaultFrames_AreNotEmpty()
  {
    var spinner = new SpinnerRenderable(null);
    Assert.NotEmpty(spinner.Frames);
  }

  [Fact]
  public void Title_CanBeSet()
  {
    var spinner = new SpinnerRenderable(null);
    spinner.Title = "Loading...";
    Assert.Equal("Loading...", spinner.Title);
  }

  [Fact]
  public void CompletedText_DefaultIsNull()
  {
    var spinner = new SpinnerRenderable(null);
    Assert.Null(spinner.CompletedText);
  }

  [Fact]
  public void Interval_DefaultIs008()
  {
    var spinner = new SpinnerRenderable(null);
    Assert.Equal(0.08, spinner.Interval);
  }
}
