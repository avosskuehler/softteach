using System;
using System.Collections.Generic;

namespace SoftTeach.Model.TeachyModel
{
  public partial class Arbeit
  {
    public Arbeit()
    {
      Aufgaben = new HashSet<Aufgabe>();
      Noten = new HashSet<Note>();
    }

    public int Id { get; set; }
    public int LerngruppeId { get; set; }
    public int FachId { get; set; }
    public int BewertungsschemaId { get; set; }
    public Bepunktungstyp Bepunktungstyp { get; set; }
    public string Bezeichnung { get; set; }
    public int LfdNr { get; set; }
    public DateTime Datum { get; set; }
    public bool IstKlausur { get; set; }

    public virtual Bewertungsschema Bewertungsschema { get; set; }
    public virtual Fach Fach { get; set; }
    public virtual Lerngruppe Lerngruppe { get; set; }
    public virtual ICollection<Aufgabe> Aufgaben { get; set; }
    public virtual ICollection<Note> Noten { get; set; }
  }
}
