using System;
using System.Collections.Generic;

namespace SoftTeach.Model.TeachyModel
{
  public partial class Dateityp
  {
    public Dateityp()
    {
      Dateiverweise = new HashSet<Dateiverweis>();
    }

    public int Id { get; set; }
    public string Bezeichnung { get; set; }
    public string Kürzel { get; set; }

    public virtual ICollection<Dateiverweis> Dateiverweise { get; set; }
  }
}
