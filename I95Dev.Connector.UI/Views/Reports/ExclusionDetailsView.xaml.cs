using System.Windows;
using System.Windows.Controls;
using I95Dev.Connector.UI.Base.Helpers;
using I95Dev.Connector.UI.Base.ViewModels.Reports;

namespace I95Dev.Connector.UI.Views.Reports
{
    /// <summary>
    /// Interaction logic for RecordViewer.xaml
    /// </summary>
    public partial class ExclusionDetailsView : Window
    {
        public ExclusionDetailsView()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ExclusionDetailsViewModel model = DataContext as ExclusionDetailsViewModel;
            if (model == null) return;
            bool jsonLoaded = model.IsJson;
            if (jsonLoaded)
            {
                TreeViewJson.Items.Clear();
                JsonViewerHelper helper = new JsonViewerHelper();
                jsonLoaded = helper.LoadJsonToTreeView(TreeViewJson, model.Parameters);
                if (jsonLoaded)
                {
                    TreeViewItem item = (TreeViewItem)TreeViewJson.Items.GetItemAt(0);
                    item.ExpandSubtree();
                    TextblockParameters.Visibility = Visibility.Collapsed;
                    TreeViewJson.Visibility = Visibility.Visible;
                }
            }
            if (jsonLoaded) return;
            TreeViewJson.Visibility = Visibility.Hidden;
            TextblockParameters.Text = model.Parameters;
            TextblockParameters.Visibility = Visibility.Visible;
        }
    }
}