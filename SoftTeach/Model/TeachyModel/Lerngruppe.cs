using SoftTeach.Model.TeachyModel;
using System;
using System.Collections.Generic;

namespace SoftTeach.Model.TeachyModel
{
  public partial class Lerngruppe
  {
    public Lerngruppe()
    {
      Arbeiten = new HashSet<Arbeit>();
      BetroffeneLerngruppen = new HashSet<BetroffeneLerngruppe>();
      Schülereinträge = new HashSet<Schülereintrag>();
      Sitzpläne = new HashSet<Sitzplan>();
      Stundenplaneinträge = new HashSet<Stundenplaneintrag>();
      Lerngruppentermine = new HashSet<Lerngruppentermin>();
    }

    public int Id { get; set; }
    public string Bezeichnung { get; set; }
    public int SchuljahrId { get; set; }
    public int FachId { get; set; }
    public int Jahrgang { get; set; }
    public Bepunktungstyp Bepunktungstyp { get; set; }
    public int NotenWichtungId { get; set; }

    public virtual Fach Fach { get; set; }
    public virtual NotenWichtung NotenWichtung { get; set; }
    public virtual Schuljahr Schuljahr { get; set; }
    public virtual ICollection<Arbeit> Arbeiten { get; set; }
    public virtual ICollection<BetroffeneLerngruppe> BetroffeneLerngruppen { get; set; }
    public virtual ICollection<Schülereintrag> Schülereinträge { get; set; }
    public virtual ICollection<Sitzplan> Sitzpläne { get; set; }
    public virtual ICollection<Stundenplaneintrag> Stundenplaneinträge { get; set; }
    public virtual ICollection<Lerngruppentermin> Lerngruppentermine { get; set; }
  }
}
