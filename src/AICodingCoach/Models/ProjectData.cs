using Ara3D.Domo;

namespace AICodingCoach.Models
{
    public class ProjectData
    {
        public string Name { get; set; } = "";
        public string Code { get; set; } = "";
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
