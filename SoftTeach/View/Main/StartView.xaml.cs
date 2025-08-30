using SoftTeach.Properties;
using SoftTeach.Setting;
using SoftTeach.UndoRedo;
using SoftTeach.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SoftTeach.View.Main
{
  /// <summary>
  /// Interaktionslogik für StartView.xaml
  /// </summary>
  public partial class StartView : NavigationWindow
  {
    public StartView()
    {
      InitializeComponent();
    }

    /// <summary>
    /// Handles the Closing event of the navWin control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.ComponentModel.CancelEventArgs"/> instance containing the event data.</param>
    private void NavigationWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
    {
      Selection.Instance.UpdateUserSettings();
      Settings.Default.Save();

      // Check if there is nothing to change
      if (((Stack<ChangeSet>)MainViewModel.UndoStack).Count == 0)
      {
        return;
      }

      var dlg = new AskForSavingChangesDialog();
      dlg.ShowDialog();
      if (dlg.Result == null)
      {
        e.Cancel = true;
      }
      else if (dlg.Result.Value)
      {
        App.UnitOfWork.SaveChanges();
      }
    }
  }
}
