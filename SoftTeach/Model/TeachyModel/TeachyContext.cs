using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using SoftTeach.Model.TeachyModel;

namespace SoftTeach.Model.TeachyModel
{
  public partial class TeachyContext : DbContext
  {
    public TeachyContext()
    {
    }

    public TeachyContext(DbContextOptions<TeachyContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Arbeit> Arbeiten { get; set; }
    public virtual DbSet<Aufgabe> Aufgaben { get; set; }
    public virtual DbSet<BetroffeneLerngruppe> BetroffeneLerngruppen { get; set; }
    public virtual DbSet<Bewertungsschema> Bewertungsschemata { get; set; }
    public virtual DbSet<Curriculum> Curricula { get; set; }
    public virtual DbSet<Dateityp> Dateitypen { get; set; }
    public virtual DbSet<Dateiverweis> Dateiverweise { get; set; }
    public virtual DbSet<Ergebnis> Ergebnisse { get; set; }
    public virtual DbSet<Fach> Fächer { get; set; }
    public virtual DbSet<Fachstundenanzahl> Fachstundenanzahlen { get; set; }
    public virtual DbSet<Ferien> Ferien { get; set; }
    public virtual DbSet<Hausaufgabe> Hausaufgaben { get; set; }
    public virtual DbSet<Lerngruppe> Lerngruppen { get; set; }
    public virtual DbSet<Modul> Module { get; set; }
    public virtual DbSet<Note> Noten { get; set; }
    public virtual DbSet<NotenWichtung> NotenWichtungen { get; set; }
    public virtual DbSet<Notentendenz> Notentendenzen { get; set; }
    public virtual DbSet<Person> Personen { get; set; }
    public virtual DbSet<Phase> Phasen { get; set; }
    public virtual DbSet<Prozentbereich> Prozentbereiche { get; set; }
    public virtual DbSet<Raum> Räume { get; set; }
    public virtual DbSet<Raumplan> Raumpläne { get; set; }
    public virtual DbSet<Reihe> Reihen { get; set; }
    public virtual DbSet<Schuljahr> Schuljahre { get; set; }
    public virtual DbSet<Schülereintrag> Schülereinträge { get; set; }
    public virtual DbSet<Sequenz> Sequenzen { get; set; }
    public virtual DbSet<Sitzplan> Sitzpläne { get; set; }
    public virtual DbSet<Sitzplaneintrag> Sitzplaneinträge { get; set; }
    public virtual DbSet<Sitzplatz> Sitzplätze { get; set; }
    public virtual DbSet<Stundenplan> Stundenpläne { get; set; }
    public virtual DbSet<Stundenplaneintrag> Stundenplaneinträge { get; set; }
    public virtual DbSet<Termin> Termine { get; set; }
    public virtual DbSet<Schultermin> Schultermine { get; set; }
    public virtual DbSet<Lerngruppentermin> Lerngruppentermine { get; set; }
    public virtual DbSet<Stunde> Stunden { get; set; }
    public virtual DbSet<Unterrichtsstunde> Unterrichtsstunden { get; set; }
    public virtual DbSet<Zensur> Zensuren { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
      if (!optionsBuilder.IsConfigured)
      {
        optionsBuilder
          .UseLazyLoadingProxies()
          .UseSqlServer("Server=VK2\\SQL2017;Database=Teachy;Trusted_Connection=True;");
      }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      modelBuilder.Entity<Arbeit>(entity =>
      {
        entity.ToTable("Arbeit");

        entity.HasIndex(e => e.LerngruppeId, "IX_FK_KlasseArbeit");

        entity.Property(e => e.Bezeichnung)
                  .IsRequired()
                  .IsUnicode(false);

        entity.Property(e => e.Datum).HasColumnType("datetime");

        entity.HasOne(d => d.Bewertungsschema)
                  .WithMany(p => p.Arbeiten)
                  .HasForeignKey(d => d.BewertungsschemaId)
                  .HasConstraintName("FK_BewertungsschemaArbeit");

        entity.HasOne(d => d.Fach)
                  .WithMany(p => p.Arbeiten)
                  .HasForeignKey(d => d.FachId)
                  .HasConstraintName("FK_FachArbeit");

        entity.HasOne(d => d.Lerngruppe)
                  .WithMany(p => p.Arbeiten)
                  .HasForeignKey(d => d.LerngruppeId)
                  .HasConstraintName("FK_LerngruppeArbeit");
      });
      //modelBuilder.Entity<Arbeit>().Navigation(e => e.Aufgaben).AutoInclude();

      modelBuilder.Entity<Aufgabe>(entity =>
      {
        entity.ToTable("Aufgabe");

        entity.HasIndex(e => e.ArbeitId, "IX_FK_ArbeitAufgabe");

        entity.HasOne(d => d.Arbeit)
                  .WithMany(p => p.Aufgaben)
                  .HasForeignKey(d => d.ArbeitId)
                  .HasConstraintName("FK_ArbeitAufgabe");
      });

      modelBuilder.Entity<BetroffeneLerngruppe>(entity =>
      {
        entity.ToTable("BetroffeneLerngruppe");

        entity.HasIndex(e => e.LerngruppeId, "IX_FK_KlasseBetroffeneKlasse");

        entity.HasIndex(e => e.TerminId, "IX_FK_TerminBetroffeneKlasse");

        entity.HasOne(d => d.Lerngruppe)
                  .WithMany(p => p.BetroffeneLerngruppen)
                  .HasForeignKey(d => d.LerngruppeId)
                  .HasConstraintName("FK_LerngruppeBetroffeneLerngruppe");

        entity.HasOne(d => d.Schultermin)
                  .WithMany(p => p.BetroffeneLerngruppen)
                  .HasForeignKey(d => d.TerminId)
                  .OnDelete(DeleteBehavior.ClientSetNull)
                  .HasConstraintName("FK_TerminBetroffeneLerngruppe");
      });

      modelBuilder.Entity<Bewertungsschema>(entity =>
      {
        entity.ToTable("Bewertungsschema");

        entity.Property(e => e.Bezeichnung)
                  .IsRequired()
                  .HasMaxLength(200);
      });
      //modelBuilder.Entity<Bewertungsschema>().Navigation(e => e.Prozentbereiche).AutoInclude();

      modelBuilder.Entity<Curriculum>(entity =>
      {
        entity.ToTable("Curriculum");

        entity.HasIndex(e => e.FachId, "IX_FK_FachCurriculum");

        entity.HasIndex(e => e.SchuljahrId, "IX_FK_JahrtypCurriculum");

        entity.HasIndex(e => e.Jahrgang, "IX_FK_KlassenstufeCurriculum");

        entity.Property(e => e.Bezeichnung).IsRequired();

        entity.Property(e => e.Halbjahr).HasDefaultValueSql("((1))");

        entity.HasOne(d => d.Fach)
                  .WithMany(p => p.Curricula)
                  .HasForeignKey(d => d.FachId)
                  .HasConstraintName("FK_FachCurriculum");

        entity.HasOne(d => d.Schuljahr)
                  .WithMany(p => p.Curricula)
                  .HasForeignKey(d => d.SchuljahrId)
                  .HasConstraintName("FK_SchuljahrCurriculum");
      });

      //modelBuilder.Entity<Curriculum>().Navigation(e => e.Reihen).AutoInclude();

      modelBuilder.Entity<Dateityp>(entity =>
      {
        entity.ToTable("Dateityp");

        entity.Property(e => e.Bezeichnung).IsRequired();

        entity.Property(e => e.Kürzel).IsRequired();
      });

      modelBuilder.Entity<Dateiverweis>(entity =>
      {
        entity.ToTable("Dateiverweis");
        entity.HasIndex(e => e.DateitypId, "IX_FK_DateitypDateiverweis");

        entity.HasIndex(e => e.StundeId, "IX_FK_StundenentwurfDateiverweis");

        entity.Property(e => e.Dateiname).IsRequired();

        entity.HasOne(d => d.Dateityp)
                  .WithMany(p => p.Dateiverweise)
                  .HasForeignKey(d => d.DateitypId)
                  .HasConstraintName("FK_DateitypDateiverweis");

        entity.HasOne(d => d.Stunde)
                  .WithMany(p => p.Dateiverweise)
                  .HasForeignKey(d => d.StundeId)
                  .HasConstraintName("FK_StundeDateiverweis");
      });

      modelBuilder.Entity<Ergebnis>(entity =>
      {
        entity.ToTable("Ergebnis");
        entity.HasIndex(e => e.AufgabeId, "IX_FK_AufgabeErgebnis");

        entity.HasIndex(e => e.SchülereintragId, "IX_FK_SchülereintragErgebnis");

        entity.HasOne(d => d.Aufgabe)
                  .WithMany(p => p.Ergebnisse)
                  .HasForeignKey(d => d.AufgabeId)
                  .HasConstraintName("FK_AufgabeErgebnis");

        entity.HasOne(d => d.Schülereintrag)
                  .WithMany(p => p.Ergebnisse)
                  .HasForeignKey(d => d.SchülereintragId)
                  .HasConstraintName("FK_SchülereintragErgebnis");
      });

      modelBuilder.Entity<Fach>(entity =>
      {
        entity.ToTable("Fach");

        entity.Property(e => e.Bezeichnung).IsRequired();

        entity.Property(e => e.Farbe).IsRequired();
      });

      modelBuilder.Entity<Fachstundenanzahl>(entity =>
      {
        entity.ToTable("Fachstundenanzahl");

        entity.HasIndex(e => e.FachId, "IX_FK_FachFachstundenanzahl");

        entity.HasIndex(e => e.Jahrgang, "IX_FK_KlassenstufeFachstundenanzahl");

        entity.HasOne(d => d.Fach)
                  .WithMany(p => p.Fachstundenanzahlen)
                  .HasForeignKey(d => d.FachId)
                  .HasConstraintName("FK_FachFachstundenanzahl");
      });

      modelBuilder.Entity<Ferien>(entity =>
      {
        entity.ToTable("Ferien");

        entity.HasIndex(e => e.SchuljahrId, "IX_FK_JahrtypFerien");

        entity.Property(e => e.Bezeichnung).IsRequired();

        entity.Property(e => e.ErsterFerientag).HasColumnType("datetime");

        entity.Property(e => e.LetzterFerientag).HasColumnType("datetime");

        entity.HasOne(d => d.Schuljahr)
                  .WithMany(p => p.Ferien)
                  .HasForeignKey(d => d.SchuljahrId)
                  .HasConstraintName("FK_SchuljahrFerien");
      });

      modelBuilder.Entity<Hausaufgabe>(entity =>
      {
        entity.ToTable("Hausaufgabe");

        entity.Property(e => e.Bezeichnung).HasMaxLength(50);

        entity.Property(e => e.Datum).HasColumnType("datetime");

        entity.HasOne(d => d.Schülereintrag)
                  .WithMany(p => p.Hausaufgaben)
                  .HasForeignKey(d => d.SchülereintragId)
                  .HasConstraintName("FK_SchülereintragHausaufgaben");
      });

      modelBuilder.Entity<Lerngruppe>(entity =>
      {
        entity.ToTable("Lerngruppe");

        entity.HasIndex(e => e.FachId, "IX_FK_FachSchülerliste");

        entity.HasIndex(e => e.SchuljahrId, "IX_FK_JahrtypSchülerliste");

        entity.HasIndex(e => e.Bezeichnung, "IX_FK_KlasseSchülerliste");

        entity.Property(e => e.Bezeichnung)
                  .IsRequired()
                  .HasMaxLength(50);

        entity.HasOne(d => d.Fach)
                  .WithMany(p => p.Lerngruppen)
                  .HasForeignKey(d => d.FachId)
                  .OnDelete(DeleteBehavior.ClientSetNull)
                  .HasConstraintName("FK_FachLerngruppe");

        entity.HasOne(d => d.NotenWichtung)
                  .WithMany(p => p.Lerngruppen)
                  .HasForeignKey(d => d.NotenWichtungId)
                  .HasConstraintName("FK_NotenWichtungenLerngruppe");

        entity.HasOne(d => d.Schuljahr)
                  .WithMany(p => p.Lerngruppen)
                  .HasForeignKey(d => d.SchuljahrId)
                  .HasConstraintName("FK_SchuljahrLerngruppe");
      });

      //modelBuilder.Entity<Lerngruppe>().Navigation(e => e.Schülereinträge).AutoInclude();
      //modelBuilder.Entity<Lerngruppe>().Navigation(e => e.Lerngruppentermine).AutoInclude();

      modelBuilder.Entity<Modul>(entity =>
      {
        entity.ToTable("Modul");

        entity.HasIndex(e => e.FachId, "IX_FK_FachModul");

        entity.HasIndex(e => e.Jahrgang, "IX_FK_JahrgangsstufeModul");

        entity.Property(e => e.Bezeichnung).IsRequired();

        entity.HasOne(d => d.Fach)
                  .WithMany(p => p.Module)
                  .HasForeignKey(d => d.FachId)
                  .HasConstraintName("FK_FachModul");
      });

      modelBuilder.Entity<Note>(entity =>
      {
        entity.ToTable("Note");

        entity.HasIndex(e => e.ZensurId, "IX_FK_ZensurNoten");

        entity.Property(e => e.Datum).HasColumnType("datetime");

        entity.HasOne(d => d.Arbeit)
                  .WithMany(p => p.Noten)
                  .HasForeignKey(d => d.ArbeitId)
                  .OnDelete(DeleteBehavior.Cascade)
                  .HasConstraintName("FK_ArbeitNoten");

        entity.HasOne(d => d.Schülereintrag)
                  .WithMany(p => p.Noten)
                  .HasForeignKey(d => d.SchülereintragId)
                  .HasConstraintName("FK_SchülereintragNoten");

        entity.HasOne(d => d.Zensur)
                  .WithMany(p => p.Noten)
                  .HasForeignKey(d => d.ZensurId)
                  .HasConstraintName("FK_ZensurNoten");
      });

      modelBuilder.Entity<NotenWichtung>(entity =>
      {
        entity.ToTable("NotenWichtung");

        entity.Property(e => e.Bezeichnung).IsRequired();
      });

      modelBuilder.Entity<Notentendenz>(entity =>
      {
        entity.ToTable("Notentendenz");

        entity.Property(e => e.Datum).HasColumnType("datetime");

        entity.HasOne(d => d.Schülereintrag)
                  .WithMany(p => p.Notentendenzen)
                  .HasForeignKey(d => d.SchülereintragId)
                  .HasConstraintName("FK_SchülereintragNotentendenz");
      });

      modelBuilder.Entity<Person>(entity =>
      {
        entity.ToTable("Person");

        entity.Property(e => e.EMail).HasColumnName("EMail");

        entity.Property(e => e.Foto).HasColumnType("image");

        entity.Property(e => e.Geburtstag).HasColumnType("datetime");

        entity.Property(e => e.Nachname).IsRequired();

        entity.Property(e => e.PLZ).HasColumnName("PLZ");

        entity.Property(e => e.Vorname).IsRequired();
      });

      modelBuilder.Entity<Phase>(entity =>
      {
        entity.HasKey(e => new { e.Id, e.StundeId })
                  .HasName("PK_Phasen");

        entity.ToTable("Phase");

        entity.HasIndex(e => e.Medium, "IX_FK_MediumPhase");

        entity.HasIndex(e => e.Sozialform, "IX_FK_SozialformPhase");

        entity.HasIndex(e => e.StundeId, "IX_FK_StundenentwurfPhasen");

        entity.Property(e => e.Id).ValueGeneratedOnAdd();

        entity.Property(e => e.Inhalt).IsRequired();

        entity.HasOne(d => d.Stunde)
                  .WithMany(p => p.Phasen)
                  .HasForeignKey(d => d.StundeId)
                  .HasConstraintName("FK_StundePhase");
      });

      modelBuilder.Entity<Prozentbereich>(entity =>
      {
        entity.ToTable("Prozentbereich");

        entity.HasOne(d => d.Bewertungsschema)
                  .WithMany(p => p.Prozentbereiche)
                  .HasForeignKey(d => d.BewertungsschemaId)
                  .HasConstraintName("FK_BewertungsschemaProzentbereich");

        entity.HasOne(d => d.Zensur)
                  .WithMany(p => p.Prozentbereiche)
                  .HasForeignKey(d => d.ZensurId)
                  .HasConstraintName("FK_ZensurProzentbereich");
      });

      modelBuilder.Entity<Raum>(entity =>
      {
        entity.ToTable("Raum");

        entity.Property(e => e.Bezeichnung).IsRequired();
      });
      //modelBuilder.Entity<Raum>().Navigation(e => e.Raumpläne).AutoInclude();

      modelBuilder.Entity<Raumplan>(entity =>
      {
        entity.ToTable("Raumplan");

        entity.Property(e => e.Bezeichnung).IsRequired();

        entity.Property(e => e.Grundriss)
                  .IsRequired()
                  .HasColumnType("image");

        entity.HasOne(d => d.Raum)
                  .WithMany(p => p.Raumpläne)
                  .HasForeignKey(d => d.RaumId)
                  .HasConstraintName("FK_RaumRaumplan");
      });
      //modelBuilder.Entity<Raumplan>().Navigation(e => e.Sitzplätze).AutoInclude();

      modelBuilder.Entity<Reihe>(entity =>
      {
        entity.ToTable("Reihe");

        entity.HasIndex(e => e.CurriculumId, "IX_FK_CurriculumReihe");

        entity.HasIndex(e => e.ModulId, "IX_FK_ModulReihe");

        entity.Property(e => e.Thema).IsRequired();

        entity.HasOne(d => d.Curriculum)
                  .WithMany(p => p.Reihen)
                  .HasForeignKey(d => d.CurriculumId)
                  .HasConstraintName("FK_CurriculumReihe");

        entity.HasOne(d => d.Modul)
                  .WithMany(p => p.Reihen)
                  .HasForeignKey(d => d.ModulId)
                  .OnDelete(DeleteBehavior.ClientSetNull)
                  .HasConstraintName("FK_ModulReihe");
      });

      //modelBuilder.Entity<Reihe>().Navigation(e => e.Sequenzen).AutoInclude();

      modelBuilder.Entity<Schuljahr>(entity =>
      {
        entity.ToTable("Schuljahr");

        entity.Property(e => e.Bezeichnung).IsRequired();
      });

      modelBuilder.Entity<Schülereintrag>(entity =>
      {
        entity.ToTable("Schülereintrag");

        entity.HasIndex(e => e.PersonId, "IX_FK_PersonSchülereintrag");

        entity.HasIndex(e => e.LerngruppeId, "IX_FK_SchülerlisteSchülereintrag");

        entity.HasOne(d => d.Lerngruppe)
                  .WithMany(p => p.Schülereinträge)
                  .HasForeignKey(d => d.LerngruppeId)
                  .OnDelete(DeleteBehavior.ClientSetNull)
                  .HasConstraintName("FK_LerngruppeSchülereintrag");

        entity.HasOne(d => d.Person)
                  .WithMany(p => p.Schülereinträge)
                  .HasForeignKey(d => d.PersonId)
                  .HasConstraintName("FK_PersonSchülereintrag");
      });

      //modelBuilder.Entity<Schülereintrag>().Navigation(e => e.Person).AutoInclude();
      //modelBuilder.Entity<Schülereintrag>().Navigation(e => e.Hausaufgaben).AutoInclude();
      //modelBuilder.Entity<Schülereintrag>().Navigation(e => e.Noten).AutoInclude();
      //modelBuilder.Entity<Schülereintrag>().Navigation(e => e.Notentendenzen).AutoInclude();
      //modelBuilder.Entity<Schülereintrag>().Navigation(e => e.Ergebnisse).AutoInclude();

      modelBuilder.Entity<Sequenz>(entity =>
      {
        entity.ToTable("Sequenz");

        entity.HasIndex(e => e.ReiheId, "IX_FK_ReiheSequenz");

        entity.Property(e => e.Thema).IsRequired();

        entity.HasOne(d => d.Reihe)
                  .WithMany(p => p.Sequenzen)
                  .HasForeignKey(d => d.ReiheId)
                  .HasConstraintName("FK_ReiheSequenz");
      });

      modelBuilder.Entity<Sitzplan>(entity =>
      {
        entity.ToTable("Sitzplan");

        entity.Property(e => e.Bezeichnung).IsRequired();

        entity.Property(e => e.GültigAb).HasColumnType("datetime");

        entity.HasOne(d => d.Lerngruppe)
                  .WithMany(p => p.Sitzpläne)
                  .HasForeignKey(d => d.LerngruppeId)
                  .HasConstraintName("FK_LerngruppeSitzplan");

        entity.HasOne(d => d.Raumplan)
                  .WithMany(p => p.Sitzpläne)
                  .HasForeignKey(d => d.RaumplanId)
                  .HasConstraintName("FK_RaumplanSitzplan");
      });

      //modelBuilder.Entity<Sitzplan>().Navigation(e => e.Sitzplaneinträge).AutoInclude();
      //modelBuilder.Entity<Sitzplan>().Navigation(e => e.Lerngruppe).AutoInclude();

      modelBuilder.Entity<Sitzplaneintrag>(entity =>
      {
        entity.ToTable("Sitzplaneintrag");

        entity.HasOne(d => d.Schülereintrag)
                  .WithMany(p => p.Sitzplaneinträge)
                  .HasForeignKey(d => d.SchülereintragId)
                  .OnDelete(DeleteBehavior.Cascade)
                  .HasConstraintName("FK_SchülereintragSitzplaneintrag");

        entity.HasOne(d => d.Sitzplan)
                  .WithMany(p => p.Sitzplaneinträge)
                  .HasForeignKey(d => d.SitzplanId)
                  .HasConstraintName("FK_SitzplanSitzplaneintrag");

        entity.HasOne(d => d.Sitzplatz)
                  .WithMany(p => p.Sitzplaneinträge)
                  .HasForeignKey(d => d.SitzplatzId)
                  .OnDelete(DeleteBehavior.ClientSetNull)
                  .HasConstraintName("FK_SitzplatzSitzplaneintrag");
      });

      modelBuilder.Entity<Sitzplatz>(entity =>
      {
        entity.ToTable("Sitzplatz");

        entity.HasOne(d => d.Raumplan)
                  .WithMany(p => p.Sitzplätze)
                  .HasForeignKey(d => d.RaumplanId)
                  .HasConstraintName("FK_RaumplanSitzplatz");
      });

      modelBuilder.Entity<Stundenplan>(entity =>
      {
        entity.ToTable("Stundenplan");

        entity.HasIndex(e => e.Halbjahr, "IX_FK_HalbjahrtypStundenplan");

        entity.HasIndex(e => e.SchuljahrId, "IX_FK_JahrtypStundenplan");

        entity.Property(e => e.Bezeichnung).IsRequired();

        entity.Property(e => e.GültigAb).HasColumnType("datetime");

        entity.Property(e => e.SchuljahrId).HasColumnName("SchuljahrID");

        entity.HasOne(d => d.Schuljahr)
                  .WithMany(p => p.Stundenpläne)
                  .HasForeignKey(d => d.SchuljahrId)
                  .HasConstraintName("FK_SchuljahrStundenplan");
      });
      //modelBuilder.Entity<Stundenplan>().Navigation(e => e.Stundenplaneinträge).AutoInclude();

      modelBuilder.Entity<Stundenplaneintrag>(entity =>
      {
        entity.ToTable("Stundenplaneintrag");

        entity.HasIndex(e => e.LerngruppeId, "IX_FK_KlasseStundenplaneintrag");

        entity.HasIndex(e => e.StundenplanId, "IX_FK_StundenplanStundenplaneintrag");

        entity.Property(e => e.LerngruppeId).HasColumnName("LerngruppeID");

        entity.HasOne(d => d.Lerngruppe)
                  .WithMany(p => p.Stundenplaneinträge)
                  .HasForeignKey(d => d.LerngruppeId)
                  .OnDelete(DeleteBehavior.ClientSetNull)
                  .HasConstraintName("FK_LerngruppeStundenplaneintrag");

        entity.HasOne(d => d.Raum)
                  .WithMany(p => p.Stundenplaneinträge)
                  .HasForeignKey(d => d.RaumId)
                  .HasConstraintName("FK_RaumStundenplaneintrag");

        entity.HasOne(d => d.Stundenplan)
                  .WithMany(p => p.Stundenplaneinträge)
                  .HasForeignKey(d => d.StundenplanId)
                  .HasConstraintName("FK_StundenplanStundenplaneintrag");
      });

      modelBuilder.Entity<Termin>()
        .HasDiscriminator<string>("Discriminator")
        .HasValue<Schultermin>("Schultermin")
        .HasValue<Lerngruppentermin>("Lerngruppentermin")
        .HasValue<Stunde>("Stunde");

      modelBuilder.Entity<Termin>(entity =>
      {
        entity.ToTable("Termin");

        entity.Property(e => e.TerminId).HasColumnName("TerminID");

        entity.Property(e => e.Datum).HasColumnType("datetime");

        entity.Property(e => e.Discriminator)
                  .IsRequired()
                  .HasMaxLength(50);

        entity.HasOne(d => d.ErsteUnterrichtsstunde)
                  .WithMany(p => p.TermineErsteUnterrichtsstunde)
                  .HasForeignKey(d => d.ErsteUnterrichtsstundeId)
                  .OnDelete(DeleteBehavior.ClientSetNull)
                  .HasConstraintName("FK_Termin_ErsteUnterrichtsstunde");

        entity.HasOne(d => d.LetzteUnterrichtsstunde)
                  .WithMany(p => p.TermineLetzteUnterrichtsstunde)
                  .HasForeignKey(d => d.LetzteUnterrichtsstundeId)
                  .OnDelete(DeleteBehavior.ClientSetNull)
                  .HasConstraintName("FK_Termin_LetzteUnterrichtsstunde");
      });

      modelBuilder.Entity<Lerngruppentermin>(entity =>
      {
        entity.HasOne(d => d.Lerngruppe)
                  .WithMany(p => p.Lerngruppentermine)
                  .HasForeignKey(d => d.LerngruppeId)
                  .HasConstraintName("FK_Termin_Lerngruppen");
      });

      modelBuilder.Entity<Stunde>(entity =>
      {
        entity.Property(e => e.IstBenotet).HasDefaultValueSql("((0))");

        entity.HasOne(d => d.Fach)
                  .WithMany(p => p.Stunden)
                  .HasForeignKey(d => d.FachId)
                  .HasConstraintName("FK_Termin_Fächer");

        entity.HasOne(d => d.Modul)
                  .WithMany(p => p.Stunden)
                  .HasForeignKey(d => d.ModulId)
                  .HasConstraintName("FK_Termin_Module");
      });

      //modelBuilder.Entity<Stunde>().Navigation(e => e.Phasen).AutoInclude();
      //modelBuilder.Entity<Stunde>().Navigation(e => e.Dateiverweise).AutoInclude();

      modelBuilder.Entity<Schultermin>(entity =>
      {
        entity.HasOne(d => d.Schuljahr)
                  .WithMany(p => p.Schultermine)
                  .HasForeignKey(d => d.SchuljahrId)
                  .OnDelete(DeleteBehavior.Cascade)
                  .HasConstraintName("FK_Termin_Schuljahre");
      });
      //modelBuilder.Entity<Schultermin>().Navigation(e => e.BetroffeneLerngruppen).AutoInclude();

      modelBuilder.Entity<Unterrichtsstunde>(entity =>
      {
        entity.ToTable("Unterrichtsstunde");

        entity.Property(e => e.Bezeichnung).IsRequired();
      });

      modelBuilder.Entity<Zensur>(entity =>
      {
        entity.ToTable("Zensur");

        entity.Property(e => e.NoteMitTendenz)
                  .IsRequired()
                  .HasMaxLength(50);
      });

      OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
  }
}
