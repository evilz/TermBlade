import { Terminal } from "@xterm/xterm";
import { FitAddon } from "@xterm/addon-fit";
import { WebLinksAddon } from "@xterm/addon-web-links";

let terminal: Terminal | null = null;
let fitAddon: FitAddon | null = null;

/**
 * Initialize the xterm.js terminal inside the given HTML element.
 * Keyboard input is forwarded to .NET via dotnetRef.invokeMethodAsync("OnTerminalData", data).
 */
export function initTerminal(
  elementId: string,
  dotnetRef: { invokeMethodAsync: (method: string, ...args: unknown[]) => Promise<void> }
): void {
  const container = document.getElementById(elementId);
  if (!container) {
    console.error(`[TermBlade] Element '${elementId}' not found.`);
    return;
  }

  terminal = new Terminal({
    cursorBlink: true,
    fontFamily: "'Cascadia Code', 'Fira Code', 'JetBrains Mono', 'Consolas', monospace",
    fontSize: 14,
    lineHeight: 1.2,
    scrollback: 1000,
    convertEol: true,
    theme: {
      background: "#1a1b26",
      foreground: "#c0caf5",
      cursor: "#c0caf5",
      cursorAccent: "#1a1b26",
      selectionBackground: "#33467c",
      black: "#15161e",
      red: "#f7768e",
      green: "#9ece6a",
      yellow: "#e0af68",
      blue: "#7aa2f7",
      magenta: "#bb9af7",
      cyan: "#7dcfff",
      white: "#a9b1d6",
      brightBlack: "#414868",
      brightRed: "#f7768e",
      brightGreen: "#9ece6a",
      brightYellow: "#e0af68",
      brightBlue: "#7aa2f7",
      brightMagenta: "#bb9af7",
      brightCyan: "#7dcfff",
      brightWhite: "#c0caf5",
    },
  });

  fitAddon = new FitAddon();
  terminal.loadAddon(fitAddon);
  terminal.loadAddon(new WebLinksAddon());

  terminal.open(container);
  fitAddon.fit();

  // Forward keyboard input to .NET
  terminal.onData((data: string) => {
    dotnetRef.invokeMethodAsync("OnTerminalData", data);
  });

  // Handle window resize
  const resizeObserver = new ResizeObserver(() => {
    fitAddon?.fit();
  });
  resizeObserver.observe(container);
}

/**
 * Write raw text (including ANSI sequences) to the terminal.
 */
export function write(text: string): void {
  terminal?.write(text);
}

/**
 * Write a line of text to the terminal.
 */
export function writeln(text: string): void {
  terminal?.writeln(text);
}

/**
 * Clear the terminal screen.
 */
export function clear(): void {
  terminal?.clear();
}

/**
 * Focus the terminal.
 */
export function focus(): void {
  terminal?.focus();
}

/**
 * Fit the terminal to its container.
 */
export function fit(): void {
  fitAddon?.fit();
}
