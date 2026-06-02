using System.Text;

namespace TermBlade.Docs.Wasm.Terminal.Commands;

/// <summary>
/// Demo command that renders ANSI previews of TermBlade components.
/// All output is purely visual — no real component instantiation required.
/// </summary>
public sealed class DemoCommand : ITerminalCommand
{
    public string Name => "demo";
    public string Description => "Show a visual demo of a component (e.g. demo button, demo table, demo tree)";

    public async Task ExecuteAsync(string[] args, ITerminalOutput output)
    {
        if (args.Length == 0)
        {
            await output.WriteLineAsync("\x1b[33mUsage: demo <component>\x1b[0m");
            await output.WriteLineAsync("\x1b[2mAvailable: button, table, tree, box, chart, spinner, calendar, text\x1b[0m");
            return;
        }

        var component = args[0].ToLowerInvariant();

        switch (component)
        {
            case "button":
                await RenderButtonDemo(output);
                break;
            case "table":
                await RenderTableDemo(output);
                break;
            case "tree":
                await RenderTreeDemo(output);
                break;
            case "box":
                await RenderBoxDemo(output);
                break;
            case "chart":
                await RenderChartDemo(output);
                break;
            case "spinner":
                await RenderSpinnerDemo(output);
                break;
            case "calendar":
                await RenderCalendarDemo(output);
                break;
            case "text":
                await RenderTextDemo(output);
                break;
            default:
                await output.WriteLineAsync($"\x1b[31mUnknown component: {component}\x1b[0m");
                await output.WriteLineAsync("\x1b[2mAvailable: button, table, tree, box, chart, spinner, calendar, text\x1b[0m");
                break;
        }
    }

    private static async Task RenderButtonDemo(ITerminalOutput output)
    {
        await output.WriteLineAsync("");
        await output.WriteLineAsync("\x1b[1;33m  Button Component Demo\x1b[0m");
        await output.WriteLineAsync("");
        await output.WriteLineAsync("  \x1b[48;2;60;60;180m\x1b[1;37m  ▸ Submit   \x1b[0m   \x1b[48;2;50;50;60m\x1b[37m  Cancel   \x1b[0m   \x1b[48;2;180;50;50m\x1b[1;37m  Delete   \x1b[0m");
        await output.WriteLineAsync("");
        await output.WriteLineAsync("  \x1b[48;2;40;160;80m\x1b[1;37m  ✓ Confirm  \x1b[0m   \x1b[48;2;200;160;0m\x1b[1;30m  ⚠ Warning \x1b[0m   \x1b[48;2;80;80;80m\x1b[2;37m  Disabled  \x1b[0m");
        await output.WriteLineAsync("");
        await output.WriteLineAsync("  \x1b[2mButtons support focus, hover, and click states.\x1b[0m");
        await output.WriteLineAsync("");
    }

    private static async Task RenderTableDemo(ITerminalOutput output)
    {
        await output.WriteLineAsync("");
        await output.WriteLineAsync("\x1b[1;33m  Table Component Demo\x1b[0m");
        await output.WriteLineAsync("");
        await output.WriteLineAsync("  \x1b[36m╭──────────────┬──────────┬────────────╮\x1b[0m");
        await output.WriteLineAsync("  \x1b[36m│\x1b[0m \x1b[1;37mComponent    \x1b[36m│\x1b[0m \x1b[1;37mStatus   \x1b[36m│\x1b[0m \x1b[1;37mVersion    \x1b[36m│\x1b[0m");
        await output.WriteLineAsync("  \x1b[36m├──────────────┼──────────┼────────────┤\x1b[0m");
        await output.WriteLineAsync("  \x1b[36m│\x1b[0m Box          \x1b[36m│\x1b[0m \x1b[32m● Stable \x1b[36m│\x1b[0m 1.0.0      \x1b[36m│\x1b[0m");
        await output.WriteLineAsync("  \x1b[36m│\x1b[0m Text         \x1b[36m│\x1b[0m \x1b[32m● Stable \x1b[36m│\x1b[0m 1.0.0      \x1b[36m│\x1b[0m");
        await output.WriteLineAsync("  \x1b[36m│\x1b[0m TreeView     \x1b[36m│\x1b[0m \x1b[33m● Beta   \x1b[36m│\x1b[0m 0.9.0      \x1b[36m│\x1b[0m");
        await output.WriteLineAsync("  \x1b[36m│\x1b[0m LineChart    \x1b[36m│\x1b[0m \x1b[33m● Beta   \x1b[36m│\x1b[0m 0.9.0      \x1b[36m│\x1b[0m");
        await output.WriteLineAsync("  \x1b[36m│\x1b[0m HeatMap      \x1b[36m│\x1b[0m \x1b[35m● Alpha  \x1b[36m│\x1b[0m 0.5.0      \x1b[36m│\x1b[0m");
        await output.WriteLineAsync("  \x1b[36m╰──────────────┴──────────┴────────────╯\x1b[0m");
        await output.WriteLineAsync("");
        await output.WriteLineAsync("  \x1b[2mTables support rounded, single, and double border styles.\x1b[0m");
        await output.WriteLineAsync("");
    }

    private static async Task RenderTreeDemo(ITerminalOutput output)
    {
        await output.WriteLineAsync("");
        await output.WriteLineAsync("\x1b[1;33m  TreeView Component Demo\x1b[0m");
        await output.WriteLineAsync("");
        await output.WriteLineAsync("  \x1b[1;37m📁 src\x1b[0m");
        await output.WriteLineAsync("  \x1b[36m├── \x1b[0m\x1b[1;37m📁 TermBlade.Core\x1b[0m");
        await output.WriteLineAsync("  \x1b[36m│   ├── \x1b[0m\x1b[37m📁 Ansi\x1b[0m");
        await output.WriteLineAsync("  \x1b[36m│   │   ├── \x1b[0m\x1b[32mAnsiCodes.cs\x1b[0m");
        await output.WriteLineAsync("  \x1b[36m│   │   ├── \x1b[0m\x1b[32mRgba.cs\x1b[0m");
        await output.WriteLineAsync("  \x1b[36m│   │   └── \x1b[0m\x1b[32mTextAttributes.cs\x1b[0m");
        await output.WriteLineAsync("  \x1b[36m│   ├── \x1b[0m\x1b[37m📁 Buffer\x1b[0m");
        await output.WriteLineAsync("  \x1b[36m│   │   ├── \x1b[0m\x1b[32mCellBuffer.cs\x1b[0m");
        await output.WriteLineAsync("  \x1b[36m│   │   └── \x1b[0m\x1b[32mCell.cs\x1b[0m");
        await output.WriteLineAsync("  \x1b[36m│   ├── \x1b[0m\x1b[37m📁 Renderables\x1b[0m");
        await output.WriteLineAsync("  \x1b[36m│   │   ├── \x1b[0m\x1b[32mBox.cs\x1b[0m");
        await output.WriteLineAsync("  \x1b[36m│   │   ├── \x1b[0m\x1b[32mText.cs\x1b[0m");
        await output.WriteLineAsync("  \x1b[36m│   │   └── \x1b[0m\x1b[2m... 25 more\x1b[0m");
        await output.WriteLineAsync("  \x1b[36m│   └── \x1b[0m\x1b[37m📁 Rendering\x1b[0m");
        await output.WriteLineAsync("  \x1b[36m└── \x1b[0m\x1b[1;37m📁 TermBlade.Razor\x1b[0m");
        await output.WriteLineAsync("");
        await output.WriteLineAsync("  \x1b[2mTreeView supports expand/collapse, icons, and nested nodes.\x1b[0m");
        await output.WriteLineAsync("");
    }

    private static async Task RenderBoxDemo(ITerminalOutput output)
    {
        await output.WriteLineAsync("");
        await output.WriteLineAsync("\x1b[1;33m  Box Component Demo\x1b[0m");
        await output.WriteLineAsync("");
        await output.WriteLineAsync("  \x1b[36m╭─────────────────────────────────╮\x1b[0m");
        await output.WriteLineAsync("  \x1b[36m│\x1b[0m                                 \x1b[36m│\x1b[0m");
        await output.WriteLineAsync("  \x1b[36m│\x1b[0m  \x1b[1;37mRounded Box\x1b[0m with padding       \x1b[36m│\x1b[0m");
        await output.WriteLineAsync("  \x1b[36m│\x1b[0m  and \x1b[3mcontent styling\x1b[0m.           \x1b[36m│\x1b[0m");
        await output.WriteLineAsync("  \x1b[36m│\x1b[0m                                 \x1b[36m│\x1b[0m");
        await output.WriteLineAsync("  \x1b[36m╰─────────────────────────────────╯\x1b[0m");
        await output.WriteLineAsync("");
        await output.WriteLineAsync("  \x1b[34m┌─────────────────────────────────┐\x1b[0m");
        await output.WriteLineAsync("  \x1b[34m│\x1b[0m  \x1b[1;37mSingle Border\x1b[0m style            \x1b[34m│\x1b[0m");
        await output.WriteLineAsync("  \x1b[34m└─────────────────────────────────┘\x1b[0m");
        await output.WriteLineAsync("");
        await output.WriteLineAsync("  \x1b[35m╔═════════════════════════════════╗\x1b[0m");
        await output.WriteLineAsync("  \x1b[35m║\x1b[0m  \x1b[1;37mDouble Border\x1b[0m style            \x1b[35m║\x1b[0m");
        await output.WriteLineAsync("  \x1b[35m╚═════════════════════════════════╝\x1b[0m");
        await output.WriteLineAsync("");
        await output.WriteLineAsync("  \x1b[2mBoxes support Rounded, Single, Double, and custom border styles.\x1b[0m");
        await output.WriteLineAsync("");
    }

    private static async Task RenderChartDemo(ITerminalOutput output)
    {
        await output.WriteLineAsync("");
        await output.WriteLineAsync("\x1b[1;33m  Chart Component Demo — Bar Chart\x1b[0m");
        await output.WriteLineAsync("");
        await output.WriteLineAsync("  \x1b[37mCommits per month:\x1b[0m");
        await output.WriteLineAsync("");
        await output.WriteLineAsync("  Jan \x1b[42m          \x1b[0m 12");
        await output.WriteLineAsync("  Feb \x1b[42m                   \x1b[0m 23");
        await output.WriteLineAsync("  Mar \x1b[42m                        \x1b[0m 31");
        await output.WriteLineAsync("  Apr \x1b[42m              \x1b[0m 18");
        await output.WriteLineAsync("  May \x1b[42m                              \x1b[0m 38");
        await output.WriteLineAsync("  Jun \x1b[42m                                    \x1b[0m 45");
        await output.WriteLineAsync("");
        await output.WriteLineAsync("  \x1b[2mSupported chart types: LineChart, BarChart, PieChart,\x1b[0m");
        await output.WriteLineAsync("  \x1b[2mDoughnutChart, CandlestickChart, HeatMap, TimeSeriesLineChart.\x1b[0m");
        await output.WriteLineAsync("");
    }

    private static async Task RenderSpinnerDemo(ITerminalOutput output)
    {
        await output.WriteLineAsync("");
        await output.WriteLineAsync("\x1b[1;33m  Spinner Component Demo\x1b[0m");
        await output.WriteLineAsync("");
        await output.WriteLineAsync("  \x1b[36m⠋\x1b[0m Loading data...      \x1b[2m(dots)\x1b[0m");
        await output.WriteLineAsync("  \x1b[33m◐\x1b[0m Processing...        \x1b[2m(circle)\x1b[0m");
        await output.WriteLineAsync("  \x1b[32m▰▰▰▰▱▱▱▱▱▱\x1b[0m 40%      \x1b[2m(progress)\x1b[0m");
        await output.WriteLineAsync("  \x1b[35m🌑\x1b[0m Syncing...           \x1b[2m(moon)\x1b[0m");
        await output.WriteLineAsync("");
        await output.WriteLineAsync("  \x1b[2mSpinners animate in the real terminal with configurable frame rates.\x1b[0m");
        await output.WriteLineAsync("");
    }

    private static async Task RenderCalendarDemo(ITerminalOutput output)
    {
        await output.WriteLineAsync("");
        await output.WriteLineAsync("\x1b[1;33m  Calendar Component Demo\x1b[0m");
        await output.WriteLineAsync("");
        await output.WriteLineAsync("  \x1b[1;37m      June 2026         \x1b[0m");
        await output.WriteLineAsync("  \x1b[36m Mo Tu We Th Fr \x1b[33mSa Su\x1b[0m");
        await output.WriteLineAsync("  \x1b[37m  1  2  3  4  5 \x1b[33m 6  7\x1b[0m");
        await output.WriteLineAsync("  \x1b[37m  8  9 10 11 12 \x1b[33m13 14\x1b[0m");
        await output.WriteLineAsync("  \x1b[37m 15 16 17 18 19 \x1b[33m20 21\x1b[0m");
        await output.WriteLineAsync("  \x1b[37m 22 23 24 25 26 \x1b[33m27 28\x1b[0m");
        await output.WriteLineAsync("  \x1b[37m 29 30            \x1b[0m");
        await output.WriteLineAsync("");
        await output.WriteLineAsync("  \x1b[2mCalendar supports date selection, range highlighting, and locales.\x1b[0m");
        await output.WriteLineAsync("");
    }

    private static async Task RenderTextDemo(ITerminalOutput output)
    {
        await output.WriteLineAsync("");
        await output.WriteLineAsync("\x1b[1;33m  Text Styling Demo\x1b[0m");
        await output.WriteLineAsync("");
        await output.WriteLineAsync("  \x1b[1mBold text\x1b[0m");
        await output.WriteLineAsync("  \x1b[2mDim text\x1b[0m");
        await output.WriteLineAsync("  \x1b[3mItalic text\x1b[0m");
        await output.WriteLineAsync("  \x1b[4mUnderlined text\x1b[0m");
        await output.WriteLineAsync("  \x1b[9mStrikethrough text\x1b[0m");
        await output.WriteLineAsync("  \x1b[7mInverse text\x1b[0m");
        await output.WriteLineAsync("");
        await output.WriteLineAsync("  \x1b[38;2;255;100;100mRed\x1b[0m \x1b[38;2;100;255;100mGreen\x1b[0m \x1b[38;2;100;100;255mBlue\x1b[0m \x1b[38;2;255;255;100mYellow\x1b[0m \x1b[38;2;255;100;255mMagenta\x1b[0m \x1b[38;2;100;255;255mCyan\x1b[0m");
        await output.WriteLineAsync("");
        await output.WriteLineAsync("  \x1b[48;2;60;20;20m\x1b[38;2;255;200;200m  Styled background with foreground color  \x1b[0m");
        await output.WriteLineAsync("");
        await output.WriteLineAsync("  \x1b[2mTermBlade supports RGB, ANSI-256, and default terminal colors.\x1b[0m");
        await output.WriteLineAsync("");
    }
}
