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
    
    public partial class PhaseNeu
    {
        public int Id { get; set; }
        public int StundeId { get; set; }
        public int Zeit { get; set; }
        public Medium Medium { get; set; }
        public Sozialform Sozialform { get; set; }
        public string Inhalt { get; set; }
        public int Reihenfolge { get; set; }
    
        public virtual StundeNeu Stunde { get; set; }
    }
}