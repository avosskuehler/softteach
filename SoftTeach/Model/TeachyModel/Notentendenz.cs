using System;
using System.Collections.Generic;

namespace SoftTeach.Model.TeachyModel
{
    public partial class Notentendenz
    {
        public int Id { get; set; }
        public DateTime Datum { get; set; }
        public Tendenz Tendenz { get; set; }
        public Tendenztyp Tendenztyp { get; set; }
        public string Bezeichnung { get; set; }
        public int SchülereintragId { get; set; }

        public virtual Schülereintrag Schülereintrag { get; set; }
    }
}
