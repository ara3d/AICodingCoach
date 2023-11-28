using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics.Metrics;
using System.Runtime.CompilerServices;
using System.Windows.Forms;

namespace CodingCanvasWpfApp
{
    public class ChatMessageViewModel : INotifyPropertyChanged
    {
        private string _text;

        public ChatMessageViewModel(bool isUser, bool isCode)
        {
            (IsUser, IsCode) = (isUser, isCode);
        }

        public bool IsUser { get; }
        public bool IsCode { get; }

        public string Text
        {
            get => _text;
            set
            {
                if (value == _text) return;
                _text = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsCode));
            }
        }

        public string DisplayText
        {
            get
            {
                if (IsCode)
                {
                    var start = _text.IndexOf("\n") + 1;
                    if (start <= 0)
                    {
                        return "";
                    }

                    var end = _text.LastIndexOf("```");
                    if (end < 0)
                    {
                        end = _text.Length;
                    }

                    return _text.Substring(start, end - start);
                }
                return _text;
            }
            set
            {
                // Does nothing. Just to allow binding. 
            }
        }

        public DateTimeOffset Time { get; set; } = DateTimeOffset.Now;

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class ChatMessagesViewModel
    {
        public ObservableCollection<ChatMessageViewModel> Messages { get; } = new();

        public ChatMessageViewModel? CurrentMessage { get; set; }

        public void AppendNonUserText(string text)
        {
            // Don't do anything, but we will terminate the current message.  
            if (string.IsNullOrEmpty(text))
            {
                return;
            }

            var codeMarkerIndex = text.IndexOf("```");
            if (codeMarkerIndex >= 0)
            {
                var before = text.Substring(0, codeMarkerIndex);
                var after = text.Substring(codeMarkerIndex + 3);
                // Go to the end of the line after ```. Often you have something like ```csharp\n
                after = after.Substring(after.IndexOf('\n') + 1);

                if (CurrentMessage != null)
                {
                    CurrentMessage.Text += before;
                    if (CurrentMessage.IsCode)
                    {
                        CurrentMessage = null;
                        AppendNonUserText(after.Trim());
                    }
                    else
                    {
                        CurrentMessage = new ChatMessageViewModel(false, true);
                        Messages.Add(CurrentMessage);
                        AppendNonUserText(after);
                    }
                }
                else
                {
                    if (!string.IsNullOrWhiteSpace(before))
                        AppendNonUserText(before);
                    CurrentMessage = new ChatMessageViewModel(false, true);
                    Messages.Add(CurrentMessage);
                    AppendNonUserText(after);
                }
            }

            if (CurrentMessage == null)
            {
                CurrentMessage = new ChatMessageViewModel(false, false);
                Messages.Add(CurrentMessage);
            }
            CurrentMessage.Text += text;
        }

        public void AppendUserText(string text)
        {
            CurrentMessage = new ChatMessageViewModel(true, false)
            {
                Text = text
            };
            Messages.Add(CurrentMessage); 
            CurrentMessage = null;
        }

        public ChatMessagesViewModel()
        {
            AppendNonUserText("Welcome to hell");
            AppendUserText("What do you mean?");
            AppendNonUserText("Don't pretend you don't know!!");
            AppendNonUserText("```csharp\n//Here is some bad code from people you don't like!```");
        }
    }
}
