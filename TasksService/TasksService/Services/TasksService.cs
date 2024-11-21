using Grpc.Core;
using Common.Rpc;
using TasksService.BusinessLogic.Services.Abstractions;
using AutoMapper;
using TasksService.BusinessLogic.DTO;
using TasksServiceProto;
using static TasksServiceProto.TasksServiceProto;
using System.Threading.Tasks;

namespace TasksService.Services
{
    public class TasksService : TasksServiceProtoBase
    {
        private ITasksService _service;
        private readonly ILogger<TasksService> _logger;
        private readonly IMapper _mapper;


        public TasksService(ITasksService service, ILogger<TasksService> logger, IMapper mapper)
        {
            _service = service;
            _logger = logger;
            _mapper = mapper;
        }

        #region Templates
        public async override Task<TemplateListReply> GetTemplateList(TemplateListRequest request, ServerCallContext context)
        {
            try
            {
                var res = new TemplateListReply();
                var templateItems = await _service.GetTemplateList(request.Id, request.CompanyId);
                res.Items.AddRange(templateItems.Select(item => _mapper.Map<TemplateListItem>(item)));
                return res;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async override Task<CreateTemplateReply> CreateTemplate(CreateTemplateRequest request, ServerCallContext context)
        {
            var res = new CreateTemplateReply();
            
            try
            {
                Guid userId = Guid.Parse(request.UserId);
                return _mapper.Map<CreateTemplateReply>(await _service.CreateTemplate(userId,  request.Name, request.Description, request.CompanyId, _mapper.Map<List<TemplateNodeDTO>>(request.Nodes), _mapper.Map<List<TemplateEdgeDTO>>(request.Edges)));

            }
            catch (Exception ex)
            {
                throw new RpcExceptionEx(new Status(StatusCode.Cancelled, ex.Message), $"Произошла ошибка при добавлении записи сервере\n{ex.Message}", ex.StackTrace?? "");
            }

        }

        public async override Task<BoolReply> UpdateTemplate(UpdateTemplateRequest request, ServerCallContext context)
        {
            try
            {
                Guid userId = Guid.Parse(request.UserId);
                return _mapper.Map<BoolReply>(await _service.UpdateTemplate(userId, request.Item.Id, request.Item.Name, request.Item.Description, request.Item.CompanyId, _mapper.Map<List<TemplateNodeDTO>>(request.Item.Nodes), _mapper.Map<List<TemplateEdgeDTO>>(request.Item.Edges)));
            }
            catch (Exception ex)
            {
                throw new RpcExceptionEx(new Status(StatusCode.Cancelled, ex.Message), "Произошла ошибка при добавлении записи сервере", ex.StackTrace);
            }
        }

        public async override Task<BoolReply> DeleteTemplate(DeleteTemplateRequest request, ServerCallContext context)
        {
            try
            {
                Guid userId = Guid.Parse(request.UserId);
                return _mapper.Map<BoolReply>(await _service.DeleteTemplate(userId, request.Id));
            }
            catch (Exception ex)
            {
                throw new RpcExceptionEx(new Status(StatusCode.Cancelled, ex.Message), $"Произошла ошибка при удалении записи сервере\n{ex.Message}", ex.StackTrace);
            }
        }
        #endregion Templates

        #region Tasks
        /// Tasks
        public override async Task<TasksListReply> GetTasksList(TasksListRequest request, ServerCallContext context)
        {
            try
            {
                var res = new TasksListReply();
                var templateItems = await _service.GetTasksList(_mapper.Map<Guid>(request.UserId), request.CompanyId, request.ProjectId, request.AreaId);
                res.Tasks.AddRange(templateItems.Select(item => _mapper.Map<TaskModel>(item)));
                return res;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public override async Task<TaskFullReply> GetTask(TaskRequest request, ServerCallContext context)
        {
            try
            {
                var task = await _service.GetTask(request.TaskId);
                return _mapper.Map<TaskFullReply>(task);
            }
            catch (Exception)
            {
                throw;
            }

        }
        public override async Task<PkMessage> CreateTask(CreateTaskRequest request, ServerCallContext context)
        {
            try
            {
                var id = await _service.CreateTask(_mapper.Map<Guid>(request.UserId), _mapper.Map<CreateTaskDTO>(request.Task));
                return _mapper.Map<PkMessage>(id);
            }
            catch (Exception)
            {
                throw;
            }
        }
        
        public override async Task<BoolReply> DeleteTask(DeleteTaskRequest request, ServerCallContext context)
        {
            try
            {
                var succes = await _service.DeleteTask(_mapper.Map<Guid>(request.UserId), request.TaskId);
                return _mapper.Map<BoolReply>(succes);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public override async Task<TaskHistoryReply> GetTaskHistory(PkMessage request, ServerCallContext context)
        {
            try
            {
                var reply = new TaskHistoryReply();
                List<TaskHistoryDTO> task = await _service.GetTaskHistory(request.Id);
                reply.History.AddRange(task.Select(x => _mapper.Map<TaskHistoryModel>(x)));
                return reply;
            }
            catch (Exception)
            {
                throw;
            }

        }



        public override async Task<BoolReply> ModifyTaskUrgency(ModifyTaskLongFieldRequest request, ServerCallContext context)
        {
            try
            {
                var succes = await _service.ModifyTaskUrgency(_mapper.Map<Guid>(request.UserId), request.TaskId, request.LongValue );
                return _mapper.Map<BoolReply>(succes);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public override async Task<BoolReply> ModifyTaskState(ModifyTaskLongFieldRequest request, ServerCallContext context)
        {
            try
            {
                var succes = await _service.ModifyTaskState(_mapper.Map<Guid>(request.UserId), request.TaskId, request.LongValue);
                return _mapper.Map<BoolReply>(succes);
            }
            catch (Exception)
            {
                throw;
            }

        }
        public override async Task<BoolReply> ModifyTaskName(ModifyTaskTextFieldRequest request, ServerCallContext context)
        {
            try
            {
                var succes = await _service.ModifyTaskName(_mapper.Map<Guid>(request.UserId), request.TaskId, request.StrValue);
                return _mapper.Map<BoolReply>(succes);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public override async Task<BoolReply> ModifyTaskDescription(ModifyTaskTextFieldRequest request, ServerCallContext context) 
        {
            try
            {
                var succes = await _service.ModifyTaskDescription(_mapper.Map<Guid>(request.UserId), request.TaskId, request.StrValue);
                return _mapper.Map<BoolReply>(succes);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public override async Task<BoolReply> ModifyTaskNodeDeadline(ModifyNodeDeadlineRequest request, ServerCallContext context) 
        {
            try
            {
                var succes = await _service.ModifyTaskNodeDeadline(_mapper.Map<Guid>(request.UserId), request.TaskId, request.NodeId, _mapper.Map<DateTime>(request.DateValue));
                return _mapper.Map<BoolReply>(succes);
            }
            catch (Exception)
            {
                throw;
            }
        }




        #endregion

        #region TaskRoutes
        public override async Task<TransitionListReply> GetFromNodeTransitions(PkMessage request, ServerCallContext context)
        {
            try
            {
                var edges = await _service.GetFromNodeTransitions(request.Id);
                return _mapper.Map<TransitionListReply>(edges);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public override async Task<TransitionListReply> GetToNodeTransitions(PkMessage request, ServerCallContext context)
        {
            try
            {
                var edges = await _service.GetToNodeTransitions(request.Id);
                return _mapper.Map<TransitionListReply>(edges);
            }
            catch (Exception)
            {
                throw;
            }
        }


        public override async Task<TaskNode> GetNode(PkMessage request, ServerCallContext context)
        {
            try
            {
                TaskNodeDTO node = await _service.GetNode(request.Id);
                return _mapper.Map<TaskNode>(node);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public override async Task<BoolReply> AppointNodeDoers(AppointDoersRequest request, ServerCallContext context)
        {
            try
            {
                bool succes = await _service.AppointNodeDoers(_mapper.Map<Guid>(request.UserId), request.NodeId, _mapper.Map<List<Guid>>(request.Doers));
                return _mapper.Map<BoolReply>(succes);
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion
        
        #region Task urgencies
        public override async Task<TasksUrgenciesListReply> GetUrgencies(TasksUrgenciesListRequest request, ServerCallContext context)
        {
            try
            {
                var result = new TasksUrgenciesListReply();
                List<UrgencyDTO> urgencies = await _service.GetUrgencies(request.Id);
                if (null != urgencies)
                    result.Urgenicies.AddRange(urgencies.Select(x => _mapper.Map<UrgencyModel>(x)));
                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public override async Task<CreateUrgencyReply> CreateUrgency(CreateUrgencyRequest request, ServerCallContext context)
        {
            try
            {
                
                long newId = await _service.CreateUrgency(_mapper.Map<Guid>(request.UserId), request.Name, request.Description);
                return _mapper.Map<CreateUrgencyReply>(newId);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public override async Task<BoolReply> ModifyUrgency(ModifyUrgencyRequest request, ServerCallContext context)
        {
            try
            {

                bool succes = await _service.ModifyUrgency(_mapper.Map<Guid>(request.UserId), request.ChangeFlags, _mapper.Map<UrgencyDTO>(request.Urgency));
                return _mapper.Map<BoolReply>(succes);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public override async Task<BoolReply> DeleteUrgency(DeleteUrgencyRequest request, ServerCallContext context)
        {
            try
            {
                bool succes = await _service.DeleteUrgency(_mapper.Map<Guid>(request.UserId), request.UrgencyId);
                return _mapper.Map<BoolReply>(succes);
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion
      
        #region Project areas
        public override async Task<ProjectAreasReply> GetProjectAreas(ProjectAreasRequest request, ServerCallContext context)
        {
            try
            {
                var result = new ProjectAreasReply();
                List<ProjectAreaDTO> areas = await _service.GetProjectAreas(request.ProjectId, request.AreaId);
                if (null != areas)
                    result.Areas.AddRange(areas.Select(x => _mapper.Map<ProjectArea>(x)));
                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public override async Task<PkMessage> CreateProjectArea(CreateProjectAreaRequest request, ServerCallContext context)
        {
            try
            {

                long newId = await _service.CreateProjectArea(_mapper.Map<Guid>(request.UserId), _mapper.Map<ProjectAreaDTO>(request.Area));
                return _mapper.Map<PkMessage>(newId);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public override async Task<BoolReply> ModifyProjectArea(ModifyProjectAreaRequest request, ServerCallContext context)
        {
            try
            {

                bool succes = await _service.ModifyProjectArea(_mapper.Map<Guid>(request.UserId), request.ChangeFlags, _mapper.Map<ProjectAreaDTO>(request.Area));
                return _mapper.Map<BoolReply>(succes);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public override async Task<BoolReply> DeleteProjectArea(DeleteProjectAreaRequest request, ServerCallContext context)
        {
            try
            {
                bool succes = await _service.DeleteProjectArea(_mapper.Map<Guid>(request.UserId), request.AreaId);
                return _mapper.Map<BoolReply>(succes);
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion

        #region Company projects
        public override async Task<CompanyProjectsReply> GetCompanyProjects(CompanyProjectsRequest request, ServerCallContext context)
        {
            try
            {
                var result = new CompanyProjectsReply();
                List<CompanyProjectDTO> projects = await _service.GetCompanyProjects(request.CompanyId, request.ProjectId);
                if (null != projects)
                    result.Projects.AddRange(projects.Select(x => _mapper.Map<CompanyProject>(x)));
                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public override async Task<PkMessage> CreateCompanyProject(CreateProjectRequest request, ServerCallContext context)
        {
            try
            {

                long newId = await _service.CreateCompanyProject(_mapper.Map<Guid>(request.UserId), _mapper.Map<CompanyProjectDTO>(request.Project));
                return _mapper.Map<PkMessage>(newId);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public override async Task<BoolReply> ModifyCompanyProject(ModifyProjectRequest request, ServerCallContext context)
        {
            try
            {

                bool succes = await _service.ModifyCompanyProject(_mapper.Map<Guid>(request.UserId), request.ChangeFlags, _mapper.Map<CompanyProjectDTO>(request.Project));
                return _mapper.Map<BoolReply>(succes);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public override async Task<BoolReply> DeleteCompanyProject(DeleteProjectRequest request, ServerCallContext context)
        {
            try
            {
                bool succes = await _service.DeleteCompanyProject(_mapper.Map<Guid>(request.UserId), request.ProjectId);
                return _mapper.Map<BoolReply>(succes);
            }
            catch (Exception)
            {
                throw;
            }

        }
        #endregion

        #region Companies
        public override async Task<CompaniesReply> GetCompanies(CompanyRequest request, ServerCallContext context)
        {
            try
            {
                var result = new CompaniesReply();
                List<CompanyDTO> comanies = await _service.GetCompanies(request.Id);
                if (null != comanies)
                    result.Companies.AddRange(comanies.Select(x => _mapper.Map<CompanyModel>(x)));
                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public override async Task<PkMessage> CreateCompany(CreateCompanyRequest request, ServerCallContext context)
        {
            try
            {

                long newId = await _service.CreateCompany(_mapper.Map<Guid>(request.CreatorId), request.Name, request.Description);
                return _mapper.Map<PkMessage>(newId);
            }
            catch (Exception)
            {
                throw;
            }

        }
        public override async Task<BoolReply> ModifyCompany(ModifyCompanyRequest request, ServerCallContext context)
        {
            try
            {

                bool succes = await _service.ModifyCompany(_mapper.Map<Guid>(request.UserId), request.ChangeFlags, request.Id, request.Name, request.Description);
                return _mapper.Map<BoolReply>(succes);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public override async Task<BoolReply> DeleteCompany(DeleteCompanyRequest request, ServerCallContext context)
        {
            try
            {
                bool succes = await _service.DeleteCompany(_mapper.Map<Guid>(request.UserId), request.CompanyId);
                return _mapper.Map<BoolReply>(succes);
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion

        #region Company Employees
        public override async Task<EmployeesReply> GetEmployees(EmployeesRequest request, ServerCallContext context)
        {
            try
            {
                var result = new EmployeesReply();
                var employees = await _service.GetEmployees(request.CompanyId);
                if (null != employees)
                    result.EmployeeIds.AddRange(_mapper.Map<List<string>>(employees));
                return result;
            }
            catch (Exception)
            {
                throw;
            }

        }
        public override async Task<BoolReply> AddEmployee(AddEmployeeRequest request, ServerCallContext context)
        {
            try
            {

                bool succes = await _service.AddEmployee(_mapper.Map<Guid>(request.CreatorId), request.CompanyId, _mapper.Map<Guid>(request.NewUserId));
                return _mapper.Map<BoolReply>(succes);
            }
            catch (Exception)
            {
                throw;
            }

        }
        public override async Task<BoolReply> RemoveEmployee(RemoveEmployeeRequest request, ServerCallContext context)
        {
            try
            {
                bool succes = await _service.RemoveEmployee(_mapper.Map<Guid>(request.DeleterId), request.CompanyId, _mapper.Map<Guid>(request.UserToDelId));
                return _mapper.Map<BoolReply>(succes);
            }
            catch (Exception)
            {
                throw;
            }

        }

        #endregion

    }
}
