namespace SoftTeach.View.Termine
{
  using System.Collections.ObjectModel;
  using System.Windows;
  using System.Windows.Controls;

  using SoftTeach.ViewModel;

  using System.Windows.Media;

  using SoftTeach.ViewModel.Datenbank;

  /// <summary>
  /// Interaction logic for BetroffeneKlassenDialog.xaml
  /// </summary>
  public partial class BetroffeneKlassenDialog : Window
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="BetroffeneKlassenDialog"/> class.
    /// </summary>
    public BetroffeneKlassenDialog()
    {
      this.InitializeComponent();
      this.Klassen = new ObservableCollection<KlasseViewModel>();
    }

    /// <summary>
    /// Holt oder setzt die collection of selected <see cref="Klassen"/>
    /// </summary>
    public ObservableCollection<KlasseViewModel> Klassen { get; set; }

    /// <summary>
    /// Der event handler für den OK und Ja Button. Setzt DialogResult=true
    /// und beendet den Dialog.
    /// </summary>
    /// <param name="sender">Source of the event</param>
    /// <param name="e">An empty <see cref="RoutedEventArgs"/></param>
    private void SaveButtonClick(object sender, RoutedEventArgs e)
    {
      this.DialogResult = true;
      this.SetKlassenCollection();
      this.Close();
    }

    /// <summary>
    /// Der event handler für den Nein Button. Setzt DialogResult=false
    /// und beendet den Dialog.
    /// </summary>
    /// <param name="sender">Source of the event</param>
    /// <param name="e">An empty <see cref="RoutedEventArgs"/></param>
    private void CancelButtonClick(object sender, RoutedEventArgs e)
    {
      this.DialogResult = false;
      this.Close();
    }

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
      int jahrgangsstufenCount = App.MainViewModel.Jahrgangsstufen.Count;
      this.Lerngruppengrid.Columns = jahrgangsstufenCount;
      for (int i = 0; i < jahrgangsstufenCount; i++)
      {
        var jahrgangsstufe = App.MainViewModel.Jahrgangsstufen[i];
        var jahrgangsGroup = new GroupBox();
        var jahrgang = jahrgangsstufe.JahrgangsstufeBezeichnung;
        jahrgangsGroup.Header = jahrgang;

        var panel = new StackPanel();
        jahrgangsGroup.Content = panel;

        var alleCheckBox = new CheckBox();
        if (this.Klassen.Count == 0)
        {
          alleCheckBox.IsChecked = true;
        }

        alleCheckBox.Content = "Alle";
        alleCheckBox.Checked += this.CheckBoxChecked;
        alleCheckBox.Unchecked += this.CheckBoxUnchecked;
        panel.Children.Add(alleCheckBox);
        var separator = new Border()
          {
            Height = 4,
            Margin = new Thickness(0, 0, 0, 2),
            BorderBrush = Brushes.DarkGray,
            BorderThickness = new Thickness(0, 0, 0, 2)
          };

        panel.Children.Add(separator);

        foreach (var klassenstufe in jahrgangsstufe.Klassenstufen)
        {
          foreach (var klasse in klassenstufe.Klassen)
          {
            var chb = new CheckBox
            {
              Content = klasse.KlasseBezeichnung,
              Tag = klasse
            };

            foreach (var ausgewählteKlasse in this.Klassen)
            {
              if (ausgewählteKlasse.KlasseBezeichnung == klasse.KlasseBezeichnung)
              {
                chb.IsChecked = true;
                break;
              }
            }

            if (this.Klassen.Count == 0)
            {
              chb.IsChecked = true;
            }

            panel.Children.Add(chb);
          }
        }

        this.Lerngruppengrid.Children.Add(jahrgangsGroup);
      }
    }

    private void CheckBoxChecked(object sender, RoutedEventArgs e)
    {
      var senderCheckBox = sender as CheckBox;
      var parent = senderCheckBox.Parent as StackPanel;
      foreach (var box in parent.Children)
      {
        var checkBox = box as CheckBox;
        if (checkBox != null && box != senderCheckBox)
        {
          checkBox.IsChecked = true;
        }
      }
    }

    private void CheckBoxUnchecked(object sender, RoutedEventArgs e)
    {
      var senderCheckBox = sender as CheckBox;
      var parent = senderCheckBox.Parent as StackPanel;
      foreach (var box in parent.Children)
      {
        var checkBox = box as CheckBox;
        if (checkBox != null && box != senderCheckBox)
        {
          checkBox.IsChecked = false;
        }
      }
    }

    /// <summary>
    /// Creates the collection of klassen the dialogs checkboxes are selected.
    /// </summary>
    private void SetKlassenCollection()
    {
      this.Klassen.Clear();
      foreach (var children in this.Lerngruppengrid.Children)
      {
        var groupBox = children as GroupBox;
        var panel = groupBox.Content as StackPanel;
        foreach (var child in panel.Children)
        {
          var checkbox = child as CheckBox;
          if (checkbox != null && checkbox.Tag != null && checkbox.IsChecked.GetValueOrDefault())
          {
            this.Klassen.Add(checkbox.Tag as KlasseViewModel);
          }
        }
      }
    }
  }
}
