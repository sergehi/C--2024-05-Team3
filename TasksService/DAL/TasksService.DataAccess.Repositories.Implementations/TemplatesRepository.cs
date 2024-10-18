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

namespace TasksService.DataAccess.Repositories.Implementations
{
    public class TemplatesRepository : ITasksRepository
    {
        private readonly IConfiguration _configuration;

        public TemplatesRepository(IConfiguration configuration)
        {
            _configuration = configuration;
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

        public async Task<bool> UpdateTemplate(long Id, string Name, string Description, long CompanyId, List<WfNodesTemplate> Nodes, List<WfEdgesTemplate> Edges)
        {
            try
            {
                WfDefinitionsTemplate newRec = new WfDefinitionsTemplate()
                {
                    Id = Id,
                    CompanyId = CompanyId,
                    Description = Description ?? "",
                    Name = Name ?? "",
                };
                long id = await createOrUpdateTempl(newRec, Nodes, Edges);
                return id != 0;
            }
            catch (Exception ex)
            {
                throw new RpcExceptionEx(new Status(StatusCode.Cancelled, ex.Message), $"Произошла ошибка при изменении записи сервере\n{ex.Message}", ex.StackTrace);
            }

        }

        public async Task<long> CreateTemplate(string Name, string Description, long CompanyId, List<WfNodesTemplate> Nodes, List<WfEdgesTemplate> Edges)
        {
            try
            {
                WfDefinitionsTemplate newRec = new WfDefinitionsTemplate()
                {
                    Id = 0,
                    CompanyId = CompanyId,
                    Description = Description ?? "",
                    Name = Name ?? "",
                };
                long id = await createOrUpdateTempl(newRec, Nodes, Edges);
                return id;
            }
            catch (Exception ex)
            {
                throw new RpcExceptionEx(new Status(StatusCode.Cancelled, ex.Message), $"Произошла ошибка при добавлении записи сервере\n{ex.Message}", ex.StackTrace);
            }
        }

        public async Task<bool> DeleteTemplate(long Id)
        {
            try
            {
                using (var dbContext = new TasksDbContext(_configuration))
                {
                    var definition = dbContext.WfdefinitionsTempls.Find(Id);
                    if (definition != null)
                        dbContext.WfdefinitionsTempls.Remove(definition);
                    await dbContext.SaveChangesAsync();
                }
                return true;
            }
            catch (Exception ex)
            {
                throw new RpcExceptionEx(new Status(StatusCode.Cancelled, ex.Message), $"Произошла ошибка при удалении записи сервере\n{ex.Message}", ex.StackTrace);
            }
        }


        private async Task<long> createOrUpdateTempl(WfDefinitionsTemplate rec, List<WfNodesTemplate> Nodes, List<WfEdgesTemplate> Edges)
        {
            var result = 0L;
            try
            {
                using (var dbContext = new TasksDbContext(_configuration))
                {
                    using var transaction = dbContext.Database.BeginTransaction();
                    try
                    {
                        if (rec.Id != 0)
                        {
                            dbContext.WfdefinitionsTempls.Attach(rec);
                            dbContext.Entry(rec).State = EntityState.Modified;
                        }
                        else
                            dbContext.WfdefinitionsTempls.Add(rec);


                        await dbContext.SaveChangesAsync();
                        result = rec.Id;

                        List<WfNodesTemplate> temp_nodes = new List<WfNodesTemplate>();
                        foreach (var wfNode in Nodes)
                        {
                            //var wfNode = WfMapper.messageNodeToWfnodesTempl(node);
                            if (null != wfNode)
                            {
                                wfNode.DefinitionId = rec.Id;

                                if (wfNode.Id != 0)
                                {
                                    dbContext.WfnodesTempls.Attach(wfNode);
                                    dbContext.Entry(wfNode).State = EntityState.Modified;
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
                                        dbContext.WfedgesTempls.Attach(fwEdge);
                                        dbContext.Entry(fwEdge).State = EntityState.Modified;
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
