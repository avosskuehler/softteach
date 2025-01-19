using System;
using System.Collections.Generic;

namespace SoftTeach.Model.TeachyModel
{
    public partial class Hausaufgabe
    {
        public int Id { get; set; }
        public DateTime Datum { get; set; }
        public string Bezeichnung { get; set; }
        public bool IstNachgereicht { get; set; }
        public int SchülereintragId { get; set; }

        public virtual Schülereintrag Schülereintrag { get; set; }
    }
}
