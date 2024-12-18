import { createChatClient } from "./ChatClient.js";
import { templates } from "./templates.js";
import "./chat.css";

async function init() {
  const chatClient = createChatClient({
    chatEndpoint: "/api/chat-proxy",
  });
  chatClient.clearMessages();

  const messagesContainer = document.getElementById("messages")!;
  const chatForm = document.getElementById("chatForm") as HTMLFormElement;
  const messageInput = document.getElementById(
    "messageInput"
  ) as HTMLInputElement;
  let isLoading = false;

  function scrollToBottom() {
    messagesContainer.scrollTop = messagesContainer.scrollHeight;
  }

  function renderMessages(
    messages: Array<{ role: "user" | "assistant"; content: string }>
  ) {
    messagesContainer.innerHTML = messages
      .map((msg) => templates.messageContainer(msg))
      .join("");
    scrollToBottom();
  }

  function setLoading(loading: boolean) {
    isLoading = loading;
    if (loading) {
      messagesContainer.insertAdjacentHTML(
        "beforeend",
        templates.loadingIndicator()
      );
    }
    scrollToBottom();
  }

  chatForm.addEventListener("submit", async (e) => {
    e.preventDefault();
    const message = messageInput.value.trim();
    if (!message || isLoading) return;

    messageInput.value = "";
    setLoading(true);

    try {
      await chatClient.submitMessage(message);
    } finally {
      setLoading(false);
    }
  });

  chatClient.subscribe((messages) => {
    renderMessages(messages);
  });

  // Load initial messages
  const messages = await chatClient.getMessages();
  renderMessages(messages);
}

init();
