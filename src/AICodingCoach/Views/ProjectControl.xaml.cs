using System.Windows.Controls;
using AICodingCoach.Controllers;
using AICodingCoach.ViewModels;

namespace AICodingCoach.Views
{
    public partial class ProjectControl : UserControl
    {
        public ProjectController Controller { get; private set; }

        public ProjectControl()
        {
            InitializeComponent();
            Loaded += ProjectControl_Loaded;
        }

        private void ProjectControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            if (Controller == null)
            {
                var viewModel = DataContext as ProjectViewModel;
                if (viewModel == null)
                    throw new Exception("Expected a ProjectViewModel as DataContext");
                Controller = new ProjectController(this, viewModel);
            }
        }
    }
}
