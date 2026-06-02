using TermBlade.Core.Input;
using TermBlade.Core.Renderables;

namespace TermBlade.Tests;

public class MultiSelectTests
{
  private static MultiSelectRenderable CreateMultiSelect()
  {
    return new MultiSelectRenderable(null)
    {
      Options =
      [
        new() { Name = "Option A", Value = "a" },
        new() { Name = "Option B", Value = "b" },
        new() { Name = "Option C", Value = "c" },
      ]
    };
  }

  [Fact]
  public void CursorIndex_DefaultIsZero()
  {
    var ms = CreateMultiSelect();
    Assert.Equal(0, ms.CursorIndex);
  }

  [Fact]
  public void HandleKey_Down_MovesCursor()
  {
    var ms = CreateMultiSelect();

    ms.HandleKey(new KeyEvent { Name = "down" });
    Assert.Equal(1, ms.CursorIndex);

    ms.HandleKey(new KeyEvent { Name = "down" });
    Assert.Equal(2, ms.CursorIndex);
  }

  [Fact]
  public void HandleKey_Up_MovesCursor()
  {
    var ms = CreateMultiSelect();
    ms.CursorIndex = 2;

    ms.HandleKey(new KeyEvent { Name = "up" });
    Assert.Equal(1, ms.CursorIndex);
  }

  [Fact]
  public void HandleKey_Down_StopsAtEnd()
  {
    var ms = CreateMultiSelect();
    ms.CursorIndex = 2;

    ms.HandleKey(new KeyEvent { Name = "down" });
    Assert.Equal(2, ms.CursorIndex);
  }

  [Fact]
  public void HandleKey_Up_StopsAtBeginning()
  {
    var ms = CreateMultiSelect();

    ms.HandleKey(new KeyEvent { Name = "up" });
    Assert.Equal(0, ms.CursorIndex);
  }

  [Fact]
  public void HandleKey_Space_TogglesSelection()
  {
    var ms = CreateMultiSelect();

    ms.HandleKey(new KeyEvent { Name = "space" });
    Assert.Contains(0, ms.SelectedIndices);

    ms.HandleKey(new KeyEvent { Name = "space" });
    Assert.DoesNotContain(0, ms.SelectedIndices);
  }

  [Fact]
  public void HandleKey_Space_EmitsSelectionChanged()
  {
    var ms = CreateMultiSelect();
    List<int>? emitted = null;
    ms.On("selectionChanged", data => emitted = data as List<int>);

    ms.HandleKey(new KeyEvent { Name = "space" });

    Assert.NotNull(emitted);
    Assert.Contains(0, emitted);
  }

  [Fact]
  public void HandleKey_Return_EmitsConfirmed()
  {
    var ms = CreateMultiSelect();
    ms.SelectedIndices.Add(1);
    List<int>? confirmed = null;
    ms.On("confirmed", data => confirmed = data as List<int>);

    ms.HandleKey(new KeyEvent { Name = "return" });

    Assert.NotNull(confirmed);
    Assert.Contains(1, confirmed);
  }

  [Fact]
  public void MultipleSelections_WorkCorrectly()
  {
    var ms = CreateMultiSelect();

    // Select first
    ms.HandleKey(new KeyEvent { Name = "space" });
    // Move down and select second
    ms.HandleKey(new KeyEvent { Name = "down" });
    ms.HandleKey(new KeyEvent { Name = "space" });

    Assert.Contains(0, ms.SelectedIndices);
    Assert.Contains(1, ms.SelectedIndices);
    Assert.Equal(2, ms.SelectedIndices.Count);
  }
}
