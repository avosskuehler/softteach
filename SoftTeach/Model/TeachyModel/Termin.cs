using System;
using System.Collections.Generic;

namespace SoftTeach.Model.TeachyModel
{
  public partial class Termin
  {
    public int TerminId { get; set; }
    public string Discriminator { get; set; }
    public System.DateTime Datum { get; set; }
    public Termintyp Termintyp { get; set; }
    public string Beschreibung { get; set; }
    public int ErsteUnterrichtsstundeId { get; set; }
    public int LetzteUnterrichtsstundeId { get; set; }
    public string Ort { get; set; }
    public bool IstGeprüft { get; set; }
    public virtual Unterrichtsstunde ErsteUnterrichtsstunde { get; set; }
    public virtual Unterrichtsstunde LetzteUnterrichtsstunde { get; set; }
  }
}
