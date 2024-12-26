import { Hono } from "hono";
import fs from "node:fs";
import https from "node:https";
const app = new Hono();

const config = {
  apiUrl: import.meta.env.VITE_API_URL ?? "http://localhost:5229",
  chatRoute: import.meta.env.VITE_CHAT_ROUTE ?? "/chat/submit",
  apiKey: import.meta.env.VITE_API_KEY ?? "my-api-key",
  apiKeyHeader: import.meta.env.VITE_API_KEY_HEADER ?? "X-API-KEY",
  isDevelopment: import.meta.env.NODE_ENV !== "production",
};

console.log("🚀 | USING CONFIG: ", config);

// Serve static files from the root directory

app.get("/", (c) => {
  return c.html(fs.readFileSync("src/index.html", "utf8"));
});

app.post("/api/chat-proxy", async (c) => {
  const newRequest = new Request(`${config.apiUrl}${config.chatRoute}`, {
    method: c.req.method,
    headers: new Headers({
      ...c.req.header(),
      [config.apiKeyHeader]: config.apiKey,
    }),
    body: c.req.raw.body,
    // @ts-ignore
    duplex: "half",
  });

  const fetchOptions = config.isDevelopment
    ? {
        agent: new https.Agent({
          rejectUnauthorized: false,
        }),
      }
    : undefined;

  let response = await fetch(newRequest, fetchOptions as any);
  if (!response.ok) {
    return Response.json(
      { error: "Failed to call Proxied Chat API", body: await response.text() },
      { status: response.status }
    );
  }
  return Response.json(await response.json());
});

export default app;
