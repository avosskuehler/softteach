//------------------------------------------------------------------------------
// <auto-generated>
//    Dieser Code wurde aus einer Vorlage generiert.
//
//    Manuelle Änderungen an dieser Datei führen möglicherweise zu unerwartetem Verhalten Ihrer Anwendung.
//    Manuelle Änderungen an dieser Datei werden überschrieben, wenn der Code neu generiert wird.
// </auto-generated>
//------------------------------------------------------------------------------

namespace SoftTeach.Model.EntityFramework
{
    using System;
    using System.Collections.Generic;
    
    public partial class Zensur
    {
        public Zensur()
        {
            this.Noten = new HashSet<Note>();
            this.Prozentbereiche = new HashSet<Prozentbereich>();
        }
    
        public int Id { get; set; }
        public int Notenpunkte { get; set; }
        public string NoteMitTendenz { get; set; }
        public int GanzeNote { get; set; }
    
        public virtual ICollection<Note> Noten { get; set; }
        public virtual ICollection<Prozentbereich> Prozentbereiche { get; set; }
    }
}
