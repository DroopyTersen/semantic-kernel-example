import { defineConfig } from "vite";
import devServer from "@hono/vite-dev-server";
import bunAdapter from "@hono/vite-dev-server/bun";
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
      entry: "src/server.tsx",
      adapter: bunAdapter,
      // exclude: [
      //   // We need to override this option since the default setting doesn't fit
      //   /.*\.tsx?($|\?)/,
      //   /.*\.(s?css|less)($|\?)/,
      //   /.*\.(svg|png)($|\?)/,
      //   /^\/@.+$/,
      //   /^\/favicon\.ico$/,
      //   /^\/(public|assets|static)\/.+/,
      //   /^\/node_modules\/.*/,
      // ],
      // injectClientScript: false, // This option is buggy, disable it and inject the code manually
    }),
  ],
});
