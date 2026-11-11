using System.Globalization;
using TermBlade.Core.Layout;
using TermBlade.Core.Renderables;
using TermBlade.Core.Rendering;
using Xunit;

namespace TermBlade.Tests;

public class LayoutDimensionTests
{
  [Theory]
  [InlineData("en-US")]
  [InlineData("fr-FR")]
  [InlineData("de-DE")]
  public void ParseAndRenderableWidthUseInvariantNumericSyntax(string cultureName)
  {
    var previousCulture = CultureInfo.CurrentCulture;
    var previousUiCulture = CultureInfo.CurrentUICulture;
    try
    {
      var culture = CultureInfo.GetCultureInfo(cultureName);
      CultureInfo.CurrentCulture = culture;
      CultureInfo.CurrentUICulture = culture;

      var fixedDimension = LayoutDimension.Parse("12.5");
      var percentDimension = LayoutDimension.Parse("12.5%");

      Assert.Equal(LayoutDimension.DimType.Fixed, fixedDimension.Type);
      Assert.Equal(12.5f, fixedDimension.Value);
      Assert.Equal(LayoutDimension.DimType.Percent, percentDimension.Type);
      Assert.Equal(12.5f, percentDimension.Value);
      Assert.Equal(25f, percentDimension.Resolve(200));

      var renderable = new WidthProbeRenderable();
      renderable.SetWidth("12.5%");
      Assert.Equal(percentDimension, renderable.LayoutNode.Width);
    }
    finally
    {
      CultureInfo.CurrentCulture = previousCulture;
      CultureInfo.CurrentUICulture = previousUiCulture;
    }
  }

  [Theory]
  [InlineData("fr-FR")]
  [InlineData("de-DE")]
  public void ParseRejectsLocaleSpecificDecimalSeparators(string cultureName)
  {
    var previousCulture = CultureInfo.CurrentCulture;
    try
    {
      CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo(cultureName);

      Assert.Throws<FormatException>(() => LayoutDimension.Parse("12,5"));
      Assert.Throws<FormatException>(() => LayoutDimension.Parse("12,5%"));
    }
    finally
    {
      CultureInfo.CurrentCulture = previousCulture;
    }
  }

  [Theory]
  [InlineData("")]
  [InlineData("not-a-dimension")]
  [InlineData("12px")]
  public void ParseRejectsMalformedStrings(string value)
  {
    Assert.Throws<FormatException>(() => LayoutDimension.Parse(value));
  }

  [Theory]
  [InlineData("-1")]
  [InlineData("-1%")]
  [InlineData("NaN")]
  [InlineData("Infinity")]
  [InlineData("NaN%")]
  public void ParseRejectsNegativeAndNonFiniteValues(string value)
  {
    Assert.Throws<ArgumentOutOfRangeException>(() => LayoutDimension.Parse(value));
  }

  [Fact]
  public void NumericFactoriesRejectNegativeAndNonFiniteValues()
  {
    Assert.Throws<ArgumentOutOfRangeException>(() => LayoutDimension.Fixed(-1));
    Assert.Throws<ArgumentOutOfRangeException>(() => LayoutDimension.Percent(float.PositiveInfinity));
    Assert.Throws<ArgumentOutOfRangeException>(() => LayoutDimension.Fixed((float)double.NaN));
  }

  private sealed class WidthProbeRenderable : Renderable
  {
    public WidthProbeRenderable() : base() { }

    protected override void RenderSelf(RenderBuffer buffer, double deltaTime) { }
  }
}
