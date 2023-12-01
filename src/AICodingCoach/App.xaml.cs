using AICodingCoach.Services;
using System.Windows;
using AICodingCoach.Views;
using Ara3D.Utils;

namespace AICodingCoach
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static App Instance => Current as App;
        public static MainWindow MainWindow => Current.MainWindow as MainWindow;
        public static MainService MainService => MainWindow.Service;
        public static DirectoryPath SourceFolder => PathUtil.GetCallerSourceFolder();
    }
}
