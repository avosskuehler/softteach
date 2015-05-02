namespace Liduv.View.Personen
{
  using Liduv.Setting;

  /// <summary>
  /// Interaction logic for MetroGruppenPage.xaml
  /// </summary>
  public partial class MetroGruppenPage
  {
   /// <summary>
    /// Initialisiert eine neue Instanz der <see cref="MetroGruppenPage"/> Klasse.
    /// </summary>
    public MetroGruppenPage()
    {
      this.InitializeComponent();
      Selection.Instance.Schülerliste.GruppenNeuEinteilenCommand.Execute(null);
    }
  }
}
