﻿//------------------------------------------------------------------------------
// <auto-generated>
//     Der Code wurde von einer Vorlage generiert.
//
//     Manuelle Änderungen an dieser Datei führen möglicherweise zu unerwartetem Verhalten der Anwendung.
//     Manuelle Änderungen an dieser Datei werden überschrieben, wenn der Code neu generiert wird.
// </auto-generated>
//------------------------------------------------------------------------------

namespace SoftTeach.Model.TeachyModel
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class TeachyEntities : DbContext
    {
        public TeachyEntities()
            : base("name=TeachyEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<ArbeitNeu> Arbeiten { get; set; }
        public virtual DbSet<AufgabeNeu> Aufgaben { get; set; }
        public virtual DbSet<BewertungsschemaNeu> Bewertungsschemata { get; set; }
        public virtual DbSet<CurriculumNeu> Curricula { get; set; }
        public virtual DbSet<DateitypNeu> Dateitypen { get; set; }
        public virtual DbSet<DateiverweisNeu> Dateiverweise { get; set; }
        public virtual DbSet<ErgebnisNeu> Ergebnisse { get; set; }
        public virtual DbSet<FachNeu> Fächer { get; set; }
        public virtual DbSet<FachstundenanzahlNeu> Fachstundenanzahlen { get; set; }
        public virtual DbSet<FerienNeu> Ferien { get; set; }
        public virtual DbSet<HausaufgabeNeu> Hausaufgaben { get; set; }
        public virtual DbSet<LerngruppeNeu> Lerngruppen { get; set; }
        public virtual DbSet<ModulNeu> Module { get; set; }
        public virtual DbSet<NoteNeu> Noten { get; set; }
        public virtual DbSet<NotentendenzNeu> Notentendenzen { get; set; }
        public virtual DbSet<PhaseNeu> Phasen { get; set; }
        public virtual DbSet<ProzentbereichNeu> Prozentbereiche { get; set; }
        public virtual DbSet<RaumNeu> Räume { get; set; }
        public virtual DbSet<RaumplanNeu> Raumpläne { get; set; }
        public virtual DbSet<ReiheNeu> Reihen { get; set; }
        public virtual DbSet<SequenzNeu> Sequenzen { get; set; }
        public virtual DbSet<SitzplaneintragNeu> Sitzplaneinträge { get; set; }
        public virtual DbSet<SitzplatzNeu> Sitzplätze { get; set; }
        public virtual DbSet<StundenplanNeu> Stundenpläne { get; set; }
        public virtual DbSet<StundenplaneintragNeu> Stundenplaneinträge { get; set; }
        public virtual DbSet<TerminNeu> Termine { get; set; }
        public virtual DbSet<UnterrichtsstundeNeu> Unterrichtsstunden { get; set; }
        public virtual DbSet<ZensurNeu> Zensuren { get; set; }
        public virtual DbSet<PersonNeu> Personen { get; set; }
        public virtual DbSet<SchülereintragNeu> Schülereinträge { get; set; }
        public virtual DbSet<SitzplanNeu> Sitzpläne { get; set; }
        public virtual DbSet<SchuljahrNeu> Schuljahre { get; set; }
        public virtual DbSet<BetroffeneLerngruppeNeu> BetroffeneLerngruppen { get; set; }
        public virtual DbSet<NotenWichtungNeu> NotenWichtungen { get; set; }
    }
}