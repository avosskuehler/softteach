using System;
using System.Collections.Generic;

namespace SoftTeach.Model.TeachyModel
{
  public partial class Curriculum
    {
        public Curriculum()
        {
            Reihen = new HashSet<Reihe>();
        }

        public int Id { get; set; }
        public int FachId { get; set; }
        public int Jahrgang { get; set; }
        public string Bezeichnung { get; set; }
        public int SchuljahrId { get; set; }
        public Halbjahr Halbjahr { get; set; }

        public virtual Fach Fach { get; set; }
        public virtual Schuljahr Schuljahr { get; set; }
        public virtual ICollection<Reihe> Reihen { get; set; }
    }
}
