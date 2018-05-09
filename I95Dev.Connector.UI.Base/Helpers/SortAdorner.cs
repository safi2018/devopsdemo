using System.ComponentModel;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

namespace I95Dev.Connector.UI.Base.Helpers
{
    public class SortAdorner : Adorner
    {
        private static readonly Geometry AscGeometry = Geometry.Parse("M 0,0 L 10,0 L 5,5 Z");

        private static readonly Geometry DescGeometry = Geometry.Parse("M 0,5 L 10,5 L 5,0 Z");

        /// <summary>
        /// Gets or sets the direction.
        /// </summary>
        /// <value>The direction.</value>
        public ListSortDirection Direction { get; private set; }

        /// <inheritdoc />
        public SortAdorner(UIElement element, ListSortDirection dir) : base(element)
        { Direction = dir; }

        /// <inheritdoc />
        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);
            if (AdornedElement.RenderSize.Width < 20) return;

            drawingContext.PushTransform(new TranslateTransform(AdornedElement.RenderSize.Width - 15, (AdornedElement.RenderSize.Height - 5) / 2));
            drawingContext.DrawGeometry(Brushes.Black, null, Direction == ListSortDirection.Ascending ? AscGeometry : DescGeometry);
            drawingContext.Pop();
        }
    }
}