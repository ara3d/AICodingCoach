using System.Diagnostics;
using AICodingCoach.Models;
using Ara3D.Domo;
using Ara3D.Services;
using Ara3D.Utils;

namespace AICodingCoach.Services
{
    /// <summary>
    /// Coordinates all of the services together in a single coherent application.
    /// In a WPF or WinForms application this should be called after the main window
    /// is constructed. 
    /// </summary>
    public class AppService : SingletonModelBackedService<AppConfigurationData>
    {
        public ApplicationFolders AppFolders { get; }
        public ApplicationData AppData { get; }
        public FilePath SettingsPath { get; }

        public WorkspaceService WorkspaceService { get; }

        public AppService()
            : base(new ServiceManager()) 
        {
            AppData = new ApplicationData("Ara3D", "AICodingCoach", new Version(1, 0));
            AppFolders = new ApplicationFolders(AppData);

            // Create the application data folder if we need to 
            AppFolders.ApplicationData.Create();
            SettingsPath = AppFolders.ApplicationData.RelativeFile("settings.json");

            // TODO: add json serialization support to repositories
            /*
            if (SettingsPath.Exists())
            {
                // We are going to try to load it into the repository.
                var text = SettingsPath.ReadAllText();
                Repository.LoadFromJson(text);
            }
            else
            {
                // We can safely directly change values in the repo, no one pays attention. 
                Repository.OnModelChanged(model =>
                {
                    Debug.WriteLine(model.ToJson());
                });

                Repository.GetDynamicModel().WorkspacesFolder
                    = AppFolders.Documents.RelativeFolder("Workspaces");
                var json = Repository.ToJson();
                SettingsPath.WriteAllText(json);
            }

            var workspaces = Repository.Model.Value.WorkspacesFolder;
            workspaces.Create();
            */


            WorkspaceService = new WorkspaceService(App);
        }
    }
}