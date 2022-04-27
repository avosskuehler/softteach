using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoftTeach.Model.TeachyModel
{
  public partial class DateiverweisNeu
  {
    /// <summary>
    /// Holt den Dateiname of this dateiverweis without path.
    /// </summary>
    public string DateinameOhnePfad
    {
      get
      {
        return Path.GetFileName(this.Dateiname);
      }
    }

    /// <summary>
    /// Holt den Dateiname of this dateiverweis without path.
    /// </summary>
    public string Pfad
    {
      get
      {
        return Path.GetDirectoryName(this.Dateiname);
      }
    }

    public override string ToString()
    {
      return string.Format("{0}", this.Dateiname);
    }
  }
}
