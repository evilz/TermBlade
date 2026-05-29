using System.Text;
using Microsoft.Extensions.Hosting;
using TermBlade.Razor.Hosting;
using TermBlade.Razor.Samples.Components;

Console.OutputEncoding = Encoding.UTF8;

var sample = args.Length > 0 ? args[0].ToLowerInvariant() : "layout";

var builder = Host.CreateDefaultBuilder(args);

var hostBuilder = sample switch
{
    "layout" => builder.UseTermBladeRazor<LayoutSample>(),
    "styled" => builder.UseTermBladeRazor<StyledSample>(),
    "editor" => builder.UseTermBladeRazor<EditorSample>(),
    "scroll" => builder.UseTermBladeRazor<ScrollSample>(),
    "input" => builder.UseTermBladeRazor<InputSample>(),
    "keypress" => builder.UseTermBladeRazor<KeypressSample>(),
    "ascii" => builder.UseTermBladeRazor<AsciiSample>(),
    "framebuffer" => builder.UseTermBladeRazor<FrameBufferSample>(),
    "code" => builder.UseTermBladeRazor<CodeSample>(),
    "markdown" => builder.UseTermBladeRazor<MarkdownSample>(),
    "diff" => builder.UseTermBladeRazor<DiffSample>(),
    "select" => builder.UseTermBladeRazor<SelectSample>(),
    "slider" => builder.UseTermBladeRazor<SliderSample>(),
    "tabs" => builder.UseTermBladeRazor<TabsSample>(),
    "console" => builder.UseTermBladeRazor<ConsoleSample>(),
    _ => throw new InvalidOperationException($"Unknown sample: '{sample}'. Available samples: layout, styled, editor, scroll, input, keypress, ascii, framebuffer, code, markdown, diff, select, slider, tabs, console")
};

var host = hostBuilder.Build();
await host.RunAsync();
