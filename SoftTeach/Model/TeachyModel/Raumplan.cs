using System;
using System.Collections.Generic;

namespace SoftTeach.Model.TeachyModel
{
    public partial class Raumplan
    {
        public Raumplan()
        {
            Sitzpläne = new HashSet<Sitzplan>();
            Sitzplätze = new HashSet<Sitzplatz>();
        }

        public int Id { get; set; }
        public string Bezeichnung { get; set; }
        public int RaumId { get; set; }
        public byte[] Grundriss { get; set; }

        public virtual Raum Raum { get; set; }
        public virtual ICollection<Sitzplan> Sitzpläne { get; set; }
        public virtual ICollection<Sitzplatz> Sitzplätze { get; set; }
    }
}
