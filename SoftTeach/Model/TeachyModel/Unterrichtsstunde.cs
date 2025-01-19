using System;
using System.Collections.Generic;

namespace SoftTeach.Model.TeachyModel
{
  public partial class Unterrichtsstunde
  {
    public Unterrichtsstunde()
    {
      TermineErsteUnterrichtsstunde = new HashSet<Termin>();
      TermineLetzteUnterrichtsstunde = new HashSet<Termin>();
    }

    public int Id { get; set; }
    public string Bezeichnung { get; set; }
    public TimeSpan Beginn { get; set; }
    public TimeSpan Ende { get; set; }
    public int Stundenindex { get; set; }

    public virtual ICollection<Termin> TermineErsteUnterrichtsstunde { get; set; }
    public virtual ICollection<Termin> TermineLetzteUnterrichtsstunde { get; set; }
  }
}
