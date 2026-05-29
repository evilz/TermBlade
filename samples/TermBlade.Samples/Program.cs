using System;

Console.OutputEncoding = System.Text.Encoding.UTF8;

var sample = args.Length > 0 ? args[0].ToLowerInvariant() : "layout";

switch (sample)
{
  case "layout":
    TermBlade.Samples.SimpleLayoutSample.Run();
    break;
  case "styled":
    TermBlade.Samples.StyledTextSample.Run();
    break;
  case "editor":
    TermBlade.Samples.EditorSample.Run();
    break;
  case "scroll":
    TermBlade.Samples.ScrollSample.Run();
    break;
  case "input":
    TermBlade.Samples.InputSample.Run();
    break;
  case "keypress":
    TermBlade.Samples.KeypressDebugSample.Run();
    break;
  case "ascii":
    TermBlade.Samples.AsciFontSample.Run();
    break;
  case "nova":
    TermBlade.Samples.NovaTuiShowcaseSample.Run();
    break;
  case "framebuffer":
    TermBlade.Samples.FrameBufferSample.Run();
    break;
  case "code":
    TermBlade.Samples.CodeSample.Run();
    break;
  case "markdown":
    TermBlade.Samples.MarkdownSample.Run();
    break;
  case "diff":
    TermBlade.Samples.DiffSample.Run();
    break;
  case "select":
    TermBlade.Samples.SelectSample.Run();
    break;
  case "slider":
    TermBlade.Samples.SliderSample.Run();
    break;
  case "tabs":
    TermBlade.Samples.TabSelectSample.Run();
    break;
  case "console":
    TermBlade.Samples.ConsoleDemoSample.Run();
    break;
  default:
    Console.WriteLine($"Unknown sample: '{sample}'");
    Console.WriteLine("Available samples: layout, styled, editor, scroll, input, keypress, ascii, nova, framebuffer, code, markdown, diff, select, slider, tabs, console");
    Environment.Exit(1);
    break;
}
