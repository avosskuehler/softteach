﻿namespace Liduv.Model
{
  using System;
  using System.ComponentModel;

  using Liduv.ExceptionHandling;
  using Liduv.Model.EntityFramework;
  using Liduv.UndoRedo;

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
      this.Context = new LiduvModelContainer();
    }

    /// <summary>
    /// Holt den Liduv database model context, which is a DbContext.
    /// </summary>
    public LiduvModelContainer Context { get; set; }

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
        string.Format("{0} Datenbankkontextänderung(en) die in die Datenbank übertragen wurden.", this.Context.SaveChanges()),
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
    }
  }
}
