namespace SoftTeach.View.Main
{
  using System.Windows;
  using System.Windows.Controls;

  using SoftTeach.Properties;
  using SoftTeach.Setting;
  using SoftTeach.View.Noten;
  using SoftTeach.View.Wochenpläne;

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
      }
      else
      {
        this.desktopView.Show();
        this.desktopView.Activate();
      }

      Configuration.Instance.IsMetroMode = false;
    }

    private void LandingPage_OnLoaded(object sender, RoutedEventArgs e)
    {
      Configuration.Instance.NavigationService = this.NavigationService;
    }

    private void NotenOnClick(object sender, RoutedEventArgs e)
    {
      Configuration.Instance.IsMetroMode = true;
      Configuration.Instance.NavigateTarget = NavigateTarget.Noten;
      this.NavigationService.Navigate(new MetroSelectLerngruppePage());
    }

    private void GruppenOnClick(object sender, RoutedEventArgs e)
    {
      Configuration.Instance.IsMetroMode = true;
      Configuration.Instance.NavigateTarget = NavigateTarget.Gruppen;
      //var schülerlisten = App.MainViewModel.LerngruppeWorkspace;
      this.NavigationService.Navigate(new MetroSelectLerngruppePage());
    }

    private void SitzpläneOnClick(object sender, RoutedEventArgs e)
    {
      Configuration.Instance.IsMetroMode = true;
      Configuration.Instance.NavigateTarget = NavigateTarget.Sitzpläne;
      App.MainViewModel.LoadSitzpläne();
      //var schülerlisten = App.MainViewModel.LerngruppeWorkspace;
      //var sitzpläne = App.MainViewModel.SitzplanWorkspace;
      this.NavigationService.Navigate(new MetroSelectLerngruppePage());
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

    private void ExitOnClick(object sender, RoutedEventArgs e)
    {
      App.MainViewModel.SaveCommand.Execute(null);

      App.Current.Shutdown();
    }
  }
}
