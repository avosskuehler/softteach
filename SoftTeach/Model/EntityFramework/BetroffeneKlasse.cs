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
    
    public partial class BetroffeneKlasse
    {
        public int Id { get; set; }
        public int TerminId { get; set; }
        public int KlasseId { get; set; }
    
        public virtual Schultermin Termin { get; set; }
        public virtual Klasse Klasse { get; set; }
    }
}
