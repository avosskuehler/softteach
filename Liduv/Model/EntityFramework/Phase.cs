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
    public partial class Phase
    {
        public Phase()
        {
            this.AbfolgeIndex = 0;
        }
    
        public int Id { get; set; }
        public int StundenentwurfId { get; set; }
        public int Zeit { get; set; }
        public int MediumId { get; set; }
        public int SozialformId { get; set; }
        public string Inhalt { get; set; }
        public int AbfolgeIndex { get; set; }
    
        public virtual Stundenentwurf Stundenentwurf { get; set; }
        public virtual Sozialform Sozialform { get; set; }
        public virtual Medium Medium { get; set; }
    }
}
