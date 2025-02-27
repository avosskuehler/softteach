﻿using System;
using System.Collections.Generic;
using System.IO;

namespace SoftTeach.Model.TeachyModel
{
  public partial class Dateiverweis
  {
    public int Id { get; set; }
    public string Dateiname { get; set; }
    public int DateitypId { get; set; }
    public int StundeId { get; set; }

    public virtual Dateityp Dateityp { get; set; }
    public virtual Stunde Stunde { get; set; }


    /// <summary>
    /// Holt den Dateiname of this dateiverweis without path.
    /// </summary>
    public string DateinameOhnePfad
    {
      get
      {
        return Path.GetFileName(this.Dateiname);
      }
    }

    /// <summary>
    /// Holt den Dateiname of this dateiverweis without path.
    /// </summary>
    public string Pfad
    {
      get
      {
        return Path.GetDirectoryName(this.Dateiname);
      }
    }

    public override string ToString()
    {
      return string.Format("{0}", this.Dateiname);
    }
  }
}
