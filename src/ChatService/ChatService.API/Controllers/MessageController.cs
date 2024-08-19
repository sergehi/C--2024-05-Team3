using AutoMapper;
using ChatService.API.Models.Message;
using ChatService.Services.Abstractions;
using ChatService.Services.Contracts.Message;
using Microsoft.AspNetCore.Mvc;

namespace ChatService.API.Controllers;

[ApiController]
[Route("[controller]")]
public class MessageController : ControllerBase
{
    private readonly IMessageService _service;
    private readonly IMapper _mapper;
    
    public MessageController(IMessageService service, IMapper mapper)
    {
        _service = service;
        _mapper = mapper;
    }

    [HttpGet("{conversationId}")]
    public async Task<IActionResult> GetAsync(int conversationId)
    {
        return Ok(_mapper.Map<List<MessageModel>>(await _service.GetAsync(conversationId)));
    }
    
    [HttpPost]
    public async Task<IActionResult> CreateAsync(CreatingMessageModel conversationModel)
    {
        var dto = _mapper.Map<CreatingMessageDto>(conversationModel);
        dto.UserId = 0;
        return Ok(await _service.CreateAsync(dto));
    }
    
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAsync(int id)
    {
        await _service.DeleteAsync(id);
        return Ok();
    }
}