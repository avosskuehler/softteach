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
    
    public abstract partial class Termin
    {
        public Termin()
        {
            this.Beschreibung = "";
            this.Ort = "";
        }
    
        public int Id { get; set; }
        public int TermintypId { get; set; }
        public string Beschreibung { get; set; }
        public int ErsteUnterrichtsstundeId { get; set; }
        public int LetzteUnterrichtsstundeId { get; set; }
        public string Ort { get; set; }
        public bool IstGeprüft { get; set; }
    
        public virtual Unterrichtsstunde ErsteUnterrichtsstunde { get; set; }
        public virtual Unterrichtsstunde LetzteUnterrichtsstunde { get; set; }
        public virtual Termintyp Termintyp { get; set; }
    }
}
