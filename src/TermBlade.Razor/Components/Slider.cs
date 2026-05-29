using Microsoft.AspNetCore.Components;
using TermBlade.Core.Renderables;
using TermBlade.Core.Rendering;

namespace TermBlade.Razor.Components;

public sealed class Slider : RenderableComponentBase<SliderRenderable>
{
    [Parameter] public float Min { get; set; } = 0;
    [Parameter] public float Max { get; set; } = 100;
    [Parameter] public float Value { get; set; } = 50;
    [Parameter] public EventCallback<float> ValueChanged { get; set; }
    [Parameter] public EventCallback<float> OnValueChanged { get; set; }
    [Parameter] public float Step { get; set; } = 1;
    [Parameter] public string Orientation { get; set; } = "horizontal";
    [Parameter] public string? TrackColor { get; set; }
    [Parameter] public string? ThumbColor { get; set; }
    [Parameter] public string? ValueColor { get; set; }

    protected override SliderRenderable CreateRenderable(CliRenderer renderer) => new(renderer);

    protected override void OnRenderableCreated(SliderRenderable renderable)
    {
        renderable.On("valueChanged", data =>
        {
            var value = data is float single ? single : Convert.ToSingle(data ?? 0f);
            DispatchEvent(async () =>
            {
                if (ValueChanged.HasDelegate)
                    await ValueChanged.InvokeAsync(value).ConfigureAwait(false);
                if (OnValueChanged.HasDelegate)
                    await OnValueChanged.InvokeAsync(value).ConfigureAwait(false);
            });
        });
    }

    protected override void ApplyParameters(SliderRenderable renderable)
    {
        renderable.Min = Min;
        renderable.Max = Max;
        renderable.Value = Value;
        renderable.Step = Step;
        renderable.Orientation = Orientation;
        renderable.TrackColor = TrackColor;
        renderable.ThumbColor = ThumbColor;
        renderable.ValueColor = ValueColor;
    }
}
