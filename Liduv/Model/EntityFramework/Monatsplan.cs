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
    
    public partial class Monatsplan
    {
        public Monatsplan()
        {
            this.Tagespläne = new HashSet<Tagesplan>();
        }
    
        public int Id { get; set; }
        public int HalbjahresplanId { get; set; }
        public int MonatstypId { get; set; }
    
        public virtual Monatstyp Monatstyp { get; set; }
        public virtual Halbjahresplan Halbjahresplan { get; set; }
        public virtual ICollection<Tagesplan> Tagespläne { get; set; }
    }
}
