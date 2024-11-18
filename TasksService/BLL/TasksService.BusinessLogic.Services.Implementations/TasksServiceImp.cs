using AutoMapper;
using TasksService.BusinessLogic.Services.Abstractions;
using TasksService.DataAccess.Entities;
using TasksService.DataAccess.Repositories.Abstractions;
using TasksService.BusinessLogic.DTO;
using System.ComponentModel.Design;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Xml.Linq;

namespace TasksService.BusinessLogic.Services.Implementations
{
    public class TasksServiceImp : ITasksService
    {
        private readonly IMapper _mapper;
        private readonly ITaskTemplatesRepository _templatesRepo;
        private readonly ITasksRepository _tasksRepo;
        private readonly IHistoryRepository _historyRepo;
        private readonly IUrgenciesRepository _urgenciesRepository;
        private readonly ICompanyRepository _companyRepository;
        private readonly IEmployeesRepository _employeesRepository;
        private readonly ICompanyProjectsRepository _companyProjectsRepository;
        private readonly IProjectAreaRepository _projectAreaRepository;

        public TasksServiceImp(ITaskTemplatesRepository repository, 
                                ITasksRepository tasksRepo, 
                                IHistoryRepository historyRepo, 
                                IUrgenciesRepository urgenciesRepository,
                                ICompanyRepository companyRepository,
                                IEmployeesRepository employeesRepository,
                                ICompanyProjectsRepository companyProjectsRepository,
                                IProjectAreaRepository projectAreaRepository,
                                IMapper mapper)
        {
            _mapper = mapper;
            _templatesRepo = repository;
            _tasksRepo = tasksRepo;
            _urgenciesRepository = urgenciesRepository;
            _historyRepo = historyRepo;
            _companyRepository = companyRepository;
            _employeesRepository = employeesRepository;
            _companyProjectsRepository = companyProjectsRepository;
            _projectAreaRepository = projectAreaRepository;
        }



        #region Templates
        public async Task<List<TemplateItemDTO>> GetTemplateList(long Id, long CompanyId)
        {
            try
            {
                List<TemplateItemDTO> res = _mapper.Map<List<TemplateItemDTO>>(await _templatesRepo.GetTemplateList(Id, CompanyId));
                return res;

            }
            catch (Exception ex)
            {
                throw;
            }

            //return _mapper.Map<List<TemplateItemDTO>>(await _repository.GetTemplateList(Id, CompanyId));
        }
        public async Task<long> CreateTemplate(string Name, string Description, long CompanyId, List<TemplateNodeDTO> Nodes, List<TemplateEdgeDTO> Edges)
        {
            var fwNodes = _mapper.Map<List<TemplateNodeDTO>, List<WfNodesTemplate>>(Nodes);
            var fwEdges = _mapper.Map<List<TemplateEdgeDTO>, List<WfEdgesTemplate>>(Edges);
            return await _templatesRepo.CreateTemplate(Name, Description, CompanyId, fwNodes, fwEdges);
        }
        public async Task<bool> UpdateTemplate(long Id, string Name, string Description, long CompanyId, List<TemplateNodeDTO> Nodes, List<TemplateEdgeDTO> Edges)
        {
            var fwNodes = _mapper.Map<List<TemplateNodeDTO>, List<WfNodesTemplate>>(Nodes);
            var fwEdges = _mapper.Map<List<TemplateEdgeDTO>, List<WfEdgesTemplate>>(Edges);
            return await _templatesRepo.UpdateTemplate(Id, Name, Description, CompanyId, fwNodes, fwEdges);
        }

        public async Task<bool> DeleteTemplate(long Id)
        {
            return await _templatesRepo.DeleteTemplate(Id);
        }

        #endregion Templates

        #region Companies
        public async Task<List<CompanyDTO>> GetCompanies(long id)
        {
            var comps = await _companyRepository.GetCompanies(id);
            if (null != comps)
                return _mapper.Map<List<CompanyDTO>>(comps);
            return new List<CompanyDTO>();
        }
        public async Task<long> CreateCompany(Guid creatorId, string name, string description)
        {
            return await _companyRepository.CreateCompany(creatorId, name, description);
        }
        public async Task<bool> ModifyCompany(Guid userId, long changeFlags, long id, string name, string description)
        {
            return await _companyRepository.ModifyCompany(userId, changeFlags, id, name, description);
        }

        public async Task<bool> DeleteCompany(Guid userId, long companyId)
        {
            return await _companyRepository.DeleteCompany(userId, companyId);
        }

        #endregion


        #region Employees
        public async Task<List<Guid>> GetEmployees(long companyId)
        {
            return await _employeesRepository.GetEmployees(companyId);
        }

        public async Task<bool> AddEmployee(Guid creatorId, long companyId, Guid newUserId)
        {
            return await _employeesRepository.AddEmployee(creatorId, companyId, newUserId);
        }

        public async Task<bool> RemoveEmployee(Guid deleterId, long companyId, Guid userToDelId)
        {
            return await _employeesRepository.RemoveEmployee(deleterId, companyId, userToDelId);
        }
        #endregion

        #region Projects
        public async Task<List<CompanyProjectDTO>> GetCompanyProjects(long companyId, long projectId)
        {
            try
            {
                return _mapper.Map <List< CompanyProjectDTO>>(await _companyProjectsRepository.GetCompanyProjects(companyId, projectId));
            }
            catch
            {
                throw;
            }

        }

        public async Task<long> CreateCompanyProject(Guid userId, CompanyProjectDTO companyProjectDTO)
        {
            try
            {
                return await _companyProjectsRepository.CreateCompanyProject(userId, _mapper.Map<CompanyProject>(companyProjectDTO));
            }
            catch
            {
                throw;
            }
        }

        public async Task<bool> ModifyCompanyProject(Guid userId, long changeFlags, CompanyProjectDTO projectDTO)
        {
            try
            {
                return await _companyProjectsRepository.ModifyCompanyProject(userId, changeFlags, _mapper.Map<CompanyProject>(projectDTO));
            }
            catch
            {
                throw;
            }
        }

        public async Task<bool> DeleteCompanyProject(Guid userId, long projectId)
        {
            try
            {
                return await _companyProjectsRepository.DeleteCompanyProject(userId, projectId);
            }
            catch
            {
                throw;
            }
        }

        #endregion

        #region Areas
        public async Task<List<ProjectAreaDTO>> GetProjectAreas(long projectId, long areaId)
        {
            return _mapper.Map<List<ProjectAreaDTO>>(await _projectAreaRepository.GetProjectAreas(projectId, areaId));
        }

        public async Task<long> CreateProjectArea(Guid userId, ProjectAreaDTO projectAreaDTO)
        {
            return await _projectAreaRepository.CreateProjectArea(userId, _mapper.Map<ProjectArea>(projectAreaDTO));
        }

        public async Task<bool> DeleteProjectArea(Guid userId, long areaId)
        {
            return await _projectAreaRepository.DeleteProjectArea(userId, areaId);
        }

        public async Task<bool> ModifyProjectArea(Guid userId, long changeFlags, ProjectAreaDTO projectAreaDTO)
        {
            return await _projectAreaRepository.ModifyProjectArea(userId, changeFlags, _mapper.Map<ProjectArea>(projectAreaDTO));   
        }


        #endregion


        #region Urgencies
        public async Task<List<UrgencyDTO>> GetUrgencies(long id)
        {
            try
            {
                List<Urgency> res = await _urgenciesRepository.GetUrgencies(id);
                if (null == res)
                    throw new Exception($"Приоритет c ID {id} не найден");


                return _mapper.Map<List<UrgencyDTO>>(res);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<bool> ModifyUrgency(Guid userId, long changeFlags, UrgencyDTO urgencyDTO)
        {
            try
            {
                return await _urgenciesRepository.ModifyUrgency(userId, changeFlags, _mapper.Map<Urgency>(urgencyDTO));
            }
            catch (Exception)
            {

                throw;
            }
        }
        public async Task<long> CreateUrgency(Guid userId, string name, string description)
        {
            try
            {
                return await _urgenciesRepository.CreateUrgency(userId, name, description);
            }
            catch (Exception)
            {

                throw;
            }
        }
        public async Task<bool> DeleteUrgency(Guid userId, long urgencyId)
        {
            try
            {
                return await _urgenciesRepository.DeleteUrgency(userId, urgencyId);
            }
            catch (Exception)
            {
                throw;
            }
        }



        #endregion  



        public async Task<TaskNodeDTO> GetNode(long id)
        {
            try
            {
                var res = await _tasksRepo.GetNode(id);
                if (null == res)
                    throw new Exception($"Узел c ID {id} не найден");

                return _mapper.Map<TaskNodeDTO>(res);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<List<TaskEdgeDTO>> GetFromNodeTransitions(long id)
        {
            try
            {
                var res = await _tasksRepo.GetFromNodeTransitions(id);
                if (null == res)
                    throw new Exception($"Узел c ID {id} не найден");

                return _mapper.Map<List<TaskEdgeDTO>>(res);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<List<TaskEdgeDTO>> GetToNodeTransitions(long id)
        {
            try
            {
                var res = await _tasksRepo.GetToNodeTransitions(id);
                if (null == res)
                    throw new Exception($"Узел c ID {id} не найден");

                return _mapper.Map<List<TaskEdgeDTO>>(res);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<bool> AppointNodeDoers(Guid userId, long nodeId, List<Guid> list)
        {
            try
            {
                return await _tasksRepo.AppointNodeDoers(userId, nodeId, list);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<long> CreateTask(Guid userId, CreateTaskDTO taskToCreate)
        {
            try
            {
                return await _tasksRepo.CreateTask(userId, _mapper.Map<DataAccess.Entities.Task>(taskToCreate));
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<List<TaskDTO>> GetTasksList(Guid userId, long companyId, long projectId, long areaId)
        {
            try
            {
                var res = await _tasksRepo.GetTasksList(userId, companyId, projectId, areaId);
                if (null == res)
                    throw new Exception($"Ошибка при получении списка задач из базы");

                return _mapper.Map<List<TaskDTO>>(res);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<FullTaskInfoDTO> GetTask(long taskId)
        {
            try
            {
                var res = await _tasksRepo.GetTask(taskId);
                if (null == res.nodes)
                    throw new Exception($"Ошибка при получении списка задач из базы");
                var nodes = _mapper.Map<List<TaskNodeDTO>>(res.nodes);
                var edges = _mapper.Map<List<TaskEdgeDTO>>(res.edges);
                var dtoTask = _mapper.Map<FullTaskInfoDTO>(res);
                dtoTask.Nodes = _mapper.Map <List<TaskNodeDTO>> (nodes);
                dtoTask.Edges = _mapper.Map<List<TaskEdgeDTO>>(edges); ;
                return dtoTask;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<bool> ModifyTaskUrgency(Guid userId, long taskId, long urgId)
        {
            try
            {
                return await _tasksRepo.ModifyTaskUrgency(userId, taskId, urgId);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<bool> ModifyTaskState(Guid userId, long taskId, long toNodeId)
        {
            try
            {
                return await _tasksRepo.ModifyTaskState(userId, taskId, toNodeId);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<bool> ModifyTaskName(Guid userId, long taskId, string newName)
        {
            try
            {
                return await _tasksRepo.ModifyTaskName(userId, taskId, newName);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<bool> ModifyTaskDescription(Guid userId, long taskId, string newDescription)
        {
            {
                try
                {
                    return await _tasksRepo.ModifyTaskDescription(userId, taskId, newDescription);
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }
        public async Task<bool> ModifyTaskNodeDeadline(Guid userId, long taskId, long NodeId, DateTime newDeadline)
        {
            {
                try
                {
                    return await _tasksRepo.ModifyTaskNodeDeadline(userId, taskId, NodeId, newDeadline);
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }




        public async Task<bool> DeleteTask(Guid userId, long taskId)
        {
            try
            {
                return await _tasksRepo.DeleteTask(userId, taskId);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<List<TaskHistoryDTO>> GetTaskHistory(long id)
        {
            try
            {
                var res = await _historyRepo.GetTaskHistory(id);
                if (null == res)
                    throw new Exception($"Ошибка при получении истории задачи из базы");

                return _mapper.Map< List<TaskHistoryDTO>>(res);
            }
            catch (Exception)
            {
                throw;
            }

        }
    }
}
