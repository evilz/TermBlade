using TermBlade.Core.Ansi;
using TermBlade.Core.Input;
using TermBlade.Core.Rendering;

namespace TermBlade.Core.Renderables;

/// <summary>
/// Represents slider renderable.
/// </summary>
public class SliderRenderable : Renderable
{
  /// <summary>
  /// Gets or sets the min.
  /// </summary>
  public float Min { get; set; } = 0;
  /// <summary>
  /// Gets or sets the max.
  /// </summary>
  public float Max { get; set; } = 100;
  /// <summary>
  /// Gets or sets the value.
  /// </summary>
  public float Value { get; set; } = 50;
  /// <summary>
  /// Gets or sets the step.
  /// </summary>
  public float Step { get; set; } = 1;
  /// <summary>
  /// Gets or sets the orientation.
  /// </summary>
  public string Orientation { get; set; } = "horizontal";
  /// <summary>
  /// Gets or sets the track color.
  /// </summary>
  public string? TrackColor { get; set; }
  /// <summary>
  /// Gets or sets the thumb color.
  /// </summary>
  public string? ThumbColor { get; set; }
  /// <summary>
  /// Gets or sets the value color.
  /// </summary>
  public string? ValueColor { get; set; }

  /// <summary>
  /// Slider renderable.
  /// </summary>
  /// <param name="base(renderer">The base(renderer value.</param>
  public SliderRenderable(CliRenderer? renderer) : base(renderer)
  {
    Focusable = true;
  }

  /// <summary>
  /// Handle key.
  /// </summary>
  /// <param name="key">The key value.</param>
  public override void HandleKey(KeyEvent key)
  {
    bool isHorizontal = Orientation == "horizontal";
    switch (key.Name)
    {
      case "left" when isHorizontal:
      case "down" when !isHorizontal:
        SetValue(Value - Step);
        break;
      case "right" when isHorizontal:
      case "up" when !isHorizontal:
        SetValue(Value + Step);
        break;
    }
  }

  /// <summary>
  /// Handle mouse.
  /// </summary>
  /// <param name="mouse">The mouse value.</param>
  public override void HandleMouse(MouseEvent mouse)
  {
    if (mouse.Button != MouseButton.Left || !mouse.Pressed)
      return;

    Focus();
    bool isHorizontal = Orientation == "horizontal";
    int trackLen = Math.Max(1, isHorizontal ? ComputedWidth : ComputedHeight);
    int position = isHorizontal
        ? Math.Clamp(mouse.X - ScreenX, 0, trackLen - 1)
        : Math.Clamp(mouse.Y - ScreenY, 0, trackLen - 1);

    float ratio = trackLen <= 1 ? 0 : (float)position / (trackLen - 1);
    if (!isHorizontal)
      ratio = 1f - ratio;

    SetValue(Min + ratio * (Max - Min), snapToStep: true);
  }

  private void SetValue(float value, bool snapToStep = false)
  {
    var next = Math.Clamp(value, Min, Max);
    if (snapToStep && Step > 0)
      next = Math.Clamp(Min + MathF.Round((next - Min) / Step) * Step, Min, Max);

    if (Math.Abs(next - Value) < 0.0001f)
      return;

    Value = next;
    Emit("valueChanged", Value);
    RequestRender();
  }

  protected override void RenderSelf(RenderBuffer buffer, double deltaTime)
  {
    int x = ScreenX, y = ScreenY, w = ComputedWidth, h = ComputedHeight;
    if (w <= 0 || h <= 0) return;

    var trackFg = TrackColor != null ? Rgba.FromCss(TrackColor) : Rgba.FromInts(80, 80, 80);
    var thumbFg = ThumbColor != null ? Rgba.FromCss(ThumbColor) : Rgba.FromInts(200, 200, 200);
    var valueFg = ValueColor != null ? Rgba.FromCss(ValueColor) : thumbFg;
    var trackBg = Focused ? Rgba.FromInts(35, 35, 50) : Rgba.FromInts(20, 20, 20);

    bool isHorizontal = Orientation == "horizontal";
    float ratio = Max > Min ? (Value - Min) / (Max - Min) : 0;

    if (isHorizontal)
    {
      int trackLen = w;
      int thumbPos = (int)(ratio * (trackLen - 1));

      for (int i = 0; i < trackLen; i++)
      {
        bool isThumb = i == thumbPos;
        bool isFilled = i < thumbPos;
        buffer.SetCell(x + i, y, isThumb ? '●' : '─',
            GetTrackColor(isThumb, isFilled, thumbFg, valueFg, trackFg), trackBg);
      }
    }
    else
    {
      int trackLen = h;
      int thumbPos = (int)((1f - ratio) * (trackLen - 1));

      for (int i = 0; i < trackLen; i++)
      {
        bool isThumb = i == thumbPos;
        bool isFilled = i > thumbPos;
        buffer.SetCell(x, y + i, isThumb ? '●' : '│',
            GetTrackColor(isThumb, isFilled, thumbFg, valueFg, trackFg), trackBg);
      }
    }
  }

  private static Rgba GetTrackColor(bool isThumb, bool isFilled, Rgba thumbFg, Rgba valueFg, Rgba trackFg)
  {
    if (isThumb)
      return thumbFg;

    return isFilled ? valueFg : trackFg;
  }
}
