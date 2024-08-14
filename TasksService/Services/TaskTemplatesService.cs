using Grpc.Core;
using Microsoft.EntityFrameworkCore;
using TasksService.Models;
using TasksTemplatesService;
using TasksService.Models.WfTemplates;
using System.Diagnostics;
using Common.Rpc;

namespace TasksService.Services
{
    public class TaskTemplatesService : TaskTemplates.TaskTemplatesBase
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<TaskTemplatesService> _logger;
        public TaskTemplatesService(IConfiguration configuration, ILogger<TaskTemplatesService> logger)
        {
            _logger = logger;
            _configuration = configuration;
        }


        public async override Task<TemplateListReply> GetTemplateList(TemplateListRequest request, ServerCallContext context)
        {
            var res = new TemplateListReply();
            try
            {
                using (var dbContext = new TasksDbContext(_configuration))
                {
                    var query = dbContext.WfdefinitionsTempls.AsQueryable();
                    if (request.Id != 0)
                        query = query.Where(x => x.Id == request.Id);
                    if (request.CompanyId != 0)
                        query = query.Where(x => x.CompanyId == request.CompanyId);
                    var result = await query.ToListAsync();

                    res.Items.AddRange(result.Select(x => new TemplateListItem() { Id = x.Id, Description = x.Description ?? "", Name = x.Name ?? "" }).ToList());
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при выполнении запроса: {Request}", request);
                throw new RpcExceptionEx(new Status(StatusCode.Cancelled, ex.Message), "Произошла ошибка при выполнении запроса на сервере", ex.StackTrace);
            }
            return res;
        }

        public async override Task<CreateTemplateReply> CreateTemplate(CreateTemplateRequest request, ServerCallContext context)
        {
            var res = new CreateTemplateReply();
            try
            {
                using (var dbContext = new TasksDbContext(_configuration))
                {
                    WfdefinitionsTempl newRec = new WfdefinitionsTempl()
                    {
                        CompanyId = request.CompanyId,
                        Description = request.Description ?? "",
                        Name = request.Name ?? "",
                    };
                    await dbContext.SaveChangesAsync();
                    res.Id = newRec.Id;

                    List<WfnodesTempl> temp_nodes = new List<WfnodesTempl>();
                    foreach (var node in request.Nodes)
                    {
                        var wfNode = WfMapper.messageNodeToWfnodesTempl(node);
                        if (null != wfNode)
                        {
                            if (wfNode.Id != 0)
                                dbContext.WfnodesTempls.Attach(wfNode);
                            else
                                dbContext.WfnodesTempls.Add(wfNode);
                            temp_nodes.Add(wfNode);
                        }
                    }
                    await dbContext.SaveChangesAsync();

                    foreach (var edge in request.Edges)
                    {
                        WfedgesTempl fwEdge = WfMapper.messageEdgeToWfedgesTempl(edge);
                        var foundFrom = temp_nodes.FirstOrDefault(x => x.InternalNum == fwEdge.NodeFrom);
                        if (null != foundFrom)
                        {
                            var foundTo = temp_nodes.FirstOrDefault(x => x.InternalNum == fwEdge.NodeTo);
                            if (null != foundTo)
                            {
                                Debug.Assert(foundFrom.Id != 0 || foundTo.Id != 0);
                                fwEdge.NodeFrom = foundFrom.Id;
                                fwEdge.NodeTo = foundTo.Id;
                                if (fwEdge.Id != 0)
                                    dbContext.WfedgesTempls.Attach(fwEdge);
                                else
                                    dbContext.WfedgesTempls.Add(fwEdge);
                            }
                        }
                    }
                    await dbContext.SaveChangesAsync();

                    return res;
                }
            }
            catch (Exception ex)
            {
                throw new RpcExceptionEx(new Status(StatusCode.Cancelled, ex.Message), $"Произошла ошибка при добавлении записи сервере\n{ex.Message}", ex.StackTrace);
            }
        }

        public async override Task<BoolReply> UpdateTemplate(UpdateTemplateRequest request, ServerCallContext context)
        {
            try
            {
                using (var dbContext = new TasksDbContext(_configuration))
                {
                }
                return new BoolReply() { Success = true };
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
                using (var dbContext = new TasksDbContext(_configuration))
                {
                    var definition = dbContext.WfdefinitionsTempls.Find(request.Id);
                    if (definition != null)
                        dbContext.WfdefinitionsTempls.Remove(definition);
                    await dbContext.SaveChangesAsync();
                }
                return new BoolReply() { Success = true };
            }
            catch (Exception ex)
            {
                throw new RpcExceptionEx(new Status(StatusCode.Cancelled, ex.Message), $"Произошла ошибка при удалении записи сервере\n{ex.Message}", ex.StackTrace);
            }
        }



    }
}
