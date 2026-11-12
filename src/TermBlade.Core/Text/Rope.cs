using System;
using System.Collections.Generic;
using System.Text;

namespace TermBlade.Core.Text
{
  // ── leaf / branch nodes ───────────────────────────────────────────────────────

  internal interface RopeNode
  {
    int Length { get; }
    int Height { get; }
    string GetText();
    string Slice(int start, int end);
    /// <summary>Split the node at <paramref name="index"/> into (left, right) leaves.</summary>
    (RopeNode Left, RopeNode Right) Split(int index);
    void AppendTo(StringBuilder builder);
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
    public int Height => 1;
    /// <summary>
    /// Gets the get text.
    /// </summary>
    public string GetText() => _v;
    public void AppendTo(StringBuilder builder) => builder.Append(_v);
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
    private readonly int _height;
    public RopeNode Left => _left;
    public RopeNode Right => _right;
    /// <summary>
    /// Rope branch.
    /// </summary>
    /// <param name="left">The left value.</param>
    /// <param name="right">The right value.</param>
    public RopeBranch(RopeNode left, RopeNode right)
    {
      _left = left;
      _right = right;
      _len = left.Length + right.Length;
      _height = 1 + Math.Max(left.Height, right.Height);
    }
    /// <summary>
    /// Gets the length.
    /// </summary>
    public int Length => _len;
    public int Height => _height;
    /// <summary>
    /// Gets the get text.
    /// </summary>
    public string GetText()
    {
      var builder = new StringBuilder(_len);
      AppendTo(builder);
      return builder.ToString();
    }
    public void AppendTo(StringBuilder builder)
    {
      _left.AppendTo(builder);
      _right.AppendTo(builder);
    }
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
        return (ll2, RopeNodeOps.Join(lr2, _right));
      }
      var (rl2, rr2) = _right.Split(index - ll);
      return (RopeNodeOps.Join(_left, rl2), rr2);
    }
  }

  internal static class RopeNodeOps
  {
    private const int MaxLeafLength = 512;

    public static RopeNode FromText(string text)
    {
      if (text.Length <= MaxLeafLength)
        return new RopeLeaf(text);

      var leaves = new List<RopeNode>((text.Length + MaxLeafLength - 1) / MaxLeafLength);
      for (int start = 0; start < text.Length; start += MaxLeafLength)
      {
        int length = Math.Min(MaxLeafLength, text.Length - start);
        leaves.Add(new RopeLeaf(text.Substring(start, length)));
      }

      return BuildBalanced(leaves, 0, leaves.Count);
    }

    public static RopeNode Join(RopeNode left, RopeNode right)
    {
      if (left.Length == 0) return right;
      if (right.Length == 0) return left;

      if (left is RopeLeaf leftLeaf
          && right is RopeLeaf rightLeaf
          && left.Length + right.Length <= MaxLeafLength)
      {
        return new RopeLeaf(leftLeaf.GetText() + rightLeaf.GetText());
      }

      if (left.Height > right.Height + 1)
      {
        var branch = (RopeBranch)left;
        return Balance(branch.Left, Join(branch.Right, right));
      }

      if (right.Height > left.Height + 1)
      {
        var branch = (RopeBranch)right;
        return Balance(Join(left, branch.Left), branch.Right);
      }

      return new RopeBranch(left, right);
    }

    private static RopeNode BuildBalanced(IReadOnlyList<RopeNode> leaves, int start, int count)
    {
      if (count == 1) return leaves[start];

      int leftCount = count / 2;
      var left = BuildBalanced(leaves, start, leftCount);
      var right = BuildBalanced(leaves, start + leftCount, count - leftCount);
      return new RopeBranch(left, right);
    }

    private static RopeNode Balance(RopeNode left, RopeNode right)
    {
      if (left.Height > right.Height + 1)
      {
        var leftBranch = (RopeBranch)left;
        if (leftBranch.Left.Height >= leftBranch.Right.Height)
          return new RopeBranch(leftBranch.Left, new RopeBranch(leftBranch.Right, right));

        var pivot = (RopeBranch)leftBranch.Right;
        return new RopeBranch(
          new RopeBranch(leftBranch.Left, pivot.Left),
          new RopeBranch(pivot.Right, right));
      }

      if (right.Height > left.Height + 1)
      {
        var rightBranch = (RopeBranch)right;
        if (rightBranch.Right.Height >= rightBranch.Left.Height)
          return new RopeBranch(new RopeBranch(left, rightBranch.Left), rightBranch.Right);

        var pivot = (RopeBranch)rightBranch.Left;
        return new RopeBranch(
          new RopeBranch(left, pivot.Left),
          new RopeBranch(pivot.Right, rightBranch.Right));
      }

      return new RopeBranch(left, right);
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
    internal int Height => _root.Height;

    /// <summary>
    /// Rope.
    /// </summary>
    /// <param name="text">The text value.</param>
    public Rope(string text = "")
    {
      ArgumentNullException.ThrowIfNull(text);
      _root = RopeNodeOps.FromText(text);
    }
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
      ArgumentNullException.ThrowIfNull(text);
      if (text.Length == 0) return this;

      index = Math.Clamp(index, 0, _root.Length);
      var insert = RopeNodeOps.FromText(text);

      if (index == 0)
        return new Rope(RopeNodeOps.Join(insert, _root));
      if (index == _root.Length)
        return new Rope(RopeNodeOps.Join(_root, insert));

      var (left, right) = _root.Split(index);
      return new Rope(RopeNodeOps.Join(RopeNodeOps.Join(left, insert), right));
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
      if (start == end) return this;

      var (left, _) = _root.Split(start);
      var (_, right) = _root.Split(end);
      return new Rope(RopeNodeOps.Join(left, right));
    }

    /// <summary>
    /// Gets the replace.
    /// </summary>
    public Rope Replace(string text) => new Rope(text);
  }
}
