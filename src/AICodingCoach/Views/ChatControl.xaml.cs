using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using AICodingCoach.ViewModels;

namespace AICodingCoach.Views
{
    public partial class ChatControl : UserControl
    {
        public ChatViewModel? ViewModel => DataContext as ChatViewModel;

        public ChatControl()
        {
            InitializeComponent();
        }

        private async void SubmitButton_OnClick(object sender, RoutedEventArgs e)
        {
            await SubmitPrompt();
        }

        public async Task SubmitPrompt()
        {
            if (ViewModel == null) return;
            var prompt = Prompt.Text;
            Prompt.Clear();
            await ViewModel.ProjectViewModel.SendPromptToChat(prompt);
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
