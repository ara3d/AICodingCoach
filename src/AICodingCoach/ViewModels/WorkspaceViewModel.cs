using System.Collections.ObjectModel;
using AICodingCoach.Services;
using Ara3D.Domo;

namespace AICodingCoach.ViewModels
{
    public class WorkspaceViewModel
    {
        public ObservableCollection<ProjectViewModel> Projects { get; } = new();
        public ProjectsService Service { get; }

        public WorkspaceViewModel(ProjectsService service)
        {
            Service = service;
            Service.Repository.RepositoryChanged += ProjectsRepositoryChanged;
        }

        private void ProjectsRepositoryChanged(object? sender, RepositoryChangeArgs e)
        {
            UpdateProjects();
        }

        public void UpdateProjects()
        {
            var models = Service.Repository.GetModels();
            var i = 0;
            while (i < models.Count)
            {
                var p = models[i];
                if (i >= Projects.Count)
                {
                    Projects.Add(new ProjectViewModel(p));
                    i++;
                }
                else
                {
                    if (Projects[i].Model.Id != p.Id)
                    {
                        Projects.RemoveAt(i);
                    }
                    else
                    {
                        i++;
                    }
                }
            }
        }
    }
}