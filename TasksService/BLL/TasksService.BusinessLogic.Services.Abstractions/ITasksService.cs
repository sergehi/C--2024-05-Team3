using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TasksService.BusinessLogic.DTO;

namespace TasksService.BusinessLogic.Services.Abstractions
{
    public interface ITasksService
    {
        // Task templates
        // Получить список шаблонов
        Task<List<TemplateItemDTO>> GetTemplateList(long Id, long CompanyId);
        // Создать шаблон
        Task<long> CreateTemplate(Guid userId, string Name, string Description, long CompanyId, List<TemplateNodeDTO> Nodes, List<TemplateEdgeDTO> Edges);
        // Обновить шаблон
        Task<bool> UpdateTemplate(Guid userId, long Id, string Name, string Description, long CompanyId, List<TemplateNodeDTO> Nodes, List<TemplateEdgeDTO> Edges);
        // Удалить шаблон
        Task<bool> DeleteTemplate(Guid userId, long Id);

        // Tasks
        // Получить список задач для пользователя userId из компании companyId для проекта projectId(если == 0 - все проекты) для направления работ areaId(если == 0 - все )
        // Результата - обрезаная структура(без информации об узлах и переходах) для вывода в списках
        Task<List<TaskDTO>> GetTasksList(Guid userId, long companyId, long projectId, long areaId);
        // Получить полную информацию о задаче
        Task<FullTaskInfoDTO> GetTask(long taskId);
        // Создать задачу
        Task<long> CreateTask(Guid userId, CreateTaskDTO taskToCreate);
        // Модифицировать задачу
        Task<bool> ModifyTaskUrgency(Guid userId, long taskId, long urgId);
        Task<bool> ModifyTaskState(Guid userId, long taskId, long toNodeId);
        Task<bool> ModifyTaskName(Guid userId, long taskId, string newName);
        Task<bool> ModifyTaskDescription(Guid userId, long taskId, string newDescription);
        Task<bool> ModifyTaskNodeDeadline(Guid userId, long taskId, long NodeId, DateTime newDeadline);


        // Удалить задачу
        Task<bool> DeleteTask(Guid userId, long taskId);
        // Получить историю изменений задачи
        Task<List<TaskHistoryDTO>> GetTaskHistory(long id);



        // TaskRoutes
        // Получить все пути из/в нод с указанным Id 
        Task<List<TaskEdgeDTO>> GetFromNodeTransitions(long id);
        Task<List<TaskEdgeDTO>> GetToNodeTransitions(long id);

        // Получить информацию о ноде
        Task<TaskNodeDTO> GetNode(long id);
        // Назначить исполнителей на узел с идентификатором nodeId
        Task<bool> AppointNodeDoers(Guid userId, long nodeId, List<Guid> list);
        
        // Срочность
        Task<List<UrgencyDTO>> GetUrgencies(long id);
        Task<long> CreateUrgency(Guid userId, string name, string description);
        Task<bool> ModifyUrgency(Guid userId, long changeFlags, UrgencyDTO urgencyDTO);
        Task<bool> DeleteUrgency(Guid userId, long urgencyId);
        
        // Project Areas - направления работ внутри проекта
        /// <summary>
        /// Получить направления работ
        /// </summary>
        /// <param name="projectId">если не 0, то выбрать для определенного проекта, иначе для всех проектов</param>
        /// <param name="areaId">если не 0, то выбрать определенное направление, иначе все</param>
        /// <returns></returns>
        Task<List<ProjectAreaDTO>> GetProjectAreas(long projectId, long areaId);
        Task<long> CreateProjectArea(Guid userId, ProjectAreaDTO projectAreaDTO);
        Task<bool> ModifyProjectArea(Guid userId, long changeFlags, ProjectAreaDTO projectAreaDTO);
        Task<bool> DeleteProjectArea(Guid userId, long areaId);
        
        // Company projects
        Task<List<CompanyProjectDTO>> GetCompanyProjects(long companyId, long projectId);
        Task<long> CreateCompanyProject(Guid userId, CompanyProjectDTO companyProjectDTO);
        Task<bool> ModifyCompanyProject(Guid userId, long changeFlags, CompanyProjectDTO projectDTO);
        Task<bool> DeleteCompanyProject(Guid userId, long projectId);
        
        // Companies
        Task<List<CompanyDTO>> GetCompanies(long id);
        Task<long> CreateCompany(Guid creatorId, string name, string description);
        Task<bool> ModifyCompany(Guid userId, long changeFlags, long id, string name, string description);
        Task<bool> DeleteCompany(Guid userId, long companyId);
        
        // Employees
        Task<List<Guid>> GetEmployees(long companyId);
        Task<bool> AddEmployee(Guid creatorId, long companyId, Guid newUserId);
        Task<bool> RemoveEmployee(Guid deleterId, long companyId, Guid userToDelId);
    }

}
