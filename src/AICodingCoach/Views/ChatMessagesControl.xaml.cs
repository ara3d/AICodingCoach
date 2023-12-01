using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using AICodingCoach.ViewModels;

namespace AICodingCoach.Views
{
    /// <summary>
    /// Interaction logic for ChatMessagesControl.xaml
    /// </summary>
    public partial class ChatMessagesControl : UserControl
    {
        public ChatMessagesViewModel ViewModel { get; } = new();

        public ChatMessagesControl()
        {
            DataContext = ViewModel;
            InitializeComponent();
            ViewModel.Messages.CollectionChanged += MessagesOnCollectionChanged;
        }

        private void MessagesOnCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                var newIndex = e.NewStartingIndex;
                var newElement = MessagesItemsCtrl.ItemContainerGenerator.ContainerFromIndex(newIndex);
                var item = (FrameworkElement)newElement;
                item?.BringIntoView();
            }
        }
    }
}
