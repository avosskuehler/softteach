using System;
using System.Collections.Generic;

namespace SoftTeach.Model.TeachyModel
{
    public partial class Schülereintrag
    {
        public Schülereintrag()
        {
            Ergebnisse = new HashSet<Ergebnis>();
            Hausaufgaben = new HashSet<Hausaufgabe>();
            Notentendenzen = new HashSet<Notentendenz>();
            Noten = new HashSet<Note>();
            Sitzplaneinträge = new HashSet<Sitzplaneintrag>();
        }

        public int Id { get; set; }
        public int LerngruppeId { get; set; }
        public int PersonId { get; set; }

        public virtual Lerngruppe Lerngruppe { get; set; }
        public virtual Person Person { get; set; }
        public virtual ICollection<Ergebnis> Ergebnisse { get; set; }
        public virtual ICollection<Hausaufgabe> Hausaufgaben { get; set; }
        public virtual ICollection<Notentendenz> Notentendenzen { get; set; }
        public virtual ICollection<Note> Noten { get; set; }
        public virtual ICollection<Sitzplaneintrag> Sitzplaneinträge { get; set; }
    }
}
