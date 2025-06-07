using Bililns_desktop.ViewModels.Pages;
using System.Runtime.InteropServices.Marshalling;
using Wpf.Ui.Abstractions.Controls;

namespace Bililns_desktop.Views.Pages
{
    public partial class DashboardPage : INavigableView<DashboardViewModel>
    {
        public DashboardViewModel ViewModel { get; }

        public DashboardPage(DashboardViewModel viewModel)
        {
            ViewModel = viewModel;
            DataContext = this;

            InitializeComponent();
        }

        private void TextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {

        }

        private void TextBox_TextChanged_1(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {

        }

        private void TextBox_TextChanged_2(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //  MessageBox.Show(aaa.Text);
            basicdis.Visibility = Visibility.Visible;
            basicdetail.Visibility = Visibility.Visible;
            videoinfo.Visibility = Visibility.Visible;
            shareinfo.Visibility = Visibility.Visible;
           // ownerinfo.Visibility = Visibility.Visible;
           handdes.Visibility = Visibility.Visible;
            aiinfo.Visibility = Visibility.Visible;


        }
    }
}
