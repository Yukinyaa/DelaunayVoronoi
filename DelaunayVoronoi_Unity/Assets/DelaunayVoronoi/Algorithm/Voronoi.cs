using System.Collections.Generic;
using System.Linq;

namespace DelaunayVoronoi
{
    public class Voronoi
    {
        public IEnumerable<Edge> GenerateEdgesFromDelaunay(IEnumerable<Triangle> triangulation)
        {
            var voronoiEdges = new HashSet<Edge>();
            foreach (var triangle in triangulation)
            {
                foreach (var neighbor in triangle.TrianglesWithSharedEdge)
                {
                    var edge = new Edge(triangle.WeightedCenter, neighbor.WeightedCenter);
                    List<Point> sharingedge = neighbor.Vertices.Where(o => triangle.Vertices.Contains(o)).ToList();
                    edge.Pointa = sharingedge[0];
                    edge.Pointb = sharingedge[1];
                    voronoiEdges.Add(edge);
                }
            }

            return voronoiEdges;
        }
    }
}