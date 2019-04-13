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
            double x = (a.X * a.W + b.X * b.W) / (a.W + b.W);
            double y = (a.Y * a.W + b.Y * b.W) / (a.W + b.W);
            return new Point(x,y);
        }
        //public Point
    }
}