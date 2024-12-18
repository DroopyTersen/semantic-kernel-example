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
      <body class="bg-gray-50">
        <div class="flex flex-col h-screen max-w-2xl mx-auto p-4">
          <div id="messages" class="flex-1 overflow-y-auto mb-4"></div>
          <form id="chatForm" class="flex items-center">
            <input
              type="text"
              id="messageInput"
              placeholder="Type your message..."
              class="flex-1 p-2 border border-gray-300 rounded-l-lg focus:outline-none focus:ring-2 focus:ring-blue-500"
            />
            <button
              type="submit"
              class="bg-blue-500 text-white p-2 rounded-r-lg hover:bg-blue-600 focus:outline-none focus:ring-2 focus:ring-blue-500"
            >
              Send
            </button>
          </form>
        </div>
      </body>
    </html>
  );
});
export default app;
