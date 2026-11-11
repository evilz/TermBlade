using System.Globalization;
using Microsoft.Extensions.DependencyInjection;
using TermBlade.Core.Layout;
using TermBlade.Core.Renderables;
using TermBlade.Core.Rendering;
using TermBlade.Razor.Components;
using TermBlade.Razor.Hosting;
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
  [InlineData(-1)]
  [InlineData(-1f)]
  [InlineData(-1d)]
  [InlineData(double.NaN)]
  [InlineData(double.PositiveInfinity)]
  public void ParseRejectsInvalidNumericValuesWithValueParameter(object value)
  {
    var exception = Assert.Throws<ArgumentOutOfRangeException>(() => LayoutDimension.Parse(value));
    Assert.Equal("value", exception.ParamName);
  }

  [Theory]
  [InlineData("-1")]
  [InlineData("-1%")]
  [InlineData("NaN")]
  [InlineData("Infinity")]
  [InlineData("NaN%")]
  public void ParseRejectsNegativeAndNonFiniteValues(string value)
  {
    var exception = Assert.Throws<ArgumentOutOfRangeException>(() => LayoutDimension.Parse(value));
    Assert.Equal("value", exception.ParamName);
  }

  [Fact]
  public void NumericFactoriesRejectNegativeAndNonFiniteValues()
  {
    Assert.Throws<ArgumentOutOfRangeException>(() => LayoutDimension.Fixed(-1));
    Assert.Throws<ArgumentOutOfRangeException>(() => LayoutDimension.Percent(float.PositiveInfinity));
    Assert.Throws<ArgumentOutOfRangeException>(() => LayoutDimension.Fixed((float)double.NaN));
  }

  [Theory]
  [InlineData("en-US")]
  [InlineData("fr-FR")]
  [InlineData("de-DE")]
  public async Task RazorWidthParameterUsesInvariantCulture(string cultureName)
  {
    var previousCulture = CultureInfo.CurrentCulture;
    var previousUiCulture = CultureInfo.CurrentUICulture;
    try
    {
      var culture = CultureInfo.GetCultureInfo(cultureName);
      CultureInfo.CurrentCulture = culture;
      CultureInfo.CurrentUICulture = culture;

      using var services = new ServiceCollection()
          .AddLogging()
          .AddTermBladeRazor()
          .Configure<TermBladeRazorOptions>(options => options.Testing = true)
          .BuildServiceProvider();
      var componentRenderer = services.GetRequiredService<NoopComponentRenderer>();
      await componentRenderer.MountComponentAsync<WidthProbeComponent>();

      var renderable = Assert.Single(services.GetRequiredService<TermBladeAppContext>().Renderer.Root.GetChildren());
      Assert.Equal("layout-width-probe", renderable.Id);
      Assert.Equal(LayoutDimension.DimType.Percent, renderable.LayoutNode.Width.Type);
      Assert.Equal(12.5f, renderable.LayoutNode.Width.Value);

#pragma warning disable BL0006 // The regression directly disposes the component renderer before its app context.
      await componentRenderer.DisposeAsync();
#pragma warning restore BL0006
    }
    finally
    {
      CultureInfo.CurrentCulture = previousCulture;
      CultureInfo.CurrentUICulture = previousUiCulture;
    }
  }

  private sealed class WidthProbeComponent : RenderableComponentBase<WidthProbeRenderable>
  {
    public WidthProbeComponent()
    {
      Id = "layout-width-probe";
      Width = "12.5%";
    }

    protected override WidthProbeRenderable CreateRenderable(TermBlade.Core.Rendering.CliRenderer renderer)
      => new(renderer);

    protected override void ApplyParameters(WidthProbeRenderable renderable) { }
  }

  private sealed class WidthProbeRenderable : Renderable
  {
    public WidthProbeRenderable() : base() { }
    public WidthProbeRenderable(CliRenderer renderer) : base(renderer) { }

    protected override void RenderSelf(RenderBuffer buffer, double deltaTime) { }
  }
}
