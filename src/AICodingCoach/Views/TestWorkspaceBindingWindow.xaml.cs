using System.Windows;
using AICodingCoach.Models;
using AICodingCoach.Services;
using AICodingCoach.ViewModels;
using Ara3D.Services;

namespace AICodingCoach.Views
{
    /// <summary>
    /// Interaction logic for TestWorkspaceBindingWindow.xaml
    /// </summary>
    public partial class TestWorkspaceBindingWindow : Window
    {
        public WorkspaceViewModel ViewModel { get; }

        public TestWorkspaceBindingWindow()
        {
            var api = new Api();
            var projectService = new ProjectsService(api);
            ViewModel = new WorkspaceViewModel(projectService);
            DataContext = ViewModel;
            InitializeComponent();
            projectService.Repository.Add(new ProjectData("Test", "Here is some code"));
            projectService.Repository.Add(new ProjectData("Test2", "And here is different code"));
        }
    }
}
