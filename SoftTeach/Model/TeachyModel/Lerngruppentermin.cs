using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace SoftTeach.Model.TeachyModel
{
  public partial class Lerngruppentermin: Termin
  {
    public Halbjahr Halbjahr { get; set; }

    public int LerngruppeId { get; set; }

    public virtual Lerngruppe Lerngruppe { get; set; }

    /// <summary>
    /// Holt den Stundenanzahl of this Termin
    /// </summary>
    public int Stundenanzahl
    {
      get
      {
        return this.LetzteUnterrichtsstunde.Stundenindex - this.ErsteUnterrichtsstunde.Stundenindex + 1;
      }
    }

    public int Breite
    {
      get
      {
        return this.Stundenanzahl * Properties.Settings.Default.Stundenbreite;
      }
    }

    public string StundenbedarfString
    {
      get
      {
        return this.Stundenanzahl + "h";
      }
    }

    public string Monat
    {
      get
      {
        return this.Datum.ToString("MMM", new CultureInfo("de-DE"));
      }
    }

    public SolidColorBrush Farbe
    {
      get
      {
        switch (this.Termintyp)
        {
          case Termintyp.Klausur:
            return Brushes.Yellow;
          case Termintyp.TagDerOffenenTür:
            return Brushes.Blue;
          case Termintyp.Wandertag:
            return Brushes.Magenta;
          case Termintyp.Abitur:
            return Brushes.Red;
          case Termintyp.MSA:
            return Brushes.Red;
          case Termintyp.Unterricht:
            return Brushes.LightBlue;
          case Termintyp.Vertretung:
            return Brushes.LightGray;
          case Termintyp.Besprechung:
            return Brushes.Orange;
          case Termintyp.Sondertermin:
            return Brushes.Orange;
          case Termintyp.Ferien:
            return Brushes.Green;
          case Termintyp.Kursfahrt:
            return Brushes.Fuchsia;
          case Termintyp.Klassenfahrt:
            return Brushes.Fuchsia;
          case Termintyp.Projekttag:
            return Brushes.Orange;
          case Termintyp.Praktikum:
            return Brushes.Orange;
          case Termintyp.Geburtstag:
            return Brushes.SteelBlue;
          case Termintyp.Veranstaltung:
            return Brushes.Maroon;
          case Termintyp.PSE:
            return Brushes.Fuchsia;
          default:
            return Brushes.Transparent;
        }
      }
    }

  }
}
