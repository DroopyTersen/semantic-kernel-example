import { Hono } from "hono";

const app = new Hono();

const config = {
  apiUrl: process.env.API_URL ?? "http://localhost:5229",
  chatRoute: process.env.CHAT_ROUTE ?? "/chat/submit",
  apiKey: process.env.API_KEY ?? "my-api-key",
  apiKeyHeader: process.env.API_KEY_HEADER ?? "X-API-KEY",
};

app.post("api/chat-proxy", async (c) => {
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

app.get("/", (c) => {
  return c.html(
    <html>
      <head>
        <script type="module" src="/src/client.ts"></script>
      </head>
      <body>
        <h1 class="text-3xl font-bold">Hello</h1>
      </body>
    </html>
  );
});
export default app;
