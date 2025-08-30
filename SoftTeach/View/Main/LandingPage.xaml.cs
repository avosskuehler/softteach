namespace SoftTeach.View.Main
{
  using SoftTeach.Properties;
  using SoftTeach.Setting;
  using SoftTeach.View.Noten;
  using SoftTeach.View.Wochenpläne;
  using System.Collections.Generic;

  /// <summary>
  /// Interaction logic for LandingPage.xaml
  /// </summary>
  public partial class LandingPage : System.Windows.Controls.Page
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

    private void WochenplanOnClick(object sender, System.Windows.RoutedEventArgs e)
    {
      Configuration.Instance.IsMetroMode = true;
      this.NavigationService.Navigate(new MetroWochenplanPage());
    }

    private void TagesplanOnClick(object sender, System.Windows.RoutedEventArgs e)
    {
      Configuration.Instance.IsMetroMode = true;
      this.NavigationService.Navigate(new MetroTagesplanPage());
    }

    private void DesktopOnClick(object sender, System.Windows.RoutedEventArgs e)
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

    private void LandingPage_OnLoaded(object sender, System.Windows.RoutedEventArgs e)
    {
      Configuration.Instance.NavigationService = this.NavigationService;
    }

    private void NotenOnClick(object sender, System.Windows.RoutedEventArgs e)
    {
      Configuration.Instance.IsMetroMode = true;
      Configuration.Instance.NavigateTarget = NavigateTarget.Noten;
      this.NavigationService.Navigate(new MetroSelectLerngruppePage());
    }

    private void GruppenOnClick(object sender, System.Windows.RoutedEventArgs e)
    {
      Configuration.Instance.IsMetroMode = true;
      Configuration.Instance.NavigateTarget = NavigateTarget.Gruppen;
      //var schülerlisten = App.MainViewModel.LerngruppeWorkspace;
      this.NavigationService.Navigate(new MetroSelectLerngruppePage());
    }

    private void SitzpläneOnClick(object sender, System.Windows.RoutedEventArgs e)
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
    /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
    private void SpeichernOnClick(object sender, System.Windows.RoutedEventArgs e)
    {
      Selection.Instance.UpdateUserSettings();
      Settings.Default.Save();
      App.UnitOfWork.SaveChanges();
    }

    private void ExitOnClick(object sender, System.Windows.RoutedEventArgs e)
    {
      App.MainViewModel.SaveCommand.Execute(null);

      App.Current.Shutdown();
    }
  }
}
