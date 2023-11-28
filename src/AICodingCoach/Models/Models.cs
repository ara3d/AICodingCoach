namespace CodingCanvasWpfApp.Models
{
    public class WorkspaceModel
    {
        public List<ProjectModel> Projects = new List<ProjectModel>();
        public string Folder; 
    }

    public class ApplicationSettingsModel
    {
        public string Personality;
    }

    public class ProjectModel
    {
        public string Name;
        public string FolderPath;
        public ConversationModel Conversation;
        public CodeModel Code;
    }

    public class MessageModel
    {
        public string Role;
        public DateTimeOffset TimeCreated;
        public string RawContent;
        public string InterpolatedContent;
        public string DisplayedContent;
    }

    public class ConversationModel
    {
        public List<MessageModel> Messages = new List<MessageModel>();
    }

    public class CodeModel
    {
        public string Text;
        public DateTimeOffset LastModified;
        public CompilationModel Compilation;
    }

    public class CompilationModel
    {
        public bool Success;
        public string AssemblyFilePath;
        public DateTimeOffset AssemblyDateTime;
        public long AssemblySize;
        public List<string> Diagnostics = new List<string>(); 
    }
}
