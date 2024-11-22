using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AutoMapper;
using TasksService.DataAccess.Entities;
using TasksService.BusinessLogic.DTO;
using Google.Protobuf.WellKnownTypes;
using System.ComponentModel.Design;

namespace TasksService.BusinessLogic.Services.Implementations.Mapping
{
    public class TasksMappingProfile : Profile
    {
        public TasksMappingProfile()
        {
            // Datetime and TimeStamp
            CreateMap<Timestamp?, DateTime?>()
                .ConvertUsing(ts => ts == null ? (DateTime?)null : ts.ToDateTime() );
            CreateMap<DateTime?, Timestamp?>()
                .ConvertUsing(dt => dt.HasValue ? Timestamp.FromDateTime(dt.Value.ToUniversalTime()) : (Timestamp?)null);
            CreateMap<Timestamp, DateTime>()
                .ConvertUsing(ts => ts.ToDateTime());
            CreateMap<DateTime, Timestamp>()
                .ConvertUsing(dt => Timestamp.FromDateTime(dt.ToUniversalTime()));

            // Edges
            CreateMap<WfEdgesTemplate, TemplateEdgeDTO>()
                .ForMember(dest => dest.InternalNum, opt => opt.MapFrom(src => src.InternalNum))
                .ForMember(dest => dest.NodeFromNum, opt => opt.MapFrom(src => src.NodeFrom))
                .ForMember(dest => dest.NodeToNum, opt => opt.MapFrom(src => src.NodeTo));
            CreateMap<TemplateEdgeDTO, WfEdgesTemplate>()
                .ForMember(dest => dest.InternalNum, opt => opt.MapFrom(src => src.InternalNum))
                .ForMember(dest => dest.NodeFrom, opt => opt.MapFrom(src => src.NodeFromNum))
                .ForMember(dest => dest.NodeTo, opt => opt.MapFrom(src => src.NodeToNum))
                .ForMember(dest => dest.NodeFromNavigation, opt => opt.Ignore())
                .ForMember(dest => dest.NodeToNavigation, opt => opt.Ignore());
            // Nodes
            CreateMap<TemplateNodeDTO, WfNodesTemplate>()
                .ForMember(dest => dest.Definition, opt => opt.Ignore())
                .ForMember(dest => dest.WfedgesTemplNodeFromNavigations, opt => opt.Ignore())
                .ForMember(dest => dest.WfedgesTemplNodeToNavigations, opt => opt.Ignore());

            CreateMap<WfNodesTemplate, TemplateNodeDTO>()
                .ForMember(dest => dest.InternalNum, opt => opt.MapFrom(src => src.InternalNum))
                .ForMember(dest => dest.DefinitionId, opt => opt.MapFrom(src => src.DefinitionId));
            // Defs
            CreateMap<WfDefinitionsTemplate, TemplateItemDTO>()
                        .ForMember(dest => dest.Nodes, opt => opt.MapFrom(src => src.WfnodesTempls))
                        .ForMember(dest => dest.Edges, opt => opt.MapFrom(src => src.WfnodesTempls.SelectMany(node => node.WfedgesTemplNodeFromNavigations)));
            CreateMap<TemplateItemDTO, WfDefinitionsTemplate>()
                .ForMember(dest => dest.WfnodesTempls, opt => opt.MapFrom(src => src.Nodes))
                .ForMember(dest => dest.Company, opt => opt.Ignore())
                .ForMember(dest => dest.Tasks, opt => opt.Ignore());

        



        // Company
        CreateMap<TasksService.DataAccess.Entities.TasksCompany, TasksService.BusinessLogic.DTO.CompanyDTO>();

            CreateMap<CompanyDTO, TasksCompany>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id)) // Игнорируем Id, если не нужно его изменять
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.CompanyProjects, opt => opt.Ignore()) // Игнорируем зависимые коллекции
                .ForMember(dest => dest.Tasks, opt => opt.Ignore()) // Игнорируем зависимые коллекции
                .ForMember(dest => dest.WfdefinitionsTempls, opt => opt.Ignore()); // Игнорируем зависимые коллекции

            CreateMap<TasksService.BusinessLogic.DTO.CompanyProjectDTO, TasksService.DataAccess.Entities.CompanyProject>()
            .ForMember(dest => dest.Company, opt => opt.Ignore()) // Игнорируем навигационное свойство
            .ForMember(dest => dest.ProjectAreas, opt => opt.Ignore()) // Игнорируем коллекцию ProjectAreas
            .ForMember(dest => dest.Tasks, opt => opt.Ignore()); // Игнорируем коллекцию Tasks

            // Сопоставление от CompanyProject к CompanyProjectDTO
            CreateMap<TasksService.DataAccess.Entities.CompanyProject, TasksService.BusinessLogic.DTO.CompanyProjectDTO>()
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description ?? string.Empty)); // Обработка null

            CreateMap<TasksService.BusinessLogic.DTO.ProjectAreaDTO, TasksService.DataAccess.Entities.ProjectArea>()
            .ForMember(dest => dest.Project, opt => opt.Ignore()) // Игнорируем навигационное свойство
            .ForMember(dest => dest.Tasks, opt => opt.Ignore()); // Игнорируем коллекцию Tasks

            // Сопоставление от ProjectArea к ProjectAreaDTO
            CreateMap<TasksService.DataAccess.Entities.ProjectArea, TasksService.BusinessLogic.DTO.ProjectAreaDTO>()
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description ?? string.Empty)); // Обработка null

            ///!!!!!!!!!!!!!!
            CreateMap<TaskDTO, DataAccess.Entities.Task>()
                .ForMember(dest => dest.Area, opt => opt.Ignore())
                .ForMember(dest => dest.Company, opt => opt.Ignore())
                .ForMember(dest => dest.Project, opt => opt.Ignore())
                //.ForMember(dest => dest.CurrentNode, opt => opt.Ignore())
                .ForMember(dest => dest.Template, opt => opt.Ignore())
                //.ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Urgency, opt => opt.MapFrom(src => src.UrgencyId))
                .ForMember(dest => dest.CreationDate, opt => opt.MapFrom(src => ConvertTimestampToDateTime(src.CreationDate)))
                .ForMember(dest => dest.DeadlineDate, opt => opt.MapFrom(src => ConvertTimestampToDateTime(src.DeadlineDate)))
                .ForMember(dest => dest.UrgencyNavigation, opt => opt.Ignore())
                .ForMember(dest => dest.CreatorId, opt => opt.MapFrom(src => src.CreatorId))
                .ForMember(dest => dest.CurrentNodeId, opt => opt.MapFrom(src => src.CurrentNode))
                .ForMember(dest => dest.Nodes, opt => opt.Ignore());

            CreateMap<DataAccess.Entities.Task, TaskDTO>()
                .ForMember(dest => dest.UrgencyId, opt => opt.MapFrom(src => src.Urgency))
                .ForMember(dest => dest.RowVersion, opt => opt.MapFrom(src => src.RowVersion))
                .ForMember(dest => dest.CreatorId, opt => opt.MapFrom(src => src.CreatorId))
                .ForMember(dest => dest.CurrentNode, opt => opt.MapFrom(src => src.CurrentNodeId));


            CreateMap<FullTaskInfoDTO, DataAccess.Entities.Task>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Task.Id))
                .ForMember(dest => dest.AreaId, opt => opt.MapFrom(src => src.Task.AreaId))
                .ForMember(dest => dest.CompanyId, opt => opt.MapFrom(src => src.Task.CompanyId))
                .ForMember(dest => dest.CreatorId, opt => opt.MapFrom(src => src.Task.CreatorId))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Task.Description))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Task.Name))
                .ForMember(dest => dest.ProjectId, opt => opt.MapFrom(src => src.Task.ProjectId))
                .ForMember(dest => dest.TemplateId, opt => opt.MapFrom(src => src.Task.TemplateId))
                .ForMember(dest => dest.Urgency, opt => opt.MapFrom(src => src.Task.UrgencyId))
                //.ForMember(dest => dest.CreationDate, opt => opt.MapFrom(src => src.Task.CreationDate))
                //.ForMember(dest => dest.DeadlineDate, opt => opt.MapFrom(src => src.Task.DeadlineDate))
                .ForMember(dest => dest.CreationDate, opt => opt.MapFrom(src => ConvertTimestampToDateTime(src.Task.CreationDate)))
                .ForMember(dest => dest.DeadlineDate, opt => opt.MapFrom(src => ConvertTimestampToDateTime(src.Task.DeadlineDate)))


                .ForMember(dest => dest.CurrentNodeId, opt => opt.MapFrom(src => src.Task.CurrentNode))
                .ForMember(dest => dest.RowVersion, opt => opt.MapFrom(src => src.Task.RowVersion))
                .ForMember(dest => dest.Area, opt => opt.Ignore())
                .ForMember(dest => dest.Company, opt => opt.Ignore())
                .ForMember(dest => dest.Project, opt => opt.Ignore())
                //.ForMember(dest => dest.CurrentNode, opt => opt.Ignore())
                .ForMember(dest => dest.Template, opt => opt.Ignore())
                .ForMember(dest => dest.UrgencyNavigation, opt => opt.Ignore());


                //.ForAllMembers(opt => opt.MapFrom(src => src.Task));

            CreateMap<DataAccess.Entities.Task, FullTaskInfoDTO>()
                .ForMember(dest => dest.Task, opt => opt.MapFrom(src => src))
                .ForMember(dest => dest.Nodes, opt => opt.Ignore()) // Здесь вы можете добавить логику для заполнения Nodes
                .ForMember(dest => dest.Edges, opt => opt.Ignore());


            CreateMap<CreateTaskDTO, DataAccess.Entities.Task>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatorId, opt => opt.Ignore())
                .ForMember(dest => dest.CreationDate, opt => opt.Ignore())
                .ForMember(dest => dest.CurrentNodeId, opt => opt.Ignore())
                .ForMember(dest => dest.Area, opt => opt.Ignore())
                .ForMember(dest => dest.Company, opt => opt.Ignore())
                .ForMember(dest => dest.Project, opt => opt.Ignore())
                //.ForMember(dest => dest.CurrentNode, opt => opt.Ignore())
                .ForMember(dest => dest.Template, opt => opt.Ignore())
                .ForMember(dest => dest.UrgencyNavigation, opt => opt.Ignore())
                .ForMember(dest => dest.RowVersion, opt => opt.Ignore())
                .ForMember(dest => dest.DeadlineDate, opt => opt.MapFrom(src => ConvertTimestampToDateTime(src.DeadlineDate)))
                .ForMember(dest => dest.Urgency, opt => opt.MapFrom(src => src.UrgencyId))
                .ForMember(dest => dest.Nodes, opt => opt.Ignore());

            CreateMap<UrgencyDTO, Urgency>()
                .ForMember(dest => dest.Tasks, opt => opt.Ignore()); 
            CreateMap<Urgency, UrgencyDTO>();

            CreateMap<DataAccess.Entities.Task, CreateTaskDTO>()
                .ForMember(dest => dest.UrgencyId, opt => opt.MapFrom(src => src.Urgency))
                .ForMember(dest => dest.CurrentNode, opt => opt.MapFrom(src => src.CurrentNodeId));

            CreateMap<TaskNode, TaskNodeDTO>()
                .ForMember(dest => dest.NodeDoers, opt => opt.MapFrom(src => src.TaskDoers != null ? src.TaskDoers.Select(td => td.EmpoyeeId) : new List<Guid>()))
                .ForMember(dest => dest.TaskId, opt => opt.MapFrom(src => src.OwnerTaskId ?? 0)) // Замените на нужное значение, если OwnerTaskId может быть null
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name ?? string.Empty)) // Обработка null
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description ?? string.Empty)); // Обработка null

            // Маппинг из TaskNodeDTO в TaskNode
            CreateMap<TaskNodeDTO, TaskNode>()
                .ForMember(dest => dest.OwnerTaskId, opt => opt.MapFrom(src => src.TaskId))
                .ForMember(dest => dest.TaskDoers, opt => opt.Ignore()) // Игнорируем, так как это коллекция, которую нужно будет заполнить отдельно
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.Terminating, opt => opt.MapFrom(src => src.Terminating))
                .ForMember(x => x.OwnerTask, opt => opt.Ignore())
                .ForMember(x => x.TaskEdgeNodeFromNavigations, opt => opt.Ignore())
                .ForMember(x => x.TaskEdgeNodeToNavigations, opt => opt.Ignore())
                .ForMember(x => x.TemplateId, opt => opt.Ignore())
                .ForMember(dest => dest.IconId, opt => opt.MapFrom(src => src.IconId));

            /*
            CreateMap<TaskNodeDTO, TaskNode>()
                .ForMember(x => x.TaskDoers, opt => opt.Ignore())
                .ForMember(dest => dest.OwnerTaskId, opt => opt.MapFrom(src => src.TaskId));
            */


            CreateMap<TaskEdge, TaskEdgeDTO>()
                        .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name ?? string.Empty)) // Обработка null
                        .ForMember(dest => dest.NodeFrom, opt => opt.MapFrom(src => src.NodeFrom))
                        .ForMember(dest => dest.NodeTo, opt => opt.MapFrom(src => src.NodeTo));

            // Маппинг из TaskEdgeDTO в TaskEdge
            CreateMap<TaskEdgeDTO, TaskEdge>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.NodeFrom, opt => opt.MapFrom(src => src.NodeFrom))
                .ForMember(dest => dest.NodeTo, opt => opt.MapFrom(src => src.NodeTo))
                .ForMember(x => x.NodeFromNavigation, opt => opt.Ignore())
                .ForMember(x => x.NodeToNavigation, opt => opt.Ignore());

            CreateMap<TasksService.DataAccess.Entities.TaskHistory, TasksService.BusinessLogic.DTO.TaskHistoryDTO>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId))
                .ForMember(dest => dest.ActionDate, opt => opt.MapFrom(src => Timestamp.FromDateTime(src.ActionDate.ToUniversalTime())))
                .ForMember(dest => dest.NewValue, opt => opt.MapFrom(src => src.ActionValue))
                .ForMember(dest => dest.OldValue, opt => opt.MapFrom(src => src.OldValue))
                .ForMember(dest => dest.ActionName, opt => opt.MapFrom(src => src.Action != null ? src.Action.Name : ""))
                .ForMember(dest => dest.ValueType, opt => opt.MapFrom(src => src.Action != null ? src.Action.FieldType : 0 ))
                .ForMember(dest => dest.ActionString, opt => opt.MapFrom(src => src.Action != null ? src.Action.Description : ""));

            // Маппинг из TaskHistoryDTO в TaskHistory
            CreateMap<TasksService.BusinessLogic.DTO.TaskHistoryDTO, TasksService.DataAccess.Entities.TaskHistory>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId))
                .ForMember(dest => dest.ActionDate, opt => opt.MapFrom(src => src.ActionDate.ToDateTime()))
                .ForMember(dest => dest.ActionValue, opt => opt.MapFrom(src => src.NewValue))
                .ForMember(dest => dest.OldValue, opt => opt.MapFrom(src => src.OldValue))
                .ForMember(dest => dest.TaskId, opt => opt.MapFrom(src => src.TaskId))
                .ForMember(dest => dest.ActionId, opt => opt.Ignore())
                .ForMember(dest => dest.NodeId, opt => opt.Ignore())
                .ForMember(dest => dest.Action, opt => opt.Ignore());
        }

        private DateTime? ConvertTimestampToDateTime(Timestamp? timestamp)
        {
            if (null == timestamp)
                return null;
            return timestamp.ToDateTime().ToUniversalTime();
        }

    }
}
