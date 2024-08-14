using TasksTemplatesService;

namespace TasksService.Models.WfTemplates
{
    public class WfMapper
    {
        public static WfnodesTempl messageNodeToWfnodesTempl(messageNode proto)
        { 
            return new WfnodesTempl 
            {
                Id = proto.Id,
                InternalNum = proto.InternalNum,
                Name = proto.Name,
                Description = proto.Description,
                DefinitionId = proto.DefinitionId
            };
        }

        public static messageNode  WfnodesTemplTomessageNode(WfnodesTempl ef)
        { 
            return new messageNode
            { 
                Id = ef.Id, 
                InternalNum = ef.InternalNum,
                Name = ef.Name,    
                Description = ef.Description,
                DefinitionId = ef.DefinitionId
            };
        }


        public static WfedgesTempl messageEdgeToWfedgesTempl(messageEdge proto)
        {
            return new WfedgesTempl
            {
                Id = proto.Id,
                InternalNum = proto.InternalNum,
                Name = proto.Name,
                NodeFrom = proto.NodeFromNum,
                NodeTo = proto.NodeToNum
            };
        }

        public static messageEdge WfedgesTemplToMessageNode(WfedgesTempl ef)
        {
            return new messageEdge
            {
                Id = ef.Id,
                InternalNum = ef.InternalNum,
                Name = ef.Name,
                NodeFromNum = ef.NodeFrom,
                NodeToNum = ef.NodeTo
            };
        }

    }
}
