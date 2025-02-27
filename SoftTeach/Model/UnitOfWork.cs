﻿namespace SoftTeach.Model
{
  using System;
  using System.ComponentModel;
  using System.ComponentModel.DataAnnotations.Schema;
  using Microsoft.EntityFrameworkCore;
  using SoftTeach.ExceptionHandling;
  using SoftTeach.Model.TeachyModel;
  using SoftTeach.UndoRedo;

  /// <summary>
  /// Diese Klasse dient zur Herstellung der Verbindung von Datenbank und ViewModel.
  /// Sie beinhaltet ausserdem die Undo/Redo Stacks.
  /// </summary>
  public class UnitOfWork
  {
    /// <summary>
    /// Initialisiert eine neue Instanz der <see cref="UnitOfWork"/> Klasse.
    /// Sie stellt die Verbindung zur Datenbank her.
    /// </summary>
    public UnitOfWork()
    {
//      this.Context = new TeachyContext(@"data source=VK2\SQL2017;initial catalog=Teachy;integrated security=True;MultipleActiveResultSets=True;App=EntityFramework");
      this.Context = new TeachyContext();
    }

    /// <summary>
    /// Holt den SoftTeach database model context, which is a DbContext.
    /// </summary>
    public TeachyContext Context { get; set; }

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

    ///// <summary>
    ///// Gibt die Datenbankverbindung frei.
    ///// Muss zur Vermeidung von Speicherleaks zum Ende aufgerufen werden.
    ///// </summary>
    //public void Dispose()
    //{
    //  if (null != this.Context)
    //  {
    //    this.Context.Dispose();
    //  }
    //  if (null != this.OldContext)
    //  {
    //    this.OldContext.Dispose();
    //  }
    //}

    //internal void Remove(object model)
    //{
    //  var entity = this.Context.Entry(model);
    //  switch (entity.State)
    //  {
    //    case System.Data.Entity.EntityState.Detached:
    //      break;
    //    case System.Data.Entity.EntityState.Unchanged:
    //      break;
    //    case System.Data.Entity.EntityState.Added:
    //      break;
    //    case System.Data.Entity.EntityState.Deleted:
    //      break;
    //    case System.Data.Entity.EntityState.Modified:
    //      break;
    //    default:
    //      break;
    //  }
    //}
  }
}
