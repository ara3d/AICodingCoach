using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace AICodingCoach.ViewModels;

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