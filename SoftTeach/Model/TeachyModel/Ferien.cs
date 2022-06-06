using System;
using System.Collections.Generic;

namespace SoftTeach.Model.TeachyModel
{
    public partial class Ferien
    {
        public int Id { get; set; }
        public int SchuljahrId { get; set; }
        public string Bezeichnung { get; set; }
        public DateTime ErsterFerientag { get; set; }
        public DateTime LetzterFerientag { get; set; }

        public virtual Schuljahr Schuljahr { get; set; }
    }
}
