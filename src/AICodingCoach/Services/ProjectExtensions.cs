using System.Diagnostics;
using AICodingCoach.Models;
using Ara3D.Domo;

namespace AICodingCoach.Services;

public static class ProjectExtensions
{
    public static void SetName(this IModel<ProjectData> project, string name)
        => project.AsDynamic().Name = name;

    public static void SetCode(this IModel<ProjectData> project, string code)
        => project.AsDynamic().Code = code;

    public static void AddMessage(this IModel<ProjectData> project, MessageData msg)
        => project.Value.ChatHistory.Add(msg);

    public static void AddNonUserMessage(this IModel<ProjectData> project, string text)
        => project.AddMessage(new MessageData(text, false));

    public static void AddUserMessage(this IModel<ProjectData> project, string text)
        => project.AddMessage(new MessageData(text, true));

    public static void AddText(this IModel<MessageData> message, string text)
        => message.AsDynamic().Text += text;

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

    public static void AddNonUserText(this IModel<ProjectData> project, string text)
    {
        // Don't do anything for null or zero-length strings 
        if (string.IsNullOrEmpty(text))
            return;

        // If there is no message, we will have to create one.
        var latestMessage = project.Value.ChatHistory.GetModels().LastOrDefault();
        if (latestMessage == null || latestMessage.Value.IsUser)
        {
            project.AddNonUserMessage(text);
            return;
        }

        latestMessage.AddText(text);
    }
}