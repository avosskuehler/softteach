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
      return string.Format("{0} - {1} {2}", this.Schuljahr != null ? this.Schuljahr.Bezeichnung : "Jahr unbekannt", this.Bezeichnung, this.Fach != null ? this.Fach.Bezeichnung : "Fach unbekannt");
    }
  }
}
