using AutoMapper;
using TasksService.BusinessLogic.DTO;
using TasksService;
using TasksServiceProto;
using Google.Protobuf.WellKnownTypes;
using Microsoft.EntityFrameworkCore.Update.Internal;

namespace TasksService.Mapping
{
    public class TasksControllerMappingProfile : Profile
    {

        public TasksControllerMappingProfile()
        {

            CreateMap<string, Guid>()
                       .ConvertUsing(str => Guid.Parse(str)); // Convert string to Guid
            CreateMap<Guid, string>()
                .ConvertUsing(guid => guid.ToString()); // Convert Guid to string

            CreateMap<TemplateNodeDTO, TemplateNode>()
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description ?? ""))
                .ForMember(dest => dest.Terminating, opt => opt.MapFrom(src => src.Terminating));
            CreateMap<TemplateNode, TemplateNodeDTO>();

            CreateMap<TemplateEdgeDTO, TemplateEdge>();
            CreateMap<TemplateEdge, TemplateEdgeDTO>();

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


            CreateMap<TaskDTO, TaskModel>();
            CreateMap<TaskModel, TaskDTO>()
                .ForMember(dest => dest.RowVersion, opt => opt.Ignore())
                .ForMember(dest => dest.DeadlineDate, opt=> opt.MapFrom(t => t== null ? (DateTime?)null : t.DeadlineDate.ToDateTime().ToUniversalTime()));

            CreateMap<CreateTaskDTO, CreateTaskModel>();
            CreateMap<CreateTaskModel, CreateTaskDTO>();

            CreateMap<TaskNodeDTO, TaskNode>()
                 .ForMember(dest => dest.NodeDoers, opt => opt.MapFrom(src => src.NodeDoers));
            CreateMap<TaskNode, TaskNodeDTO>()
                .ForMember(dest => dest.NodeDoers, opt => opt.MapFrom(src => src.NodeDoers));

            CreateMap<TaskEdgeDTO, TaskEdge>();
            CreateMap<TaskEdge, TaskEdgeDTO>();


            CreateMap<TaskFullReply, FullTaskInfoDTO>();
            CreateMap<FullTaskInfoDTO, TaskFullReply>();


            CreateMap<PkMessage, long>()
                .ConvertUsing(src => src.Id);
            CreateMap<long, PkMessage>()
                .ConvertUsing(src => new PkMessage { Id = src });

            CreateMap<List<TaskEdgeDTO>, TransitionListReply>()
                     .ForMember(dest => dest.Edges, opt => opt.MapFrom(src => src));


            CreateMap<UrgencyDTO, UrgencyModel>();
            CreateMap<UrgencyModel, UrgencyDTO>();

            CreateMap <long, CreateUrgencyReply>()
                .ConvertUsing(src => new CreateUrgencyReply { Id = src });

            CreateMap<ProjectAreaDTO, ProjectArea>();
            CreateMap<ProjectArea, ProjectAreaDTO>();

            CreateMap<CompanyProjectDTO, CompanyProject>(); 
            CreateMap<CompanyProject, CompanyProjectDTO>();

            CreateMap<CompanyModel, CompanyDTO>();
            CreateMap<CompanyDTO, CompanyModel>();

            CreateMap<TaskHistoryDTO, TaskHistoryModel>()
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId))
                .ForMember(dest => dest.ActionDate, opt => opt.MapFrom(src => src.ActionDate))
                .ForMember(dest => dest.NewValue, opt => opt.MapFrom(src => src.NewValue))
                .ForMember(dest => dest.OldValue, opt => opt.MapFrom(src => src.OldValue))
                .ForMember(dest => dest.ValueType, opt => opt.MapFrom(src => src.ValueType))
                .ForMember(dest => dest.ActionName, opt => opt.MapFrom(src => src.ActionName))
                .ForMember(dest => dest.ActionString, opt => opt.MapFrom(src => src.ActionString));

            CreateMap<TaskHistoryModel, TaskHistoryDTO>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.TaskId, opt => opt.Ignore());


            // Datetime and TimeStamp
            /*
            CreateMap<Timestamp?, DateTime?>()
                .ConvertUsing(ts => ts == null ? (DateTime?)null : ts.ToDateTime().ToUniversalTime());
            CreateMap<DateTime?, Timestamp?>()
                .ConvertUsing(dt => dt.HasValue ? Timestamp.FromDateTime(dt.Value.ToUniversalTime()) : (Timestamp?)null);
            */
        }



    }
}
