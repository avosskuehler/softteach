namespace SoftTeach.View.Termine
{
  using System.Collections.Generic;
  using System.Collections.ObjectModel;
  using System.ComponentModel;
  using System.Windows;
  using System.Windows.Controls;
  using System.Windows.Data;
  using System.Windows.Media;

  using SoftTeach.ViewModel.Datenbank;
  using SoftTeach.ViewModel.Personen;

  /// <summary>
  /// Interaction logic for BetroffeneKlassenDialog.xaml
  /// </summary>
  public partial class BetroffeneKlassenDialog : Window
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="BetroffeneKlassenDialog"/> class.
    /// </summary>
    public BetroffeneKlassenDialog(SchuljahrViewModel schuljahr)
    {
      this.InitializeComponent();
      this.DataContext = this;
      this.Schuljahr = schuljahr;
      App.MainViewModel.LoadLerngruppen();

      this.Lerngruppen = new List<LerngruppenVorlage>();
      foreach (var lerngruppe in App.MainViewModel.Lerngruppen)
      {
        this.Lerngruppen.Add(new LerngruppenVorlage(lerngruppe.Model));
      }

      this.LerngruppenViewSource = new CollectionViewSource() { Source = this.Lerngruppen };
      using (this.LerngruppenViewSource.DeferRefresh())
      {
        this.LerngruppenViewSource.Filter += this.LerngruppenViewSource_Filter;
        this.LerngruppenViewSource.SortDescriptions.Add(new SortDescription("LerngruppeFach.FachBezeichnung", System.ComponentModel.ListSortDirection.Ascending));
        this.LerngruppenViewSource.SortDescriptions.Add(new SortDescription("LerngruppeSchuljahr.SchuljahrJahr", ListSortDirection.Ascending));
        this.LerngruppenViewSource.SortDescriptions.Add(new SortDescription("LerngruppeJahrgang", ListSortDirection.Ascending));
      }
    }

    /// <summary>
    /// Holt oder setzt die JahrespläneViewSource
    /// </summary>
    public List<LerngruppenVorlage> Lerngruppen { get; set; }

    /// <summary>
    /// Holt oder setzt die JahrespläneViewSource
    /// </summary>
    public CollectionViewSource LerngruppenViewSource { get; set; }

    /// <summary>
    /// Holt oder setzt ein gefiltertes View der Lerngruppen
    /// </summary>
    public ICollectionView LerngruppenView => this.LerngruppenViewSource.View;


    public SchuljahrViewModel Schuljahr { get; set; }

    /// <summary>
    /// Filtert die Lerngruppen nach Schuljahr und Termintyp
    /// </summary>
    /// <param name="item">Die Lerngruppe, das gefiltert werden soll</param>
    /// <returns>True, wenn das Objekt in der Liste bleiben soll.</returns>
    private void LerngruppenViewSource_Filter(object sender, FilterEventArgs e)
    {
      var lerngruppeViewModel = e.Item as LerngruppenVorlage;
      if (lerngruppeViewModel == null)
      {
        e.Accepted = false;
        return;
      }

      if (this.Schuljahr != null)
      {
        if (lerngruppeViewModel.LerngruppeSchuljahr.SchuljahrBezeichnung != this.Schuljahr.SchuljahrBezeichnung)
        {
          e.Accepted = false;
          return;
        }
      }

      e.Accepted = true;
      return;
    }

    /// <summary>
    /// Der event handler für den Nein Button. Setzt DialogResult=false
    /// und beendet den Dialog.
    /// </summary>
    /// <param name="sender">Source of the event</param>
    /// <param name="e">An empty <see cref="RoutedEventArgs"/></param>
    private void CancelButtonClick(object sender, RoutedEventArgs e)
    {
      this.DialogResult = false;
      this.Close();
    }

    private void AusgewählteButtonClick(object sender, RoutedEventArgs e)
    {
      this.DialogResult = true;
      this.Close();
    }

    private void AlleButtonClick(object sender, RoutedEventArgs e)
    {
      foreach (var lg in this.Lerngruppen)
      {
        lg.IstBetroffen = true;
      }

      this.DialogResult = true;
      this.Close();
    }
  }
}
