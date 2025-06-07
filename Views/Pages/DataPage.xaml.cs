using Bililns_desktop.ViewModels.Pages;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Wpf.Ui.Abstractions.Controls;

namespace Bililns_desktop.Views.Pages
{
    public partial class DataPage : INavigableView<DataViewModel>
    {
        public DataViewModel ViewModel { get; }

        public DataPage(DataViewModel viewModel)
        {
            ViewModel = viewModel;
            DataContext = this;

            InitializeComponent();
        }

      

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void CartesianChart_Loaded(object sender, RoutedEventArgs e)
        {

        }
        private void OnPreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            e.Handled = true;
            var parent = VisualTreeHelper.GetParent((DependencyObject)sender);
            while (parent != null && !(parent is ScrollViewer))
                parent = VisualTreeHelper.GetParent(parent);
            if (parent is ScrollViewer sv)
            {
                var args = new MouseWheelEventArgs(e.MouseDevice, e.Timestamp, e.Delta)
                {
                    RoutedEvent = UIElement.MouseWheelEvent,
                    Source = sender
                };
                sv.RaiseEvent(args);
            }
        }


    }
}
