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
    
    public partial class Sitzplan
    {
        public Sitzplan()
        {
            this.Sitzplaneinträge = new HashSet<Sitzplaneintrag>();
        }
    
        public int Id { get; set; }
        public int RaumplanId { get; set; }
        public int SchülerlisteId { get; set; }
        public System.DateTime GültigAb { get; set; }
    
        public virtual Raumplan Raumplan { get; set; }
        public virtual Schülerliste Schülerliste { get; set; }
        public virtual ICollection<Sitzplaneintrag> Sitzplaneinträge { get; set; }
    }
}
