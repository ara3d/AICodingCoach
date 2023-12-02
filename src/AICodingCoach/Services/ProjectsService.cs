using AICodingCoach.Models;
using Ara3D.Domo;
using Ara3D.Services;

namespace AICodingCoach.Services
{
    public class ProjectsService : AggregateModelBackedService<ProjectData>
    {
        public ProjectsService(IApi api)
            : base(api)
        { }

        public IModel<ProjectData> NewProject()
            => Repository.Add();

        public IModel<ProjectData>? FindProject(Guid id)
            => Repository.GetModel(id);
    }

    public static class ProjectExtensions
    {
        public static void Rename(this IModel<ProjectData> project, string name)
            => project.AsDynamic().Name = name;

        public static void UpdateCode(this IModel<ProjectData> project, string code)
            => project.AsDynamic().Code = code;
    }
}