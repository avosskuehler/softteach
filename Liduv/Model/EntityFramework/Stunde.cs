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
    
    public partial class Stunde : Lerngruppentermin
    {
        public Stunde()
        {
            this.IstBenotet = false;
        }
    
        public Nullable<int> StundenentwurfId { get; set; }
        public bool IstBenotet { get; set; }
    
        public virtual Stundenentwurf Stundenentwurf { get; set; }
    }
}
