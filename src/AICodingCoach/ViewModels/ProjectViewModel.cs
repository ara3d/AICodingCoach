using AICodingCoach.Models;
using AICodingCoach.Services;
using AICodingCoach.Utilities;
using Ara3D.Domo;
using Ara3D.Utils;

namespace AICodingCoach.ViewModels
{
    public class ProjectViewModel
    {
        public string Name => Model.Value.Name;
        public string Code => Model.Value.Code;

        public IModel<ProjectData> Model { get; }
        public ChatViewModel ChatViewModel { get; } 
        public ChatService ChatService { get; }

        public INamedCommand CloneCommand { get; }
        public INamedCommand DeleteCommand { get; }
        public INamedCommand CopyCodeCommand { get; }

        public INamedCommand[] Commands { get; }

        public ProjectViewModel(IModel<ProjectData> model, ChatService chatService)
        {
            Model = model;
            ChatService = chatService;
            ChatViewModel = new ChatViewModel(this);

            Commands = new[]
            {
                CopyCodeCommand = new NamedCommand("Copy code", CopyCode),
                CloneCommand = new NamedCommand("Clone", () => Model.Clone()),
                DeleteCommand = new NamedCommand("Delete", () => Model.Delete()),
            };

            Model.Value.ChatHistory.RepositoryChanged += ChatHistory_RepositoryChanged; 
        }

        public void CopyCode(object? parameter)
        {
            if (parameter is string code)
                Model.AsDynamic().Code = code;
        }

        public ChatMessageViewModel ToViewModel(IModel<MessageData> model)
            => new(model, ChatViewModel);

        private void ChatHistory_RepositoryChanged(object? sender, RepositoryChangeArgs e)
        {
            var chatHistory = Model.Value.ChatHistory;
            var models = chatHistory.GetModels();
            ChatViewModel.Messages.SynchronizeObservableCollection(models, ToViewModel,
                vm => vm.Model.Id);
        }

        public async Task SendPromptToChat(string prompt)
        {
            Model.AddUserMessage(prompt);
            await ChatService.SendPromptAsync(prompt, (_, s) =>
            {
                Model.AddNonUserText(s);
            });
        }
    }
}