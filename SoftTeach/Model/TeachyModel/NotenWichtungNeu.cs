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
    
    public partial class NotenWichtungNeu
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public NotenWichtungNeu()
        {
            this.Lerngruppen = new HashSet<LerngruppeNeu>();
        }
    
        public int Id { get; set; }
        public string Bezeichnung { get; set; }
        public float MündlichQualität { get; set; }
        public float MündlichQuantität { get; set; }
        public float MündlichSonstige { get; set; }
        public float MündlichGesamt { get; set; }
        public float SchriftlichKlassenarbeit { get; set; }
        public float SchriftlichSonstige { get; set; }
        public float SchriftlichGesamt { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<LerngruppeNeu> Lerngruppen { get; set; }
    }
}
