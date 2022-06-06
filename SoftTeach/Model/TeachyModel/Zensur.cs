using System;
using System.Collections.Generic;

namespace SoftTeach.Model.TeachyModel
{
  public partial class Zensur
  {
    public Zensur()
    {
      Noten = new HashSet<Note>();
      Prozentbereiche = new HashSet<Prozentbereich>();
    }

    public int Id { get; set; }
    public int Notenpunkte { get; set; }
    public string NoteMitTendenz { get; set; }
    public int GanzeNote { get; set; }

    public virtual ICollection<Note> Noten { get; set; }
    public virtual ICollection<Prozentbereich> Prozentbereiche { get; set; }
  }
}
