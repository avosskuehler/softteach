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
    
    public partial class ErgebnisseNeu
    {
        public int Id { get; set; }
        public int AufgabeId { get; set; }
        public int SchülereintragId { get; set; }
        public Nullable<double> Punktzahl { get; set; }
    
        public virtual AufgabeNeu Aufgabe { get; set; }
        public virtual SchülereinträgeNeu Schülereintrag { get; set; }
    }
}
