using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TasksService.DataAccess.Entities;

namespace TasksService.DataAccess.Repositories.Abstractions
{
    public interface ITasksRepository
    {
        Task<List<Entities.Task>> GetTasksList(Guid userId, long companyId, long projectId, long areaId);
        // Получить полную информацию о задаче
        Task<(Entities.Task task, List<TaskNode> nodes, List<TaskEdge> edges)> GetTask(long taskId);
        // Создать задачу
        Task<long> CreateTask(Guid userId, Entities.Task taskToCreate);
        // Удалить задачу
        Task<bool> DeleteTask(Guid userId, long taskId);

        // TaskRoutes
        // Получить все пути из/в нод с указанным Id 
        Task<List<TaskEdge>> GetFromNodeTransitions(long nodeId);
        Task<List<TaskEdge>> GetToNodeTransitions(long nodeId);
        // Получить информацию о ноде
        Task<TaskNode> GetNode(long id);

        // Modifications
        Task<bool> ModifyTaskUrgency(Guid userId, long taskId, long urgId);
        Task<bool> ModifyTaskState(Guid userId, long taskId, long toNodeId);
        Task<bool> ModifyTaskName(Guid userId, long taskId, string newName);
        Task<bool> ModifyTaskDescription(Guid userId, long taskId, string newDescription);
        Task<bool> ModifyTaskNodeDeadline(Guid userId, long taskId, long NodeId, DateTime newDeadline);
        // Назначить исполнителей на узел с идентификатором nodeId
        Task<bool> AppointNodeDoers(Guid userId, long nodeId, List<Guid> list);


    }
}
