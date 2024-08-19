using AutoMapper;
using ChatService.API.Models.Conversation;
using ChatService.Services.Abstractions;
using ChatService.Services.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace ChatService.API.Controllers;

[ApiController]
[Route("[controller]")]
public class ConversationController : ControllerBase
{
    public ConversationController(IConversationService service, IMapper mapper)
    {
        _service = service;
        _mapper = mapper;
    }

    private readonly IConversationService _service;
    private readonly IMapper _mapper;

    [HttpGet("{taskId}")]
    public async Task<IActionResult> GetAsync(int taskId)
    {
        return Ok(_mapper.Map<List<ConversationModel>>(await _service.GetAsync(taskId)));
    }
    
    [HttpPost]
    public async Task<IActionResult> CreateAsync(CreatingConversationModel ConversationModel)
    {
        return Ok(await _service.CreateAsync(_mapper.Map<CreatingConversationDto>(ConversationModel)));
    }
        
    [HttpPut("{id}")]
    public async Task<IActionResult> EditAsync(int id, UpdatingConversationModel ConversationModel)
    {
        await _service.UpdateAsync(id, _mapper.Map<UpdatingConversationModel, UpdatingConversationDto>(ConversationModel));
        return Ok();
    }
        
    [HttpPost("Cancel/{id}")]
    public async Task<IActionResult> CancelAsync(int id)
    {
        await _service.CancelAsync(id);
        return Ok();
    }
}