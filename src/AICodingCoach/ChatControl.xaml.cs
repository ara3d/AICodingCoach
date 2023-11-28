using System.Windows;
using System.Windows.Controls;

namespace CodingCanvasWpfApp
{
    /// <summary>
    /// Interaction logic for ChatControl.xaml
    /// </summary>
    public partial class ChatControl : UserControl
    {
        private ChatController Controller { get; }
        
        public ChatControl()
        {
            InitializeComponent();
            Controller = new ChatController(this);
            // TODO: this seems weird. 
            ChatHistory.DataContext = ChatHistory.ViewModel;
            ChatHistory.ViewModel.AppendNonUserText($"Hello, I am your AI Coding Coach!");
        }

        private async void SubmitButton_OnClick(object sender, RoutedEventArgs e)
        {
            var prompt = Prompt.Text;
            ChatHistory.ViewModel.AppendUserText(prompt);
            Prompt.Clear();
            await Controller.Service.SendPromptAsync(prompt, (i, s) =>
            {
                ChatHistory.ViewModel.AppendNonUserText(s);
            });
            ChatHistory.ViewModel.CurrentMessage = null;
        }
    }
}
