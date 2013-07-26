//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Liduv.Model.EntityFramework
{
    using System;
    using System.Collections.Generic;
    
        [Serializable]
    public partial class Arbeit
    {
        public Arbeit()
        {
            this.Aufgaben = new HashSet<Aufgabe>();
            this.Noten = new HashSet<Note>();
        }
    
        public int Id { get; set; }
        public int JahrtypId { get; set; }
        public int HalbjahrtypId { get; set; }
        public int KlasseId { get; set; }
        public int LfdNr { get; set; }
        public string Bezeichnung { get; set; }
        public int FachId { get; set; }
        public int BewertungsschemaId { get; set; }
        public string Bepunktungstyp { get; set; }
        public System.DateTime Datum { get; set; }
        public bool IstKlausur { get; set; }
    
        public virtual ICollection<Aufgabe> Aufgaben { get; set; }
        public virtual Klasse Klasse { get; set; }
        public virtual Jahrtyp Jahrtyp { get; set; }
        public virtual Halbjahrtyp Halbjahrtyp { get; set; }
        public virtual Fach Fach { get; set; }
        public virtual ICollection<Note> Noten { get; set; }
        public virtual Bewertungsschema Bewertungsschema { get; set; }
    }
}
