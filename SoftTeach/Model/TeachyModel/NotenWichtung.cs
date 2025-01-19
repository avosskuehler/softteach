using System;
using System.Collections.Generic;

namespace SoftTeach.Model.TeachyModel
{
    public partial class NotenWichtung
    {
        public NotenWichtung()
        {
            Lerngruppen = new HashSet<Lerngruppe>();
        }

        public int Id { get; set; }
        public string Bezeichnung { get; set; }
        public float MündlichQualität { get; set; }
        public float MündlichQuantität { get; set; }
        public float MündlichSonstige { get; set; }
        public float MündlichGesamt { get; set; }
        public float SchriftlichKlassenarbeit { get; set; }
        public float SchriftlichSonstige { get; set; }
        public float SchriftlichGesamt { get; set; }

        public virtual ICollection<Lerngruppe> Lerngruppen { get; set; }
    }
}
