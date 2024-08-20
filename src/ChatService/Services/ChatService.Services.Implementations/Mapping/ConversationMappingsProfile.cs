using AutoMapper;
using ChatService.Entities;
using ChatService.Services.Abstractions;
using ChatService.Services.Contracts;

namespace ChatService.Services.Implementations.Mapping;

public class ConversationMappingsProfile : Profile
{
    public ConversationMappingsProfile()
    {
        CreateMap<Conversation, ConversationDto>().ReverseMap();
        CreateMap<CreatingConversationDto, Conversation>()
            .ForMember(d => d.Id, map => map.Ignore())
            .ForMember(d => d.CreatedDate, map => map.Ignore())
            .ForMember(d => d.CreatedBy, map => map.Ignore())
            .ForMember(d => d.UpdatedDate, map => map.Ignore())
            .ForMember(d => d.UpdatedBy, map => map.Ignore())
            .ForMember(d => d.IsCancel, map => map.Ignore())
            .ForMember(d => d.Messages, map => map.Ignore());
        CreateMap<UpdatingConversationDto, Conversation>()
            .ForMember(d => d.Id, map => map.Ignore())
            .ForMember(d => d.TaskId, map => map.Ignore())
            .ForMember(d => d.CreatedDate, map => map.Ignore())
            .ForMember(d => d.CreatedBy, map => map.Ignore())
            .ForMember(d => d.UpdatedDate, map => map.Ignore())
            .ForMember(d => d.UpdatedBy, map => map.Ignore())
            .ForMember(d => d.IsCancel, map => map.Ignore())
            .ForMember(d => d.Messages, map => map.Ignore());
    }
}