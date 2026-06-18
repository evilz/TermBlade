using System;
using System.Text;

namespace TermBlade.Core.Text
{
  // ── leaf / branch nodes ───────────────────────────────────────────────────────

  internal interface RopeNode
  {
    int Length { get; }
    string GetText();
    string Slice(int start, int end);
    /// <summary>Split the node at <paramref name="index"/> into (left, right) leaves.</summary>
    (RopeNode Left, RopeNode Right) Split(int index);
  }

  internal sealed class RopeLeaf : RopeNode
  {
    private readonly string _v;
    /// <summary>
    /// Rope leaf.
    /// </summary>
    /// <param name="value">The value value.</param>
    public RopeLeaf(string value) { _v = value; }
    /// <summary>
    /// Gets the length.
    /// </summary>
    public int Length => _v.Length;
    /// <summary>
    /// Gets the get text.
    /// </summary>
    public string GetText() => _v;
    /// <summary>
    /// Gets the slice.
    /// </summary>
    /// <param name="start">The start value.</param>
    /// <param name="end">The end value.</param>
    public string Slice(int start, int end) => _v[Math.Max(0, start)..Math.Min(_v.Length, end)];
    /// <summary>
    /// Public.
    /// </summary>
    /// <param name="RopeNode">The RopeNode value.</param>
    /// <param name="index">The index value.</param>
    public (RopeNode, RopeNode) Split(int index)
    {
      index = Math.Clamp(index, 0, _v.Length);
      return (new RopeLeaf(_v[..index]), new RopeLeaf(_v[index..]));
    }
  }

  internal sealed class RopeBranch : RopeNode
  {
    private readonly RopeNode _left, _right;
    private readonly int _len;
    /// <summary>
    /// Rope branch.
    /// </summary>
    /// <param name="left">The left value.</param>
    /// <param name="right">The right value.</param>
    public RopeBranch(RopeNode left, RopeNode right) { _left = left; _right = right; _len = left.Length + right.Length; }
    /// <summary>
    /// Gets the length.
    /// </summary>
    public int Length => _len;
    /// <summary>
    /// Gets the get text.
    /// </summary>
    public string GetText() => _left.GetText() + _right.GetText();
    /// <summary>
    /// Slice.
    /// </summary>
    /// <param name="start">The start value.</param>
    /// <param name="end">The end value.</param>
    public string Slice(int start, int end)
    {
      int ll = _left.Length;
      if (end <= ll) return _left.Slice(start, end);
      if (start >= ll) return _right.Slice(start - ll, end - ll);
      return _left.Slice(start, ll) + _right.Slice(0, end - ll);
    }
    /// <summary>
    /// Public.
    /// </summary>
    /// <param name="RopeNode">The RopeNode value.</param>
    /// <param name="index">The index value.</param>
    public (RopeNode, RopeNode) Split(int index)
    {
      int ll = _left.Length;
      if (index <= ll)
      {
        var (ll2, lr2) = _left.Split(index);
        return (ll2, new RopeBranch(lr2, _right));
      }
      var (rl2, rr2) = _right.Split(index - ll);
      return (new RopeBranch(_left, rl2), rr2);
    }
  }

  // ── public Rope ───────────────────────────────────────────────────────────────

  /// <summary>
  /// Persistent (copy-on-write) rope for efficient text editing.
  /// Mutating operations return a new Rope without allocating a full string copy.
  /// </summary>
  public sealed class Rope
  {
    private readonly RopeNode _root;

    /// <summary>
    /// Gets the length.
    /// </summary>
    public int Length => _root.Length;

    /// <summary>
    /// Rope.
    /// </summary>
    /// <param name="text">The text value.</param>
    public Rope(string text = "") { _root = new RopeLeaf(text); }
    private Rope(RopeNode root) { _root = root; }

    /// <summary>
    /// Gets the get text.
    /// </summary>
    public string GetText() => _root.GetText();

    /// <summary>
    /// Get range.
    /// </summary>
    /// <param name="start">The start value.</param>
    /// <param name="end">The end value.</param>
    public string GetRange(int start, int end)
        => _root.Slice(Math.Clamp(start, 0, _root.Length),
                       Math.Clamp(end, 0, _root.Length));

    /// <summary>
    /// Insert.
    /// </summary>
    /// <param name="index">The index value.</param>
    /// <param name="text">The text value.</param>
    public Rope Insert(int index, string text)
    {
      index = Math.Clamp(index, 0, _root.Length);
      var (left, right) = _root.Split(index);
      var insert = new RopeLeaf(text);
      return new Rope(new RopeBranch(new RopeBranch(left, insert), right));
    }

    /// <summary>
    /// Delete.
    /// </summary>
    /// <param name="start">The start value.</param>
    /// <param name="end">The end value.</param>
    public Rope Delete(int start, int end)
    {
      start = Math.Clamp(start, 0, _root.Length);
      end = Math.Clamp(end, start, _root.Length);
      var (left, _) = _root.Split(start);
      var (_, right) = _root.Split(end);
      return new Rope(new RopeBranch(left, right));
    }

    /// <summary>
    /// Gets the replace.
    /// </summary>
    public Rope Replace(string text) => new Rope(text);
  }
}
