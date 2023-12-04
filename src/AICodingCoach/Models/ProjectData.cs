using Ara3D.Domo;

namespace AICodingCoach.Models
{
    public class ProjectData
    {
        public readonly string Name = "";
        public readonly string Code = "";
        public readonly AggregateRepository<MessageData> ChatHistory = new();

        public ProjectData(string name, string code = "")
        {
            Name = name;
            Code = code;
        }

        public ProjectData() 
        { }
    }
}
