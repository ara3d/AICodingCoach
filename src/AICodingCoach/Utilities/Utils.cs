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
            Func<TModel, TViewModel> viewModelFromModel,
            Func<TViewModel, Guid> idFromViewModel)
            where TModel : IModel
        {
            var i = 0;
            while (i < models.Count)
            {
                var model = models[i];
                if (i >= viewModels.Count)
                {
                    viewModels.Add(viewModelFromModel(model));
                    i++;
                }
                else
                {
                    var viewModel = viewModels[i];
                    var id = idFromViewModel(viewModel);
                    if (model.Id != id)
                    {
                        viewModels.RemoveAt(i);
                    }
                    else
                    {
                        i++;
                    }
                }
            }

            while (i < viewModels.Count)
            {
                viewModels.RemoveAt(i);
            }
        }
    }
}
