using AutoMapper;
using ChatService.API.Models.Conversation;
using ChatService.Services.Contracts;

namespace ChatService.API.Mappings;

public class ConversationMappingsProfile:Profile
{
    public ConversationMappingsProfile()
    {
        CreateMap<ConversationDto, ConversationModel>().ReverseMap();
        CreateMap<CreatingConversationModel, CreatingConversationDto>().ReverseMap();
        CreateMap<UpdatingConversationModel, UpdatingConversationDto>().ReverseMap();
    }
}