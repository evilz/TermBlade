using TermBlade.Core.Input;
using TermBlade.Core.Renderables;

namespace TermBlade.Tests;

public class ConfirmTests
{
  [Fact]
  public void DefaultValue_IsFalse()
  {
    var confirm = new ConfirmRenderable(null);
    Assert.False(confirm.Value);
  }

  [Fact]
  public void HandleKey_Left_TogglesValue()
  {
    var confirm = new ConfirmRenderable(null);
    confirm.Value = true;

    confirm.HandleKey(new KeyEvent { Name = "left" });
    Assert.False(confirm.Value);

    confirm.HandleKey(new KeyEvent { Name = "left" });
    Assert.True(confirm.Value);
  }

  [Fact]
  public void HandleKey_Right_TogglesValue()
  {
    var confirm = new ConfirmRenderable(null);

    confirm.HandleKey(new KeyEvent { Name = "right" });
    Assert.True(confirm.Value);
  }

  [Fact]
  public void HandleKey_Y_SetsValueTrue()
  {
    var confirm = new ConfirmRenderable(null);
    bool? emitted = null;
    confirm.On("confirmed", data => emitted = data is bool b && b);

    confirm.HandleKey(new KeyEvent { Name = "y" });

    Assert.True(confirm.Value);
    Assert.True(emitted);
  }

  [Fact]
  public void HandleKey_N_SetsValueFalse()
  {
    var confirm = new ConfirmRenderable(null);
    confirm.Value = true;
    bool? emitted = null;
    confirm.On("confirmed", data => emitted = data is bool b && b);

    confirm.HandleKey(new KeyEvent { Name = "n" });

    Assert.False(confirm.Value);
    Assert.False(emitted);
  }

  [Fact]
  public void HandleKey_Return_EmitsConfirmed()
  {
    var confirm = new ConfirmRenderable(null);
    confirm.Value = true;
    bool confirmed = false;
    confirm.On("confirmed", data => confirmed = data is bool b && b);

    confirm.HandleKey(new KeyEvent { Name = "return" });

    Assert.True(confirmed);
  }

  [Fact]
  public void Message_DefaultIsAreYouSure()
  {
    var confirm = new ConfirmRenderable(null);
    Assert.Equal("Are you sure?", confirm.Message);
  }
}
