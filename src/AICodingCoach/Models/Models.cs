using Ara3D.Domo;

namespace AICodingCoach.Models
{
    public class ProjectData
    {
        public readonly string Name = "";
        public readonly string Code = "";
        public ProjectData(string name, string code)
        {
            Name = name;
            Code = code;
        }
        public ProjectData() 
        { }
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
