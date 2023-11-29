using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace CodingCanvasWpfApp
{
    public class ChatMessageViewModel : INotifyPropertyChanged
    {
        private string _text;
        public ICommand CopyCodeCommand { get; }

        public ChatMessageViewModel(bool isUser, ICommand copyCodeCommand)
        {
            IsUser = isUser;
            CopyCodeCommand = copyCodeCommand;
        }

        public bool IsUser { get; }
        public bool IsCode => _text.Trim().StartsWith("```");

        public string Text
        {
            get
            {
                var tmp = _text.Trim();
                if (tmp.StartsWith("```"))
                {
                    var index = tmp.IndexOf('\n');
                    tmp = tmp.Substring(index + 1);
                }

                var end = tmp.LastIndexOf("```");
                if (end > 0)
                {
                    tmp = tmp.Substring(0, end).Trim();
                }

                return tmp;
            }
            set
            { }
        }

        public string RawText
        {
            get => _text;
            set
            {
                if (value == _text) return;
                _text = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsCode));
                OnPropertyChanged(nameof(Text));
            }
        }

        public DateTimeOffset Time { get; set; } = DateTimeOffset.Now;

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class CopyCommand : ICommand
    {
        public string Name { get; }
        public Action<object?> Action { get; }

        public CopyCommand(string name, Action<object?> action)
        {
            Name = name;
            Action = action;
        }

        public bool CanExecute(object? parameter)
        {
            return true;
        }

        public void Execute(object? parameter)
        {
            Action(parameter);
        }

        public event EventHandler? CanExecuteChanged;
    }

    public class ChatMessagesViewModel
    {
        public ChatMessagesViewModel()
        {
            CopyCodeCommand = new CopyCommand("Copy", CopyCodeMethod);
            AppendNonUserText("Welcome home!");
            AppendUserText("Thank you!");
            AppendNonUserText("So here is some code:");
            AppendNonUserText("```csharp\npublic int Test(int x)\n  => x + 1;```");
        }

        public Action<string>? OnCopyCode { get; set; }

        public void CopyCodeMethod(object? parameter)
        {
            if (parameter is string code)
                OnCopyCode?.Invoke(code);
        }

        public ICommand CopyCodeCommand { get; }

        public ObservableCollection<ChatMessageViewModel> Messages { get; } = new();

        public ChatMessageViewModel? CurrentMessage { get; set; }

        public static (string, string) SplitAt(string text, int n, int length)
        {
            return (text.Substring(0, n), text.Substring(n + length, text.Length - (n + length)));
        }

        public static List<string> Split(string text)
        {
            var r = new List<string>();
            while (text.Length > 0)
            {
                var n = text.IndexOf("```", StringComparison.Ordinal);
                if (n == 0)
                {
                    var end = text.IndexOf("```", 3);

                    if (end >= 0)
                    {
                        var before = text.Substring(0, end + 3);
                        r.Add(before);
                        text = text.Substring(end + 3);
                    }
                    else
                    {
                        r.Add(text);
                        return r;
                    }
                }
                else if (n > 0)
                {
                    var (before, after) = SplitAt(text, n, 0);
                    Debug.Assert(before.Length > 0);
                    r.Add(before);
                    text = after;
                }
                else
                {
                    r.Add(text);
                    return r;
                }
            }

            return r;
        }

        public void AppendNonUserText(string text)
        {
            // Don't do anything for null or zero-length strings 
            if (string.IsNullOrEmpty(text))
            {
                return;
            }
            // If there is no message, we will have to create one. 
            if (CurrentMessage == null)
            {
                CurrentMessage = new ChatMessageViewModel(false, CopyCodeCommand);
                Messages.Add(CurrentMessage);
            }
            // Add text to the current message
            CurrentMessage.RawText += text;

            // It might warrant being split up, so try that. 
            var tmp = Split(CurrentMessage.RawText);
            if (tmp.Count > 1)
            {
                // Remove the last message 
                Messages.RemoveAt(Messages.Count - 1);

                // Now add all of the split messages
                for (var i = 0; i < tmp.Count; i++)
                {
                    CurrentMessage = new ChatMessageViewModel(CurrentMessage.IsUser, CopyCodeCommand) { RawText = tmp[i] };
                    Messages.Add(CurrentMessage);
                }
            }
        }

        public void AppendUserText(string text)
        {
            CurrentMessage = new ChatMessageViewModel(true, CopyCodeCommand)
            {
                RawText = text
            };
            Messages.Add(CurrentMessage); 
            CurrentMessage = null;
        }
    }
}
