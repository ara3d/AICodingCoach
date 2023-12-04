using System.Windows;

namespace AICodingCoach.Views
{
    public partial class WorkspaceWindow : Window
    {
        public WorkspaceWindow()
        {
            InitializeComponent();
            App.Instance.Initialize();
        }
    }
}