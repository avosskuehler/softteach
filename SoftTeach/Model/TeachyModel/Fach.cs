using SoftTeach.Model.TeachyModel;
using System;
using System.Collections.Generic;

namespace SoftTeach.Model.TeachyModel
{
  public partial class Fach
  {
    public Fach()
    {
      Arbeiten = new HashSet<Arbeit>();
      Curricula = new HashSet<Curriculum>();
      Fachstundenanzahlen = new HashSet<Fachstundenanzahl>();
      Lerngruppen = new HashSet<Lerngruppe>();
      Module = new HashSet<Modul>();
      Stunden = new HashSet<Stunde>();
    }

    public int Id { get; set; }
    public string Bezeichnung { get; set; }
    public string Farbe { get; set; }

    public virtual ICollection<Arbeit> Arbeiten { get; set; }
    public virtual ICollection<Curriculum> Curricula { get; set; }
    public virtual ICollection<Fachstundenanzahl> Fachstundenanzahlen { get; set; }
    public virtual ICollection<Lerngruppe> Lerngruppen { get; set; }
    public virtual ICollection<Modul> Module { get; set; }
    public virtual ICollection<Stunde> Stunden { get; set; }
  }
}
