using System;
using System.Collections.Generic;

namespace SoftTeach.Model.TeachyModel
{
  public partial class BetroffeneLerngruppe
    {
        public int Id { get; set; }
        public int TerminId { get; set; }
        public int LerngruppeId { get; set; }

        public virtual Lerngruppe Lerngruppe { get; set; }
        public virtual Schultermin Schultermin { get; set; }
    }
}
