using AICodingCoach.Models;
using AICodingCoach.Services;
using AICodingCoach.Utilities;
using Ara3D.Domo;
using Ara3D.Utils;

namespace AICodingCoach.ViewModels
{
    public class ProjectViewModel : IDisposable
    {
        public ProjectService ProjectService { get; }
        public string Name => Model.Value.Name;
        public IModel<ProjectData> Model => ProjectService.Model;

        public ChatViewModel ChatViewModel { get; private set; } 

        public INamedCommand CloneCommand => ProjectService.CloneCommand;
        public INamedCommand DeleteCommand => ProjectService.DeleteCommand;
        public INamedCommand CopyCodeCommand => ProjectService.CopyCodeCommand;

        public ProjectViewModel(ProjectService projectService)
        {
            ProjectService = projectService;
            ChatViewModel = new ChatViewModel(this);
            Model.Value.ChatHistory.RepositoryChanged += ChatHistory_RepositoryChanged;
        }

        public ChatMessageViewModel ToViewModel(IModel<MessageData> model)
            => new(model, ChatViewModel);

        private void ChatHistory_RepositoryChanged(object? sender, RepositoryChangeArgs e)
        {
            ChatViewModel.Messages.SynchronizeObservableCollection(
                Model.Value.ChatHistory, 
                ToViewModel,
                vm => vm.Model.Id,
                vm => vm.Dispose());
        }

        public void Dispose()
        {
            Model.Value.ChatHistory.RepositoryChanged -= ChatHistory_RepositoryChanged;
            ChatViewModel?.Dispose();
            ChatViewModel = null;
        }
    }
}