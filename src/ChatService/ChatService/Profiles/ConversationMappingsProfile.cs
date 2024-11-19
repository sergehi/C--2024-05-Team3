using AutoMapper;
using ChatProto;
using ChatService.Services.Contracts;
using CreatingConversationModel = ChatService.Models.ConversationModels.CreatingConversationModel;
using UpdatingConversationModel = ChatService.Models.ConversationModels.UpdatingConversationModel;

namespace ChatService.Profiles;

public class ConversationMappingsProfile:Profile
{
    public ConversationMappingsProfile()
    {
        CreateMap<CreatingConversationModel, CreatingConversationDto>().ReverseMap();
        CreateMap<UpdatingConversationModel, UpdatingConversationDto>().ReverseMap();
        CreateMap<CreateConversationRequest, CreatingConversationDto>().ReverseMap();
        CreateMap<UpdateConversationRequest, UpdatingConversationDto>().ReverseMap();
    }
}