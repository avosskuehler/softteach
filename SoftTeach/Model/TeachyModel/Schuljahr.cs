using System;
using System.Collections.Generic;

namespace SoftTeach.Model.TeachyModel
{
  public partial class Schuljahr
  {
    public Schuljahr()
    {
      Curricula = new HashSet<Curriculum>();
      Ferien = new HashSet<Ferien>();
      Lerngruppen = new HashSet<Lerngruppe>();
      Stundenpläne = new HashSet<Stundenplan>();
      Schultermine = new HashSet<Schultermin>();
    }

    public int Id { get; set; }
    public string Bezeichnung { get; set; }
    public int Jahr { get; set; }

    public virtual ICollection<Curriculum> Curricula { get; set; }
    public virtual ICollection<Ferien> Ferien { get; set; }
    public virtual ICollection<Lerngruppe> Lerngruppen { get; set; }
    public virtual ICollection<Stundenplan> Stundenpläne { get; set; }
    public virtual ICollection<Schultermin> Schultermine { get; set; }
  }
}
