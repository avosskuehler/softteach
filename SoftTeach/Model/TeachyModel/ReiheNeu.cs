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
    
    public partial class ReiheNeu
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ReiheNeu()
        {
            this.Sequenzen = new HashSet<SequenzNeu>();
        }
    
        public int Id { get; set; }
        public int ModulId { get; set; }
        public string Thema { get; set; }
        public int Stundenbedarf { get; set; }
        public int CurriculumId { get; set; }
        public int Reihenfolge { get; set; }
    
        public virtual CurriculumNeu Curriculum { get; set; }
        public virtual ModulNeu Modul { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SequenzNeu> Sequenzen { get; set; }
    }
}