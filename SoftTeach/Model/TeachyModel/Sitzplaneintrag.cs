using System;
using System.Collections.Generic;

namespace SoftTeach.Model.TeachyModel
{
    public partial class Sitzplaneintrag
    {
        public int Id { get; set; }
        public int SitzplanId { get; set; }
        public int SitzplatzId { get; set; }
        public int? SchülereintragId { get; set; }

        public virtual Schülereintrag Schülereintrag { get; set; }
        public virtual Sitzplan Sitzplan { get; set; }
        public virtual Sitzplatz Sitzplatz { get; set; }
    }
}
