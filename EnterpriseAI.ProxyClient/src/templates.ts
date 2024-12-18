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
      <div class="inline-block min-w-[60ch] py-2 px-4 rounded-lg ${
        message.role === "user"
          ? "bg-blue-100 text-blue-900"
          : "bg-gray-200 text-gray-900"
      }">
        <div class="prose">
          ${message.content}
        </div>
      </div>
    </div>
  `,
};
