# TermBlade architecture

This reference-and-explanation document helps contributors understand the rendering pipeline.

## Data flow

```text
Application state -> Renderable tree -> Layout -> RenderBuffer/CellBuffer -> ANSI diff -> terminal
```

Layout determines positions and sizes without writing escape sequences. Renderables write cells into a frame buffer. The renderer compares the new frame with the previous frame and emits only changed cells, reducing terminal traffic and flicker.

## Core concepts

### Cells and colors

A cell contains a Unicode code point, foreground and background `Rgba` values, and `TextAttributes`. `Rgba` preserves color intent: true color, ANSI-256 indexed color, or terminal default. Blending and comparison must preserve that intent rather than treating every color as RGB.

### Text

`TextBuffer` stores styled display text. `EditBuffer` provides cursor movement and undo/redo over a rope. Text operations are separate from terminal rendering so editors can be tested without a real console.

### Renderer lifecycle

`CliRenderer` owns terminal interaction, input threads, resize handling, and the render loop. `Renderer` is the smaller diff engine used when the caller controls the loop. Both must restore cursor, alternate-screen, mouse, and raw-mode state during disposal.

### Razor hosting

`TermBlade.Razor` adapts Razor components to the renderable tree. Components update state through Blazor lifecycle methods and render into TermBlade buffers; they must not synchronously block the WebAssembly dispatcher.

## Extension guidance

Add a core renderable when behavior is useful without Razor. Add a Razor wrapper when markup parameters improve composition. Add a sample for user-visible behavior and a focused test for every public behavior. Keep type registration explicit in the gallery and documentation preview system so trimming and inventory tests remain reliable.
