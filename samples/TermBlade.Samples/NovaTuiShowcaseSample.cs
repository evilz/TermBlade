using TermBlade.Core.Input;
using TermBlade.Core.Rendering;
using TermBlade.Core.Renderables;

namespace TermBlade.Samples;

internal static class NovaTuiShowcaseSample
{
    private const string BackgroundColor = "#1b1c2a";
    private const string AccentPurple = "#6057ff";
    private const string AccentPink = "#ff5ef2";
    private const string AccentGray = "#6b7280";
    private const string AccentMint = "#14d8a6";

    public static void Run()
    {
        var renderer = new CliRenderer(new CliRendererConfig
        {
            ExitOnCtrlC = true,
            TargetFps = 30,
            BackgroundColor = BackgroundColor
        });

        var root = new BoxRenderable(renderer, new BoxOptions
        {
            BackgroundColor = BackgroundColor
        });
        root.SetWidth("100%");
        root.SetHeight("100%");
        renderer.Root.Add(root);

        AddText(root, renderer, "╱╱╱╱╱", AccentPurple, 2, 1, 8);
        AddText(root, renderer, "╱╱╱╱╱", AccentPurple, 2, 2, 8);
        AddText(root, renderer, "╱╱╱╱╱", AccentPurple, 2, 3, 8);

        AddText(root, renderer, "NovaTUI", AccentPink, 10, 1, 14);

        var logo = new ASCIIFontRenderable(renderer)
        {
            Text = "NOVA",
            Font = "block",
            Color = "#c56aff",
            BackgroundColor = BackgroundColor
        };
        logo.Position = "absolute";
        logo.Left = 9;
        logo.Top = 2;
        logo.SetWidth(42);
        logo.SetHeight(4);
        root.Add(logo);

        AddText(root, renderer, "v1.0.0", "#6f6bff", 38, 1, 8);
        AddText(root, renderer, new string('╱', 60), AccentPurple, 50, 1, 62);
        AddText(root, renderer, "~", "#9ca3af", 2, 6, 2);

        AddText(root, renderer, "◇ phi4-mini-reasoning via Ollama", "#d1d5db", 2, 8, 40);

        AddText(root, renderer, "LSPs", AccentGray, 2, 10, 12);
        AddText(root, renderer, "MCPs", AccentGray, 30, 10, 12);
        AddText(root, renderer, "Skills", AccentGray, 60, 10, 16);

        AddText(root, renderer, "None", AccentGray, 2, 12, 12);
        AddText(root, renderer, "None", AccentGray, 30, 12, 12);
        AddText(root, renderer, "nova-config", "#d1d5db", 62, 12, 18);
        AddText(root, renderer, "nova-hooks", "#d1d5db", 62, 13, 18);
        AddText(root, renderer, "jq", "#d1d5db", 62, 14, 18);

        foreach (int top in new[] { 12, 13, 14 })
        {
            AddText(root, renderer, "●", AccentMint, 60, top, 2);
        }

        AddText(root, renderer, ">", "#2ef2d3", 3, 30, 2);
        AddText(root, renderer, "R", AccentPink, 5, 30, 2, "#2a1930");
        AddText(root, renderer, "eady?", AccentGray, 6, 30, 8);

        AddText(root, renderer, "::: ", AccentMint, 2, 31, 4);
        AddText(root, renderer, "::: ", AccentMint, 2, 32, 4);

        AddText(
            root,
            renderer,
            "/ or ctrl+p commands • ctrl+l models • ctrl+j newline • ctrl+c quit • ctrl+g more",
            AccentGray,
            2,
            34,
            110);

        renderer.KeyInput.On("keypress", (KeyEvent key) =>
        {
            if (key.Name is "q" or "escape")
            {
                renderer.Destroy();
            }
        });

        renderer.Start();
    }

    private static void AddText(BoxRenderable root, CliRenderer renderer, string content, string foregroundColor, int left, int top, int width, string? backgroundColor = null)
    {
        var text = new TextRenderable(renderer, new TextOptions
        {
            Content = content,
            Fg = foregroundColor,
            Bg = backgroundColor ?? BackgroundColor,
            Wrap = false,
            Height = 1
        });
        text.Position = "absolute";
        text.Left = left;
        text.Top = top;
        text.SetWidth(width);
        root.Add(text);
    }
}
