import { defineConfig } from "vite";
import devServer from "@hono/vite-dev-server";
import nodeAdapter from "@hono/vite-dev-server/node";
// https://vitejs.dev/config/
export default defineConfig({
  server: {
    port: 4444,
  },
  build: {
    outDir: "build",
  },
  plugins: [
    devServer({
      entry: "src/server.ts",
      adapter: nodeAdapter,
    }),
  ],
});
