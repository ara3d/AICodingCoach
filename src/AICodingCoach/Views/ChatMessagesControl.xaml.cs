using System.Windows.Controls;
using AICodingCoach.ViewModels;

namespace CodingCanvasWpfApp
{
    /// <summary>
    /// Interaction logic for ChatMessagesControl.xaml
    /// </summary>
    public partial class ChatMessagesControl : UserControl
    {
        public ChatMessagesViewModel ViewModel { get; } = new();

        public ChatMessagesControl()
        {
            DataContext = ViewModel;
            InitializeComponent();
        }
    }
}
