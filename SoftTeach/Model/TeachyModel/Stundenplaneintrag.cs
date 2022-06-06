using System;
using System.Collections.Generic;

namespace SoftTeach.Model.TeachyModel
{
    public partial class Stundenplaneintrag
    {
        public int Id { get; set; }
        public int StundenplanId { get; set; }
        public int ErsteUnterrichtsstundeIndex { get; set; }
        public int LetzteUnterrichtsstundeIndex { get; set; }
        public int WochentagIndex { get; set; }
        public int LerngruppeId { get; set; }
        public int RaumId { get; set; }

        public virtual Lerngruppe Lerngruppe { get; set; }
        public virtual Raum Raum { get; set; }
        public virtual Stundenplan Stundenplan { get; set; }
    }
}
