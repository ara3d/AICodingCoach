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
        var systemPrompt = App.SourceFolder.RelativeFile("SystemPrompt.txt").ReadAllText();
        ChatService = new ChatService(systemPrompt);
        Commands = new[]
        {
            CopyCodeCommand = new NamedCommand("Copy code", CopyCode),
            CloneCommand = new NamedCommand("Clone", () => Model.Clone()),
            DeleteCommand = new NamedCommand("Delete", () => Model.Delete()),
        };
    }

    public const string CodeMarker = "```";

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

    public static IReadOnlyList<string> SplitMessageText(string text)
    {
        var r = new List<string>();
        if (text.IsNullOrEmpty())
            return r;

        var codeStart = text.IndexOf(CodeMarker, StringComparison.Ordinal);
        if (codeStart == 0)
        {
            var end = text.IndexOf(CodeMarker, CodeMarker.Length, StringComparison.Ordinal);
            if (end < 0)
            {
                r.Add(text);
                return r;
            }
            else
            {
                var code = text.Substring(0, end + 3);
                r.Add(code);
                var rest = text.Substring(end + 3);
                var splitRest = SplitMessageText(rest);
                if (splitRest.Count == 0)
                {
                    // Make sure we start a new message. Even if there is no more code. 
                    r.Add("");
                }
                else
                {
                    // Add everything else that got split. 
                    r.AddRange(splitRest);
                }
            }
        }
        else if (codeStart > 0)
        {
            var prefix = text.Substring(0, codeStart);
            r.AddRange(SplitMessageText(prefix));
            var rest = text.Substring(codeStart);
            r.AddRange(SplitMessageText(rest));
        }
        else
        {
            Debug.Assert(!text.Contains(CodeMarker));

            // Not in code, so we split at line markers. 
            r.AddRange(text.Split('\n', StringSplitOptions.RemoveEmptyEntries));
            
            // An ending line marker triggers creating a new message 
            if (text.EndsWith("\n"))
                r.Add("");
        }

        return r;
    }


    public void NewTextHandler(int _, string text)
    {
        // Don't do anything for null or zero-length strings 
        if (string.IsNullOrEmpty(text))
            return;

        // Get the most recent message 
        var latestMessage = Model.Value
            .ChatHistory
            .GetModels()
            .MaxBy(m => m.Value.Time);

        // If there is no message, or last one was from the user,
        // we will have to create one.
        if (latestMessage == null || latestMessage.Value.IsUser)
        {
            latestMessage = Model.Value.ChatHistory.Add(new MessageData("", false));
        }

        var curText = latestMessage.Value.Text;
        var newText = curText + text;
        var groups = SplitMessageText(newText);
        if (groups.Count == 0) 
            return;
        latestMessage.AsDynamic().Text = groups[0];
        for (var i = 1; i < groups.Count; ++i)
        {
            Model.AddNonUserMessage(groups[i]);
        }
    }
}