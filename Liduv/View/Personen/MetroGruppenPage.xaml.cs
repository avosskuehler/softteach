namespace Liduv.View.Personen
{
  using System.Collections.Generic;
  using System.Windows;
  using System.Windows.Controls;
  using System.Windows.Input;
  using System.Windows.Media;

  using Liduv.Setting;
  using Liduv.ViewModel.Noten;
  using Liduv.ViewModel.Personen;

  using OxyPlot.Wpf;

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
