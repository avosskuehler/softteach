namespace Liduv.ViewModel.Helper.PushBinding
{
  using System.Windows;
  using System.Collections.Specialized;

  public class PushBindingCollection : FreezableCollection<PushBinding>
    {
        public PushBindingCollection() { }

        public PushBindingCollection(DependencyObject targetObject)
        {
            this.TargetObject = targetObject;
            ((INotifyCollectionChanged)this).CollectionChanged += this.CollectionChanged;
        }

        void CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (PushBinding pushBinding in e.NewItems)
                {
                    pushBinding.SetupTargetBinding(this.TargetObject);
                }
            }
        }

        public DependencyObject TargetObject
        {
            get;
            private set;
        }
    }
}
