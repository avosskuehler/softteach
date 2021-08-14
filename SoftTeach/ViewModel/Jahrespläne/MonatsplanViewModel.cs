namespace SoftTeach.ViewModel.Jahrespläne
{
  using System;
  using System.Collections.ObjectModel;
  using System.Collections.Specialized;
  using System.Linq;

  using SoftTeach.Model.EntityFramework;
  using SoftTeach.ViewModel.Datenbank;
  using SoftTeach.ViewModel.Helper;

  /// <summary>
  /// ViewModel of an individual monatsplan
  /// </summary>
  public class MonatsplanViewModel : ViewModelBase
  {
    /// <summary>
    /// The monatstyp currently assigned to this monatsplan
    /// </summary>
    private MonatstypViewModel monatstyp;

    /// <summary>
    /// The tagesplan currently selected
    /// </summary>
    private TagesplanViewModel currentTagesplan;

    /// <summary>
    /// Initialisiert eine neue Instanz der <see cref="MonatsplanViewModel"/> Klasse. 
    /// </summary>
    /// <param name="monatsplan">
    /// The underlying monatsplan this ViewModel is to be based on
    /// </param>
    public MonatsplanViewModel(Monatsplan monatsplan)
    {
      if (monatsplan == null)
      {
        throw new ArgumentNullException("monatsplan");
      }

      this.Model = monatsplan;
      this.Tagespläne = new ObservableCollection<TagesplanViewModel>();
      foreach (var tagesplan in monatsplan.Tagespläne)
      {
        var vm = new TagesplanViewModel(tagesplan);
        //App.MainViewModel.Tagespläne.Add(vm);
        this.Tagespläne.Add(vm);
      }

      this.Tagespläne.CollectionChanged += this.TagespläneCollectionChanged;
    }

    /// <summary>
    /// Holt den underlying Monatsplan this ViewModel is based on
    /// </summary>
    public Monatsplan Model { get; private set; }

    /// <summary>
    /// Holt oder setzt die currently selected tagesplan
    /// </summary>
    public TagesplanViewModel CurrentTagesplan
    {
      get
      {
        return this.currentTagesplan;
      }

      set
      {
        this.currentTagesplan = value;
        this.RaisePropertyChanged("CurrentTagesplan");
      }
    }

    /// <summary>
    /// Holt oder setzt die fach currently assigned to this Monatsplan
    /// </summary>
    public MonatstypViewModel MonatsplanMonatstyp
    {
      get
      {
        // We need to reflect any changes made in the model so we check the current value before returning
        if (this.Model.Monatstyp == null)
        {
          return null;
        }

        if (this.monatstyp == null || this.monatstyp.Model != this.Model.Monatstyp)
        {
          this.monatstyp = App.MainViewModel.Monatstypen.SingleOrDefault(d => d.Model == this.Model.Monatstyp);
        }

        return this.monatstyp;
      }

      set
      {
        if (value.MonatstypBezeichnung == this.monatstyp.MonatstypBezeichnung) return;
        this.UndoablePropertyChanging(this, "MonatsplanMonatstyp", this.monatstyp, value);
        this.monatstyp = value;
        this.Model.Monatstyp = value.Model;
        this.RaisePropertyChanged("MonatsplanMonatstyp");
      }
    }

    /// <summary>
    /// Holt den year for the current monatsplan
    /// </summary>
    public int MonatsplanJahr
    {
      get
      {
        return this.Model.Halbjahresplan.Jahresplan.Jahrtyp.Jahr;
      }
    }

    /// <summary>
    /// Holt den index of the month for the current monatsplan
    /// </summary>
    [DependsUpon("MonatsplanMonatstyp")]
    public int MonatsplanMonatindex
    {
      get
      {
        return this.Model.Monatstyp.MonatIndex;
      }
    }

    /// <summary>
    /// Holt den Tagespläne for this monatsplan
    /// </summary>
    public ObservableCollection<TagesplanViewModel> Tagespläne { get; private set; }

    /// <summary>
    /// Gibt eine lesbare Repräsentation des ViewModels
    /// </summary>
    /// <returns>Ein <see cref="string"/> mit einer Kurzform des ViewModels.</returns>
    public override string ToString()
    {
      return "Monatsplan: " + this.MonatsplanMonatstyp.MonatstypBezeichnung;
    }

    /// <summary>
    /// Tritt auf, wenn die TagespläneCollection verändert wurde.
    /// Gibt die Änderungen an den Undostack weiter.
    /// </summary>
    /// <param name="sender">Die auslösende Collection</param>
    /// <param name="e">Die NotifyCollectionChangedEventArgs mit den Infos.</param>
    private void TagespläneCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
      this.UndoableCollectionChanged(this, "Tagespläne", this.Tagespläne, e, true, "Änderung der Tagespläne");
    }
  }
}
