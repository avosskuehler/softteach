﻿//------------------------------------------------------------------------------
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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class SoftTeachDataModel : DbContext
    {
        public SoftTeachDataModel()
            : base("name=SoftTeachDataModel")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public DbSet<Stundenentwurf> Stundenentwürfe { get; set; }
        public DbSet<Phase> Phasen { get; set; }
        public DbSet<Medium> Medien { get; set; }
        public DbSet<Sozialform> Sozialformen { get; set; }
        public DbSet<Modul> Module { get; set; }
        public DbSet<Fach> Fächer { get; set; }
        public DbSet<Halbjahresplan> Halbjahrespläne { get; set; }
        public DbSet<Klasse> Klassen { get; set; }
        public DbSet<Unterrichtsstunde> Unterrichtsstunden { get; set; }
        public DbSet<Jahrgangsstufe> Jahrgangsstufen { get; set; }
        public DbSet<Tagesplan> Tagespläne { get; set; }
        public DbSet<Monatsplan> Monatspläne { get; set; }
        public DbSet<Jahresplan> Jahrespläne { get; set; }
        public DbSet<Halbjahrtyp> Halbjahrtypen { get; set; }
        public DbSet<Monatstyp> Monatstypen { get; set; }
        public DbSet<Stundenplaneintrag> Stundenplaneinträge { get; set; }
        public DbSet<Jahrtyp> Jahrtypen { get; set; }
        public DbSet<Stundenplan> Stundenpläne { get; set; }
        public DbSet<Ferien> Ferien { get; set; }
        public DbSet<BetroffeneKlasse> BetroffeneKlassen { get; set; }
        public DbSet<Termintyp> Termintypen { get; set; }
        public DbSet<Dateiverweis> Dateiverweise { get; set; }
        public DbSet<Dateityp> Dateitypen { get; set; }
        public DbSet<Termin> Termine { get; set; }
        public DbSet<Reihe> Reihen { get; set; }
        public DbSet<Curriculum> Curricula { get; set; }
        public DbSet<Sequenz> Sequenzen { get; set; }
        public DbSet<Fachstundenanzahl> Fachstundenanzahlen { get; set; }
        public DbSet<Klassenstufe> Klassenstufen { get; set; }
        public DbSet<Schulwoche> Schulwochen { get; set; }
        public DbSet<Schultag> Schultage { get; set; }
        public DbSet<Person> Personen { get; set; }
        public DbSet<Note> Noten { get; set; }
        public DbSet<Zensur> Zensuren { get; set; }
        public DbSet<Arbeit> Arbeiten { get; set; }
        public DbSet<Aufgabe> Aufgaben { get; set; }
        public DbSet<Ergebnis> Ergebnisse { get; set; }
        public DbSet<Schülereintrag> Schülereinträge { get; set; }
        public DbSet<Hausaufgabe> Hausaufgaben { get; set; }
        public DbSet<Notentendenz> Notentendenzen { get; set; }
        public DbSet<Tendenz> Tendenzen { get; set; }
        public DbSet<Tendenztyp> Tendenztypen { get; set; }
        public DbSet<NotenWichtung> NotenWichtungen { get; set; }
        public DbSet<Prozentbereich> Prozentbereiche { get; set; }
        public DbSet<Bewertungsschema> Bewertungsschemata { get; set; }
        public DbSet<Raum> Räume { get; set; }
        public DbSet<Raumplan> Raumpläne { get; set; }
        public DbSet<Sitzplan> Sitzpläne { get; set; }
        public DbSet<Sitzplaneintrag> Sitzplaneinträge { get; set; }
        public DbSet<Sitzplatz> Sitzplätze { get; set; }
        public DbSet<Schülerliste> Schülerlisten { get; set; }
    }
}