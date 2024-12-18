using System.Text.Json.Serialization;
using EnterpriseAI.Core.Models;
using EnterpriseAI.Core.Services;
using Microsoft.AspNetCore.Mvc;

namespace EnterpriseAI.API.Controllers;

[ApiController]
[Route("[controller]")]
public class ChatController : ControllerBase
{
    private readonly KernelService _kernelService;

    public ChatController(KernelService kernelService)
    {
        _kernelService = kernelService;
    }

    [HttpGet("{conversationId}")]
    [ProducesResponseType(typeof(IEnumerable<ChatMessage>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IEnumerable<ChatMessage>>> GetConversation(string conversationId)
    {
        var conversation = new ConversationService(conversationId, _kernelService.Kernel);
        var messages = await conversation.GetAllMessagesAsync();
        return Ok(messages);
    }

    [HttpPost("{conversationId}/submit")]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> SubmitMessage(
        string conversationId,
        [FromBody] SubmitMessageRequest request
    )
    {
        if (string.IsNullOrWhiteSpace(request.Message))
        {
            return BadRequest("Message cannot be empty");
        }

        var conversation = new ConversationService(conversationId, _kernelService.Kernel);
        var response = await conversation.SubmitMessageAsync(request.Message);

        return Ok(response);
    }

    [HttpPost("submit")]
    [ProducesResponseType(typeof(ChatResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> SubmitChatAsync([FromBody] SubmitChatRequest request)
    {
        if (request.Messages == null || !request.Messages.Any())
        {
            return BadRequest("Messages array cannot be empty");
        }

        var chatMessages = request
            .Messages.Select(m => new ChatMessage(Enum.Parse<MessageRole>(m.Role, true), m.Content))
            .ToList();

        var answerGenerator = new AnswerGenerator(_kernelService.Kernel);
        var response = await answerGenerator.AnswerQuestion(chatMessages);

        return Ok(new ChatResponse(response));
    }
}

public record SubmitMessageRequest([property: JsonPropertyName("message")] string Message);

public record SubmitChatRequest([property: JsonPropertyName("messages")] List<Message> Messages);

public record Message(
    [property: JsonPropertyName("role")] string Role,
    [property: JsonPropertyName("content")] string Content
);

public record ChatResponse([property: JsonPropertyName("content")] string Content);
