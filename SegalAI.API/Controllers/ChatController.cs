using Microsoft.AspNetCore.Mvc;
using SegalAI.Core.Models;
using SegalAI.Core.Repositories;
using SegalAI.Core.Services;
using System.Text.Json.Serialization;

namespace SegalAI.API.Controllers;

[ApiController]
[Route("[controller]")]
public class ChatController : ControllerBase
{
  private readonly KernelService _kernelService;
  private readonly IChatRepository _chatRepository;
  private readonly Dictionary<string, RagService> _conversations = new();

  public ChatController(KernelService kernelService, IChatRepository chatRepository)
  {
    _kernelService = kernelService;
    _chatRepository = chatRepository;
  }

  [HttpGet("{conversationId}")]
  [ProducesResponseType(typeof(IEnumerable<ChatMessage>), StatusCodes.Status200OK)]
  [ProducesResponseType(StatusCodes.Status404NotFound)]
  public ActionResult<IEnumerable<ChatMessage>> GetConversation(string conversationId)
  {
    var conversation = GetOrCreateConversation(conversationId);
    var messages = conversation.GetAllMessages();
    return Ok(messages);
  }

  [HttpPost("{conversationId}/submit")]
  [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
  [ProducesResponseType(StatusCodes.Status400BadRequest)]
  public async Task<IActionResult> SubmitMessage(
      string conversationId,
      [FromBody] SubmitMessageRequest request)
  {
    if (string.IsNullOrWhiteSpace(request.Message))
    {
      return BadRequest("Message cannot be empty");
    }

    var conversation = GetOrCreateConversation(conversationId);
    var response = await conversation.SubmitMessage(request.Message);

    return Ok(response);
  }

  private RagService GetOrCreateConversation(string conversationId)
  {
    return new RagService(conversationId, _kernelService.Kernel, _chatRepository);
  }
}

public record SubmitMessageRequest(
    [property: JsonPropertyName("message")]
    string Message
);