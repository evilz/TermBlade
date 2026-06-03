import { defineConfig } from "vite";
import { resolve } from "path";

export default defineConfig({
  build: {
    lib: {
      entry: resolve(__dirname, "wwwroot/terminal/terminal.ts"),
      formats: ["es"],
      fileName: () => "terminal.js",
    },
    outDir: "wwwroot/dist",
    emptyOutDir: true,
    rollupOptions: {
      output: {
        assetFileNames: "[name][extname]",
      },
    },
  },
});
