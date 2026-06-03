namespace TermBlade.Docs.Wasm.Terminal.Commands;

public sealed class ComponentsCommand : ITerminalCommand
{
  public string Name => "components";
  public string Description => "List all TermBlade components";

  public async Task ExecuteAsync(string[] args, ITerminalOutput output)
  {
    await output.WriteLineAsync("\x1b[1;33m 🗡️ TermBlade Components:\x1b[0m");
    await output.WriteLineAsync("");

    var categories = new (string Category, string[] Components)[]
    {
            ("Layout", ["Box", "ScrollBox", "ScrollBar"]),
            ("Text & Input", ["Text", "Input", "Textarea", "Confirm"]),
            ("Selection", ["Select", "MultiSelect", "TabSelect", "Slider"]),
            ("Data Display", ["TreeView", "Calendar", "Code", "Markdown", "Diff"]),
            ("Charts", ["LineChart", "BarChart", "PieChart", "DoughnutChart",
                         "CandlestickChart", "HeatMap", "TimeSeriesLineChart"]),
            ("Visual", ["Spinner", "ASCIIFont", "LineNumbers", "FrameBuffer"])
    };

    foreach (var (category, components) in categories)
    {
      await output.WriteLineAsync($"  \x1b[1;35m{category}\x1b[0m");
      foreach (var comp in components)
      {
        await output.WriteLineAsync($"    \x1b[36m•\x1b[0m {comp}");
      }
      await output.WriteLineAsync("");
    }

    await output.WriteLineAsync("  \x1b[2mUse 'demo <component>' to see a live preview.\x1b[0m");
    await output.WriteLineAsync("");
  }
}
