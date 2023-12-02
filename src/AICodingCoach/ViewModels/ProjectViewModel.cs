using System.Diagnostics;
using AICodingCoach.Models;
using Ara3D.Domo;
using Ara3D.Utils;

namespace AICodingCoach.ViewModels
{
    public class ProjectViewModel
    {
        public IModel<ProjectData> Model { get; }

        public INamedCommand Clone { get; }
        public INamedCommand Delete { get; }

        public ProjectViewModel(IModel<ProjectData> model)
        {
            Model = model;
            Debug.WriteLine($"{model.Value.Name} {model.Value.Code}");
            var tmp = model.AsDynamic();
            Debug.WriteLine($"{tmp.Name} {tmp.Code}");
        }
    }
}