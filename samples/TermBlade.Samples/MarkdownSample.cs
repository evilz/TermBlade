using TermBlade.Core.Input;
using TermBlade.Core.Rendering;
using TermBlade.Core.Renderables;

namespace TermBlade.Samples;

internal static class MarkdownSample
{
  public static void Run()
  {
    var config = new CliRendererConfig { ExitOnCtrlC = true, TargetFps = 30 };
    var renderer = new CliRenderer(config);

    var root = renderer.Root;

    var box = new BoxRenderable(renderer, new BoxOptions
    {
      Border = true,
      BorderStyle = "rounded",
      BorderColor = "#e5c07b",
      Title = " Markdown Viewer ",
      FlexDirection = TermBlade.Core.Layout.FlexDirection.Column,
      FlexGrow = 1
    });
    box.SetWidth("100%");
    box.SetHeight("100%");
    root.Add(box);

    var md = new MarkdownRenderable(renderer)
    {
      Content = """
# TermBlade Framework

## Features

- **Terminal UI** components for .NET
- *Flexbox* layout engine
- `Syntax highlighting` for code
- Markdown rendering with headings, lists, quotes and rules
- Unicode content: ✅ ⚠️ 🚀 and CJK text like 世界

## Getting Started

```csharp
var renderer = new CliRenderer();
var box = new BoxRenderable(renderer);
renderer.Root.Add(box);
renderer.Start();
```

### Components

1. Box - Container with optional border
2. Text - Styled text display
3. Input - Single-line text input
4. Select - Scrollable list
5. Slider - Keyboard and mouse driven values

> This is a **blockquote** showing important information.
> It can span multiple lines in the sample content.

## Task List

- [x] Render headings
- [x] Render code blocks
- [ ] Add your own custom renderer

## Table-style Content

| Key | Description |
| --- | --- |
| Tab | Move focus |
| Esc | Exit samples |

---

Visit [GitHub](https://github.com/evilz/termblade) for more info.
"""
    };
    md.SetWidth("100%");
    md.FlexGrow = 1;
    box.Add(md);

    var hint = new TextRenderable(renderer, new TextOptions
    {
      Content = "Press q or Escape to quit",
      Fg = "#888888"
    });
    box.Add(hint);

    renderer.KeyInput.On("keypress", (KeyEvent key) =>
    {
      if (key.Name == "q" || key.Name == "escape") renderer.Destroy();
    });

    renderer.Start();
  }
}
