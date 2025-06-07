using Bililns_desktop.ViewModels.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Wpf.Ui.Abstractions.Controls;

namespace Bililns_desktop.Views.Pages
{
    /// <summary>
    /// Aidoc.xaml 的交互逻辑
    /// </summary>
    public partial class Aidoc : INavigableView<AidocViewModel>
    {
        public Aidoc()
        {
            InitializeComponent();
        }
        public AidocViewModel ViewModel { get; }

        public Aidoc(AidocViewModel viewModel)
        {
            ViewModel = viewModel;
            DataContext = this;

            InitializeComponent();
        }
    }
}
