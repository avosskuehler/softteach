﻿namespace SoftTeach.View.Datenbank
{
  using System.Windows;

  /// <summary>
  /// Interaction logic for LerngruppeWorkspace.xaml
  /// </summary>
  public partial class LerngruppeWorkspace : Window
  {
    public LerngruppeWorkspace()
    {
      this.InitializeComponent();
    }

    private void OKClick(object sender, RoutedEventArgs e)
    {
      this.Close();
    }
  }
}
