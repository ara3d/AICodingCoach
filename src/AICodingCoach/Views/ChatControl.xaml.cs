using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using AICodingCoach.Services;

namespace AICodingCoach.Views
{
    /// <summary>
    /// Interaction logic for ChatControl.xaml
    /// </summary>
    public partial class ChatControl : UserControl
    {
        public ChatService ChatService { get; }
        
        public ChatControl()
        {
            InitializeComponent();
            ChatService = new ChatService();
            ChatHistory.DataContext = ChatHistory.ViewModel;
            ChatHistory.ViewModel.AppendNonUserText($"Hello, I am your AI Coding Coach! 👋");
        }

        private async void SubmitButton_OnClick(object sender, RoutedEventArgs e)
        {
            await SubmitPrompt();
        }

        public async Task SubmitPrompt()
        {
            var prompt = Prompt.Text;
            ChatHistory.ViewModel.AppendUserText(prompt);
            Prompt.Clear();
            await ChatService.SendPromptAsync(prompt, (i, s) =>
            {
                ChatHistory.ViewModel.AppendNonUserText(s);
            });
            ChatHistory.ViewModel.CurrentMessage = null;
        }

        private async void Prompt_OnKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                await SubmitPrompt();
            }
        }
    }
}
