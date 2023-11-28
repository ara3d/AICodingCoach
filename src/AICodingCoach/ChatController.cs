using System.IO;
using Ara3D.Utils;

namespace CodingCanvasWpfApp
{
    public class ChatController
    {
        public ChatControl Control { get; }
        public ChatService Service { get; } = new ChatService();

        public string SystemRolePrompt { get; }

        public ChatController(ChatControl control)
        {
            Control = control;

            try
            {
                SystemRolePrompt = File.ReadAllText(PathUtil.GetCallerSourceFolder().RelativeFile("SystemPrompt.txt"));
            }
            catch
            {
                SystemRolePrompt = "You are a C# coding instructor";
            }
            Service.SendSystemPrompt(SystemRolePrompt);
        }
    }
}
