import { Terminal } from "@xterm/xterm";
import { FitAddon } from "@xterm/addon-fit";
import { WebLinksAddon } from "@xterm/addon-web-links";

let terminal: Terminal | null = null;
let fitAddon: FitAddon | null = null;
let resizeObserver: ResizeObserver | null = null;

type DotNetTerminalRef = {
  invokeMethodAsync: (method: string, ...args: unknown[]) => Promise<void>;
};

function getTerminalFontFamily(): string {
  return getComputedStyle(document.documentElement).getPropertyValue("--font-terminal").trim() || "monospace";
}

function createTerminal(readOnly: boolean): Terminal {
  return new Terminal({
    cursorBlink: !readOnly,
    cursorStyle: readOnly ? "block" : "block",
    disableStdin: readOnly,
    fontFamily: getTerminalFontFamily(),
    fontSize: readOnly ? 13 : 14,
    lineHeight: 1.2,
    scrollback: readOnly ? 0 : 1000,
    convertEol: true,
    theme: {
      background: "#1a1b26",
      foreground: "#c0caf5",
      cursor: readOnly ? "#1a1b26" : "#c0caf5",
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
}

function openTerminal(elementId: string, readOnly: boolean): void {
  dispose();

  const container = document.getElementById(elementId);
  if (!container) {
    console.error(`[TermBlade] Element '${elementId}' not found.`);
    return;
  }

  terminal = createTerminal(readOnly);
  fitAddon = new FitAddon();
  terminal.loadAddon(fitAddon);

  if (!readOnly) {
    terminal.loadAddon(new WebLinksAddon());
  }

  terminal.open(container);
  fitAddon.fit();

  resizeObserver = new ResizeObserver(() => {
    fitAddon?.fit();
  });
  resizeObserver.observe(container);
}

/**
 * Initialize the xterm.js terminal inside the given HTML element.
 * Keyboard input is forwarded to .NET via dotnetRef.invokeMethodAsync("OnTerminalData", data).
 */
export function initTerminal(elementId: string, dotnetRef: DotNetTerminalRef): void {
  openTerminal(elementId, false);

  terminal?.onData((data: string) => {
    dotnetRef.invokeMethodAsync("OnTerminalData", data);
  });
}

/**
 * Initialize a read-only xterm.js terminal for embedded component previews.
 */
export function initReadOnlyTerminal(elementId: string): void {
  openTerminal(elementId, true);
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

/**
 * Dispose the current xterm.js instance.
 */
export function dispose(): void {
  resizeObserver?.disconnect();
  resizeObserver = null;
  terminal?.dispose();
  terminal = null;
  fitAddon = null;
}
