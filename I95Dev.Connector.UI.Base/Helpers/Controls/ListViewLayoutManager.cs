using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace I95Dev.Connector.UI.Base.Helpers.Controls
{
    public class ListViewLayoutManager
    {
        public static readonly DependencyProperty EnabledProperty = DependencyProperty.RegisterAttached(
            "Enabled",
            typeof(bool),
            typeof(ListViewLayoutManager),
            new FrameworkPropertyMetadata(OnLayoutManagerEnabledChanged));

        public ListViewLayoutManager(ListView listView)
        {
            if (listView == null)
            {
                throw new ArgumentNullException("listView");
            }

            this.listView = listView;
            this.listView.Loaded += ListViewLoaded;
            this.listView.Unloaded += ListViewUnloaded;
        }

        public ListView ListView
        {
            get { return listView; }
        }

        public ScrollBarVisibility VerticalScrollBarVisibility
        {
            get { return verticalScrollBarVisibility; }
            set { verticalScrollBarVisibility = value; }
        }

        public static void SetEnabled(DependencyObject dependencyObject, bool enabled)
        {
            dependencyObject.SetValue(EnabledProperty, enabled);
        }

        public void Refresh()
        {
            InitColumns();
            DoResizeColumns();
        }

        private void RegisterEvents(DependencyObject start)
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(start); i++)
            {
                Visual childVisual = VisualTreeHelper.GetChild(start, i) as Visual;
                if (childVisual is Thumb)
                {
                    GridViewColumn gridViewColumn = FindParentColumn(childVisual);
                    if (gridViewColumn != null)
                    {
                        Thumb thumb = childVisual as Thumb;
                        if (ProportionalColumn.IsProportionalColumn(gridViewColumn))
                        {
                            thumb.IsHitTestVisible = false;
                        }
                    }
                }
                else if (childVisual is GridViewColumnHeader)
                {
                    GridViewColumnHeader columnHeader = childVisual as GridViewColumnHeader;
                    columnHeader.SizeChanged += GridColumnHeaderSizeChanged;
                }
                else if (scrollViewer == null && childVisual is ScrollViewer)
                {
                    scrollViewer = childVisual as ScrollViewer;
                    scrollViewer.ScrollChanged += ScrollViewerScrollChanged;
                    // assume we do the regulation of the horizontal scrollbar
                    scrollViewer.HorizontalScrollBarVisibility = ScrollBarVisibility.Hidden;
                    scrollViewer.VerticalScrollBarVisibility = verticalScrollBarVisibility;
                }

                RegisterEvents(childVisual);  // recursive
            }
        }

        private void UnregisterEvents(DependencyObject start)
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(start); i++)
            {
                Visual childVisual = VisualTreeHelper.GetChild(start, i) as Visual;
                if (childVisual is Thumb)
                {
                    GridViewColumn gridViewColumn = FindParentColumn(childVisual);
                    if (gridViewColumn != null)
                    {
                        Thumb thumb = childVisual as Thumb;
                        if (ProportionalColumn.IsProportionalColumn(gridViewColumn))
                        {
                            thumb.IsHitTestVisible = true;
                        }
                    }
                }
                else if (childVisual is GridViewColumnHeader)
                {
                    GridViewColumnHeader columnHeader = childVisual as GridViewColumnHeader;
                    columnHeader.SizeChanged -= GridColumnHeaderSizeChanged;
                }
                else if (scrollViewer == null && childVisual is ScrollViewer)
                {
                    scrollViewer = childVisual as ScrollViewer;
                    scrollViewer.ScrollChanged -= ScrollViewerScrollChanged;
                }

                UnregisterEvents(childVisual);  // recursive
            }
        }

        private static GridViewColumn FindParentColumn(DependencyObject element)
        {
            if (element == null)
            {
                return null;
            }

            while (element != null)
            {
                GridViewColumnHeader gridViewColumnHeader = element as GridViewColumnHeader;
                if (gridViewColumnHeader != null)
                {
                    return gridViewColumnHeader.Column;
                }
                element = VisualTreeHelper.GetParent(element);
            }

            return null;
        }

        private GridViewColumnHeader FindColumnHeader(DependencyObject start, GridViewColumn gridViewColumn)
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(start); i++)
            {
                Visual childVisual = VisualTreeHelper.GetChild(start, i) as Visual;
                if (childVisual is GridViewColumnHeader)
                {
                    GridViewColumnHeader gridViewHeader = childVisual as GridViewColumnHeader;
                    if (Equals(gridViewHeader.Column, gridViewColumn))
                    {
                        return gridViewHeader;
                    }
                }
                GridViewColumnHeader childGridViewHeader = FindColumnHeader(childVisual, gridViewColumn);  // recursive
                if (childGridViewHeader != null)
                {
                    return childGridViewHeader;
                }
            }
            return null;
        }

        private void InitColumns()
        {
            GridView view = listView.View as GridView;
            if (view == null)
            {
                return;
            }

            foreach (GridViewColumn gridViewColumn in view.Columns)
            {
                FindColumnHeader(listView, gridViewColumn);
            }
        }

        protected virtual void ResizeColumns()
        {
            GridView view = listView.View as GridView;
            if (view == null || view.Columns.Count == 0)
            {
                return;
            }

            double actualWidth = double.PositiveInfinity;
            if (scrollViewer != null)
            {
                actualWidth = scrollViewer.ViewportWidth;
            }
            if (double.IsInfinity(actualWidth))
            {
                actualWidth = listView.ActualWidth;
            }
            if (double.IsInfinity(actualWidth) || actualWidth <= 0)
            {
                return;
            }

            double resizeableRegionCount = 0;
            double otherColumnsWidth = 0;

            foreach (GridViewColumn gridViewColumn in view.Columns)
            {
                if (ProportionalColumn.IsProportionalColumn(gridViewColumn))
                {
                    double? proportionalWidth = ProportionalColumn.GetProportionalWidth(gridViewColumn);
                    if (proportionalWidth != null)
                    {
                        resizeableRegionCount += proportionalWidth.Value;
                    }
                }
                else
                {
                    otherColumnsWidth += gridViewColumn.ActualWidth;
                }
            }

            if (resizeableRegionCount <= 0)
            {
                if (scrollViewer != null)
                {
                    scrollViewer.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
                }
                return;
            }

            double resizeableColumnsWidth = actualWidth - otherColumnsWidth;
            if (resizeableColumnsWidth <= 0)
            {
                return;
            }

            double resizeableRegionWidth = resizeableColumnsWidth / resizeableRegionCount;
            foreach (GridViewColumn gridViewColumn in view.Columns)
            {
                if (!ProportionalColumn.IsProportionalColumn(gridViewColumn)) continue;
                double? proportionalWidth = ProportionalColumn.GetProportionalWidth(gridViewColumn);
                if (proportionalWidth != null)
                {
                    gridViewColumn.Width = proportionalWidth.Value * resizeableRegionWidth;
                }
            }
        }

        private void DoResizeColumns()
        {
            if (resizing)
            {
                return;
            }

            resizing = true;
            try
            {
                ResizeColumns();
            }
            finally
            {
                resizing = false;
            }
        }

        private void ListViewLoaded(object sender, RoutedEventArgs e)
        {
            RegisterEvents(listView);
            InitColumns();
            DoResizeColumns();
            loaded = true;
        }

        private void ListViewUnloaded(object sender, RoutedEventArgs e)
        {
            if (!loaded)
            {
                return;
            }
            UnregisterEvents(listView);
            loaded = false;
        }

        private void GridColumnHeaderSizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (autoSizedColumn == null)
            {
                return;
            }

            GridViewColumnHeader gridViewColumnHeader = sender as GridViewColumnHeader;
            if (gridViewColumnHeader == null || !Equals(gridViewColumnHeader.Column, autoSizedColumn)) return;
            if (gridViewColumnHeader.Width.Equals(double.NaN))
            {
                gridViewColumnHeader.Column.Width = gridViewColumnHeader.ActualWidth;
                DoResizeColumns();
            }

            autoSizedColumn = null;
        }

        private void ScrollViewerScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (loaded && Math.Abs(e.ViewportWidthChange - 0) > ZeroWidthRange)
            {
                DoResizeColumns();
            }
        }

        private static void OnLayoutManagerEnabledChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            ListView listView = dependencyObject as ListView;
            if (listView == null) return;
            bool enabled = (bool)e.NewValue;
            if (enabled)
            {
                new ListViewLayoutManager(listView);
            }
        }

        private readonly ListView listView;
        private ScrollViewer scrollViewer;
        private bool loaded;
        private bool resizing;
        private ScrollBarVisibility verticalScrollBarVisibility = ScrollBarVisibility.Auto;
        private GridViewColumn autoSizedColumn;
        private const double ZeroWidthRange = 0.1;
    }
}