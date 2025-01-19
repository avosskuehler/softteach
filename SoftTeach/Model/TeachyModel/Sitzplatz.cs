using System;
using System.Collections.Generic;

namespace SoftTeach.Model.TeachyModel
{
    public partial class Sitzplatz
    {
        public Sitzplatz()
        {
            Sitzplaneinträge = new HashSet<Sitzplaneintrag>();
        }

        public int Id { get; set; }
        public int RaumplanId { get; set; }
        public double LinksObenX { get; set; }
        public double LinksObenY { get; set; }
        public double Breite { get; set; }
        public double Höhe { get; set; }
        public double Drehwinkel { get; set; }
        public int Reihenfolge { get; set; }

        public virtual Raumplan Raumplan { get; set; }
        public virtual ICollection<Sitzplaneintrag> Sitzplaneinträge { get; set; }
    }
}
