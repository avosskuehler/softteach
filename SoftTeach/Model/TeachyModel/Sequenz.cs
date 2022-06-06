using System;
using System.Collections.Generic;

namespace SoftTeach.Model.TeachyModel
{
    public partial class Sequenz
    {
        public int Id { get; set; }
        public int ReiheId { get; set; }
        public int Reihenfolge { get; set; }
        public int Stundenbedarf { get; set; }
        public string Thema { get; set; }

        public virtual Reihe Reihe { get; set; }
    }
}
