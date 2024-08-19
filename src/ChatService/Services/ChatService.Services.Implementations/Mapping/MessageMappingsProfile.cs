using AutoMapper;
using ChatService.Entities;
using ChatService.Services.Contracts.Message;

namespace ChatService.Services.Implementations.Mapping;

public class MessageMappingsProfile:Profile
{
    public MessageMappingsProfile()
    {
        CreateMap<Message, MessageDto>();
        CreateMap<CreatingMessageDto, Message>()
            .ForMember(d => d.CreatedDate, map => map.Ignore())
            .ForMember(d => d.CreatedBy, map => map.Ignore())
            .ForMember(d => d.IsDelete, map => map.Ignore())
            .ForMember(d => d.Conversation, map => map.Ignore())
            .ForMember(d => d.MessageType, map => map.Ignore())
            .ForMember(d => d.MediaFiles, map => map.Ignore())
            .ForMember(d => d.Id, map => map.Ignore());
        CreateMap<Reaction, ReactionDto>();
    }
}