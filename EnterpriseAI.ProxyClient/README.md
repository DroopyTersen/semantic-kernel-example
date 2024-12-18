# Enterprise AI Proxy Client

A lightweight TypeScript client application demonstrating Enterprise AI API integration with minimal dependencies.

**Demonstrates**:

- Secure API key handling via proxy server, so no client-side API key exposure
- Browser based conversation persistence via `sessionStorage` api.
- Markdown rendering support

![Demo GIF](../docs/assets/chat-demo.gif)

## Tech Stack

- TypeScript with no UI Framework
- Tailwind CSS
- Node.js server for a proxy'ing Chat request

## How it works

- `server.ts`: Minimal web server to serve demo app and proxy chat requests.
- `index.html`: Application shell
- `ChatClient.ts`: Core chat API logic and state management
- `chatUI.ts`: DOM manipulation and UI updates
- `markdownConverter.ts`: Message formatting

```mermaid
sequenceDiagram
    participant User
    participant WebApp as App
    participant ProxyServer as API Proxy
    participant APIServer as API Server
    participant AzureSearch as Azure AI Search
    participant AzureOpenAI as Azure OpenAI

    User->>WebApp: Submit message
    Note over WebApp: ChatClient.ts calls API
    WebApp->>ProxyServer: POST /api/chat-proxy
    Note over ProxyServer: server.ts adds X-API-Key header
    ProxyServer->>APIServer: POST /chat/submit
    Note over APIServer: ApiKeyAuthMiddleware.cs validates
    Note over APIServer: ChatController.cs handles request

    APIServer->>AzureSearch: search_relevant_context
    Note over AzureSearch: Via AzureAISearchPlugin.cs
    AzureSearch-->>APIServer: Return context

    APIServer->>AzureOpenAI: GetChatMessageContentAsync
    Note over AzureOpenAI: Uses system prompt + context
    AzureOpenAI-->>APIServer: Return completion

    APIServer-->>ProxyServer: Return JSON response
    ProxyServer-->>WebApp: Forward response
    Note over WebApp: ChatClient.ts persists conversation in sessionStorage
    WebApp-->>User: Display response
    Note over WebApp: chatUI.ts updates DOM
```

## Developer Setup

1. Install NPM dependencies
2. Update any configuration for API url or API key in `./src/server.ts`
3. Start the real API server (the dotnet one)
4. Start the proxy client demo

```bash
cd EnterpriseAI.ProxyClient
npm install
npm run dev
```

The following is the default configuration:

```ts
const config = {
  apiUrl: process.env.API_URL ?? "http://localhost:5229",
  chatRoute: process.env.CHAT_ROUTE ?? "/chat/submit",
  apiKey: process.env.API_KEY ?? "my-api-key",
  apiKeyHeader: process.env.API_KEY_HEADER ?? "X-API-KEY",
};
```
