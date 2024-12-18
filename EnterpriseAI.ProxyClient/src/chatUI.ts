import { createChatClient } from "./ChatClient.js";
import "./chat.css";
import { convertMarkdownToHtml } from "./convertMarkdownToHtml.js";

const chatClient = createChatClient({
  chatEndpoint: "/api/chat-proxy",
});

const ui = {
  elements: {
    messagesContainer: document.getElementById("messages")!,
    loadingIndicator: document.getElementById("loadingIndicator")!,
    clearButton: document.getElementById("clearButton")! as HTMLButtonElement,
    chatForm: document.getElementById("chatForm")! as HTMLFormElement,
    messageInput: document.getElementById("messageInput")! as HTMLInputElement,
    scrollContainer: document.getElementById("scrollContainer")!,
  },
  isLoading: false,
  setLoading: (loading: boolean) => {
    ui.isLoading = loading;
    if (loading) {
      ui.elements.loadingIndicator.classList.add("loading");
      ui.elements.messageInput.disabled = true;
    } else {
      ui.elements.loadingIndicator.classList.remove("loading");
      ui.elements.messageInput.disabled = false;
    }
  },
  bindEvents: () => {
    chatClient.subscribe((messages) => {
      ui.renderMessages(messages);
    });

    ui.elements.chatForm.addEventListener("submit", async (e) => {
      e.preventDefault();
      const message = ui.elements.messageInput.value.trim();
      if (!message || ui.isLoading) return;

      ui.elements.messageInput.value = "";
      ui.setLoading(true);

      try {
        await chatClient.submitMessage(message);
      } catch (error) {
        console.error("Error submitting message:", error);
        alert("Failed to send message. Please try again.");
      } finally {
        ui.setLoading(false);
        ui.elements.messageInput.focus();
      }
    });

    ui.elements.clearButton.addEventListener("click", () => {
      chatClient.clearMessages();
    });
  },
  scrollToBottom: () => {
    if (ui.elements.scrollContainer) {
      ui.elements.scrollContainer.scrollTop =
        ui.elements.scrollContainer.scrollHeight;
    }
  },
  renderMessages: (
    messages: { role: "user" | "assistant"; content: string }[]
  ) => {
    ui.elements.messagesContainer.innerHTML = messages
      .map(ui.messageToHtml)
      .join("");

    ui.scrollToBottom();
  },
  messageToHtml: (message: { role: "user" | "assistant"; content: string }) => {
    return `
      <div class="mb-4 ${message.role === "user" ? "text-right" : "text-left"}">
        <div class="mb-1 text-sm font-semibold ${
          message.role === "user" ? "text-blue-600" : "text-gray-600"
        }">
          ${message.role === "user" ? "User" : "AI"}
        </div>
        <div class="inline-block min-w-[60ch] py-2 px-4 prose rounded-lg ${
          message.role === "user"
            ? "bg-blue-100 text-blue-900"
            : "bg-gray-200 text-gray-900"
        }">
          ${convertMarkdownToHtml(message.content)}
        </div>
      </div>
    `;
  },

  init: () => {
    ui.bindEvents();
    ui.scrollToBottom();
  },
};

ui.init();
