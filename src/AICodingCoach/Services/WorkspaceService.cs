using AICodingCoach.Models;
using Ara3D.Domo;
using Ara3D.Services;

namespace AICodingCoach.Services
{
    public class WorkspaceService : AggregateModelBackedService<ProjectData>
    {
        public WorkspaceService(IApi api)
            : base(api)
        { }

        public IModel<ProjectData> NewProject(string name = "Test")
        {
            var model = Repository.Add(new ProjectData(name));
            model.AddNonUserMessage($"Hello, I am your AI Coding Coach! 👋");
            return model;   
        }

        public IModel<ProjectData>? FindProject(Guid id)
            => Repository.GetModel(id);
    }

}