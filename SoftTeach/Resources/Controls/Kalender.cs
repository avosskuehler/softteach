using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace SoftTeach.Resources.Controls
{
  public class Kalender : Control
  {
    static Kalender()
    {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(Kalender), new FrameworkPropertyMetadata(typeof(Kalender)));
    }
  }
}
