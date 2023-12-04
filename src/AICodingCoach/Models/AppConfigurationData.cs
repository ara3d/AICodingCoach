using Ara3D.Utils;

namespace AICodingCoach.Models
{
    public class AppConfigurationData
    {
        public string Mode { get; set; } = "Development";
        public DirectoryPath WorkspacesFolder { get; set; }
    }
}