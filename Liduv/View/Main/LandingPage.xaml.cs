namespace Liduv.View.Main
{
  using System.Windows;
  using System.Windows.Controls;

  using Liduv.Setting;
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
      if (this.desktopView == null)
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
  }
}
