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
    
    public partial class Schulwoche
    {
        public Schulwoche()
        {
            this.Schultage = new HashSet<Schultag>();
        }
    
        public int Id { get; set; }
        public int JahrtypId { get; set; }
        public System.DateTime Montagsdatum { get; set; }
    
        public virtual Jahrtyp Jahrtyp { get; set; }
        public virtual ICollection<Schultag> Schultage { get; set; }
    }
}