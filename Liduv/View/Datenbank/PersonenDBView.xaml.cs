using System;

namespace Liduv.View.Datenbank
{
  using System.Windows;
  using System.Windows.Data;

  /// <summary>
  /// Interaction logic for PersonenDBView.xaml
  /// </summary>
  public partial class PersonenDBView : Window
  {
    /// <summary>
    /// The <see cref="DependencyProperty"/> for the <see cref="FilteredPersons"/>.
    /// Is needed to get updates on the tables once the selections are changed
    /// </summary>
    public static readonly DependencyProperty AllPersonsProperty = DependencyProperty.Register(
      "AllPersons",
      typeof(CollectionView),
      typeof(PersonenDBView),
      new FrameworkPropertyMetadata(null));

    /// <summary>
    /// The backup filter.
    /// </summary>
    private readonly Predicate<object> backupFilter;

    /// <summary>
    /// Initializes a new instance of the <see cref="PersonenDBView"/> class.
    /// </summary>
    public PersonenDBView()
    {
      this.AllPersons = (CollectionView)CollectionViewSource.GetDefaultView(App.MainViewModel.Personen);

      this.backupFilter = this.AllPersons.Filter;

      // Remove filter
      this.AllPersons.Filter = null;
      this.DataContext = this;
      this.InitializeComponent();
    }

    /// <summary>
    /// Holt oder setzt die Filtered persons dependency property which is a subset of
    /// AllPersons to display filtered views of the long list of persons
    /// </summary>
    public CollectionView AllPersons
    {
      get { return (CollectionView)this.GetValue(AllPersonsProperty); }
      set { this.SetValue(AllPersonsProperty, value); }
    }

    private void OKClick(object sender, RoutedEventArgs e)
    {
      this.Close();
    }

    private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
    {
      this.AllPersons.Filter = this.backupFilter;
    }
  }
}
