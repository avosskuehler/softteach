//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Liduv.Model.EntityFramework
{
    using System;
    using System.Collections.Generic;
    
    public partial class Aufgabe
    {
        public Aufgabe()
        {
            this.Ergebnisse = new HashSet<Ergebnis>();
        }
    
        public int Id { get; set; }
        public int ArbeitId { get; set; }
        public int LfdNr { get; set; }
        public int MaxPunkte { get; set; }
        public string Bezeichnung { get; set; }
    
        public virtual ICollection<Ergebnis> Ergebnisse { get; set; }
        public virtual Arbeit Arbeit { get; set; }
    }
}
