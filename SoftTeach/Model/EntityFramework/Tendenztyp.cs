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
    
    public partial class Tendenztyp
    {
        public Tendenztyp()
        {
            this.Notentendenzen = new HashSet<Notentendenz>();
        }
    
        public int Id { get; set; }
        public string Bezeichnung { get; set; }
    
        public virtual ICollection<Notentendenz> Notentendenzen { get; set; }
    }
}
