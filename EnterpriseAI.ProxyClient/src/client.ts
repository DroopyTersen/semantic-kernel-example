import { createChatClient } from "./ChatClient.js";
import { templates } from "./templates.js";
import "./chat.css";
import { convertMarkdownToHtml } from "./convertMarkdownToHtml.js";

async function init() {
  const chatClient = createChatClient({
    chatEndpoint: "/api/chat-proxy",
  });
  // chatClient.clearMessages();

  const messagesContainer = document.getElementById("messages")!;
  const loadingIndicator = document.getElementById("loadingIndicator")!;
  const clearButton = document.getElementById("clearButton")!;
  const chatForm = document.getElementById("chatForm") as HTMLFormElement;
  const messageInput = document.getElementById(
    "messageInput"
  ) as HTMLInputElement;
  let isLoading = false;

  function scrollToBottom() {
    let scrollContainer = document.getElementById("scrollContainer");
    if (scrollContainer) {
      scrollContainer.scrollTop = scrollContainer.scrollHeight;
    }
  }

  function renderMessages(
    messages: Array<{ role: "user" | "assistant"; content: string }>
  ) {
    messagesContainer.innerHTML = messages
      .map((msg) =>
        templates.messageContainer({
          ...msg,
          content: convertMarkdownToHtml(msg.content),
        })
      )
      .join("");
    scrollToBottom();
  }

  function setLoading(loading: boolean) {
    isLoading = loading;
    if (loading) {
      loadingIndicator.classList.add("loading");
      messageInput.disabled = true;
    } else {
      loadingIndicator.classList.remove("loading");
      messageInput.disabled = false;
    }
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

  clearButton.addEventListener("click", () => {
    chatClient.clearMessages();
  });

  chatClient.subscribe((messages) => {
    renderMessages(messages);
  });

  // Load initial messages
  const messages = await chatClient.getMessages();
  renderMessages(messages);
  setLoading(false);
}

init();
