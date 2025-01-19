namespace SoftTeach.ViewModel.Personen
{
  using System.ComponentModel;
  using System.Windows.Data;

  using Helper;

  using SoftTeach.Model.TeachyModel;

  /// <summary>
  /// ViewModel for managing Personen
  /// </summary>
  public class PersonenWorkspaceViewModel : ViewModelBase
  {
    /// <summary>
    /// Gibt an, ob nur Schüler angezeigt werden.
    /// </summary>
    private bool isShowingSchüler;

    /// <summary>
    /// Gibt an, wonach die Personenliste sortiert wird.
    /// </summary>
    private string sortByItem;

    /// <summary>
    /// The Person currently selected
    /// </summary>
    private PersonViewModel currentPerson;

    /// <summary>
    /// Initialisiert eine neue Instanz der <see cref="PersonenWorkspaceViewModel"/> Klasse. 
    /// </summary>
    public PersonenWorkspaceViewModel()
    {
      this.AddPersonCommand = new DelegateCommand(this.AddPerson);
      this.DeletePersonCommand = new DelegateCommand(this.DeleteCurrentPerson, () => this.CurrentPerson != null);
      this.CurrentPerson = App.MainViewModel.Personen.Count > 0 ? App.MainViewModel.Personen[0] : null;

      // Re-act to any changes from outside this ViewModel
      App.MainViewModel.Personen.CollectionChanged += (sender, e) =>
      {
        if (e.OldItems != null && e.OldItems.Contains(this.CurrentPerson))
        {
          this.CurrentPerson = null;
        }
      };

      this.FilteredPersons = CollectionViewSource.GetDefaultView(App.MainViewModel.Personen);
      this.IsShowingSchüler = true;
      this.FilteredPersons.Filter = this.Personenfilter;
    }

    /// <summary>
    /// Holt den Befehl zur adding a new Person
    /// </summary>
    public DelegateCommand AddPersonCommand { get; private set; }

    /// <summary>
    /// Holt den Befehl zur deleting the current Person
    /// </summary>
    public DelegateCommand DeletePersonCommand { get; private set; }

    /// <summary>
    /// Holt oder setzt die person currently selected in this workspace
    /// </summary>
    public PersonViewModel CurrentPerson
    {
      get
      {
        return this.currentPerson;
      }

      set
      {
        this.currentPerson = value;
        this.DeletePersonCommand.RaiseCanExecuteChanged();
        this.RaisePropertyChanged("CurrentPerson");
      }
    }

    /// <summary>
    /// Holt oder setzt einen Wert, der angibt, ob the table should show schüler
    /// </summary>
    public bool IsShowingSchüler
    {
      get
      {
        return this.isShowingSchüler;
      }

      set
      {
        this.isShowingSchüler = value;
        this.RaisePropertyChanged("IsShowingSchüler");
        this.FilteredPersons.Refresh();
      }
    }

    /// <summary>
    /// Holt einen Wert, der angibt, ob das View nur Lehrer anzeigt
    /// </summary>
    [DependsUpon("IsShowingSchüler")]
    public bool IsShowingLehrer
    {
      get
      {
        return !this.IsShowingSchüler;
      }
    }

    /// <summary>
    /// Holt den header for the person table
    /// </summary>
    [DependsUpon("IsShowingSchüler")]
    public string PersonenÜberschrift
    {
      get { return this.IsShowingSchüler ? "Schülerliste" : "Lehrerliste"; }
    }

    /// <summary>
    /// Holt oder setzt die Filtered persons dependency property which is a subset of
    /// AllPersons to display filtered views of the long list of persons
    /// </summary>
    public ICollectionView FilteredPersons { get; set; }

    /// <summary>
    /// Holt oder setzt einen String der angibt, wonach die Personenliste sortiert wird.
    /// </summary>
    public string SortByItem
    {
      get
      {
        return this.sortByItem;
      }

      set
      {
        this.sortByItem = value;
        this.RaisePropertyChanged("SortByItem");
        this.FilteredPersons.SortDescriptions.Clear();
        this.FilteredPersons.SortDescriptions.Add(new SortDescription("Person" + value, ListSortDirection.Ascending));
        this.FilteredPersons.Refresh();
      }
    }

    /// <summary>
    /// Fügt eine neue Person hinzu.
    /// </summary>
    private void AddPerson()
    {
      var person = new Person();
      var vm = new PersonViewModel(person);
      App.MainViewModel.Personen.Add(vm);
      this.CurrentPerson = vm;
    }

    /// <summary>
    /// Löscht die aktuelle Person
    /// </summary>
    private void DeleteCurrentPerson()
    {
      App.MainViewModel.Personen.RemoveTest(this.CurrentPerson);
      this.CurrentPerson = null;
    }

    /// <summary>
    /// The filter predicate that filters the person table view only showing schüler
    /// </summary>
    /// <param name="de">The <see cref="PersonViewModel"/> that should be filtered</param>
    /// <returns>True if the given object should remain in the list.</returns>
    private bool Personenfilter(object de)
    {
      var person = de as PersonViewModel;

      // Return members depending on property
      return person != null && this.IsShowingSchüler ? person.PersonIstSchüler : person.PersonIstLehrer;
    }
  }
}
