using System;
using System.Collections.Generic;

namespace SoftTeach.Model.TeachyModel
{
    public partial class Raum
    {
        public Raum()
        {
            Raumpläne = new HashSet<Raumplan>();
            Stundenplaneinträge = new HashSet<Stundenplaneintrag>();
        }

        public int Id { get; set; }
        public string Bezeichnung { get; set; }

        public virtual ICollection<Raumplan> Raumpläne { get; set; }
        public virtual ICollection<Stundenplaneintrag> Stundenplaneinträge { get; set; }
    }
}
