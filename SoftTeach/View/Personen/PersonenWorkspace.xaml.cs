namespace SoftTeach.View.Personen
{
  using System.Windows;
  using System.Windows.Controls;

  /// <summary>
  /// Interaction logic for PersonenWorkspace.xaml
  /// </summary>
  public partial class PersonenWorkspace : Window
  {
    public PersonenWorkspace()
    {
      this.InitializeComponent();
      this.DataContext = App.MainViewModel.PersonenWorkspace;
    }

    private void OKClick(object sender, RoutedEventArgs e)
    {
      this.DialogResult = true;
      this.Close();
    }

    private void Image_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
      var image = sender as Image;
      if (image != null)
      {
        var data = new DataObject("OwnBitmapSource", image.Source);
        DragDrop.DoDragDrop(this.PersonImage, data, DragDropEffects.Move);
      }
    }
  }
}
