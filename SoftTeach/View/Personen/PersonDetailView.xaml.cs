

using System.Windows;
using System.Windows.Controls;

namespace SoftTeach.View.Personen
{
  /// <summary>
  /// Interaction logic for PersonDetailView.xaml
  /// </summary>
  public partial class PersonDetailView
  {
    public PersonDetailView()
    {
      this.InitializeComponent();
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
