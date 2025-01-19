using System;
using System.Collections.Generic;

namespace SoftTeach.Model.TeachyModel
{
  public partial class Ergebnis
    {
        public int Id { get; set; }
        public int AufgabeId { get; set; }
        public int SchülereintragId { get; set; }
        public double? Punktzahl { get; set; }

        public virtual Aufgabe Aufgabe { get; set; }
        public virtual Schülereintrag Schülereintrag { get; set; }
    }
}
