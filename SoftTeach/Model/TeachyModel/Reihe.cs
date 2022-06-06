using System;
using System.Collections.Generic;

namespace SoftTeach.Model.TeachyModel
{
    public partial class Reihe
    {
        public Reihe()
        {
            Sequenzen = new HashSet<Sequenz>();
        }

        public int Id { get; set; }
        public int ModulId { get; set; }
        public string Thema { get; set; }
        public int Stundenbedarf { get; set; }
        public int CurriculumId { get; set; }
        public int Reihenfolge { get; set; }

        public virtual Curriculum Curriculum { get; set; }
        public virtual Modul Modul { get; set; }
        public virtual ICollection<Sequenz> Sequenzen { get; set; }
    }
}
