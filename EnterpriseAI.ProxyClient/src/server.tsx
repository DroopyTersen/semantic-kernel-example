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
        <div
          class="flex flex-col h-screen overflow-y-auto"
          id="scrollContainer"
        >
          <div class="max-w-3xl mx-auto w-full relative">
            <div id="messages" class="flex-1 mb-4 px-2 min-h-[40vh]"></div>
          </div>
          <form
            id="chatForm"
            class=" w-full py-2 flex items-center sticky bottom-0 max-w-3xl mx-auto"
          >
            <div
              id="loadingIndicator"
              class="loading-indicator absolute inset-0 bg-white/40 w-full h-full flex items-center justify-center z-20"
            >
              <span className="font-bold uppercase animate-bounce">
                Loading...
              </span>
            </div>
            <div>
              <button
                type="button"
                id="clearButton"
                class="bg-red-100 text-red-800 p-2 rounded-lg hover:bg-red-200 focus:outline-none focus:ring-2 focus:ring-red-500 mr-2"
              >
                Clear
              </button>
            </div>
            <input
              autofocus
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
