using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AutoMapper;
using TasksService.DataAccess.Entities;
using TaslsService.BusinessLogic.DTO;

namespace TasksService.BusinessLogic.Services.Implementations.Mapping
{
    public class TasksMappingProfile : Profile
    {
        public TasksMappingProfile()
        {
            // Edges
            CreateMap<WfEdgesTemplate, EdgeDTO>()
                .ForMember(dest => dest.InternalNum, opt => opt.MapFrom(src => src.InternalNum))
                .ForMember(dest => dest.NodeFromNum, opt => opt.MapFrom(src => src.NodeFrom))
                .ForMember(dest => dest.NodeToNum, opt => opt.MapFrom(src => src.NodeTo));
            CreateMap<EdgeDTO, WfEdgesTemplate>()
                .ForMember(dest => dest.InternalNum, opt => opt.MapFrom(src => src.InternalNum))
                .ForMember(dest => dest.NodeFrom, opt => opt.MapFrom(src => src.NodeFromNum))
                .ForMember(dest => dest.NodeTo, opt => opt.MapFrom(src => src.NodeToNum))
                .ForMember(dest => dest.NodeFromNavigation, opt => opt.Ignore())
                .ForMember(dest => dest.NodeToNavigation, opt => opt.Ignore());
            // Nodes
            CreateMap<NodeDTO, WfNodesTemplate>()
                .ForMember(dest => dest.Definition, opt => opt.Ignore())
                .ForMember(dest => dest.WfedgesTemplNodeFromNavigations, opt => opt.Ignore())
                .ForMember(dest => dest.WfedgesTemplNodeToNavigations, opt => opt.Ignore());

            CreateMap<WfNodesTemplate, NodeDTO>()
                .ForMember(dest => dest.InternalNum, opt => opt.MapFrom(src => src.InternalNum))
                .ForMember(dest => dest.DefinitionId, opt => opt.MapFrom(src => src.DefinitionId));
            // Defs
            CreateMap<WfDefinitionsTemplate, TemplateItemDTO>()
                        .ForMember(dest => dest.Nodes, opt => opt.MapFrom(src => src.WfnodesTempls))
                        .ForMember(dest => dest.Edges, opt => opt.MapFrom(src => src.WfnodesTempls.SelectMany(node => node.WfedgesTemplNodeFromNavigations)));
            CreateMap<TemplateItemDTO, WfDefinitionsTemplate>()
                .ForMember(dest => dest.WfnodesTempls, opt => opt.MapFrom(src => src.Nodes))
                .ForMember(dest => dest.Company, opt => opt.Ignore());

            // Company
            CreateMap<CompanyDTO, TasksCompany>()
                .ForMember(x => x.WfdefinitionsTempls, opt => opt.Ignore());
        }
    }
}
