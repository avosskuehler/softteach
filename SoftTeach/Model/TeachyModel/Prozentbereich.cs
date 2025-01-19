using System;
using System.Collections.Generic;

namespace SoftTeach.Model.TeachyModel
{
    public partial class Prozentbereich
    {
        public int Id { get; set; }
        public int ZensurId { get; set; }
        public double VonProzent { get; set; }
        public double BisProzent { get; set; }
        public int BewertungsschemaId { get; set; }

        public virtual Bewertungsschema Bewertungsschema { get; set; }
        public virtual Zensur Zensur { get; set; }
    }
}
