using System;
using System.Collections.Generic;

namespace SoftTeach.Model.TeachyModel
{
  public partial class Unterrichtsstunde
  {
    public Unterrichtsstunde()
    {
      TermineErsteUnterrichtsstunde = new HashSet<Lerngruppentermin>();
      TermineLetzteUnterrichtsstunde = new HashSet<Lerngruppentermin>();
    }

    public int Id { get; set; }
    public string Bezeichnung { get; set; }
    public TimeSpan Beginn { get; set; }
    public TimeSpan Ende { get; set; }
    public int Stundenindex { get; set; }

    public virtual ICollection<Lerngruppentermin> TermineErsteUnterrichtsstunde { get; set; }
    public virtual ICollection<Lerngruppentermin> TermineLetzteUnterrichtsstunde { get; set; }
  }
}
