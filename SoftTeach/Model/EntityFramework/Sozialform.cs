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
    
    public partial class Sozialform
    {
        public Sozialform()
        {
            this.Phase = new HashSet<Phase>();
        }
    
        public int Id { get; set; }
        public string Bezeichnung { get; set; }
    
        public virtual ICollection<Phase> Phase { get; set; }
    }
}
