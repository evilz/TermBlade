using TermBlade.Core.Ansi;
using TermBlade.Core.Input;
using TermBlade.Core.Rendering;

namespace TermBlade.Core.Renderables;

public class ConfirmRenderable : Renderable
{
  public string Message { get; set; } = "Are you sure?";
  public bool Value { get; set; }
  public string YesLabel { get; set; } = "Yes";
  public string NoLabel { get; set; } = "No";
  public string? Fg { get; set; }
  public string? Bg { get; set; }
  public string? ActiveBg { get; set; } = "#0055aa";

  public ConfirmRenderable(CliRenderer? renderer) : base(renderer)
  {
    Focusable = true;
  }

  public override void HandleKey(KeyEvent key)
  {
    switch (key.Name)
    {
      case "left":
      case "right":
      case "tab":
        Value = !Value;
        Emit("valueChanged", Value);
        RequestRender();
        break;
      case "y":
        Value = true;
        Emit("valueChanged", Value);
        Emit("confirmed", Value);
        RequestRender();
        break;
      case "n":
        Value = false;
        Emit("valueChanged", Value);
        Emit("confirmed", Value);
        RequestRender();
        break;
      case "return":
        Emit("confirmed", Value);
        break;
    }
  }

  protected override void RenderSelf(RenderBuffer buffer, double deltaTime)
  {
    int x = ScreenX, y = ScreenY, w = ComputedWidth, h = ComputedHeight;
    if (w <= 0 || h <= 0) return;

    var fg = Fg != null ? Rgba.FromCss(Fg) : Rgba.FromInts(255, 255, 255);
    var bg = Bg != null ? Rgba.FromCss(Bg) : Rgba.FromInts(0, 0, 0);
    var activeBg = ActiveBg != null ? Rgba.FromCss(ActiveBg) : Rgba.FromInts(0, 85, 170);

    buffer.FillRect(x, y, w, h, bg);

    // Draw message
    var msg = Message.Length > w ? Message[..w] : Message;
    buffer.DrawText(x, y, msg, fg, bg);

    // Draw Yes/No buttons on the next row
    if (h > 1)
    {
      var yesBg = Value ? activeBg : bg;
      var noBg = !Value ? activeBg : bg;

      var yesText = $" {YesLabel} ";
      var noText = $" {NoLabel} ";

      buffer.DrawText(x, y + 1, yesText, fg, yesBg);
      buffer.DrawText(x + yesText.Length + 1, y + 1, noText, fg, noBg);
    }
  }
}
