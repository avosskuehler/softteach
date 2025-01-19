using SoftTeach.ViewModel.Jahrespläne;
using SoftTeach.ViewModel.Wochenpläne;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoftTeach.ViewModel.Helper
{
  public class TagGeändertEventArgs : EventArgs
  {
    public TagViewModel Tag { get; private set; }

    public TagGeändertEventArgs(TagViewModel tag)
    {
      this.Tag = tag;
    }
  }
}
