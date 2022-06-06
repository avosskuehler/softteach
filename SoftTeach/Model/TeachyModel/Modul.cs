using System;
using System.Collections.Generic;

namespace SoftTeach.Model.TeachyModel
{
  public partial class Modul
  {
    public Modul()
    {
      Reihen = new HashSet<Reihe>();
      Stunden = new HashSet<Stunde>();
    }

    public int Id { get; set; }
    public int FachId { get; set; }
    public string Bezeichnung { get; set; }
    public int Jahrgang { get; set; }
    public string Bausteine { get; set; }
    public int Stundenbedarf { get; set; }

    public virtual Fach Fach { get; set; }
    public virtual ICollection<Reihe> Reihen { get; set; }
    public virtual ICollection<Stunde> Stunden { get; set; }
  }
}
