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
    
    public partial class Prozentbereich
    {
        public int Id { get; set; }
        public int ZensurId { get; set; }
        public float VonProzent { get; set; }
        public float BisProzent { get; set; }
        public int BewertungsschemaId { get; set; }
    
        public virtual Zensur Zensur { get; set; }
        public virtual Bewertungsschema Bewertungsschema { get; set; }
    }
}
