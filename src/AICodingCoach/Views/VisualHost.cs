using System.Globalization;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace AICodingCoach.Views
{
    public class VisualHost : FrameworkElement
    {
        // Create a collection of child visual objects.
        private readonly VisualCollection _children;

        public VisualHost()
        {
            _children = new VisualCollection(this);

            // Add the event handler for MouseLeftButtonUp.
            MouseLeftButtonUp += MyVisualHost_MouseLeftButtonUp;
        }

        public void Clear()
        {
            _children.Clear();
        }

        public void SetVisual(DrawingVisual vis)
        {
            Clear();
            _children.Add(vis);
        }

        // Capture the mouse event and hit test the coordinate point value against
        // the child visual objects.
        private void MyVisualHost_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            // Retreive the coordinates of the mouse button event.
            var pt = e.GetPosition((UIElement)sender);

            // Initiate the hit test by setting up a hit test result callback method.
            VisualTreeHelper.HitTest(this, null, MyCallback, new PointHitTestParameters(pt));
        }

        // If a child visual object is hit, toggle its opacity to visually indicate a hit.
        public HitTestResultBehavior MyCallback(HitTestResult result)
        {
            if (result.VisualHit.GetType() == typeof(DrawingVisual))
            {
                ((DrawingVisual)result.VisualHit).Opacity =
                    ((DrawingVisual)result.VisualHit).Opacity == 1.0 ? 0.4 : 1.0;
            }

            // Stop the hit test enumeration of objects in the visual tree.
            return HitTestResultBehavior.Stop;
        }
        
        protected override int VisualChildrenCount => _children.Count;
        protected override Visual GetVisualChild(int index) => _children[index];
    }
}