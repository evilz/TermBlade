using TermBlade.Core.Ansi;
using TermBlade.Core.Rendering;

namespace TermBlade.Core.Renderables;

public class ASCIIFontRenderable : Renderable
{
    public string Text { get; set; } = "";
    public string Font { get; set; } = "normal";
    public string? Color { get; set; }
    public string? BackgroundColor { get; set; }
    public string SelectionBg { get; set; } = "#4a5568";
    public string SelectionFg { get; set; } = "#ffffff";

    private int? _selectionStart;
    private int? _selectionEnd;

    // 5x3 character patterns: each char is 3 rows of 5 chars
    private static readonly Dictionary<char, string[]> Font5x3 = new()
    {
        ['A'] = new[] { " ▄▄▄ ", "█▀▀▀█", "█   █" },
        ['B'] = new[] { "█▀▀▄ ", "█▀▀▄ ", "█▄▄▀ " },
        ['C'] = new[] { " ▄▄▄▄", "█    ", " ▀▀▀▀" },
        ['D'] = new[] { "█▀▀▄ ", "█   █", "█▄▄▀ " },
        ['E'] = new[] { "█▀▀▀▀", "█▀▀  ", "█▄▄▄▄" },
        ['F'] = new[] { "█▀▀▀▀", "█▀▀  ", "█    " },
        ['G'] = new[] { " ▄▄▄▄", "█  ▄▄", " ▀▀▀█" },
        ['H'] = new[] { "█   █", "█▀▀▀█", "█   █" },
        ['I'] = new[] { "▀█▀", " █ ", "▄█▄" },
        ['J'] = new[] { "  ▄▄█", "   █ ", "▀▄▄▀ " },
        ['K'] = new[] { "█  ▄▀", "█▀▀▄ ", "█  ▀▄" },
        ['L'] = new[] { "█    ", "█    ", "█▄▄▄▄" },
        ['M'] = new[] { "█▄ ▄█", "█ ▀ █", "█   █" },
        ['N'] = new[] { "█▄  █", "█ ▀ █", "█  ▀█" },
        ['O'] = new[] { " ▄▄▄ ", "█   █", " ▀▀▀ " },
        ['P'] = new[] { "█▀▀▄ ", "█▄▄▀ ", "█    " },
        ['Q'] = new[] { " ▄▄▄ ", "█   █", " ▀▀▀█" },
        ['R'] = new[] { "█▀▀▄ ", "█▀▄  ", "█  ▀▄" },
        ['S'] = new[] { " ▄▄▄▄", " ▀▀▄▄", "▄▄▄▀ " },
        ['T'] = new[] { "▀█▀▀▀", " █   ", " █   " },
        ['U'] = new[] { "█   █", "█   █", " ▀▀▀ " },
        ['V'] = new[] { "█   █", "▀▄ ▄▀", "  ▀  " },
        ['W'] = new[] { "█   █", "█ ▄ █", "▀▄▀▄▀" },
        ['X'] = new[] { "▀▄ ▄▀", " ▀▀▀ ", "▄▀ ▀▄" },
        ['Y'] = new[] { "▀▄ ▄▀", " ▀▀  ", " █   " },
        ['Z'] = new[] { "▄▄▄▀▀", "  ▄▀ ", "▀▄▄▄▄" },
        ['0'] = new[] { " ▄▄▄ ", "█ ▄ █", " ▀▀▀ " },
        ['1'] = new[] { " ▄█ ", "  █ ", "  █ " },
        ['2'] = new[] { " ▄▄▄ ", "  ▄▄▀", "▄▄▄▄ " },
        ['3'] = new[] { "▄▄▄▄ ", "  ▀▄ ", "▄▄▄▀ " },
        ['4'] = new[] { "█  █ ", "▀▀▀█ ", "   █ " },
        ['5'] = new[] { "▄▄▄▄▄", "▀▀▀▄ ", "▄▄▄▀ " },
        ['6'] = new[] { " ▄▄▄ ", "█▀▀▄ ", " ▀▀▀ " },
        ['7'] = new[] { "▄▄▄▄▄", "  ▄▀ ", " ▀   " },
        ['8'] = new[] { " ▄▄▄ ", " ▄▄▄ ", " ▀▀▀ " },
        ['9'] = new[] { " ▄▄▄ ", " ▀▀▄█", " ▀▀▀ " },
        [' '] = new[] { "   ", "   ", "   " },
        ['!'] = new[] { " █ ", " █ ", " ▄ " },
        ['?'] = new[] { " ▄▄ ", "  ▀▀", "  ▄ " },
        ['.'] = new[] { "  ", "  ", " ▄" },
        [','] = new[] { "  ", "  ", " ▄" },
        [':'] = new[] { " ▄", "  ", " ▄" },
        ['-'] = new[] { "   ", "▄▄▄", "   " },
        ['/'] = new[] { "  ▄▀", " ▄▀ ", "▄▀  " },
    };

    private static readonly Dictionary<char, string[]> TinyFont = new()
    {
        ['A'] = new[] { "A", "A", "A" }, ['B'] = new[] { "B", "B", "B" }, ['C'] = new[] { "C", "C", "C" },
        ['D'] = new[] { "D", "D", "D" }, ['E'] = new[] { "E", "E", "E" }, ['F'] = new[] { "F", "F", "F" },
        ['G'] = new[] { "G", "G", "G" }, ['H'] = new[] { "H", "H", "H" }, ['I'] = new[] { "I", "I", "I" },
        ['J'] = new[] { "J", "J", "J" }, ['K'] = new[] { "K", "K", "K" }, ['L'] = new[] { "L", "L", "L" },
        ['M'] = new[] { "M", "M", "M" }, ['N'] = new[] { "N", "N", "N" }, ['O'] = new[] { "O", "O", "O" },
        ['P'] = new[] { "P", "P", "P" }, ['Q'] = new[] { "Q", "Q", "Q" }, ['R'] = new[] { "R", "R", "R" },
        ['S'] = new[] { "S", "S", "S" }, ['T'] = new[] { "T", "T", "T" }, ['U'] = new[] { "U", "U", "U" },
        ['V'] = new[] { "V", "V", "V" }, ['W'] = new[] { "W", "W", "W" }, ['X'] = new[] { "X", "X", "X" },
        ['Y'] = new[] { "Y", "Y", "Y" }, ['Z'] = new[] { "Z", "Z", "Z" }, [' '] = new[] { " ", " ", " " },
        ['0'] = new[] { "0", "0", "0" }, ['1'] = new[] { "1", "1", "1" }, ['2'] = new[] { "2", "2", "2" },
        ['3'] = new[] { "3", "3", "3" }, ['4'] = new[] { "4", "4", "4" }, ['5'] = new[] { "5", "5", "5" },
        ['6'] = new[] { "6", "6", "6" }, ['7'] = new[] { "7", "7", "7" }, ['8'] = new[] { "8", "8", "8" },
        ['9'] = new[] { "9", "9", "9" }
    };

    private static readonly Dictionary<char, string[]> ShadeFont = BuildTransformedFont('░');
    private static readonly Dictionary<char, string[]> SlickFont = BuildTransformedFont('╱');
    private const int GlyphSpacing = 1;

    public ASCIIFontRenderable(CliRenderer? renderer) : base(renderer) { }

    public bool ContainsPoint(int x, int y)
        => x >= ScreenX && x < ScreenX + ComputedWidth && y >= ScreenY && y < ScreenY + ComputedHeight;

    public int ColumnToTextIndex(int x)
    {
        int relative = Math.Max(0, x - ScreenX);
        var font = GetFont();
        const int spacing = GlyphSpacing;
        int col = 0;
        for (int i = 0; i < Text.Length; i++)
        {
            char c = char.ToUpperInvariant(Text[i]);
            if (!font.TryGetValue(c, out var pattern))
                pattern = font.TryGetValue(' ', out var space) ? space : Font5x3[' '];
            int charWidth = pattern[0].Length + spacing;
            if (relative < col + charWidth)
                return i;
            col += charWidth;
        }
        return Text.Length;
    }

    public void SetSelection(int start, int end)
    {
        _selectionStart = Math.Clamp(Math.Min(start, end), 0, Text.Length);
        _selectionEnd = Math.Clamp(Math.Max(start, end), 0, Text.Length);
        RequestRender();
    }

    public void ClearSelection()
    {
        _selectionStart = null;
        _selectionEnd = null;
        RequestRender();
    }

    public bool HasSelection() => _selectionStart.HasValue && _selectionEnd.HasValue && _selectionEnd > _selectionStart;

    public string GetSelectedText()
    {
        if (!HasSelection()) return "";
        return Text[_selectionStart!.Value.._selectionEnd!.Value];
    }

    public static IReadOnlyList<string> SupportedFonts { get; } = new[] { "tiny", "block", "shade", "slick" };

    protected override void RenderSelf(RenderBuffer buffer, double deltaTime)
    {
        int x = ScreenX, y = ScreenY, w = ComputedWidth, h = ComputedHeight;
        if (w <= 0 || h <= 0) return;

        var fg = Color != null ? Rgba.FromCss(Color) : Rgba.FromInts(255, 255, 255);
        var bg = BackgroundColor != null ? Rgba.FromCss(BackgroundColor) : Rgba.FromInts(0, 0, 0, 0);
        if (BackgroundColor != null)
            buffer.FillRect(x, y, w, h, bg);

        var font = GetFont();
        int spacing = GlyphSpacing;
        int col = x;
        for (int i = 0; i < Text.Length; i++)
        {
            char c = char.ToUpperInvariant(Text[i]);
            if (!font.TryGetValue(c, out var pattern))
                pattern = font.TryGetValue(' ', out var space) ? space : Font5x3[' '];

            int charWidth = pattern[0].Length + spacing;
            if (col + charWidth > x + w) break;

            bool selected = _selectionStart.HasValue && _selectionEnd.HasValue && i >= _selectionStart.Value && i < _selectionEnd.Value;
            var charFg = selected ? Rgba.FromCss(SelectionFg) : fg;
            var charBg = selected ? Rgba.FromCss(SelectionBg) : bg;

            for (int row = 0; row < 3 && row < h; row++)
            {
                var rowStr = row < pattern.Length ? pattern[row] : "";
                buffer.DrawText(col, y + row, rowStr, charFg, charBg);
            }

            col += charWidth;
        }
    }

    private Dictionary<char, string[]> GetFont()
    {
        return Font.ToLowerInvariant() switch
        {
            "tiny" => TinyFont,
            "shade" => ShadeFont,
            "slick" => SlickFont,
            _ => Font5x3
        };
    }

    private static Dictionary<char, string[]> BuildTransformedFont(char fill)
    {
        var result = new Dictionary<char, string[]>(Font5x3.Count);
        foreach (var (ch, rows) in Font5x3)
        {
            result[ch] = rows.Select(row => new string(row.Select(c => c == ' ' ? ' ' : fill).ToArray())).ToArray();
        }
        return result;
    }
}
