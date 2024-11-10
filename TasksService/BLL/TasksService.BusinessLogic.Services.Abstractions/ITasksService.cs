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
        Task<long> CreateTemplate(string Name, string Description, long CompanyId, List<TemplateNodeDTO> Nodes, List<TemplateEdgeDTO> Edges);
        // Обновить шаблон
        Task<bool> UpdateTemplate(long Id, string Name, string Description, long CompanyId, List<TemplateNodeDTO> Nodes, List<TemplateEdgeDTO> Edges);
        // Удалить шаблон
        Task<bool> DeleteTemplate(long Id);

        // Tasks
        // Получить список задач для пользователя userId из компании companyId для проекта projectId(если == 0 - все проекты) для направления работ areaId(если == 0 - все )
        // Результата - обрезаная структура(без информации об узлах и переходах) для вывода в списках
        Task<List<TaskDTO>> GetTasksList(long userId, long companyId, long projectId, long areaId);
        // Получить полную информацию о задаче
        Task<FullTaskInfoDTO> GetTask(long taskId);
        // Создать задачу
        Task<long> CreateTask(long userId, CreateTaskDTO taskToCreate);
        // Модифицировать задачу
        Task<bool> ModifyTaskUrgency(long userId, long taskId, long urgId);
        Task<bool> ModifyTaskState(long userId, long taskId, long toNodeId);
        Task<bool> ModifyTaskName(long userId, long taskId, string newName);
        Task<bool> ModifyTaskDescription(long userId, long taskId, string newDescription);
        Task<bool> ModifyTaskNodeDeadline(long userId, long taskId, long NodeId, DateTime newDeadline);


        // Удалить задачу
        Task<bool> DeleteTask(long userId, long taskId);
        // Получить историю изменений задачи
        Task<List<TaskHistoryDTO>> GetTaskHistory(long id);



        // TaskRoutes
        // Получить все пути из/в нод с указанным Id 
        Task<List<TaskEdgeDTO>> GetFromNodeTransitions(long id);
        Task<List<TaskEdgeDTO>> GetToNodeTransitions(long id);

        // Получить информацию о ноде
        Task<TaskNodeDTO> GetNode(long id);
        // Назначить исполнителей на узел с идентификатором nodeId
        Task<bool> AppointNodeDoers(long userId, long nodeId, List<long> list);
        
        // Срочность
        Task<List<UrgencyDTO>> GetUrgencies(long id);
        Task<long> CreateUrgency(long userId, string name, string description);
        Task<bool> ModifyUrgency(long userId, long changeFlags, UrgencyDTO urgencyDTO);
        Task<bool> DeleteUrgency(long userId, long urgencyId);
        
        // Project Areas - направления работ внутри проекта
        /// <summary>
        /// Получить направления работ
        /// </summary>
        /// <param name="projectId">если не 0, то выбрать для определенного проекта, иначе для всех проектов</param>
        /// <param name="areaId">если не 0, то выбрать определенное направление, иначе все</param>
        /// <returns></returns>
        Task<List<ProjectAreaDTO>> GetProjectAreas(long projectId, long areaId);
        Task<long> CreateProjectArea(long userId, ProjectAreaDTO projectAreaDTO);
        Task<bool> ModifyProjectArea(long userId, long changeFlags, ProjectAreaDTO projectAreaDTO);
        Task<bool> DeleteProjectArea(long userId, long areaId);
        
        // Company projects
        Task<List<CompanyProjectDTO>> GetCompanyProjects(long companyId, long projectId);
        Task<long> CreateCompanyProject(long userId, CompanyProjectDTO companyProjectDTO);
        Task<bool> ModifyCompanyProject(long userId, long changeFlags, CompanyProjectDTO projectDTO);
        Task<bool> DeleteCompanyProject(long userId, long projectId);
        
        // Companies
        Task<List<CompanyDTO>> GetCompanies(long id);
        Task<long> CreateCompany(long creatorId, string name, string description);
        Task<bool> ModifyCompany(long userId, long changeFlags, long id, string name, string description);
        Task<bool> DeleteCompany(long userId, long companyId);
        
        // Employees
        Task<List<long>> GetEmployees(long companyId);
        Task<bool> AddEmployee(long creatorId, long companyId, long newUserId);
        Task<bool> RemoveEmployee(long deleterId, long companyId, long userToDelId);
    }

}
