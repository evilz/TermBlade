using System;
using System.Text;
using TermBlade.Core.Text;
using Xunit;

namespace TermBlade.Tests
{
  public class RopeTests
  {
    [Fact]
    public void RepeatedAppendsKeepRopeHeightLogarithmic()
    {
      var rope = new Rope();
      var expected = new StringBuilder();

      for (int i = 0; i < 8_192; i++)
      {
        rope = rope.Insert(rope.Length, "x");
        expected.Append('x');
      }

      Assert.Equal(expected.ToString(), rope.GetText());
      Assert.True(rope.Height <= 2 * CeilingLog2(rope.Length + 1),
        $"Expected logarithmic height, got {rope.Height} for {rope.Length} characters.");
    }

    [Fact]
    public void SequentialSmallAppendsCoalesceIntoBoundedLeaves()
    {
      var rope = new Rope();

      for (int i = 0; i < 2_048; i++)
        rope = rope.Insert(rope.Length, "x");

      Assert.Equal(2_048, rope.Length);
      Assert.Equal(3, rope.Height);
      Assert.Equal(new string('x', 2_048), rope.GetText());
    }

    [Fact]
    public void RandomizedEditsMatchStringModelAndStayBalanced()
    {
      var random = new Random(0x62);
      var rope = new Rope("seed");
      var expected = new StringBuilder("seed");

      for (int step = 0; step < 1_000; step++)
      {
        if (expected.Length == 0 || random.Next(2) == 0)
        {
          int index = random.Next(expected.Length + 1);
          int count = random.Next(1, 5);
          var text = new string((char)('a' + random.Next(26)), count);
          rope = rope.Insert(index, text);
          expected.Insert(index, text);
        }
        else
        {
          int start = random.Next(expected.Length);
          int count = random.Next(1, Math.Min(12, expected.Length - start) + 1);
          rope = rope.Delete(start, start + count);
          expected.Remove(start, count);
        }

        Assert.Equal(expected.ToString(), rope.GetText());
        Assert.True(rope.Height <= 2 * CeilingLog2(rope.Length + 1),
          $"Expected logarithmic height, got {rope.Height} for {rope.Length} characters.");
      }
    }

    [Fact]
    public void EditingPreservesEarlierRopeVersions()
    {
      var original = new Rope("persistent text");
      var inserted = original.Insert(10, " rope");
      var deleted = inserted.Delete(0, 5);

      Assert.Equal("persistent text", original.GetText());
      Assert.Equal("persistent rope text", inserted.GetText());
      Assert.Equal("stent rope text", deleted.GetText());
    }

    private static int CeilingLog2(int value)
    {
      int height = 0;
      int capacity = 1;
      while (capacity < value)
      {
        capacity <<= 1;
        height++;
      }
      return Math.Max(1, height);
    }
  }
}
