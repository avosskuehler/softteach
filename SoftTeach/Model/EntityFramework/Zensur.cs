//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace SoftTeach.Model.EntityFramework
{
    using System;
    using System.Collections.Generic;
    
    public partial class Zensur
    {
        public Zensur()
        {
            this.Noten = new HashSet<Note>();
            this.Prozentbereiche = new HashSet<Prozentbereich>();
        }
    
        public int Id { get; set; }
        public int Notenpunkte { get; set; }
        public string NoteMitTendenz { get; set; }
        public int GanzeNote { get; set; }
    
        public virtual ICollection<Note> Noten { get; set; }
        public virtual ICollection<Prozentbereich> Prozentbereiche { get; set; }
    }
}