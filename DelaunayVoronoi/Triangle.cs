using System.Collections.Generic;
using System.Linq;

namespace DelaunayVoronoi
{
    public class Triangle
    {
        public Point[] Vertices { get; private set;  }
        public Point Circumcenter { get; private set; }
        public double RadiusSquared;

        public IEnumerable<Triangle> TrianglesWithSharedEdge {
            get {
                var neighbors = new HashSet<Triangle>();
                foreach (var vertex in Vertices)
                {
                    var trianglesWithSharedEdge = vertex.AdjacentTriangles.Where(o =>
                    {
                        return o != this && SharesEdgeWith(o);
                    });
                    neighbors.UnionWith(trianglesWithSharedEdge);
                }

                return neighbors;
            }
        }

        public Triangle(Point point1, Point point2, Point point3)
        {
            Vertices = new Point[3];
            if (!IsCounterClockwise(point1, point2, point3))
            {
                Vertices[0] = point1;
                Vertices[1] = point3;
                Vertices[2] = point2;
            }
            else
            {
                Vertices[0] = point1;
                Vertices[1] = point2;
                Vertices[2] = point3;
            }

            Vertices[0].AdjacentTriangles.Add(this);
            Vertices[1].AdjacentTriangles.Add(this);
            Vertices[2].AdjacentTriangles.Add(this);
            UpdateCircumcircle();
        }

        private void UpdateCircumcircle()
        {
            // https://codefound.wordpress.com/2013/02/21/how-to-compute-a-circumcircle/#more-58
            // https://en.wikipedia.org/wiki/Circumscribed_circle
            var p0 = Vertices[0];
            var p1 = Vertices[1];
            var p2 = Vertices[2];
            var dA = p0.X * p0.X + p0.Y * p0.Y;
            var dB = p1.X * p1.X + p1.Y * p1.Y;
            var dC = p2.X * p2.X + p2.Y * p2.Y;

            var aux1 = (dA * (p2.Y - p1.Y) + dB * (p0.Y - p2.Y) + dC * (p1.Y - p0.Y));
            var aux2 = -(dA * (p2.X - p1.X) + dB * (p0.X - p2.X) + dC * (p1.X - p0.X));
            var div = (2 * (p0.X * (p2.Y - p1.Y) + p1.X * (p0.Y - p2.Y) + p2.X * (p1.Y - p0.Y)));

            if (div == 0)
            {
                throw new System.Exception();
            }

            var center = new Point(aux1 / div, aux2 / div);
            Circumcenter = center;
            RadiusSquared = (center.X - p0.X) * (center.X - p0.X) + (center.Y - p0.Y) * (center.Y - p0.Y);
        }
        public Point WeightedCenter //magic :D
        {
            get 
            {
                //var p0 = Point.WeightedMidPoint(Vertices[0],Vertices[1]);
                //var p1 = Point.WeightedMidPoint(Vertices[1],Vertices[2]);
                //var p2 = Point.WeightedMidPoint(Vertices[0],Vertices[2]);
                var pa = Vertices[0];
                var pb = Vertices[1];
                var pc = Vertices[2];
                if (pb.W < pa.W && pb.W < pc.W)
                { var tmp = pa; pa = pb; pb = tmp; }
                else if (pc.W < pa.W)
                { var tmp = pa; pa = pc; pc = tmp; }

                var m1 = (pb.X - pa.X) / (pb.Y - pa.Y);
                var m2 = (pc.X - pa.X) / (pc.Y - pa.Y);

                var ab = Point.WeightedMidPoint(pa, pb);
                var ac = Point.WeightedMidPoint(pa, pc);

                double a1, b1, c1;
                double a2, b2, c2;

                if (double.IsInfinity(m1)) { a1 = 1; b1 = 0.000001; c1 = -ab.X; }
                else { a1 = m1;b1 = -1;c1 = -ab.X * m1 + ab.Y; }

                if (double.IsInfinity(m2)) { a2 = 1; b2 = 0.000001; c2 = -ac.X; }
                else { a2 = m2; b2 = -1; c2 = -ac.X * m2 + ac.Y; }

                if (a1 * b2 - a2 * b1 == 0) return Point.WeightedMidPoint(ab, ac);//parral or same, so triangle itself is obsolite. so returning midpoint.

                var k = (b1 * c2 - b2 * c1) / (a1 * b2 - a2 * b1);

                return new Point(k, -a1 / b1 * k - c1 / b1);
            }
        }

        private bool IsCounterClockwise(Point point1, Point point2, Point point3)
        {
            var result = (point2.X - point1.X) * (point3.Y - point1.Y) -
                (point3.X - point1.X) * (point2.Y - point1.Y);
            return result > 0;
        }

        public bool SharesEdgeWith(Triangle triangle)
        {
            var sharedVertices = Vertices.Where(o => triangle.Vertices.Contains(o)).Count();
            return sharedVertices == 2;
        }

        public bool IsPointInsideCircumcircle(Point point)
        {
            var d_squared = (point.X - Circumcenter.X) * (point.X - Circumcenter.X) +
                (point.Y - Circumcenter.Y) * (point.Y - Circumcenter.Y);
            return d_squared < RadiusSquared;
        }
    }
}