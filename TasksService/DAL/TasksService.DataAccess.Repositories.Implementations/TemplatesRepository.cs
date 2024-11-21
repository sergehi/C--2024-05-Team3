//using System.Diagnostics;
using Common.Rpc;
using Grpc.Core;
using Microsoft.Extensions.Configuration;
using TasksService.DataAccess.Entities;
using TasksService.DataAccess.EntityFramework;
using TasksService.DataAccess.Repositories.Abstractions;
using Microsoft.EntityFrameworkCore;
using System.Xml.Linq;
using Common.Repositories;
using Common;
using System.Threading.Tasks;

namespace TasksService.DataAccess.Repositories.Implementations
{
    public class TemplatesRepository : ITaskTemplatesRepository
    {
        private readonly IConfiguration _configuration;
        private readonly IHistoryRepository _historyRepo;

        public TemplatesRepository(IConfiguration configuration, IHistoryRepository historyRepo)
        {
            _configuration = configuration;
            _historyRepo = historyRepo;
        }

        public async Task<List<WfDefinitionsTemplate>> GetTemplateList(long Id, long CompanyId)
        {
            var res = new List<WfDefinitionsTemplate>();
            try
            {
                using (var dbContext = new TasksDbContext(_configuration))
                {
                    var query = dbContext.WfdefinitionsTempls
                         .Include(x => x.WfnodesTempls)
                            .ThenInclude(y=> y.WfedgesTemplNodeFromNavigations)
                         .Include(x => x.WfnodesTempls)
                            .ThenInclude(node => node.WfedgesTemplNodeToNavigations)
                         .AsQueryable();
                    if (Id != 0)
                        query = query.Where(x => x.Id == Id);
                    if (CompanyId != 0)
                        query = query.Where(x => x.CompanyId == CompanyId);
                    var result = await query.ToListAsync();

                    res.AddRange(result);
                }
            }
            catch (Exception ex)
            {
                //_logger.LogError(ex, "Ошибка при выполнении запроса: {Request}", request);
                throw new RpcExceptionEx(new Status(StatusCode.Cancelled, ex.Message), "Произошла ошибка при выполнении запроса на сервере", ex.StackTrace);
            }
            return res;

        }

        public async Task<bool> UpdateTemplate(long userId, long templId, string Name, string Description, long CompanyId, List<WfNodesTemplate> Nodes, List<WfEdgesTemplate> Edges)
        {
            try
            {
                long id = await updateTemplate(userId, templId, Name, Description, CompanyId, Nodes, Edges);
                return id != 0;
            }
            catch (Exception ex)
            {
                throw new RpcExceptionEx(new Status(StatusCode.Cancelled, ex.Message), $"Произошла ошибка при изменении записи сервере\n{ex.Message}", ex.StackTrace);
            }

        }

        public async Task<long> CreateTemplate(long userId, string Name, string Description, long CompanyId, List<WfNodesTemplate> Nodes, List<WfEdgesTemplate> Edges)
        {
            try
            {
                long id = await createTemplate(userId, Name, Description, CompanyId, Nodes, Edges);
                return id;
            }
            catch (Exception ex)
            {
                throw new RpcExceptionEx(new Status(StatusCode.Cancelled, ex.Message), $"Произошла ошибка при добавлении записи сервере\n{ex.Message}", ex.StackTrace);
            }
        }

        public async Task<bool> DeleteTemplate(long userId, long templId)
        {
            try
            {
                using (var dbContext = new TasksDbContext(_configuration))
                {
                    var definition = dbContext.WfdefinitionsTempls.Find(templId);
                    if (definition != null)
                        dbContext.WfdefinitionsTempls.Remove(definition);
                    await dbContext.SaveChangesAsync();
                    if (null != definition)
                        RabbitMQService<WfDefinitionsTemplate>.SendToRabbit(definition, LoggerService.ELogAction.LaDelete, userId.ToString());
                }
                return true;
            }
            catch (Exception ex)
            {
                throw new RpcExceptionEx(new Status(StatusCode.Cancelled, ex.Message), $"Произошла ошибка при удалении записи сервере\n{ex.Message}", ex.StackTrace);
            }
        }


        private async Task<long> createTemplate(long userId, string Name, string Description, long CompanyId, List<WfNodesTemplate> Nodes, List<WfEdgesTemplate> Edges)
        {
            try
            {
                using (var dbContext = new TasksDbContext(_configuration))
                {
                    using var transaction = dbContext.Database.BeginTransaction();
                    try
                    {
                        var templateRecord = new WfDefinitionsTemplate()
                        {
                            CompanyId = CompanyId,
                            Description = Description ?? "",
                            Name = Name ?? ""
                        };
                        foreach (var node in Nodes) 
                        {
                            var  edges_from = Edges.Where(x => x.NodeFrom == node.InternalNum);
                            var edges_to = Edges.Where(x => x.NodeTo == node.InternalNum);
                            foreach(var edge in edges_from)
                                node.WfedgesTemplNodeFromNavigations.Add(edge);
                            foreach (var edge in edges_to)
                                node.WfedgesTemplNodeToNavigations.Add(edge);

                            templateRecord.WfnodesTempls.Add(node);
                        }
                        dbContext.WfdefinitionsTempls.Add(templateRecord);
                        await dbContext.SaveChangesAsync();
                        transaction.Commit();
                        RabbitMQService<WfDefinitionsTemplate>.SendToRabbit(templateRecord, LoggerService.ELogAction.LaCreate, userId.ToString());
                        return templateRecord.Id;

                    }
                    catch (Exception)
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }

        }


        private async Task<long> updateTemplate(long userId, long Id, string Name, string Description, long CompanyId, List<WfNodesTemplate> Nodes, List<WfEdgesTemplate> Edges)
        {
            try
            {
                using (var dbContext = new TasksDbContext(_configuration))
                {
                    using var transaction = dbContext.Database.BeginTransaction();
                    try
                    {
                        var templateRecord = new WfDefinitionsTemplate()
                        {
                            Id = Id,
                            CompanyId = CompanyId,
                            Description = Description ?? "",
                            Name = Name ?? ""
                        };
                        templateRecord.WfnodesTempls.Clear();
                        foreach (var node in Nodes)
                        {
                            var edges_from = Edges.Where(x => x.NodeFrom == node.InternalNum);
                            var edges_to = Edges.Where(x => x.NodeTo == node.InternalNum);
                            foreach (var edge in edges_from)
                                node.WfedgesTemplNodeFromNavigations.Add(edge);
                            foreach (var edge in edges_to)
                                node.WfedgesTemplNodeToNavigations.Add(edge);

                            templateRecord.WfnodesTempls.Add(node);
                        }

                        dbContext.WfdefinitionsTempls.Update(templateRecord);
                        await dbContext.SaveChangesAsync();
                        transaction.Commit();

                        RabbitMQService<WfDefinitionsTemplate>.SendToRabbit(templateRecord, LoggerService.ELogAction.LaUpdate, userId.ToString());
                        return templateRecord.Id;

                    }
                    catch (Exception)
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }

        }



        private async Task<long> createOrUpdateTempl(long Id, string Name, string Description, long CompanyId, List<WfNodesTemplate> Nodes, List<WfEdgesTemplate> Edges)
        {
            var result = 0L;
            var templateRecord = new WfDefinitionsTemplate()
            {
                Id = Id,
                CompanyId = CompanyId,
                Description = Description ?? "",
                Name = Name ?? "",
            };

            try
            {
                using (var dbContext = new TasksDbContext(_configuration))
                {
                    using var transaction = dbContext.Database.BeginTransaction();
                    try
                    {
                        if (templateRecord.Id != 0)
                        {
                            templateRecord = await dbContext.WfdefinitionsTempls.FirstOrDefaultAsync(x => x.Id == Id);
                            if (null == templateRecord)
                                throw new Exception($"Обновляемая запись шаблона с идентификатором {Id} в базе не найдена");

                            templateRecord.CompanyId = CompanyId;
                            templateRecord.Name = Name;
                            templateRecord.Description = Description;

                            dbContext.WfdefinitionsTempls.Update(templateRecord);
                            //dbContext.WfdefinitionsTempls.Attach(rec);
                            //dbContext.Entry(rec).State = EntityState.Modified;
                        }
                        else
                        {

                            dbContext.WfdefinitionsTempls.Add(templateRecord);
                        }
                        await dbContext.SaveChangesAsync();
                        result = templateRecord.Id;


                        List<WfNodesTemplate> temp_nodes = new List<WfNodesTemplate>();
                        foreach (var wfNode in Nodes)
                        {
                            //var wfNode = WfMapper.messageNodeToWfnodesTempl(node);
                            if (null != wfNode)
                            {
                                wfNode.DefinitionId = templateRecord.Id;

                                if (wfNode.Id != 0)
                                {
                                    dbContext.WfnodesTempls.Update(wfNode);
                                    //dbContext.WfnodesTempls.Attach(wfNode);
                                    //dbContext.Entry(wfNode).State = EntityState.Modified;
                                }
                                else
                                    dbContext.WfnodesTempls.Add(wfNode);

                                temp_nodes.Add(wfNode);
                            }
                        }
                        await dbContext.SaveChangesAsync();

                        foreach (var fwEdge in Edges)
                        {
                            //WfedgesTempl fwEdge = WfMapper.messageEdgeToWfedgesTempl(edge);
                            var foundFrom = temp_nodes.FirstOrDefault(x => x.InternalNum == fwEdge.NodeFrom);
                            if (null != foundFrom)
                            {
                                var foundTo = temp_nodes.FirstOrDefault(x => x.InternalNum == fwEdge.NodeTo);
                                if (null != foundTo)
                                {
                                    //Debug.Assert(foundFrom.Id != 0 || foundTo.Id != 0);
                                    fwEdge.NodeFrom = foundFrom.Id;
                                    fwEdge.NodeTo = foundTo.Id;
                                    if (fwEdge.Id != 0)
                                    {
                                        //dbContext.WfedgesTempls.Attach(fwEdge);
                                        //dbContext.Entry(fwEdge).State = EntityState.Modified;
                                        dbContext.WfedgesTempls.Update(fwEdge);

                                    }
                                    else
                                        dbContext.WfedgesTempls.Add(fwEdge);
                                }
                            }
                        }
                        await dbContext.SaveChangesAsync();
                        transaction.Commit();
                        return result;

                    }
                    catch (Exception)
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
