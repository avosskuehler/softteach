using System;
using System.Collections.Generic;

namespace SoftTeach.Model.TeachyModel
{
  public partial class Phase
  {
    public int Id { get; set; }
    public int StundeId { get; set; }
    public int Zeit { get; set; }
    public Medium Medium { get; set; }
    public Sozialform Sozialform { get; set; }
    public string Inhalt { get; set; }
    public int Reihenfolge { get; set; }

    public virtual Stunde Stunde { get; set; }
  }
}
