namespace AICodingCoach.Models;

public class MessageData
{
    public bool IsUser { get; }
    public bool IsCode => Text.StartsWith("```");
    public DateTimeOffset Time { get; } = DateTimeOffset.Now;
    public string Text { get; } = "";

    public MessageData() 
    { }

    public MessageData(string text, bool isUser)
    {
        Text = text;
        IsUser = isUser;
    }
}