using System.Collections.Generic;

namespace DelaunayVoronoi
{
    public class Point
    {
        public double X { get; private set; }
        public double Y { get; private set; }
        public double W { get; private set; }
        public HashSet<Triangle> AdjacentTriangles { get; private set; }

        public Point(double x, double y, double w = 1)
        {
            AdjacentTriangles = new HashSet<Triangle>();
            X = x;
            Y = y;
            W = w;
        }
        static public Point WeightedMidPoint(Point a, Point b)
        {
            if (a.W == 0 && b.W == 0)
            {
                double xx = (a.X + b.X) / 2;
                double yy = (a.Y + b.Y) / 2;
                return new Point(xx, yy);
            }
            double x = (a.X * b.W + b.X * a.W) / (a.W + b.W);
            double y = (a.Y * b.W + b.Y * a.W) / (a.W + b.W);
            return new Point(x,y);
        }
        //public Point
    }
}