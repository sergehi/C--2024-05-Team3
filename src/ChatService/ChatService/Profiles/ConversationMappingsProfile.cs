using AutoMapper;
using ChatService.Models.ConversationModels;
using ChatService.Services.Contracts;

namespace ChatService.Profiles;

public class ConversationMappingsProfile:Profile
{
    public ConversationMappingsProfile()
    {
        CreateMap<ConversationDto, ConversationModel>().ReverseMap();
        CreateMap<CreatingConversationModel, CreatingConversationDto>().ReverseMap();
        CreateMap<UpdatingConversationModel, UpdatingConversationDto>().ReverseMap();
    }
}