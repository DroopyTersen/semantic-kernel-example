using Microsoft.SemanticKernel.ChatCompletion;
using SegalAI.Core.Models;

public class AnswerGenerator
{
  private readonly IChatCompletionService _chatService;

  public AnswerGenerator(IChatCompletionService chatService)
  {
    _chatService = chatService;
  }

  private static string GetSystemPrompt() => $@"
You will be provided a list of potentially relevant Data Source pages. Some pages will also include an image of the page if that page contains visual elements like images, charts, or tables.

Use this information as context to answer the question. If provided, make sure to account for visual information represented on the image of the page.

<rules>
- You can only use relevant pages provided to you. Do not make anything up.
- If you don't know the answer, say ""I don't know"" or ""The answer is not in the provided data sources"".
- Provide clear direct concise answers
</rules>

[Today's Date: {DateTimeOffset.UtcNow:yyyy-MM-dd}]

Before answering:
1. Think through your reasoning process step by step
2. Review your thinking critically for assumptions or unsupported claims
3. Provide a complete response with proper citations and evidence


## Respond with Your Thinking Process!

Before answering thinkg through how you will answer the question. Respond with a detailed step by step reasoning process, and show you work using evidence from the provided data sources.

1. First outline how you will answer the question in a <thinking> tag.
  - Use <thinking> tags to enclose your thinking process.
  - Inside of <thinking> tags, step through how you will answer the question, and explain your thinking process
  - Inside of <thinking> tags, make sure to reference specific evidence from the data sources to support your answer.
2. Use a <review> tag to critically analyze your thinking process.
    - IMPORTANT!!! Inside of <review> tags you should think VERY CRITICALLY about your thinking process. Are there any claims you are making that you cannot definitively prove from the data sources? Are there any assumptions you are making? Any logical fallacies? 
  - Look very carefully for ANYTHING that might be incorrect.
  - List any claims you are making in the outlined answer and ensure you have direct evidence from the data sources to support each claim.
3. Finally generate the final response according to the provided instructions. Make sure to incorporate any feedback from the review step. The previous <thinking> and <review> tags will be removed from your response, so please provide a full and complete response here, using formatted text that follows other instructions you have been provided on how to format your response.

Here is a template of you you should respond:
<thinking>
[Explain your step by step thinking process for answering the question]
</thinking>

<review>
[Critically analyze your thinking process. List any claims you are making in the outlined answer and ensure you have direct evidence from the data sources to support each claim]
</review>

<final_output>
[Final (AND COMPLETE!)response goes here, accounting for any feedback you received in the review step. Provide a full and complete response here. It should include both the answer and how you came to the answer and you should use formatted text that follows other instructions you have been provided on how to format your response. This might involve restating some things from the <thinking> step but that is okay because it helps show the user a complete answer, and how you came to your conclusions.]
<final_output>
";

  public async Task<string> AnswerQuestion(string relevantContext, IEnumerable<ChatMessage> conversation)
  {
    var messages = new ChatHistory(); // Create fresh history
    messages.AddSystemMessage(GetSystemPrompt());

    // Add existing conversation messages
    foreach (var message in conversation)
    {
      messages.Add(ChatMessage.ToKernelMessage(message));
    }

    if (!string.IsNullOrEmpty(relevantContext))
    {
      messages.AddUserMessage($"Relevant context:\n\n{relevantContext}\n\nPlease use this context to help answer the user's question.");
    }

    var result = await _chatService.GetChatMessageContentAsync(messages);
    // TODO: Once we are happy with the answers, regex to only return inner <final_output>
    return result?.Content ?? string.Empty;
  }
}
