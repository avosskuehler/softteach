namespace SoftTeach.Model.TeachyModel
{
  using System;
  using System.Collections.Generic;

  public partial class Schultermin : Termin
  {
    public Schultermin()
    {
      this.BetroffeneLerngruppen = new HashSet<BetroffeneLerngruppe>();
    }

    public int SchuljahrId { get; set; }
    public virtual Schuljahr Schuljahr { get; set; }
    public virtual ICollection<BetroffeneLerngruppe> BetroffeneLerngruppen { get; set; }
  }
}
