//------------------------------------------------------------------------------
// <auto-generated>
//     Der Code wurde von einer Vorlage generiert.
//
//     Manuelle Änderungen an dieser Datei führen möglicherweise zu unerwartetem Verhalten der Anwendung.
//     Manuelle Änderungen an dieser Datei werden überschrieben, wenn der Code neu generiert wird.
// </auto-generated>
//------------------------------------------------------------------------------

namespace SoftTeach.Model.TeachyModel
{
    using System;
    using System.Collections.Generic;
    
    public partial class RaumplanNeu
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public RaumplanNeu()
        {
            this.Sitzplätze = new HashSet<SitzplatzNeu>();
            this.Sitzpläne = new HashSet<SitzplanNeu>();
        }
    
        public int Id { get; set; }
        public string Bezeichnung { get; set; }
        public int RaumId { get; set; }
        public byte[] Grundriss { get; set; }
    
        public virtual RaumNeu Raum { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SitzplatzNeu> Sitzplätze { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SitzplanNeu> Sitzpläne { get; set; }
    }
}