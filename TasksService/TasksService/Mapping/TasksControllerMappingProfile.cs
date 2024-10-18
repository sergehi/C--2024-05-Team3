using AutoMapper;
using TasksTemplatesService;
using TaslsService.BusinessLogic.DTO;

namespace TasksService.Mapping
{
    public class TasksControllerMappingProfile : Profile
    {

        public TasksControllerMappingProfile()
        {
            CreateMap<NodeDTO, messageNode>()
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description ?? ""))
                .ForMember(dest => dest.Terminator, opt => opt.MapFrom(src => src.Terminator));
            CreateMap<messageNode, NodeDTO>();

            CreateMap<EdgeDTO, messageEdge>();
            CreateMap<messageEdge, EdgeDTO>();

            CreateMap<TemplateListItem, TemplateItemDTO>();

            CreateMap<TemplateItemDTO, TemplateListItem>()
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description ?? ""))
                .ForMember(dest => dest.Nodes, opt => opt.MapFrom(src => src.Nodes))
                .ForMember(dest => dest.Edges, opt => opt.MapFrom(src => src.Edges));


            CreateMap<CreateTemplateReply, long>()
                .ConvertUsing(src => src.Id);
            CreateMap<long, CreateTemplateReply>()
                .ConvertUsing(src => new CreateTemplateReply { Id = src });

            CreateMap<BoolReply, bool>()
                .ConvertUsing(src => src.Success);
            CreateMap<bool, BoolReply>()
                .ConvertUsing(src => new BoolReply { Success = src });

        }

    }
}
