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
    
    public partial class Reihe
    {
        public Reihe()
        {
            this.Sequenzen = new HashSet<Sequenz>();
        }
    
        public int Id { get; set; }
        public int ModulId { get; set; }
        public string Thema { get; set; }
        public int Stundenbedarf { get; set; }
        public int CurriculumId { get; set; }
        public int AbfolgeIndex { get; set; }
    
        public virtual Modul Modul { get; set; }
        public virtual ICollection<Sequenz> Sequenzen { get; set; }
        public virtual Curriculum Curriculum { get; set; }
    }
}