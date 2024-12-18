import { Hono } from "hono";
import fs from "node:fs";
const app = new Hono();

const config = {
  apiUrl: process.env.API_URL ?? "http://localhost:5229",
  chatRoute: process.env.CHAT_ROUTE ?? "/chat/submit",
  apiKey: process.env.API_KEY ?? "my-api-key",
  apiKeyHeader: process.env.API_KEY_HEADER ?? "X-API-KEY",
};

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

  return fetch(newRequest);
});

export default app;
