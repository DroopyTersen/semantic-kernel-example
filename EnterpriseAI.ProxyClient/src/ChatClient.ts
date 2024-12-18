type MessageRole = "user" | "assistant";

interface ChatMessage {
  role: MessageRole;
  content: string;
  timestamp: number;
}

export type SubmitChatRequestMessage = Omit<ChatMessage, "timestamp">;

interface ChatClientOptions {
  chatEndpoint: string;
  storageKey?: string; // optional, defaults to something like "chat_messages"
}

/**
 * Creates a chat client that can manage a single conversation.
 * It maintains a message history in sessionStorage and interacts with a backend endpoint.
 */
export function createChatClient(options: ChatClientOptions) {
  const { chatEndpoint, storageKey = "chat_messages" } = options;

  // Load messages from session storage
  let messages: ChatMessage[] = [];
  const stored = sessionStorage.getItem(storageKey);
  if (stored) {
    try {
      messages = JSON.parse(stored) as ChatMessage[];
    } catch {
      // If parsing fails, ignore and start fresh
      messages = [];
    }
  }

  let subscribers: Array<(messages: ChatMessage[]) => void> = [];

  function notifySubscribers() {
    // Always pass a copy so subscribers can’t mutate directly
    const snapshot = [...messages];
    subscribers.forEach((cb) => cb(snapshot));
  }

  function saveMessages() {
    sessionStorage.setItem(storageKey, JSON.stringify(messages));
  }

  async function submitMessage(content: string): Promise<void> {
    // 1. Add the user's message immediately
    const userMessage: ChatMessage = {
      role: "user",
      content,
      timestamp: Date.now(),
    };
    messages.push(userMessage);
    saveMessages();
    notifySubscribers();

    let assistantMessageContent: string;

    // 2. Call the API with the full conversation history
    try {
      const response = await fetch(chatEndpoint, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({
          messages: messages.map((m) => ({ role: m.role, content: m.content })),
        }),
      });

      if (!response.ok) {
        // Handle error scenarios (e.g., bad request, server error)
        // Could throw or just return silently.
        // For now, just return and do nothing more:
        return;
      }

      assistantMessageContent = await response.text();
    } catch (error) {
      // Handle network or parsing errors
      return;
    }

    // 3. Add the assistant's response
    const assistantMessage: ChatMessage = {
      role: "assistant",
      content: assistantMessageContent,
      timestamp: Date.now(),
    };
    messages.push(assistantMessage);
    saveMessages();
    notifySubscribers();
  }

  function getMessages(): ChatMessage[] {
    return [...messages]; // return a copy
  }

  function clearMessages(): void {
    messages = [];
    saveMessages();
    notifySubscribers();
  }

  function subscribe(callback: (messages: ChatMessage[]) => void): () => void {
    subscribers.push(callback);
    // immediately call with current messages
    callback([...messages]);

    // return an unsubscribe function
    return () => {
      subscribers = subscribers.filter((cb) => cb !== callback);
    };
  }

  return {
    submitMessage,
    getMessages,
    clearMessages,
    subscribe,
  };
}
