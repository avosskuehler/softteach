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
    public partial class Schülerliste
    {
        public Schülerliste()
        {
            this.Schülereinträge = new HashSet<Schülereintrag>();
        }
    
        public int Id { get; set; }
        public int JahrtypId { get; set; }
        public int HalbjahrtypId { get; set; }
        public int KlasseId { get; set; }
        public int FachId { get; set; }
        public int NotenWichtungId { get; set; }
    
        public virtual Halbjahrtyp Halbjahrtyp { get; set; }
        public virtual Klasse Klasse { get; set; }
        public virtual ICollection<Schülereintrag> Schülereinträge { get; set; }
        public virtual Fach Fach { get; set; }
        public virtual NotenWichtung NotenWichtung { get; set; }
        public virtual Jahrtyp Jahrtyp { get; set; }
    }
}
