using Ara3D.Domo;

namespace AICodingCoach.Models;

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

    public static void AddSeparator(this IModel<ProjectData> project)
        => project.AddMessage(new MessageData(null, false));

}