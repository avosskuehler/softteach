using System;
using System.Collections.Generic;

namespace SoftTeach.Model.TeachyModel
{
    public partial class Sitzplan
    {
        public Sitzplan()
        {
            Sitzplaneinträge = new HashSet<Sitzplaneintrag>();
        }

        public int Id { get; set; }
        public string Bezeichnung { get; set; }
        public int RaumplanId { get; set; }
        public int LerngruppeId { get; set; }
        public DateTime GültigAb { get; set; }

        public virtual Lerngruppe Lerngruppe { get; set; }
        public virtual Raumplan Raumplan { get; set; }
        public virtual ICollection<Sitzplaneintrag> Sitzplaneinträge { get; set; }
    }
}
