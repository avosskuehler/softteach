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
    public partial class Dateiverweis
    {
        public int Id { get; set; }
        public string Dateiname { get; set; }
        public int DateitypId { get; set; }
        public int StundenentwurfId { get; set; }
    
        public virtual Dateityp Dateityp { get; set; }
        public virtual Stundenentwurf Stundenentwurf { get; set; }
    }
}
