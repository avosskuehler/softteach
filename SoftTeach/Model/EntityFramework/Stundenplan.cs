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
    
    public partial class Stundenplan
    {
        public Stundenplan()
        {
            this.Stundenplaneinträge = new HashSet<Stundenplaneintrag>();
        }
    
        public int Id { get; set; }
        public string Bezeichnung { get; set; }
        public int JahrtypId { get; set; }
        public int HalbjahrtypId { get; set; }
        public System.DateTime GültigAb { get; set; }
    
        public virtual Halbjahrtyp Halbjahrtyp { get; set; }
        public virtual Jahrtyp Jahrtyp { get; set; }
        public virtual ICollection<Stundenplaneintrag> Stundenplaneinträge { get; set; }
    }
}
