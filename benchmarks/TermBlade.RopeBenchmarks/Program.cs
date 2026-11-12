using System.Diagnostics;
using TermBlade.Core.Text;

const int AppendCount = 2_048;
const int TraversalLength = 3_072;
const int TraversalCalls = 3;
const int Repetitions = 3;

Console.WriteLine($"Rope benchmark; runtime={Environment.Version}; os={Environment.OSVersion}; appendCount={AppendCount}; traversalLength={TraversalLength}; traversalCalls={TraversalCalls}; repetitions={Repetitions}");
Measure("legacy append", () => BuildLegacy(AppendCount).Length, Repetitions);
Measure("balanced append", () => BuildBalanced(AppendCount).Length, Repetitions);

var legacyTraversal = BuildLegacy(TraversalLength);
var balancedTraversal = BuildBalanced(TraversalLength);
Measure("legacy GetText x3", () => ReadRepeatedly(legacyTraversal, TraversalCalls), Repetitions);
Measure("balanced GetText x3", () => ReadRepeatedly(balancedTraversal, TraversalCalls), Repetitions);

static int ReadRepeatedly(LegacyRope rope, int calls)
{
  int length = 0;
  for (int i = 0; i < calls; i++)
    length += rope.GetText().Length;
  return length;
}

static int ReadRepeatedly(Rope rope, int calls)
{
  int length = 0;
  for (int i = 0; i < calls; i++)
    length += rope.GetText().Length;
  return length;
}

static Rope BuildBalanced(int length)
{
  var rope = new Rope();
  for (int i = 0; i < length; i++)
    rope = rope.Insert(rope.Length, "x");
  return rope;
}

static LegacyRope BuildLegacy(int length)
{
  var rope = new LegacyRope();
  for (int i = 0; i < length; i++)
    rope = rope.Insert(rope.Length, "x");
  return rope;
}

static void Measure(string name, Func<int> operation, int repetitions)
{
  _ = operation();
  GC.Collect();
  GC.WaitForPendingFinalizers();
  GC.Collect();

  long allocatedBefore = GC.GetAllocatedBytesForCurrentThread();
  var stopwatch = Stopwatch.StartNew();
  int checksum = 0;
  for (int i = 0; i < repetitions; i++)
    checksum += operation();
  stopwatch.Stop();

  long allocated = GC.GetAllocatedBytesForCurrentThread() - allocatedBefore;
  Console.WriteLine($"{name}: {stopwatch.Elapsed.TotalMilliseconds / repetitions:F3} ms/op; {allocated / repetitions:N0} B/op; checksum={checksum}");
}

internal sealed class LegacyRope
{
  private readonly LegacyNode _root;

  public int Length => _root.Length;

  public LegacyRope(string text = "") { _root = new LegacyLeaf(text); }
  private LegacyRope(LegacyNode root) { _root = root; }

  public string GetText() => _root.GetText();

  public LegacyRope Insert(int index, string text)
  {
    index = Math.Clamp(index, 0, _root.Length);
    var (left, right) = _root.Split(index);
    var insert = new LegacyLeaf(text);
    return new LegacyRope(new LegacyBranch(new LegacyBranch(left, insert), right));
  }
}

internal abstract class LegacyNode
{
  public abstract int Length { get; }
  public abstract string GetText();
  public abstract (LegacyNode Left, LegacyNode Right) Split(int index);
}

internal sealed class LegacyLeaf(string value) : LegacyNode
{
  public override int Length => value.Length;
  public override string GetText() => value;

  public override (LegacyNode Left, LegacyNode Right) Split(int index)
  {
    index = Math.Clamp(index, 0, value.Length);
    return (new LegacyLeaf(value[..index]), new LegacyLeaf(value[index..]));
  }
}

internal sealed class LegacyBranch(LegacyNode left, LegacyNode right) : LegacyNode
{
  public override int Length => left.Length + right.Length;
  public override string GetText() => left.GetText() + right.GetText();

  public override (LegacyNode Left, LegacyNode Right) Split(int index)
  {
    int leftLength = left.Length;
    if (index <= leftLength)
    {
      var (leftPart, rightPart) = left.Split(index);
      return (leftPart, new LegacyBranch(rightPart, right));
    }

    var (rightPartLeft, rightPartRight) = right.Split(index - leftLength);
    return (new LegacyBranch(left, rightPartLeft), rightPartRight);
  }
}
