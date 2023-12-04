using System.ComponentModel;
using System.Windows.Input;
using AICodingCoach.Models;
using Ara3D.Domo;
using Ara3D.Utils;

namespace AICodingCoach.ViewModels
{
    public class ChatMessageViewModel : INotifyPropertyChanged, IDisposable
    {
        public IModel<MessageData> Model { get; }
        public ChatViewModel Parent { get; }
        public bool IsUser => Model.Value.IsUser;
        public bool IsCode => Model.Value.IsCode;
        public DateTimeOffset Time => Model.Value.Time;

        public ICommand CopyCodeCommand => Parent.ProjectViewModel.CopyCodeCommand;

        public event PropertyChangedEventHandler? PropertyChanged;

        public ChatMessageViewModel(IModel<MessageData> model, ChatViewModel parent)
        {
            Model = model;
            Parent = parent;
            Model.PropertyChanged += Model_PropertyChanged;
        }

        private void Model_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            PropertyChanged?.Invoke(sender, e);
        }

        public void Dispose()
        {
            Model.PropertyChanged -= Model_PropertyChanged;
            PropertyChanged = null;
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
    }
}