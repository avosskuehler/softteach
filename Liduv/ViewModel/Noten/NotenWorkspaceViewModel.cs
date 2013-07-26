namespace Liduv.ViewModel.Noten
{
  using System;
  using System.ComponentModel;
  using System.Windows.Data;

  using Helper;

  /// <summary>
  /// ViewModel for managing Note
  /// </summary>
  public class NotenWorkspaceViewModel : ViewModelBase
  {
    /// <summary>
    /// The Note currently selected
    /// </summary>
    private NoteViewModel currentNote;

    /// <summary>
    /// Gibt an, ob nur Schüler angezeigt werden.
    /// </summary>
    private bool isMündlich;

    /// <summary>
    /// Initialisiert eine neue Instanz der <see cref="NotenWorkspaceViewModel"/> Klasse. 
    /// </summary>
    /// <param name="noteViewModel"> The note View Model. </param>
    public NotenWorkspaceViewModel(NoteViewModel noteViewModel)
    {
      this.CurrentNote = noteViewModel;

      this.FilteredNotentypen = CollectionViewSource.GetDefaultView(Enum.GetValues(typeof(Notentyp)));
      this.IsMündlich = true;
      this.FilteredNotentypen.Filter = this.Notentypfilter;
    }

    /// <summary>
    /// Holt oder setzt die Filtered persons dependency property which is a subset of
    /// AllPersons to display filtered views of the long list of persons
    /// </summary>
    public ICollectionView FilteredNotentypen { get; set; }

    /// <summary>
    /// Holt oder setzt die Note currently selected in this workspace
    /// </summary>
    public NoteViewModel CurrentNote
    {
      get
      {
        return this.currentNote;
      }

      set
      {
        this.currentNote = value;
        this.RaisePropertyChanged("CurrentNote");
      }
    }

    /// <summary>
    /// Holt oder setzt einen Wert, der angibt, ob the table should show schüler
    /// </summary>
    public bool IsMündlich
    {
      get
      {
        return this.isMündlich;
      }

      set
      {
        this.isMündlich = value;
        this.CurrentNote.NoteNotentyp = this.isMündlich ? Notentyp.MündlichQualität : Notentyp.SchriftlichSonstige;
        this.RaisePropertyChanged("IsMündlich");
        this.FilteredNotentypen.Refresh();
      }
    }

    /// <summary>
    /// The filter predicate that filters the person table view only showing schüler
    /// </summary>
    /// <param name="de">The <see cref="PersonViewModel"/> that should be filtered</param>
    /// <returns>True if the given object should remain in the list.</returns>
    private bool Notentypfilter(object de)
    {
      var notentyp = (Notentyp)de;

      // Return members depending on property
      switch (notentyp)
      {
        case Notentyp.MündlichQualität:
        case Notentyp.MündlichQuantität:
        case Notentyp.MündlichSonstige:
          return this.IsMündlich;
        case Notentyp.SchriftlichSonstige:
          return !this.IsMündlich;
        case Notentyp.MündlichGesamt:
        case Notentyp.SchriftlichGesamt:
        case Notentyp.SchriftlichKlassenarbeit:
          return false;
        default:
          return false;
      }
    }
  }
}
