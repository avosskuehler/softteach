using System;
using System.Collections.Generic;

namespace SoftTeach.Model.TeachyModel
{
    public partial class Stundenplan
    {
        public Stundenplan()
        {
            Stundenplaneinträge = new HashSet<Stundenplaneintrag>();
        }

        public int Id { get; set; }
        public string Bezeichnung { get; set; }
        public int SchuljahrId { get; set; }
        public Halbjahr Halbjahr { get; set; }
        public DateTime GültigAb { get; set; }

        public virtual Schuljahr Schuljahr { get; set; }
        public virtual ICollection<Stundenplaneintrag> Stundenplaneinträge { get; set; }
    }
}
