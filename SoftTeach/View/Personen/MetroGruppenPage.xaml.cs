namespace SoftTeach.View.Personen
{
  using SoftTeach.Setting;

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
      Selection.Instance.Lerngruppe.GruppenNeuEinteilenCommand.Execute(null);
    }
  }
}
