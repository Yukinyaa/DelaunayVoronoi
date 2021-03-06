﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace DelaunayVoronoi
{
    public class DelaunayTriangulator
    {
        public float MaxX { get; set; }
        public float MaxY { get; set; }
        private IEnumerable<Triangle> border;

        public IEnumerable<Point> GeneratePoints(int amount, float maxX, float maxY)
        {
            MaxX = maxX;
            MaxY = maxY;

            // TODO make more beautiful
            var point0 = new Point(0, 0);
            var point1 = new Point(0, MaxY);
            var point2 = new Point(MaxX, MaxY);
            var point3 = new Point(MaxX, 0);
            var points = new List<Point>() { point0, point1, point2, point3 };
            var tri1 = new Triangle(point0, point1, point2);
            var tri2 = new Triangle(point0, point2, point3);
            border = new List<Triangle>() { tri1, tri2 };

            var random = new Random();
            for (int i = 0; i < amount - 4; i++)
            {
                var pointX = (float)random.NextDouble() * MaxX;
                var pointY = (float)random.NextDouble() * MaxY;
                points.Add(new Point(pointX, pointY));
            }

            return points;
        }

        public IEnumerable<Point> GenerateWeightedPoints(int amount, float maxX, float maxY)
        {
            MaxX = maxX;
            MaxY = maxY;

            // TODO make more beautiful
            var point0 = new Point(0, 0, 0);
            var point1 = new Point(0, MaxY, 0);
            var point2 = new Point(MaxX, MaxY, 0);
            var point3 = new Point(MaxX, 0, 0);
            var points = new List<Point>() { point0, point1, point2, point3 };
            var tri1 = new Triangle(point0, point1, point2);
            var tri2 = new Triangle(point0, point2, point3);
            border = new List<Triangle>() { tri1, tri2 };

            var random = new Random();
            for (int i = 0; i < amount - 4; i++)
            {
                var pointX = (float)random.NextDouble() * MaxX;
                var pointY = (float)random.NextDouble() * MaxY;
                var pointW = (float)random.NextDouble() * 9 + 1;
                points.Add(new Point(pointX, pointY, pointW));
            }

            return points;
        }

        public IEnumerable<Triangle> BowyerWatson(IEnumerable<Point> points)
        {
            var point0 = new Point(-MaxX, -MaxY, 0);
            var point1 = new Point(-MaxX, MaxY, 0);
            var point2 = new Point(MaxX, MaxY, 0);
            var point3 = new Point(MaxX, -MaxY, 0);
            var tri1 = new Triangle(point0, point1, point2);
            var tri2 = new Triangle(point0, point2, point3);
            border = new List<Triangle>() { tri1, tri2 };

            //var supraTriangle = GenerateSupraTriangle();
            var triangulation = new HashSet<Triangle>(border);

            foreach (var point in points)
            {
                var badTriangles = FindBadTriangles(point, triangulation);
                var polygon = FindHoleBoundaries(badTriangles);

                foreach (var triangle in badTriangles)
                {
                    foreach (var vertex in triangle.Vertices)
                    {
                        vertex.AdjacentTriangles.Remove(triangle);
                    }
                }
                triangulation.RemoveWhere(o => badTriangles.Contains(o));

                foreach (var edge in polygon)
                {
                    var triangle = new Triangle(point, edge.Point1, edge.Point2);
                    triangulation.Add(triangle);
                }
            }

            //triangulation.RemoveWhere(o => o.Vertices.Any(v => supraTriangle.Vertices.Contains(v)));
            return triangulation;
        }

        private List<Edge> FindHoleBoundaries(HashSet<Triangle> badTriangles)
        {
            var edges = new List<Edge>();
            foreach (var triangle in badTriangles)
            {
                edges.Add(new Edge(triangle.Vertices[0], triangle.Vertices[1]));
                edges.Add(new Edge(triangle.Vertices[1], triangle.Vertices[2]));
                edges.Add(new Edge(triangle.Vertices[2], triangle.Vertices[0]));
            }
            var grouped = edges.GroupBy(o => o);
            var boundaryEdges = edges.GroupBy(o => o).Where(o => o.Count() == 1).Select(o => o.First());
            return boundaryEdges.ToList();
        }

        private Triangle GenerateSupraTriangle()
        {
            //   1  -> maxX
            //  / \
            // 2---3
            // |
            // v maxY
            var margin = 500;
            var point1 = new Point(0.5f * MaxX, -2 * MaxX - margin);
            var point2 = new Point(-2 * MaxY - margin, 2 * MaxY + margin);
            var point3 = new Point(2 * MaxX + MaxY + margin, 2 * MaxY + margin);
            return new Triangle(point1, point2, point3);
        }

        private HashSet<Triangle> FindBadTriangles(Point point, HashSet<Triangle> triangles)
        {
            var badTriangles = triangles.Where(o => o.IsPointInsideCircumcircle(point));
            return new HashSet<Triangle>(badTriangles);
        }
    }
}