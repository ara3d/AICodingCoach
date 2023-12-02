using System.Diagnostics;
using AICodingCoach.Models;
using Ara3D.Domo;
using Ara3D.Services;
using Ara3D.Utils;

namespace AICodingCoach.Services
{
    /// <summary>
    /// Coordinates all of the services together in a single coherent application.
    /// In a WPF or WinForms application this should be called from the constructor
    /// of the main window. 
    /// </summary>
    public class MainService 
    {
        public Api Api { get; } 
        public ApplicationFolders AppFolders { get; }
        public ApplicationData AppData { get; }
        public SingletonRepository<AppConfigurationModel> AppConfigurationRepo { get; }
        public FilePath SettingsPath { get; }

        public MainService()
        {
            Api = new Api();
            // TODO: this conflicts with the application ID.
            AppData = new ApplicationData("Ara3D", "AICodingCoach", new Version(1, 0));
            AppFolders = new ApplicationFolders(AppData);

            // Create the application data folder if we need to 
            AppFolders.ApplicationData.Create();
            SettingsPath = AppFolders.ApplicationData.RelativeFile("settings.json");
            AppConfigurationRepo = new SingletonRepository<AppConfigurationModel>();

            if (SettingsPath.Exists())
            {
                // We are going to try to load it into the repository.
                var text = SettingsPath.ReadAllText();
                AppConfigurationRepo.LoadFromJson(text);
            }
            else
            {
                // We can safely directly change values in the repo, no one pays attention. 
                AppConfigurationRepo.OnModelChanged(model =>
                {
                    Debug.WriteLine(model.ToJson());
                });

                AppConfigurationRepo.GetDynamicModel().WorkspacesFolder
                    = AppFolders.Documents.RelativeFolder("Workspaces");
                var json = AppConfigurationRepo.ToJson();
                SettingsPath.WriteAllText(json);
            }

            var workspaces = AppConfigurationRepo.Model.Value.WorkspacesFolder;
            workspaces.Create();
        }
    }
}