using AutoMapper;
using ChatProto;
using ChatService.Services.Abstractions;
using ChatService.Services.Contracts;
using ChatService.Services.Contracts.Message;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.EntityFrameworkCore;

namespace ChatService.Services;

public class MessageService: MessageProto.MessageProtoBase
{
    private readonly IMapper _mapper;
    private readonly IMessageService _service;
    private readonly ILogger<MessageService> _logger;

    public MessageService(ILogger<MessageService> logger, IMapper mapper, IMessageService service)
    {
        _logger = logger;
        _mapper = mapper;
        _service = service;
    }
    
    public async override Task<GetMessagesResponse> GetMessages(GetMessagesRequest request, ServerCallContext context)
    {
        // Returning all mock conversations as if they belong to the same task ID
        var response = new GetMessagesResponse();
        var getList = await _service.GetAsync(request.ConversationId);
        foreach (var item in getList)
        {
            MessageItem message = new();
            message.TypeId = item.TypeId;
            message.Text = item.Text;
            message.UserId = item.UserId;
            response.Messages.Add(message);
        }
        return response;
    }

    public override Task<CreateMessageResponse> CreateMessage(CreateMessageRequest request, ServerCallContext context)
    {
        var res = new CreateMessageResponse();
        var creatingConversationDto = _mapper.Map<CreatingMessageDto>(request);
        res.Id = _service.Create(creatingConversationDto);
        return Task.FromResult(res);
    }

    public override Task<Empty> DeleteMessage(DeleteMessageRequest request, ServerCallContext context)
    {
        _service.Delete(request.Id);
        return Task.FromResult(new Empty());
    }
}