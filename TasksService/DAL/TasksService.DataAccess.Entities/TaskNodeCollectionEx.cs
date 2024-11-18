using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TasksService.DataAccess.Entities
{

    // Класс на основе IEqualityComparer для объединения двух коллекций с Гранями
    public class EdgeComparer : IEqualityComparer<TaskEdge>
    {
        public bool Equals(TaskEdge? x, TaskEdge? y)
        {
            return x is not null 
                && y is not null 
                && x.NodeFrom == y.NodeFrom 
                && x.NodeTo == y.NodeTo;
        }

        public int GetHashCode(TaskEdge obj)
        {
            return ($"{obj.Name}{obj.Id}{obj.NodeFrom}{obj.NodeTo}").GetHashCode();
        }
    }

    // Расширение ICollection<TaskNode> для сбора всех Граней 
    public static class TaskNodeCollectionEx
    {
        public static List<TaskEdge> CollectEdges(this ICollection<TaskNode> collection)
        {
            var edges = new List<TaskEdge>();
            try
            {
                foreach (var node in collection)
                {
                    edges = edges.Union(node.TaskEdgeNodeToNavigations, new EdgeComparer()).ToList();
                    edges = edges.Union(node.TaskEdgeNodeFromNavigations, new EdgeComparer()).ToList();
                }
            }
            catch(Exception) 
            {
                throw;
            }
            return edges;
        }
    }
}
