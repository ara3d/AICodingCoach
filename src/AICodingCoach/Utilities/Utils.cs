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
            IAggregateRepository<TModel> repo,
            Func<IModel<TModel>, TViewModel> viewModelFromModel,
            Func<TViewModel, Guid> idFromViewModel,
            Action<TViewModel> dispose)
        {
            var hashSet = new HashSet<Guid>();

            // Remove view models no longer in the repo
            foreach (var vm in viewModels.ToList())
            {
                var id = idFromViewModel(vm);
                hashSet.Add(id);
                var model = repo.GetModel(id);
                if (model == null)
                {
                    viewModels.Remove(vm);
                    dispose?.Invoke(vm);
                }
            }

            // Add missing view models 
            foreach (var model in repo.GetModels())
            {
                if (hashSet.Contains(model.Id))
                    continue;
                var vm = viewModelFromModel(model);
                viewModels.Add(vm);
            }
        }
    }
}
