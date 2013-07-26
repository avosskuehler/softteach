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
    public partial class Person
    {
        public Person()
        {
            this.Vorname = "";
            this.Nachname = "";
            this.Geschlecht = false;
            this.Titel = "";
            this.Telefon = "";
            this.Fax = "";
            this.Handy = "";
            this.EMail = "";
            this.PLZ = "";
            this.Straße = "";
            this.Hausnummer = "";
            this.Ort = "";
            this.Schülereintrag = new HashSet<Schülereintrag>();
        }
    
        public int Id { get; set; }
        public string Vorname { get; set; }
        public string Nachname { get; set; }
        public bool Geschlecht { get; set; }
        public Nullable<System.DateTime> Geburtstag { get; set; }
        public string Titel { get; set; }
        public string Telefon { get; set; }
        public string Fax { get; set; }
        public string Handy { get; set; }
        public string EMail { get; set; }
        public string PLZ { get; set; }
        public string Straße { get; set; }
        public string Hausnummer { get; set; }
        public string Ort { get; set; }
        public bool IstLehrer { get; set; }
        public byte[] Foto { get; set; }
    
        public virtual ICollection<Schülereintrag> Schülereintrag { get; set; }
    }
}
