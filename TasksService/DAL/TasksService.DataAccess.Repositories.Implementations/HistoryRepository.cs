﻿using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Common.Rpc;
using Grpc.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using TasksService.DataAccess.Entities;
using TasksService.DataAccess.EntityFramework;
using TasksService.DataAccess.Repositories.Abstractions;
using static NpgsqlTypes.NpgsqlTsQuery;

namespace TasksService.DataAccess.Repositories.Implementations
{
    public class HistoryRepository : IHistoryRepository
    {
        private readonly IConfiguration _configuration;
        public HistoryRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<List<TaskHistory>> GetTaskHistory(long TaskId)
        {
            try
            {
                var prjs = new List<TaskHistory>();
                using (var dbContext = new TasksDbContext(_configuration))
                {
                    if (TaskId != 0)
                        return await dbContext.TaskHistories.Where(e => e.TaskId == TaskId).ToListAsync();
                    throw new Exception("Не указан идентификатор задачи");
                }
            }
            catch (Exception ex)
            {
                throw new RpcExceptionEx(new Status(StatusCode.Cancelled, ex.Message), "Произошла ошибка при попытке получения истории изменений задачи", ex.StackTrace ?? "");
            }
        }

        public async Task<bool> RegisterTaskUrgencyChanged(long UserId, long TaskId, string OldValue, string NewValue)
        {
            return await registerTaskChange(UserId, TaskId, 0, ETaskModifyFlags.TMF_MODIFY_URGENCY, OldValue, NewValue);
        }
        public async Task<bool> RegisterTaskTypeChanged(long UserId, long TaskId, string OldValue, string NewValue)
        {
            return await registerTaskChange(UserId, TaskId, 0, ETaskModifyFlags.TMF_MODIFY_TYPE, OldValue, NewValue);
        }

        public async Task<bool> RegisterTaskStateChanged(long UserId, long TaskId, string OldValue, string NewValue)
        {
            return await registerTaskChange(UserId, TaskId, 0, ETaskModifyFlags.TMF_MODIFY_STATE, OldValue, NewValue);
        }

        public async Task<bool> RegisterTaskNameChanged(long UserId, long TaskId, string OldValue, string NewValue)
        {
            return await registerTaskChange(UserId, TaskId, 0, ETaskModifyFlags.TMF_MODIFY_NAME, OldValue, NewValue);
        }

        public async Task<bool> RegisterTaskDescriptionChanged(long UserId, long TaskId, string OldValue, string NewValue)
        {
            return await registerTaskChange(UserId, TaskId, 0, ETaskModifyFlags.TMF_MODIFY_DESCRIPTION, OldValue, NewValue);
        }

        public async Task<bool> RegisterTaskDeadlineChanged(long UserId, long TaskId, long NodeId, string OldValue, string NewValue)
        {
            return await registerTaskChange(UserId, TaskId, NodeId, ETaskModifyFlags.TMF_MODIFY_DEADLINE, OldValue, NewValue);
        }

        public async Task<bool> RegisterTaskDoersAppointed(long UserId, long TaskId, long NodeId, string OldValue, string NewValue)
        {
            return await registerTaskChange(UserId, TaskId, NodeId, ETaskModifyFlags.TMF_REC_DOERS, OldValue, NewValue);
        }

        public async Task<bool> RegisterDeleteTask(long UserId, long TaskId, string OldValue, string NewValue)
        {
            return await registerTaskChange(UserId, TaskId, 0, ETaskModifyFlags.TMF_REC_DELETE, OldValue, NewValue);
        }

        public async Task<bool> RegisterCreateTask(long UserId, long TaskId, string NewValue)
        {
            return await registerTaskChange(UserId, TaskId, 0, ETaskModifyFlags.TMF_REC_CREATE, string.Empty, NewValue);
        }

        public async Task<bool> registerTaskChange(long UserId, long TaskId, long NodeId, ETaskModifyFlags ChangeFlag, string OldValue, string NewValue)
        {
            try
            {
                if (ChangeFlag == ETaskModifyFlags.TMF_NONE)
                    return true;

                var taskHistory = new TaskHistory()
                {
                    TaskId = TaskId,
                    NodeId = NodeId,
                    UserId = UserId,
                    ActionValue = NewValue,
                    OldValue = OldValue
                };
                using (var dbContext = new TasksDbContext(_configuration))
                {
                    var action = await dbContext.TaskActions.FirstOrDefaultAsync(t => t.ActionId == (long)ChangeFlag);
                    if (action == null)
                        throw new Exception($"Не найдено описание действия \"{ChangeFlag}\" в базе данных");

                    taskHistory.ActionId = action.ActionId;
                    dbContext.TaskHistories.Add(taskHistory);
                    await dbContext.SaveChangesAsync();
                }
                return true;
            }
            catch (Exception ex)
            {
                throw new RpcExceptionEx(new Status(StatusCode.Cancelled, ex.Message), "Произошла ошибка при попытке регистрации действий для истории", ex.StackTrace ?? "");
            }
        }
    }
}