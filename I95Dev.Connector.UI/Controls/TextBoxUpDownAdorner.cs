using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace I95Dev.Connector.UI.Controls
{
    /// <inheritdoc />
    public class TextBoxUpDownAdorner : Adorner
    {
        private readonly StreamGeometry triangle;
        private bool shown;
        private double x, top, bottom;

        /// <summary>
        /// The outline
        /// </summary>
        private readonly Pen outline = new Pen(new SolidColorBrush(Color.FromArgb(64, 255, 255, 255)), 5);

        /// <summary>
        /// The fill
        /// </summary>
        private readonly Brush fill = Brushes.Black;

        /// <inheritdoc />
        public TextBoxUpDownAdorner(TextBox adornedBox) : base(adornedBox)
        {
            triangle = new StreamGeometry
            {
                FillRule = FillRule.Nonzero
            };
            using (StreamGeometryContext c = triangle.Open())
            {
                c.BeginFigure(new Point(-10, 0), true /* filled */, true /* closed */);
                c.LineTo(new Point(10, 0), true, false);
                c.LineTo(new Point(0, 15), true, false);
            }
            triangle.Freeze();

            MouseDown += (s, e) =>
            {
                if (Clicked == null) return;
                bool up = e.GetPosition(AdornedElement).Y < (top + bottom) / 2;
                Clicked((TextBox)AdornedElement, up ? 1 : -1);
            };

            adornedBox.LostFocus += RelevantEventOccurred;
            adornedBox.SelectionChanged += RelevantEventOccurred;
        }

        private void RelevantEventOccurred(object sender, RoutedEventArgs e)
        {
            // In OnRender, GetRectFromCharacterIndex may return Infinity values,
            // so measure the location of the selection here instead.
            var box = AdornedElement as TextBox;
            if (box != null && box.IsFocused)
            {
                int start = box.SelectionStart, len = box.SelectionLength;
                if (shown == len > 0)
                {
                    Rect rect1 = box.GetRectFromCharacterIndex(start);
                    Rect rect2 = box.GetRectFromCharacterIndex(start + len);
                    top = rect1.Top - 2;
                    bottom = rect1.Bottom + 2;
                    x = (rect1.Left + rect2.Left) / 2;
                }
            }
            else
                shown = false;

            InvalidateVisual();
        }

        /// <summary>
        /// Occurs when [click].
        /// </summary>
        public event ClickedEventHandler Clicked;

        public delegate void ClickedEventHandler(object sender, int value);

        /// <inheritdoc />
        protected override void OnRender(DrawingContext drawingContext)
        {
            if (!shown) return;
            drawingContext.PushTransform(new TranslateTransform(x, top));
            drawingContext.PushTransform(new ScaleTransform(1, -1));
            drawingContext.DrawGeometry(fill, outline, triangle);
            drawingContext.Pop();
            drawingContext.Pop();
            drawingContext.PushTransform(new TranslateTransform(x, bottom));
            drawingContext.DrawGeometry(fill, outline, triangle);
            drawingContext.Pop();
        }
    }
}