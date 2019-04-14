using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DelaunayVoronoi;
using System.Diagnostics;

public class VoronoiGenerator {

    static private DelaunayTriangulator delaunay = new DelaunayTriangulator();
    static private Voronoi voronoi = new Voronoi();

    static public void UpdatePoints()
    {
        var voronoiPoints = new List<VoronoiPoint>(Resources.FindObjectsOfTypeAll<VoronoiPoint>());

        if (voronoiPoints.Count < 4)
            UnityEngine.Debug.LogWarning("Too little voronoi points!");

        List<Point> points = new List<Point>();
        delaunay.MaxX = 0;
        delaunay.MaxY = 0;
        foreach (var vp in voronoiPoints)
        {
            points.Add(vp.p = new Point(vp.transform.position.x, vp.transform.position.z));
            delaunay.MaxX = Mathf.Max(Mathf.Abs(vp.p.X), delaunay.MaxX);
            delaunay.MaxY = Mathf.Max(Mathf.Abs(vp.p.Y), delaunay.MaxY);
        }
        delaunay.MaxX += 10;
        delaunay.MaxY += 10;

        //points.Add( new Point(-delaunay.MaxX, -delaunay.MaxY, 0));
        //points.Add(new Point(delaunay.MaxX, -delaunay.MaxY, 0));
        //points.Add(new Point(-delaunay.MaxX, delaunay.MaxY, 0));
        //points.Add(new Point(delaunay.MaxX, delaunay.MaxY, 0));



        var delaunayTimer = Stopwatch.StartNew();
        var triangulation = delaunay.BowyerWatson(points);
        delaunayTimer.Stop();

        foreach (var triangle in triangulation)
        {
            UnityEngine.Debug.DrawLine(triangle.Vertices[0].AsXZPoint(), triangle.Vertices[1].AsXZPoint(), Color.green);
            UnityEngine.Debug.DrawLine(triangle.Vertices[2].AsXZPoint(), triangle.Vertices[1].AsXZPoint(), Color.green);
            UnityEngine.Debug.DrawLine(triangle.Vertices[0].AsXZPoint(), triangle.Vertices[2].AsXZPoint(), Color.green);
        }

        var voronoiTimer = Stopwatch.StartNew();
        var vornoiEdges = voronoi.GenerateEdgesFromDelaunay(triangulation);
        voronoiTimer.Stop();

        var v_edges = new Dictionary<Point, List<Edge>>();


        //sort edges by point
        foreach (var edge in vornoiEdges)
        {
            if (!v_edges.ContainsKey(edge.Pointa)) v_edges.Add(edge.Pointa, new List<Edge>());
            if (!v_edges.ContainsKey(edge.Pointb)) v_edges.Add(edge.Pointb, new List<Edge>());

            v_edges[edge.Pointa].Add(edge);
            v_edges[edge.Pointb].Add(edge);
        }
        //retreive verticies from point
        foreach (var vp in voronoiPoints)
        {
            var unsortededge = new List<Edge>(v_edges[vp.p]);
            Vector3[] vertices = new Vector3[unsortededge.Count+1];
            vertices[0] = unsortededge[0].Point1.AsXZPoint();
            vertices[1] = unsortededge[0].Point2.AsXZPoint();


            UnityEngine.Debug.DrawLine(vertices[0], vertices[1]);


            /*
            var lastpoint = unsortededge[0].Point2;
            unsortededge.RemoveRange(0, 1);
            int i = 1;
            while (unsortededge.Count != 0)
            {
                var nextPoint = unsortededge.FindLast(e => e.Point1.Equals(lastpoint));
                if (nextPoint != null)
                {
                    unsortededge.Remove(nextPoint);
                    vertices[++i] = nextPoint.Point2.AsXZPoint();
                    lastpoint = nextPoint.Point2;
                    continue;
                }
                nextPoint = unsortededge.FindLast(e => e.Point2.Equals(lastpoint));
                if (nextPoint != null)
                {
                    unsortededge.Remove(nextPoint);
                    vertices[++i] = nextPoint.Point2.AsXZPoint();
                    lastpoint = nextPoint.Point2;
                }
                else throw new System.Exception("verticies err");
            }
            var triangles = new TriangulatorXZ(vertices).Triangulate();

            // Create the mesh
            Mesh msh = new Mesh();
            msh.vertices = vertices;
            msh.triangles = triangles;
            msh.RecalculateNormals();
            msh.RecalculateBounds();

            vp.GetComponent<MeshFilter>().mesh = msh;
            */
        }
    }
	// Update is called once per frame
	void Update () {
		
	}
}
