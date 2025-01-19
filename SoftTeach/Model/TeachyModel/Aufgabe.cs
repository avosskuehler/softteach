using SoftTeach.Model.TeachyModel;
using System;
using System.Collections.Generic;

namespace SoftTeach.Model.TeachyModel
{
  public partial class Aufgabe
  {
    public Aufgabe()
    {
      Ergebnisse = new HashSet<Ergebnis>();
    }

    public int Id { get; set; }
    public int ArbeitId { get; set; }
    public int LfdNr { get; set; }
    public double MaxPunkte { get; set; }
    public string Bezeichnung { get; set; }

    public virtual Arbeit Arbeit { get; set; }
    public virtual ICollection<Ergebnis> Ergebnisse { get; set; }
  }
}
