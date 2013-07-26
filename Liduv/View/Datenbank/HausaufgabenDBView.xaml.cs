using System;

namespace Liduv.View.Datenbank
{
  using System.Windows;
  using System.Windows.Data;

  /// <summary>
  /// Interaction logic for HausaufgabenDBView.xaml
  /// </summary>
  public partial class HausaufgabenDBView : Window
  {
    /// <summary>
    /// Initialisiert eine neue Instanz der <see cref="HausaufgabenDBView"/> Klasse. 
    /// </summary>
    public HausaufgabenDBView()
    {
      this.DataContext = this;
      this.InitializeComponent();
    }

    private void OKClick(object sender, RoutedEventArgs e)
    {
      this.Close();
    }
  }
}
