using Ara3D.Domo;
using Ara3D.Utils;

namespace AICodingCoach.Models
{
    public class AppConfigurationModel
    {
        public string Mode { get; set; } = "Development";
        public DirectoryPath WorkspacesFolder { get; set; }
    }
}