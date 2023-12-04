using System.Diagnostics;
using AICodingCoach.Models;
using Ara3D.Domo;
using Ara3D.Utils;

namespace AICodingCoach.Services;

public class ProjectService
{
    public ProjectService(IModel<ProjectData> model)
    {
        Model = model;
        ChatService = new ChatService();
        Commands = new[]
        {
            CopyCodeCommand = new NamedCommand("Copy code", CopyCode),
            CloneCommand = new NamedCommand("Clone", () => Model.Clone()),
            DeleteCommand = new NamedCommand("Delete", () => Model.Delete()),
        };
    }

    public IModel<ProjectData> Model { get; }
    public IModel<MessageData> LatestMessage { get; }
    public ChatService ChatService { get; }

    public INamedCommand CloneCommand { get; }
    public INamedCommand DeleteCommand { get; }
    public INamedCommand CopyCodeCommand { get; }

    public INamedCommand[] Commands { get; }

    public void CopyCode(object? parameter)
    {
        if (parameter is string code)
            Model.AsDynamic().Code = code;
    }

    public async Task SendPromptToChat(string prompt)
    {
        Model.AddUserMessage(prompt);
        await ChatService.SendPromptAsync(prompt, NewTextHandler);
    }

    public static (string, string) SplitAt(string text, int n, int length)
        => (text.Substring(0, n), text.Substring(n + length, text.Length - (n + length)));


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

    public static IReadOnlyList<string> SplitMessageText(string text)
    {
        var lines = text.Split('\n');
        if (lines.Length <= 1)
            return lines;
        var groups = new List<string>();
        var inCode = false;
        foreach (var line in lines)
        {
            if (inCode)
            {
                groups[^1] += line;
            }
            else
            {
                groups.Add(line);
            }

            if (line.StartsWith("```"))
            {
                inCode = !inCode;
            }
        }

        return groups;
    }


    public void NewTextHandler(int _, string text)
    {
        // Don't do anything for null or zero-length strings 
        if (string.IsNullOrEmpty(text))
            return;

        // If there is no message, we will have to create one.
        var latestMessage = Model.Value
            .ChatHistory
            .GetModels()
            .OrderBy(m => m.Value.Time)
            .LastOrDefault();
        if (latestMessage == null || latestMessage.Value.IsUser)
        {
            Model.AddNonUserMessage(text);
            return;
        }

        var curText = latestMessage.Value.Text;

        if (curText.IsNullOrEmpty())
        {
            latestMessage.AsDynamic().Text = text;
            return;
        }

        var newText = curText + text;
        var groups = SplitMessageText(newText);
        if (groups.Count == 0) return;
        latestMessage.AsDynamic().Text = groups[0];
        for (var i = 1; i < groups.Count; ++i)
        {
            Model.AddNonUserMessage(groups[i]);
        }
    }
}