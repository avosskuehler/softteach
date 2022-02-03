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
    
    public partial class Jahrtyp
    {
        public Jahrtyp()
        {
            this.Jahresplan = new HashSet<Jahresplan>();
            this.Stundenplan = new HashSet<Stundenplan>();
            this.Ferien = new HashSet<Ferien>();
            this.Schultermin = new HashSet<Schultermin>();
            this.Curriculum = new HashSet<Curriculum>();
            this.Schulwochen = new HashSet<Schulwoche>();
            this.Arbeit = new HashSet<Arbeit>();
            this.Schülerlisten = new HashSet<Schülerliste>();
        }
    
        public int Id { get; set; }
        public string Bezeichnung { get; set; }
        public int Jahr { get; set; }
    
        public virtual ICollection<Jahresplan> Jahresplan { get; set; }
        public virtual ICollection<Stundenplan> Stundenplan { get; set; }
        public virtual ICollection<Ferien> Ferien { get; set; }
        public virtual ICollection<Schultermin> Schultermin { get; set; }
        public virtual ICollection<Curriculum> Curriculum { get; set; }
        public virtual ICollection<Schulwoche> Schulwochen { get; set; }
        public virtual ICollection<Arbeit> Arbeit { get; set; }
        public virtual ICollection<Schülerliste> Schülerlisten { get; set; }
    }
}
