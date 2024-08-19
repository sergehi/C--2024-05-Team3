using AutoMapper;
using ChatService.API.Models.Message;
using ChatService.Services.Contracts.Message;

namespace ChatService.API.Mappings;

public class MessageMappingsProfile:Profile
{
    public MessageMappingsProfile()
    {
        CreateMap<MessageDto, MessageModel>();
        CreateMap<CreatingMessageModel, CreatingMessageDto>()
            .ForMember(d => d.UserId, map => map.Ignore());
    }
}