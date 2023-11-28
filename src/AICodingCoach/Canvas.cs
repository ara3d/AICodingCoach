using System.Windows;
using System.Windows.Media;
using Ara3D.Math;

namespace CodingCanvasWpfApp
{
    public class Canvas
    {
        public Pen CurrentPen { get; set; }
        public DrawingContext Context { get; }

        public Canvas(DrawingContext context)
        {
            CurrentPen = new Pen(new SolidColorBrush(Colors.Blue), 5);
            Context = context;
        }

        public Canvas SetPenThickness(double width)
        {
            CurrentPen.Thickness = (float)width;
            return this;
        }

        public Canvas SetPenColor(Color color)
        {
            CurrentPen = new Pen(new SolidColorBrush(color), CurrentPen.Thickness);
            return this;
        }

        public Canvas DrawLine(float x0, float y0, float x1, float y1)
        {
            return DrawLine(new Line2D((x0,y0), (x1,y1)));
        }

        public Canvas DrawLine(Line2D line)
        {
            Context.DrawLine(CurrentPen, ToPoint(line.A), ToPoint(line.B));
            return this;
        }

        public Point ToPoint(Vector2 v)
        {
            return new Point(v.X, v.Y);
        }
    }
}
