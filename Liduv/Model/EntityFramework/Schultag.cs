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
    public partial class Schultag
    {
        public int Id { get; set; }
        public int SchulwocheId { get; set; }
        public int TermintypId { get; set; }
        public System.DateTime Datum { get; set; }
    
        public virtual Schulwoche Schulwoche { get; set; }
        public virtual Termintyp Termintyp { get; set; }
    }
}
