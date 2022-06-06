using SoftTeach.Model.TeachyModel;
using System;
using System.Collections.Generic;

namespace SoftTeach.Model.TeachyModel
{
  public partial class Bewertungsschema
  {
    public Bewertungsschema()
    {
      Arbeiten = new HashSet<Arbeit>();
      Prozentbereiche = new HashSet<Prozentbereich>();
    }

    public int Id { get; set; }
    public string Bezeichnung { get; set; }

    public virtual ICollection<Arbeit> Arbeiten { get; set; }
    public virtual ICollection<Prozentbereich> Prozentbereiche { get; set; }
  }
}
