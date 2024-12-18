interface TemplateMessage {
  role: "user" | "assistant";
  content: string;
}

export const templates = {
  messageContainer: (message: TemplateMessage) => `
    <div class="mb-4 ${message.role === "user" ? "text-right" : "text-left"}">
      <div class="mb-1 text-sm font-semibold ${
        message.role === "user" ? "text-blue-600" : "text-gray-600"
      }">
        ${message.role === "user" ? "User" : "AI"}
      </div>
      <div class="inline-block p-3 rounded-lg ${
        message.role === "user"
          ? "bg-blue-100 text-blue-900"
          : "bg-gray-200 text-gray-900"
      }">
        ${message.content}
      </div>
    </div>
  `,

  loadingIndicator: () => `
    <div class="text-left mb-4">
      <div class="mb-1 text-sm font-semibold text-gray-600">AI</div>
      <div class="inline-block p-3 rounded-lg bg-gray-200 text-gray-900">
        <div class="flex items-center">
          <div class="w-2 h-2 bg-gray-500 rounded-full mr-1 animate-bounce"></div>
          <div class="w-2 h-2 bg-gray-500 rounded-full mr-1 animate-bounce [animation-delay:200ms]"></div>
          <div class="w-2 h-2 bg-gray-500 rounded-full animate-bounce [animation-delay:400ms]"></div>
        </div>
      </div>
    </div>
  `,
};
