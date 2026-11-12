namespace TermBlade.Core.Buffer;

/// <summary>
/// Validates dimensions and cell counts shared by cell and render buffers.
/// </summary>
internal static class BufferDimensions
{
  internal static int GetCellCount(int width, int height)
  {
    if (width <= 0)
    {
      throw new ArgumentOutOfRangeException(nameof(width), width, "Width must be positive.");
    }

    if (height <= 0)
    {
      throw new ArgumentOutOfRangeException(nameof(height), height, "Height must be positive.");
    }

    int cellCount;
    try
    {
      cellCount = checked(width * height);
    }
    catch (OverflowException)
    {
      throw new ArgumentOutOfRangeException(nameof(width), width, "The combined buffer dimensions exceed the supported cell count.");
    }

    if (cellCount > Array.MaxLength)
    {
      throw new ArgumentOutOfRangeException(nameof(width), width, "The combined buffer dimensions exceed the supported cell count.");
    }

    return cellCount;
  }
}
