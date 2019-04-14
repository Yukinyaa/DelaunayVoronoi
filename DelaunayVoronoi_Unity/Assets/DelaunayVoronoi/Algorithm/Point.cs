using System.Collections.Generic;
using UnityEngine;
namespace DelaunayVoronoi
{
    public class Point
    {
        public float X { get; private set; }
        public float Y { get; private set; }
        public float W { get; private set; }
        public HashSet<Triangle> AdjacentTriangles { get; private set; }

        public Point(float x, float y, float w = 1)
        {
            AdjacentTriangles = new HashSet<Triangle>();
            X = x;
            Y = y;
            W = w;
        }
        public Vector3 AsXZPoint(float y = 0)
        {
            return new Vector3((float)X, y, (float)Y);
        }
        static public Point WeightedMidPoint(Point a, Point b)
        {
            if (a.W == 0 && b.W == 0)
            {
                float xx = (a.X + b.X) / 2;
                float yy = (a.Y + b.Y) / 2;
                return new Point(xx, yy);
            }
            float x = (a.X * b.W + b.X * a.W) / (a.W + b.W);
            float y = (a.Y * b.W + b.Y * a.W) / (a.W + b.W);
            return new Point(x,y);
        }
        public override bool Equals(object obj)
        {
            if (obj is Point)
            {
                Point p = obj as Point;
                return X == p.X && Y == p.Y;
            }
            return base.Equals(obj);
        }
        //public Point
    }
}