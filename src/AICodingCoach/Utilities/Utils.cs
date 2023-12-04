using System.Collections.ObjectModel;
using Ara3D.Domo;

namespace AICodingCoach.Utilities
{
    public static class Utils
    {
        /// <summary>
        /// Given an Observable collection of view models, a list of models,
        /// and a function to create a view model from a model, will
        /// synchronize the two lists.
        /// This might eventually go in Domo or some kind of WPF utilities.
        /// </summary>
        public static void SynchronizeObservableCollection<TModel, TViewModel>(
            this ObservableCollection<TViewModel> viewModels,
            IReadOnlyList<TModel> models,
            Func<TModel, TViewModel> func)
            where TModel : IModel
        {
            var i = 0;
            while (i < models.Count)
            {
                var p = models[i];
                if (i >= viewModels.Count)
                {
                    viewModels.Add(func(p));
                    i++;
                }
                else
                {
                    var model = models[i];
                    if (model.Id != p.Id)
                    {
                        viewModels.RemoveAt(i);
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
