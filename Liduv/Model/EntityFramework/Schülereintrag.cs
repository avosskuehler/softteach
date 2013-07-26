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
    
    public partial class Schülereintrag
    {
        public Schülereintrag()
        {
            this.Ergebnisse = new HashSet<Ergebnis>();
            this.Noten = new HashSet<Note>();
            this.Hausaufgaben = new HashSet<Hausaufgabe>();
            this.Notentendenzen = new HashSet<Notentendenz>();
        }
    
        public int Id { get; set; }
        public int SchülerlisteId { get; set; }
        public int PersonId { get; set; }
    
        public virtual Schülerliste Schülerliste { get; set; }
        public virtual ICollection<Ergebnis> Ergebnisse { get; set; }
        public virtual Person Person { get; set; }
        public virtual ICollection<Note> Noten { get; set; }
        public virtual ICollection<Hausaufgabe> Hausaufgaben { get; set; }
        public virtual ICollection<Notentendenz> Notentendenzen { get; set; }
    }
}
