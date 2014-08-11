namespace Liduv.ViewModel.Sitzpläne
{
  using System;
  using System.Collections.ObjectModel;
  using System.Collections.Specialized;
  using System.Linq;
  using System.Windows.Controls;
  using System.Windows.Forms;
  using System.Windows.Media;
  using System.Windows.Media.Imaging;
  using System.Windows.Shapes;

  using Liduv.Model.EntityFramework;
  using Liduv.UndoRedo;
  using Liduv.ViewModel.Datenbank;
  using Liduv.ViewModel.Helper;

  /// <summary>
  /// ViewModel of an individual Raumplan
  /// </summary>
  public class RaumplanViewModel : ViewModelBase, IComparable
  {
    /// <summary>
    /// The raum currently assigned to this Raumplan
    /// </summary>
    private RaumViewModel raum;

    /// <summary>
    /// Die momentan ausgewählte Sitzplatz
    /// </summary>
    private SitzplatzViewModel currentSitzplatz;

    /// <summary>
    /// Initialisiert eine neue Instanz der <see cref="RaumplanViewModel"/> Klasse. 
    /// </summary>
    /// <param name="raumplan">
    /// The underlying raumplan this ViewModel is to be based on
    /// </param>
    public RaumplanViewModel(Raumplan raumplan)
    {
      if (raumplan == null)
      {
        throw new ArgumentNullException("raumplan");
      }

      this.Model = raumplan;

      this.OpenImageFileCommand = new DelegateCommand(this.OpenImageFile);

      // Build data structures for phasen
      this.Sitzplätze = new ObservableCollection<SitzplatzViewModel>();
      foreach (var sitzplatz in raumplan.Sitzplätze)
      {
        var vm = new SitzplatzViewModel(sitzplatz);
        App.MainViewModel.Sitzplätze.Add(vm);
        this.Sitzplätze.Add(vm);
      }

      this.Sitzplätze.CollectionChanged += this.SitzplätzeCollectionChanged;
    }

    /// <summary>
    /// Holt den Befehl zum Laden einer Raumplandatei
    /// </summary>
    public DelegateCommand OpenImageFileCommand { get; private set; }

    /// <summary>
    /// Holt den Befehl zur Erstellung eines neuen Sitzplatzes
    /// </summary>
    public DelegateCommand AddSitzplatzCommand { get; private set; }

    /// <summary>
    /// Holt den Befehl zur Bearbeitung eines Sitzplatzes
    /// </summary>
    public DelegateCommand EditSitzplatzCommand { get; private set; }

    /// <summary>
    /// Holt den Befehl zur Löschung eines Sitzplatzes
    /// </summary>
    public DelegateCommand DeleteSitzplatzCommand { get; private set; }

    /// <summary>
    /// Holt den underlying Raumplan this ViewModel is based on
    /// </summary>
    public Raumplan Model { get; private set; }

    /// <summary>
    /// Holt oder setzt die Sitzplätze dieser Raumplan
    /// </summary>
    public ObservableCollection<SitzplatzViewModel> Sitzplätze { get; set; }

    /// <summary>
    /// Holt oder setzt die currently selected phase
    /// </summary>
    public SitzplatzViewModel CurrentSitzplatz
    {
      get
      {
        return this.currentSitzplatz;
      }

      set
      {
        this.currentSitzplatz = value;
        this.RaisePropertyChanged("Sitzplatz");
        this.DeleteSitzplatzCommand.RaiseCanExecuteChanged();
      }
    }

    /// <summary>
    /// Holt oder setzt die Bezeichnung
    /// </summary>
    public string RaumplanBezeichnung
    {
      get
      {
        return this.Model.Bezeichnung;
      }

      set
      {
        if (value == this.Model.Bezeichnung) return;
        this.UndoablePropertyChanging(this, "RaumplanBezeichnung", this.Model.Bezeichnung, value);
        this.Model.Bezeichnung = value;
        this.RaisePropertyChanged("RaumplanBezeichnung");
      }
    }

    /// <summary>
    /// Holt oder setzt den Dateinamen für die Raumplanskizze
    /// </summary>
    public string RaumplanDateiname
    {
      get
      {
        return this.Model.Dateiname;
      }

      set
      {
        if (value == this.Model.Dateiname) return;
        this.UndoablePropertyChanging(this, "RaumplanDateiname", this.Model.Dateiname, value);
        this.Model.Dateiname = value;
        this.RaisePropertyChanged("RaumplanDateiname");
      }
    }

    /// <summary>
    /// Holt ein Imagesource für den raumplan
    /// </summary>
    [DependsUpon("RaumplanDateiname")]
    public ImageSource RaumplanImage
    {
      get
      {
        var raumplanImage = new BitmapImage();
        raumplanImage.BeginInit();
        raumplanImage.UriSource = new Uri(this.RaumplanDateiname);
        raumplanImage.EndInit();
        return raumplanImage;
      }
    }

    /// <summary>
    /// Holt oder setzt den Raum für den Raumplan
    /// </summary>
    public RaumViewModel RaumplanRaum
    {
      get
      {
        // We need to reflect any changes made in the model so we check the current value before returning
        if (this.Model.Raum == null)
        {
          return null;
        }

        if (this.raum == null || this.raum.Model != this.Model.Raum)
        {
          this.raum = App.MainViewModel.Räume.SingleOrDefault(d => d.Model == this.Model.Raum);
        }

        return this.raum;
      }

      set
      {
        if (value == null) return;
        if (this.raum != null)
        {
          if (value.RaumBezeichnung == this.raum.RaumBezeichnung) return;
        }

        this.UndoablePropertyChanging(this, "RaumplanRaum", this.raum, value);
        this.raum = value;
        this.Model.Raum = value.Model;
        this.RaisePropertyChanged("RaumplanRaum");
      }
    }

    /// <summary>
    /// Holt die Anzahl der verfügbaren Sitzplätze
    /// </summary>
    public int RaumplanAnzahlSitzplätze
    {
      get
      {
        return this.Sitzplätze.Count();
      }
    }

    /// <summary>
    /// Compares the current object with another object of the same type.
    /// </summary>
    /// <param name="obj">An object to compare with this object.</param>
    /// <returns>A value that indicates the relative order of the objects being compared. </returns>
    public int CompareTo(object obj)
    {
      var otherRaumplanViewModel = obj as RaumplanViewModel;
      if (otherRaumplanViewModel != null)
      {
        return StringLogicalComparer.Compare(this.RaumplanDateiname, otherRaumplanViewModel.RaumplanDateiname);
      }

      throw new ArgumentException("Object is not a RaumplanViewModel");
    }

    /// <summary>
    /// Gibt eine lesbare Repräsentation des ViewModels
    /// </summary>
    /// <returns>Ein <see cref="string"/> mit einer Kurzform des ViewModels.</returns>
    public override string ToString()
    {
      return "Raumplan: " + this.RaumplanDateiname;
    }

    /// <summary>
    /// Öffnet eine Raumplanbilddatei
    /// </summary>
    private void OpenImageFile()
    {
      var ofd = new OpenFileDialog
      {
        CheckFileExists = true,
        CheckPathExists = true,
        AutoUpgradeEnabled = true,
        InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures),
        Multiselect = false,
        Title = "Bitte Raumplangrundriss auswählen"
      };

      if (ofd.ShowDialog() == DialogResult.OK)
      {
        this.RaumplanDateiname = ofd.FileName;
      }
    }

    /// <summary>
    /// Handles addition a new phase to this raumplan
    /// </summary>
    public void AddSitzplatz(Rectangle sitzplatzShape)
    {
      var sitzplatz = new Sitzplatz();
      sitzplatz.LinksObenX = (int)Canvas.GetLeft(sitzplatzShape);
      sitzplatz.LinksObenY = (int)Canvas.GetTop(sitzplatzShape);
      sitzplatz.Breite = sitzplatzShape.Width;
      sitzplatz.Höhe = sitzplatzShape.Height;
      sitzplatz.Raumplan = this.Model;
      var sitzplatzViewModel = new SitzplatzViewModel(sitzplatz);

      using (new UndoBatch(App.MainViewModel, string.Format("Neuer Sitzplatz {0} erstellt.", sitzplatzViewModel), false))
      {
        App.MainViewModel.Sitzplätze.Add(sitzplatzViewModel);
        this.Sitzplätze.Add(sitzplatzViewModel);
        this.CurrentSitzplatz = sitzplatzViewModel;
      }
    }

    /// <summary>
    /// Handles deletion of the given phase
    /// </summary>
    /// <param name="sitzplatzViewModel"> The sitzplatz View Model. </param>
    private void DeleteSitzplatz(SitzplatzViewModel sitzplatzViewModel)
    {
      using (new UndoBatch(App.MainViewModel, string.Format("Sitzplatz {0} gelöscht.", sitzplatzViewModel), false))
      {
        App.MainViewModel.Sitzplätze.RemoveTest(sitzplatzViewModel);
        var result = this.Sitzplätze.RemoveTest(sitzplatzViewModel);
      }
    }

    /// <summary>
    /// Tritt auf, wenn die ModelCollection verändert wurde.
    /// Gibt die Änderungen an den Undostack weiter.
    /// </summary>
    /// <param name="sender">Die auslösende Collection</param>
    /// <param name="e">Die NotifyCollectionChangedEventArgs mit den Infos.</param>
    private void SitzplätzeCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
      this.UndoableCollectionChanged(this, "Sitzplätze", this.Sitzplätze, e, false, "Änderung der Sitzplätze");
    }

    /// <summary>
    /// Löscht alle bestehenden Sitzplätze
    /// </summary>
    public void RemoveAllSitzplätze()
    {
      foreach (var sitzplatzViewModel in this.Sitzplätze)
      {
        this.DeleteSitzplatz(sitzplatzViewModel);
      }
    }
  }
}