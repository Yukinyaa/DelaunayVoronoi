using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Shapes;

namespace DelaunayVoronoi
{
    public partial class MainWindow : Window
    {
        private DelaunayTriangulator delaunay = new DelaunayTriangulator();
        private Voronoi voronoi = new Voronoi();

        public MainWindow()
        {
            InitializeComponent();

            var points = delaunay.GeneratePoints(6, 800, 400);

            var delaunayTimer = Stopwatch.StartNew();
            var triangulation = delaunay.BowyerWatson(points);
            delaunayTimer.Stop();
            DrawTriangulation(triangulation);

            var voronoiTimer = Stopwatch.StartNew();
            var vornoiEdges = voronoi.GenerateEdgesFromDelaunay(triangulation);
            voronoiTimer.Stop();
            DrawVoronoi(vornoiEdges);

            DrawPoints(points);
        }

        private void DrawPoints(IEnumerable<Point> points)
        {
            foreach (var point in points)
            {
                var myEllipse = new Ellipse();
                myEllipse.Fill = System.Windows.Media.Brushes.Red;
                myEllipse.HorizontalAlignment = HorizontalAlignment.Left;
                myEllipse.VerticalAlignment = VerticalAlignment.Top;
                myEllipse.Width = 1;
                myEllipse.Height = 1;
                var ellipseX = point.X - 0.5 * myEllipse.Height;
                var ellipseY = point.Y - 0.5 * myEllipse.Width;
                myEllipse.Margin = new Thickness(ellipseX, ellipseY, 0, 0);

                Canvas.Children.Add(myEllipse);
            }
        }
        private void DrawPoint(Point p, System.Windows.Media.Brush brush, int size)
        {
            var line1 = new Line();
            var line2 = new Line();

            line1.Stroke = brush;
            line2.Stroke = brush;
            line1.StrokeThickness = 0.5;
            line2.StrokeThickness = 0.5;

            line1.X1 = p.X - size;
            line1.X2 = p.X + size;
            line1.Y1 = p.Y + size;
            line1.Y2 = p.Y - size;

            line2.X1 = p.X - size;
            line2.X2 = p.X + size;
            line2.Y1 = p.Y - size;
            line2.Y2 = p.Y + size;

            Canvas.Children.Add(line1);
            Canvas.Children.Add(line2);
        }

        private void DrawLine(Point p1, Point p2, System.Windows.Media.Brush brush)
        {

            var line = new Line();
            line.Stroke = brush;
            line.StrokeThickness = 0.5;

            line.X1 = p1.X;
            line.X2 = p2.X;
            line.Y1 = p1.Y;
            line.Y2 = p2.Y;

            Canvas.Children.Add(line);
        }
        private void DrawTriangulation(IEnumerable<Triangle> triangulation)
        {
            var edges = new List<Edge>();
            foreach (var triangle in triangulation)
            {
                edges.Add(new Edge(triangle.Vertices[0], triangle.Vertices[1]));
                edges.Add(new Edge(triangle.Vertices[1], triangle.Vertices[2]));
                edges.Add(new Edge(triangle.Vertices[2], triangle.Vertices[0]));

                DrawPoint(Point.WeightedMidPoint(triangle.Vertices[0], triangle.Vertices[1]), System.Windows.Media.Brushes.Blue, 3);

                var pa = triangle.Vertices[0];
                var pb = triangle.Vertices[1];
                var pc = triangle.Vertices[2];
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
                else {
                    a1 = m1; b1 = -1; c1 = -ab.X * m1 + ab.Y;
                    DrawPoint(triangle.Circumcenter, System.Windows.Media.Brushes.Cyan, 3);
                }

                if (double.IsInfinity(m2)) { a2 = 1; b2 = 0.000001; c2 = -ac.X; }
                else { a2 = m2; b2 = -1; c2 = -ac.X * m2 + ac.Y; }
                



                DrawPoint(triangle.Circumcenter,System.Windows.Media.Brushes.Cyan, 3);

                DrawPoint(triangle.WeightedCenter, System.Windows.Media.Brushes.Black, 3);


            }

            foreach (var edge in edges)
            {
                var line = new Line();
                line.Stroke = System.Windows.Media.Brushes.LightSteelBlue;
                line.StrokeThickness = 0.5;

                line.X1 = edge.Point1.X;
                line.X2 = edge.Point2.X;
                line.Y1 = edge.Point1.Y;
                line.Y2 = edge.Point2.Y;
                
                Canvas.Children.Add(line);
            }
        }

        private void DrawVoronoi(IEnumerable<Edge> voronoiEdges)
        {
            foreach (var edge in voronoiEdges)
            {
                var line = new Line();
                line.Stroke = System.Windows.Media.Brushes.DarkViolet;
                line.StrokeThickness = 1;

                line.X1 = edge.Point1.X;
                line.X2 = edge.Point2.X;
                line.Y1 = edge.Point1.Y;
                line.Y2 = edge.Point2.Y;

                Canvas.Children.Add(line);
            }
        }
    }
}