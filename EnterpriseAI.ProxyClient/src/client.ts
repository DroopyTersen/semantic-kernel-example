import { createChatClient } from "./ChatClient.js";
import "./chat.css";
async function init() {
  let chatClient = createChatClient({
    chatEndpoint: "/api/chat-proxy",
  });

  let messages = await chatClient.getMessages();

  console.log(messages);

  chatClient.subscribe((messages) => {
    console.log("NEW MESSAGES", messages);
  });
}

init();
