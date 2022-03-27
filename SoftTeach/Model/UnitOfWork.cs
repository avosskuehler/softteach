namespace SoftTeach.Model
{
  using System;
  using System.ComponentModel;
  using System.ComponentModel.DataAnnotations.Schema;
  using System.Data.Entity;
  using SoftTeach.ExceptionHandling;
  using SoftTeach.Model.TeachyModel;
  using SoftTeach.UndoRedo;

  public class TempTeachyEntities : TeachyModel.TeachyEntities
  {
    protected override void OnModelCreating(DbModelBuilder modelBuilder)
    {
      modelBuilder.Entity<ArbeitNeu>()
        .Property(a => a.Id)
        .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
      modelBuilder.Entity<AufgabeNeu>()
        .Property(a => a.Id)
        .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
      modelBuilder.Entity<BewertungsschemaNeu>()
        .Property(a => a.Id)
        .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
      modelBuilder.Entity<CurriculumNeu>()
        .Property(a => a.Id)
        .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
      modelBuilder.Entity<DateitypNeu>()
        .Property(a => a.Id)
        .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
      modelBuilder.Entity<ErgebnisNeu>()
        .Property(a => a.Id)
        .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
      modelBuilder.Entity<FachstundenanzahlNeu>()
        .Property(a => a.Id)
        .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
      modelBuilder.Entity<FerienNeu>()
        .Property(a => a.Id)
        .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
      modelBuilder.Entity<FachNeu>()
        .Property(a => a.Id)
        .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
      modelBuilder.Entity<HausaufgabeNeu>()
        .Property(a => a.Id)
        .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
      modelBuilder.Entity<ModulNeu>()
        .Property(a => a.Id)
        .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
      modelBuilder.Entity<NoteNeu>()
        .Property(a => a.Id)
        .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
      modelBuilder.Entity<NotentendenzNeu>()
        .Property(a => a.Id)
        .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
      modelBuilder.Entity<NotenWichtungNeu>()
        .Property(a => a.Id)
        .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
      modelBuilder.Entity<PersonNeu>()
        .Property(a => a.Id)
        .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
      modelBuilder.Entity<ProzentbereichNeu>()
        .Property(a => a.Id)
        .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
      modelBuilder.Entity<RaumplanNeu>()
        .Property(a => a.Id)
        .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
      modelBuilder.Entity<ReiheNeu>()
        .Property(a => a.Id)
        .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
      modelBuilder.Entity<SchuljahrNeu>()
        .Property(a => a.Id)
        .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
      modelBuilder.Entity<SchülereintragNeu>()
        .Property(a => a.Id)
        .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
      modelBuilder.Entity<SequenzNeu>()
        .Property(a => a.Id)
        .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
      modelBuilder.Entity<SitzplaneintragNeu>()
        .Property(a => a.Id)
        .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
      modelBuilder.Entity<SitzplanNeu>()
        .Property(a => a.Id)
        .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
      modelBuilder.Entity<SitzplatzNeu>()
        .Property(a => a.Id)
        .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
      modelBuilder.Entity<StundenplaneintragNeu>()
        .Property(a => a.Id)
        .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
      modelBuilder.Entity<StundenplanNeu>()
        .Property(a => a.Id)
        .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
      modelBuilder.Entity<TerminNeu>()
        .Property(a => a.Id)
        .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
      modelBuilder.Entity<UnterrichtsstundeNeu>()
        .Property(a => a.Id)
        .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
      modelBuilder.Entity<ZensurNeu>()
        .Property(a => a.Id)
        .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
      base.OnModelCreating(modelBuilder);
    }
  }

  /// <summary>
  /// Diese Klasse dient zur Herstellung der Verbindung von Datenbank und ViewModel.
  /// Sie beinhaltet ausserdem die Undo/Redo Stacks.
  /// </summary>
  public class UnitOfWork : IDisposable
  {
    /// <summary>
    /// Initialisiert eine neue Instanz der <see cref="UnitOfWork"/> Klasse.
    /// Sie stellt die Verbindung zur Datenbank her.
    /// </summary>
    public UnitOfWork()
    {
      this.OldContext = new EntityFramework.SoftTeachDataModel();
      this.Context = new TempTeachyEntities();
    }

    /// <summary>
    /// Holt den SoftTeach database model context, which is a DbContext.
    /// </summary>
    public EntityFramework.SoftTeachDataModel OldContext { get; set; }

    /// <summary>
    /// Holt den SoftTeach database model context, which is a DbContext.
    /// </summary>
    public TempTeachyEntities Context { get; set; }

    /// <summary>
    /// Diese Methode speichert alle Änderungen an der Datenbank
    /// </summary>
    public void SaveChanges()
    {
      var worker = new BackgroundWorker();
      worker.RunWorkerCompleted += this.workerCompleted;
      worker.DoWork += (s, args) => UndoService.Current[App.MainViewModel].UpdateContextFromViewModelsInUndoStack();
      worker.RunWorkerAsync();
    }

    private void workerCompleted(object sender, RunWorkerCompletedEventArgs e)
    {
      InformationDialog.Show(
        "Gespeichert.",
        string.Format("{0} Änderung(en), die in die Datenbank übertragen wurden.", this.Context.SaveChanges()),
        false);
      UndoService.Current[App.MainViewModel].Clear();
    }

    /// <summary>
    /// Gibt die Datenbankverbindung frei.
    /// Muss zur Vermeidung von Speicherleaks zum Ende aufgerufen werden.
    /// </summary>
    public void Dispose()
    {
      if (null != this.Context)
      {
        this.Context.Dispose();
      }
      if (null != this.OldContext)
      {
        this.OldContext.Dispose();
      }
    }

    internal void Remove(object model)
    {
      var entity = this.Context.Entry(model);
      switch (entity.State)
      {
        case System.Data.Entity.EntityState.Detached:
          break;
        case System.Data.Entity.EntityState.Unchanged:
          break;
        case System.Data.Entity.EntityState.Added:
          break;
        case System.Data.Entity.EntityState.Deleted:
          break;
        case System.Data.Entity.EntityState.Modified:
          break;
        default:
          break;
      }
    }
  }
}
