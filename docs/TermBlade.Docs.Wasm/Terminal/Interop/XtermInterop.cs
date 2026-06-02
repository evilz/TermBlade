using Microsoft.JSInterop;

namespace TermBlade.Docs.Wasm.Terminal.Interop;

/// <summary>
/// Blazor interop service for xterm.js.
/// Wraps the JavaScript module to provide a clean C# API.
/// </summary>
public sealed class XtermInterop : ITerminalOutput, IAsyncDisposable
{
    private readonly IJSRuntime _js;
    private IJSObjectReference? _module;

    public XtermInterop(IJSRuntime js)
    {
        _js = js;
    }

    /// <summary>
    /// Initialize the xterm.js terminal in the given HTML element.
    /// </summary>
    public async Task InitAsync(string elementId, DotNetObjectReference<object> dotnetRef)
    {
        _module = await _js.InvokeAsync<IJSObjectReference>("import", "./dist/terminal.js");
        await _module.InvokeVoidAsync("initTerminal", elementId, dotnetRef);
    }

    public async Task WriteAsync(string text)
    {
        if (_module is not null)
            await _module.InvokeVoidAsync("write", text);
    }

    public async Task WriteLineAsync(string text)
    {
        if (_module is not null)
            await _module.InvokeVoidAsync("writeln", text);
    }

    public async Task ClearAsync()
    {
        if (_module is not null)
            await _module.InvokeVoidAsync("clear");
    }

    public async Task FocusAsync()
    {
        if (_module is not null)
            await _module.InvokeVoidAsync("focus");
    }

    public async Task FitAsync()
    {
        if (_module is not null)
            await _module.InvokeVoidAsync("fit");
    }

    public async ValueTask DisposeAsync()
    {
        if (_module is not null)
        {
            await _module.DisposeAsync();
            _module = null;
        }
    }
}
