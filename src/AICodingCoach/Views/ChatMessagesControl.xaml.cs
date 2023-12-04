using System.Collections.Specialized;
using System.Windows;

namespace AICodingCoach.Views
{
    public partial class ChatMessagesControl
    {
        public ChatMessagesControl()
        {
            InitializeComponent();
            var iface = (INotifyCollectionChanged)MessagesItemsCtrl.Items;
            iface.CollectionChanged += MessagesOnCollectionChanged;
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
