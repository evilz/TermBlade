using System.Diagnostics;
using TermBlade.Core.Text;

namespace TermBlade.RopeBenchmarks;

internal static class Program
{
  private const int AppendCount = 2_048;
  private const int TraversalLength = 3_072;
  private const int TraversalCalls = 3;
  private const int Repetitions = 3;

  private static void Main()
  {
    Console.WriteLine($"Rope benchmark; runtime={Environment.Version}; os={Environment.OSVersion}; appendCount={AppendCount}; traversalLength={TraversalLength}; traversalCalls={TraversalCalls}; repetitions={Repetitions}");
    Measure("legacy append", () => BuildLegacy(AppendCount).Length, Repetitions);
    Measure("balanced append", () => BuildBalanced(AppendCount).Length, Repetitions);

    var legacyTraversal = BuildLegacy(TraversalLength);
    var balancedTraversal = BuildBalanced(TraversalLength);
    Measure("legacy GetText x3", () => ReadLegacyRepeatedly(legacyTraversal, TraversalCalls), Repetitions);
    Measure("balanced GetText x3", () => ReadBalancedRepeatedly(balancedTraversal, TraversalCalls), Repetitions);
  }

  private static int ReadLegacyRepeatedly(LegacyRope rope, int calls)
  {
    int length = 0;
    for (int i = 0; i < calls; i++)
      length += rope.GetText().Length;
    return length;
  }

  private static int ReadBalancedRepeatedly(Rope rope, int calls)
  {
    int length = 0;
    for (int i = 0; i < calls; i++)
      length += rope.GetText().Length;
    return length;
  }

  private static Rope BuildBalanced(int length)
  {
    var rope = new Rope();
    for (int i = 0; i < length; i++)
      rope = rope.Insert(rope.Length, "x");
    return rope;
  }

  private static LegacyRope BuildLegacy(int length)
  {
    var rope = new LegacyRope();
    for (int i = 0; i < length; i++)
      rope = rope.Insert(rope.Length, "x");
    return rope;
  }

  private static void Measure(string name, Func<int> operation, int repetitions)
  {
    _ = operation();

    long allocatedBefore = GC.GetAllocatedBytesForCurrentThread();
    var stopwatch = Stopwatch.StartNew();
    int checksum = 0;
    for (int i = 0; i < repetitions; i++)
      checksum += operation();
    stopwatch.Stop();

    long allocated = GC.GetAllocatedBytesForCurrentThread() - allocatedBefore;
    Console.WriteLine($"{name}: {stopwatch.Elapsed.TotalMilliseconds / repetitions:F3} ms/op; {allocated / repetitions:N0} B/op; checksum={checksum}");
  }

  private sealed class LegacyRope
  {
    private readonly ILegacyNode _root;

    public int Length => _root.Length;

    public LegacyRope(string text = "") { _root = new LegacyLeaf(text); }
    private LegacyRope(ILegacyNode root) { _root = root; }

    public string GetText() => _root.GetText();

    public LegacyRope Insert(int index, string text)
    {
      index = Math.Clamp(index, 0, _root.Length);
      var (left, right) = _root.Split(index);
      var insert = new LegacyLeaf(text);
      return new LegacyRope(new LegacyBranch(new LegacyBranch(left, insert), right));
    }
  }

  private interface ILegacyNode
  {
    int Length { get; }
    string GetText();
    (ILegacyNode Left, ILegacyNode Right) Split(int index);
  }

  private sealed class LegacyLeaf(string value) : ILegacyNode
  {
    public int Length => value.Length;
    public string GetText() => value;

    public (ILegacyNode Left, ILegacyNode Right) Split(int index)
    {
      index = Math.Clamp(index, 0, value.Length);
      return (new LegacyLeaf(value[..index]), new LegacyLeaf(value[index..]));
    }
  }

  private sealed class LegacyBranch : ILegacyNode
  {
    private readonly ILegacyNode _left;
    private readonly ILegacyNode _right;
    private readonly int _length;

    public LegacyBranch(ILegacyNode left, ILegacyNode right)
    {
      _left = left;
      _right = right;
      _length = left.Length + right.Length;
    }

    public int Length => _length;
    public string GetText() => _left.GetText() + _right.GetText();

    public (ILegacyNode Left, ILegacyNode Right) Split(int index)
    {
      if (index <= _left.Length)
      {
        var (leftPart, rightPart) = _left.Split(index);
        return (leftPart, new LegacyBranch(rightPart, _right));
      }

      var (rightPartLeft, rightPartRight) = _right.Split(index - _left.Length);
      return (new LegacyBranch(_left, rightPartLeft), rightPartRight);
    }
  }
}
