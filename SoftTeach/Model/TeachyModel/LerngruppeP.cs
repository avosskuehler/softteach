using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoftTeach.Model.TeachyModel
{
  public partial class LerngruppeNeu
  {
    public override string ToString()
    {
      return string.Format("{0} - {1} {2}", this.Schuljahr.Bezeichnung, this.Bezeichnung, this.Fach.Bezeichnung);
    }
  }
}
