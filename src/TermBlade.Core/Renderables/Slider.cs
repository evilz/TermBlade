using TermBlade.Core.Ansi;
using TermBlade.Core.Input;
using TermBlade.Core.Rendering;

namespace TermBlade.Core.Renderables;

public class SliderRenderable : Renderable
{
  public float Min { get; set; } = 0;
  public float Max { get; set; } = 100;
  public float Value { get; set; } = 50;
  public float Step { get; set; } = 1;
  public string Orientation { get; set; } = "horizontal";
  public string? TrackColor { get; set; }
  public string? ThumbColor { get; set; }
  public string? ValueColor { get; set; }

  public SliderRenderable(CliRenderer? renderer) : base(renderer)
  {
    Focusable = true;
  }

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
            isThumb ? thumbFg : isFilled ? valueFg : trackFg, trackBg);
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
            isThumb ? thumbFg : isFilled ? valueFg : trackFg, trackBg);
      }
    }
  }
}
