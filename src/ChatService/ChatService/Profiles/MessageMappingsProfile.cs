using AutoMapper;
using ChatProto;
using ChatService.API.Models.Message;
using ChatService.Services.Contracts.Message;

namespace ChatService.Profiles;

public class MessageMappingsProfile:Profile
{
    public MessageMappingsProfile()
    {
        CreateMap<MessageDto, MessageModel>();
        CreateMap<CreatingMessageModel, CreatingMessageDto>()
            .ForMember(d => d.UserId, map => map.Ignore());
        CreateMap<ReactionDto, ReactionModel>();
        
        CreateMap<CreateMessageRequest, CreatingMessageDto>().ReverseMap();
    }
}