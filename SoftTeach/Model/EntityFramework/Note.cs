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
    
    public partial class Note
    {
        public Note()
        {
            this.NotenTermintyp = "Einzeln";
        }
    
        public int Id { get; set; }
        public int SchülereintragId { get; set; }
        public int ZensurId { get; set; }
        public System.DateTime Datum { get; set; }
        public Nullable<int> ArbeitId { get; set; }
        public int Wichtung { get; set; }
        public bool IstSchriftlich { get; set; }
        public string Bezeichnung { get; set; }
        public string Notentyp { get; set; }
        public string NotenTermintyp { get; set; }
    
        public virtual Zensur Zensur { get; set; }
        public virtual Schülereintrag Schülereintrag { get; set; }
        public virtual Arbeit Arbeit { get; set; }
    }
}