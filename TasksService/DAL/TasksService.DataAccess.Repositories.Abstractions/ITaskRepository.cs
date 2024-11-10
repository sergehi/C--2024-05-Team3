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
        Task<List<Entities.Task>> GetTasksList(long userId, long companyId, long projectId, long areaId);
        // Получить полную информацию о задаче
        Task<Entities.Task> GetTask(long taskId);
        // Создать задачу
        Task<long> CreateTask(long userId, Entities.Task taskToCreate);
        // Удалить задачу
        Task<bool> DeleteTask(long userId, long taskId);

        // TaskRoutes
        // Получить все пути из/в нод с указанным Id 
        Task<List<TaskEdge>> GetFromNodeTransitions(long nodeId);
        Task<List<TaskEdge>> GetToNodeTransitions(long nodeId);
        // Получить информацию о ноде
        Task<TaskNode> GetNode(long id);

        // Modifications
        Task<bool> ModifyTaskUrgency(long userId, long taskId, long urgId);
        //Task<bool> ModifyTaskType(long userId, long taskId, long typeId);
        Task<bool> ModifyTaskState(long userId, long taskId, long toNodeId);
        Task<bool> ModifyTaskName(long userId, long taskId, string newName);
        Task<bool> ModifyTaskDescription(long userId, long taskId, string newDescription);
        Task<bool> ModifyTaskNodeDeadline(long userId, long taskId, long NodeId, DateTime newDeadline);
        // Назначить исполнителей на узел с идентификатором nodeId
        Task<bool> AppointNodeDoers(long userId, long nodeId, List<long> list);


    }
}
