using System.Collections.ObjectModel;
using AICodingCoach.Models;
using AICodingCoach.Utilities;
using Ara3D.Domo;

namespace AICodingCoach.ViewModels
{
    public class ChatViewModel : IDisposable
    {
        public IAggregateRepository<MessageData> ChatHistoryRepository
            => ProjectViewModel.Model.Value.ChatHistory;

        public ChatViewModel(ProjectViewModel projectViewModel)
        {
            ProjectViewModel = projectViewModel;
            ChatHistoryRepository.RepositoryChanged += ChatHistory_RepositoryChanged; 
        }

        public void Dispose()
        {
            ChatHistoryRepository.RepositoryChanged -= ChatHistory_RepositoryChanged;
        }

        private void ChatHistory_RepositoryChanged(object? sender, RepositoryChangeArgs e)
        {
            Messages.SynchronizeObservableCollection(ChatHistoryRepository,
                md => new ChatMessageViewModel(md, this),
                vm => vm.Model.Id,
                vm => { });
        }

        public ObservableCollection<ChatMessageViewModel> Messages { get; } = new();

        public ProjectViewModel ProjectViewModel { get; }
    }
}
