namespace SoftTeach.Model.TeachyModel
{
  using System;
  using System.Collections.Generic;

  public partial class Stunde : Lerngruppentermin
  {
    public Stunde()
    {
      this.Dateiverweise = new HashSet<Dateiverweis>();
      this.Phasen = new HashSet<Phase>();
    }

    public bool IstBenotet { get; set; }
    public int FachId { get; set; }
    public int Jahrgang { get; set; }
    public Nullable<int> ModulId { get; set; }
    public bool Kopieren { get; set; }
    public bool Computer { get; set; }
    public string Hausaufgaben { get; set; }
    public string Ansagen { get; set; }

    public virtual Fach Fach { get; set; }
    public virtual Modul Modul { get; set; }
    public virtual ICollection<Dateiverweis> Dateiverweise { get; set; }
    public virtual ICollection<Phase> Phasen { get; set; }
  }
}
