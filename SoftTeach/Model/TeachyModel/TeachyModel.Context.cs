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
        public virtual DbSet<BetroffeneKlasseNeu> BetroffeneKlassen { get; set; }
        public virtual DbSet<BewertungsschemataNeu> Bewertungsschemata { get; set; }
        public virtual DbSet<CurriculaNeu> Curricula { get; set; }
        public virtual DbSet<DateitypNeu> Dateitypen { get; set; }
        public virtual DbSet<DateiverweisNeu> Dateiverweise { get; set; }
        public virtual DbSet<ErgebnisseNeu> Ergebnisse { get; set; }
        public virtual DbSet<FächerNeu> Fächer { get; set; }
        public virtual DbSet<FachstundenanzahlenNeu> Fachstundenanzahlen { get; set; }
        public virtual DbSet<FerienNeu> Ferien { get; set; }
        public virtual DbSet<HausaufgabenNeu> Hausaufgaben { get; set; }
        public virtual DbSet<LerngruppeNeu> Lerngruppen { get; set; }
        public virtual DbSet<ModuleNeu> Module { get; set; }
        public virtual DbSet<NotenNeu> Noten { get; set; }
        public virtual DbSet<NotentendenzenNeu> Notentendenzen { get; set; }
        public virtual DbSet<NotenWichtungNeu> NotenWichtungen { get; set; }
        public virtual DbSet<PersonenNeu> Personen { get; set; }
        public virtual DbSet<PhaseNeu> Phasen { get; set; }
        public virtual DbSet<ProzentbereicheNeu> Prozentbereiche { get; set; }
        public virtual DbSet<RaumNeu> Räume { get; set; }
        public virtual DbSet<RaumplanNeu> Raumpläne { get; set; }
        public virtual DbSet<ReihenNeu> Reihen { get; set; }
        public virtual DbSet<SchülereinträgeNeu> Schülereinträge { get; set; }
        public virtual DbSet<SchülerlistenNeu> Schülerlisten { get; set; }
        public virtual DbSet<SchuljahrNeu> Schuljahre { get; set; }
        public virtual DbSet<SequenzenNeu> Sequenzen { get; set; }
        public virtual DbSet<SitzplanNeu> Sitzpläne { get; set; }
        public virtual DbSet<SitzplaneintragNeu> Sitzplaneinträge { get; set; }
        public virtual DbSet<SitzplatzNeu> Sitzplätze { get; set; }
        public virtual DbSet<StundenplanNeu> Stundenpläne { get; set; }
        public virtual DbSet<StundenplaneintragNeu> Stundenplaneinträge { get; set; }
        public virtual DbSet<TerminNeu> Termine { get; set; }
        public virtual DbSet<UnterrichtsstundeNeu> Unterrichtsstunden { get; set; }
        public virtual DbSet<ZensurenNeu> Zensuren { get; set; }
    }
}
