using System;
using System.Collections.Generic;

namespace SoftTeach.Model.TeachyModel
{
  public partial class Fachstundenanzahl
  {
    public int Id { get; set; }
    public int FachId { get; set; }
    public int Jahrgang { get; set; }
    public int Stundenzahl { get; set; }
    public int Teilungsstundenzahl { get; set; }

    public virtual Fach Fach { get; set; }
  }
}
