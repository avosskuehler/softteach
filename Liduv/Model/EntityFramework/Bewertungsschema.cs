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
    public partial class Bewertungsschema
    {
        public Bewertungsschema()
        {
            this.Prozentbereiche = new HashSet<Prozentbereich>();
            this.Arbeiten = new HashSet<Arbeit>();
        }
    
        public int Id { get; set; }
        public string Bezeichnung { get; set; }
    
        public virtual ICollection<Prozentbereich> Prozentbereiche { get; set; }
        public virtual ICollection<Arbeit> Arbeiten { get; set; }
    }
}
