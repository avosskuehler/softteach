namespace Liduv.View.Main
{
  using System.Security.RightsManagement;
  using System.Windows;
  using System.Windows.Controls;

  using Liduv.Properties;
  using Liduv.Setting;
  using Liduv.View.Noten;
  using Liduv.View.Wochenpläne;

  /// <summary>
  /// Interaction logic for LandingPage.xaml
  /// </summary>
  public partial class LandingPage : Page
  {
    /// <summary>
    /// The desktop view
    /// </summary>
    private MainRibbonView desktopView;

    /// <summary>
    /// Initializes a new instance of the <see cref="LandingPage"/> class.
    /// </summary>
    public LandingPage()
    {
      this.InitializeComponent();
    }

    private void WochenplanOnClick(object sender, RoutedEventArgs e)
    {
      Configuration.Instance.IsMetroMode = true;
      this.NavigationService.Navigate(new MetroWochenplanPage());
    }

    private void TagesplanOnClick(object sender, RoutedEventArgs e)
    {
      Configuration.Instance.IsMetroMode = true;
      this.NavigationService.Navigate(new MetroTagesplanPage());
    }

    private void DesktopOnClick(object sender, RoutedEventArgs e)
    {
      if (this.desktopView == null || !this.desktopView.IsLoaded)
      {
        this.desktopView = new MainRibbonView();
        this.desktopView.Show();
        Configuration.Instance.IsMetroMode = false;
      }
      else
      {
        this.desktopView.Show();
        this.desktopView.Activate();
      }
    }

    private void LandingPage_OnLoaded(object sender, RoutedEventArgs e)
    {
      Configuration.Instance.NavigationService = this.NavigationService;
    }

    private void NotenOnClick(object sender, RoutedEventArgs e)
    {
      Configuration.Instance.IsMetroMode = true;
      Configuration.Instance.NavigateTarget = NavigateTarget.Noten;
      this.NavigationService.Navigate(new MetroSelectSchülerlistePage());
    }

    private void GruppenOnClick(object sender, RoutedEventArgs e)
    {
      Configuration.Instance.IsMetroMode = true;
      Configuration.Instance.NavigateTarget = NavigateTarget.Gruppen;
      this.NavigationService.Navigate(new MetroSelectSchülerlistePage());
    }

    private void SitzpläneOnClick(object sender, RoutedEventArgs e)
    {
      Configuration.Instance.IsMetroMode = true;
      Configuration.Instance.NavigateTarget = NavigateTarget.Sitzpläne;
      this.NavigationService.Navigate(new MetroSelectSchülerlistePage());
    }

    /// <summary>
    /// Speichern der Datenbank.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
    private void SpeichernOnClick(object sender, RoutedEventArgs e)
    {
      Selection.Instance.UpdateUserSettings();
      Settings.Default.Save();
      App.UnitOfWork.SaveChanges();
    }
  }
}
