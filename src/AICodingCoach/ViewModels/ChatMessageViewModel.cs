using System.ComponentModel;
using System.Windows.Input;
using AICodingCoach.Models;
using Ara3D.Domo;

namespace AICodingCoach.ViewModels
{
    public class ChatMessageViewModel : INotifyPropertyChanged
    {
        public IModel<MessageData> Model { get; }
        public ChatViewModel Parent { get; }

        public ICommand CopyCodeCommand => Parent.ProjectViewModel.CopyCodeCommand;
        public bool IsUser => Model.Value.IsUser;
        public bool IsCode => Model.Value.Text.Trim().StartsWith("```");
        public DateTimeOffset Time => Model.Value.TimeCreated;

        public event PropertyChangedEventHandler? PropertyChanged;

        public ChatMessageViewModel(IModel<MessageData> model, ChatViewModel parent)
        {
            Model = model;
            Parent = parent;
            Model.Repository.RepositoryChanged += Repository_RepositoryChanged;
        }

        private void Repository_RepositoryChanged(object? sender, RepositoryChangeArgs e)
        {
            OnPropertyChanged();    
        }

        public string Text
        {
            get
            {
                var tmp = Model.Value.Text.Trim();
                if (tmp.StartsWith("```"))
                {
                    var index = tmp.IndexOf('\n');
                    tmp = tmp.Substring(index + 1);
                }

                var end = tmp.LastIndexOf("```", StringComparison.Ordinal);
                if (end > 0)
                {
                    tmp = tmp.Substring(0, end).Trim();
                }

                return tmp;
            }
            // ReSharper disable once ValueParameterNotUsed
            set
            { }
        }

        protected void OnPropertyChanged()
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(null));
        }
    }
}