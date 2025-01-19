using SoftTeach.Model.TeachyModel;
using System;
using System.Collections.Generic;

namespace SoftTeach.Model.TeachyModel
{
  public partial class Note
    {
        public int Id { get; set; }
        public int SchülereintragId { get; set; }
        public int ZensurId { get; set; }
        public DateTime Datum { get; set; }
        public int Wichtung { get; set; }
        public bool IstSchriftlich { get; set; }
        public int? ArbeitId { get; set; }
        public string Bezeichnung { get; set; }
        public Notentyp Notentyp { get; set; }
        public NotenTermintyp NotenTermintyp { get; set; }

        public virtual Arbeit Arbeit { get; set; }
        public virtual Schülereintrag Schülereintrag { get; set; }
        public virtual Zensur Zensur { get; set; }
    }
}
