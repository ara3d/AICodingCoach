using System.Diagnostics;
using System.Windows;
using AICodingCoach.Services;

namespace AICodingCoach.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainService Service = new MainService();

        public MainWindow()
        {
            InitializeComponent();
            var w = new TestWorkspaceBindingWindow();
            w.Show();
        }
    }
}