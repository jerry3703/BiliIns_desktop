using System.Collections.ObjectModel;
using Wpf.Ui.Controls;

namespace Bililns_desktop.ViewModels.Windows
{
    public partial class MainWindowViewModel : ObservableObject
    {
        [ObservableProperty]
        private string _applicationTitle = "BiliIns_桌面版本";

        [ObservableProperty]
        private ObservableCollection<object> _menuItems = new()
        {
            new NavigationViewItem()
            {
                Content = "主页-基本数据查看",
                Icon = new SymbolIcon { Symbol = SymbolRegular.BoxMultiple24 },
                TargetPageType = typeof(Views.Pages.DashboardPage)
            },
            new NavigationViewItem()
            {
                Content = "数据对比",
                Icon = new SymbolIcon { Symbol = SymbolRegular.DataHistogram24 },
                TargetPageType = typeof(Views.Pages.DataPage)
            },
            /*
              new NavigationViewItem()
            {
                Content = "AI分析",
                Icon = new SymbolIcon { Symbol = SymbolRegular.ChatCursor24 },
                TargetPageType = typeof(Views.Pages.Aidoc)
            }
            */
        };

        [ObservableProperty]
        private ObservableCollection<object> _footerMenuItems = new()
        {
            new NavigationViewItem()
            {
                Content = "关于/设置",
                Icon = new SymbolIcon { Symbol = SymbolRegular.Settings24 },
                TargetPageType = typeof(Views.Pages.SettingsPage)
            }
        };

        [ObservableProperty]
        private ObservableCollection<MenuItem> _trayMenuItems = new()
        {
            new MenuItem { Header = "Home", Tag = "tray_home" }
        };
    }
}
