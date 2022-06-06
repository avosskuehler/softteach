using System;
using System.Collections.Generic;

namespace SoftTeach.Model.TeachyModel
{
    public partial class Person
    {
        public Person()
        {
            Schülereinträge = new HashSet<Schülereintrag>();
        }

        public int Id { get; set; }
        public string Vorname { get; set; }
        public string Nachname { get; set; }
        public Geschlecht Geschlecht { get; set; }
        public DateTime? Geburtstag { get; set; }
        public string Titel { get; set; }
        public string Telefon { get; set; }
        public string Fax { get; set; }
        public string Handy { get; set; }
        public string EMail { get; set; }
        public string PLZ { get; set; }
        public string Straße { get; set; }
        public string Hausnummer { get; set; }
        public string Ort { get; set; }
        public bool IstLehrer { get; set; }
        public byte[] Foto { get; set; }

        public virtual ICollection<Schülereintrag> Schülereinträge { get; set; }
    }
}
