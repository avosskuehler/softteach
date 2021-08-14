namespace SoftTeach.UndoRedo
{
  using System;
  using System.Collections.Generic;
  using System.Linq;

  using SoftTeach.UndoRedo.ChangeTypes;
  using SoftTeach.ViewModel.Helper;

  /// <summary>
  /// Tracks the ChangeSets and behavior for a single root object (or document).
  /// </summary>
  public class UndoRoot
  {

    #region Member Variables

    // WeakReference because we don't want the undo stack to keep something locked in memory.
    private WeakReference _Root;

    // The list of undo / redo actions.
    private Stack<ChangeSet> _UndoStack;
    private Stack<ChangeSet> _RedoStack;

    // Tracks whether a batch (or batches) has been started.
    private int _IsInBatchCounter = 0;

    // Determines whether the undo framework will consolidate (or de-dupe) changes to the same property within the batch.
    private bool _ConsolidateChangesForSameInstance = false;

    // When in a batch, changes are grouped into this ChangeSet.
    private ChangeSet _CurrentBatchChangeSet;

    // Is the system currently undoing or redoing a changeset.
    private bool _IsUndoingOrRedoing = false;

    #endregion

    #region Events

    public event EventHandler UndoStackChanged;

    public event EventHandler RedoStackChanged;

    #endregion

    #region Constructors

    /// <summary>
    /// Create a new UndoRoot to track undo / redo actions for a given instance / document.
    /// </summary>
    /// <param name="root">The "root" instance of the object hierarchy. All changesets will 
    /// need to passs a reference to this instance when they track changes.</param>
    public UndoRoot(object root)
    {
      this._Root = new WeakReference(root);
      this._UndoStack = new Stack<ChangeSet>();
      this._RedoStack = new Stack<ChangeSet>();
    }

    #endregion

    #region Public Properties

    /// <summary>
    /// The instance that represents the root (or document) for this set of changes.
    /// </summary>
    /// <remarks>
    /// This is needed so that a single instance of the application can track undo histories 
    /// for multiple "root" or "document" instances at the same time. These histories should not 
    /// overlap or show in the same undo history.
    /// </remarks>
    public object Root
    {
      get
      {
        if (null != this._Root && this._Root.IsAlive)
          return this._Root.Target;
        else
          return null;
      }
    }

    /// <summary>
    /// A collection of undoable change sets for the current Root.
    /// </summary>
    public IEnumerable<ChangeSet> UndoStack
    {
      get { return this._UndoStack; }
    }

    /// <summary>
    /// A collection of redoable change sets for the current Root.
    /// </summary>
    public IEnumerable<ChangeSet> RedoStack
    {
      get { return this._RedoStack; }
    }

    /// <summary>
    /// Is this UndoRoot currently collecting changes as part of a batch.
    /// </summary>
    public bool IsInBatch
    {
      get
      {
        return this._IsInBatchCounter > 0;
      }
    }

    /// <summary>
    /// Is this UndoRoot currently undoing or redoing a change set.
    /// </summary>
    public bool IsUndoingOrRedoing
    {
      get
      {
        return this._IsUndoingOrRedoing;
      }
    }

    /// <summary>
    /// Should changes to the same property be consolidated (de-duped).
    /// </summary>
    public bool ConsolidateChangesForSameInstance
    {
      get
      {
        return this._ConsolidateChangesForSameInstance;
      }
    }

    public bool CanUndo
    {
      get
      {
        return this._UndoStack.Count > 0;
      }
    }

    public bool CanRedo
    {
      get
      {
        return this._RedoStack.Count > 0;
      }
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Tells the UndoRoot that all subsequent changes should be part of a single ChangeSet.
    /// </summary>
    public void BeginChangeSetBatch(string batchDescription, bool consolidateChangesForSameInstance)
    {
      // We don't want to add additional changes representing the operations that happen when undoing or redoing a change.
      if (this._IsUndoingOrRedoing)
        return;

      this._IsInBatchCounter++;

      if (this._IsInBatchCounter == 1)
      {
        this._ConsolidateChangesForSameInstance = consolidateChangesForSameInstance;
        this._CurrentBatchChangeSet = new ChangeSet(this, batchDescription, null);
        this._UndoStack.Push(this._CurrentBatchChangeSet);
        this.OnUndoStackChanged();
      }
    }

    /// <summary>
    /// Tells the UndoRoot that it can stop collecting Changes into a single ChangeSet.
    /// </summary>
    public void EndChangeSetBatch()
    {
      this._IsInBatchCounter--;

      if (this._IsInBatchCounter < 0)
        this._IsInBatchCounter = 0;

      //// Remove empty changesets from the stack.
      //if (this._CurrentBatchChangeSet.Changes.Count == 0)
      //{
      //  this._UndoStack.Pop();
      //  this.OnUndoStackChanged();
      //}

      if (this._IsInBatchCounter == 0)
      {
        this._ConsolidateChangesForSameInstance = false;
        this._CurrentBatchChangeSet = null;
      }
    }

    /// <summary>
    /// Undo the first available ChangeSet.
    /// </summary>
    public void Undo()
    {
      var last = this._UndoStack.FirstOrDefault();
      if (null != last)
        this.Undo(last);
    }

    /// <summary>
    /// Macht die Datenbankänderungen des UndoStacks
    /// Dauert lange: Set.Add() dauert ~300ms pro Aufruf. 
    /// </summary>
    public void UpdateContextFromViewModelsInUndoStack()
    {
      App.UnitOfWork.Context.Configuration.AutoDetectChangesEnabled = false;
      var addAndRemoveChanges = new List<CollectionAddRemoveChangeBase>();
      foreach (var changeSet in this.UndoStack.Reverse())
      {
        addAndRemoveChanges.Clear();
        foreach (var change in changeSet.Changes)
        {
          // Skip changes, that should not be written to the database context
          if (!change.IsDataContextRelevant)
          {
            continue;
          }

          if (change is CollectionAddChange)
          {
            addAndRemoveChanges.Add(change as CollectionAddChange);
          }
          else if (change is CollectionRemoveChange)
          {
            addAndRemoveChanges.Add(change as CollectionRemoveChange);
          }
        }

        //foreach (var collectionAddRemoveChangeBase in addAndRemoveChanges)
        //{
        //  if (collectionAddRemoveChangeBase.Element is ViewModelBase)
        //  {
        //    var viewModel = collectionAddRemoveChangeBase.Element as ViewModelBase;
        //    if (collectionAddRemoveChangeBase is CollectionAddChange)
        //    {
        //      var modelProperty = viewModel.GetType().GetProperty("Model");
        //      var model = modelProperty.GetValue(viewModel);
        //      var set = App.UnitOfWork.Context.Set(model.GetType());
        //      set.Add(model);
        //      Console.WriteLine("Added to context: " + model);

        //    }
        //    else if (collectionAddRemoveChangeBase is CollectionRemoveChange)
        //    {
        //      var modelProperty = viewModel.GetType().GetProperty("Model");
        //      var model = modelProperty.GetValue(viewModel);
        //      var set = App.UnitOfWork.Context.Set(model.GetType());
        //      set.Remove(model);
        //      Console.WriteLine("Removed from context: " + model);
        //    }
        //  }
        //}


        var sameElementChanges = addAndRemoveChanges.GroupBy(o => o.Element);

        foreach (var sameElementChange in sameElementChanges)
        {
          var adds = sameElementChange.Count(o => o is CollectionAddChange);
          var removes = sameElementChange.Count(o => o is CollectionRemoveChange);

          if (adds > removes)
          {
            if (sameElementChange.Key is ViewModelBase)
            {
              var viewModel = sameElementChange.Key as ViewModelBase;
              var modelProperty = viewModel.GetType().GetProperty("Model");
              var model = modelProperty.GetValue(viewModel);
              var set = App.UnitOfWork.Context.Set(model.GetType());
              set.Add(model);
              Console.WriteLine("Added to context: " + model);
            }
          }
          else if (removes > adds)
          {
            if (sameElementChange.Key is ViewModelBase)
            {
              var viewModel = sameElementChange.Key as ViewModelBase;
              var modelProperty = viewModel.GetType().GetProperty("Model");
              var model = modelProperty.GetValue(viewModel);
              var set = App.UnitOfWork.Context.Set(model.GetType());
              set.Remove(model);
              Console.WriteLine("Removed from context: " + model);
            }
          }
          else
          {
            // Kein Grund zur Aktualisierung der Datenbank,
            // Element wurde gleich häufig erstellt und gelöscht
          }
        }
      }

      App.UnitOfWork.Context.Configuration.AutoDetectChangesEnabled = true;
    }

    /// <summary>
    /// Undo all changesets up to and including the lastChangeToUndo.
    /// </summary>
    public void Undo(ChangeSet lastChangeToUndo)
    {
      if (this.IsInBatch)
        throw new InvalidOperationException("Cannot perform an Undo when the Undo Service is collecting a batch of changes. The batch must be completed first.");

      if (!this._UndoStack.Contains(lastChangeToUndo))
        throw new InvalidOperationException("The specified change does not exist in the list of undoable changes. Perhaps it has already been undone.");

      //System.Diagnostics.Debug.WriteLine("Starting UNDO: " + lastChangeToUndo.Description);

      bool done = false;
      this._IsUndoingOrRedoing = true;

      try
      {
        do
        {
          var changeSet = this._UndoStack.Pop();
          this.OnUndoStackChanged();

          if (changeSet == lastChangeToUndo || this._UndoStack.Count == 0)
            done = true;

          changeSet.Undo();

          this._RedoStack.Push(changeSet);
          this.OnRedoStackChanged();

        } while (!done);
      }
      finally
      {
        this._IsUndoingOrRedoing = false;
      }

    }

    /// <summary>
    /// Redo the first available ChangeSet.
    /// </summary>
    public void Redo()
    {
      var last = this._RedoStack.FirstOrDefault();
      if (null != last)
        this.Redo(last);
    }

    /// <summary>
    /// Redo ChangeSets up to and including the lastChangeToRedo.
    /// </summary>
    public void Redo(ChangeSet lastChangeToRedo)
    {
      if (this.IsInBatch)
        throw new InvalidOperationException("Cannot perform a Redo when the Undo Service is collecting a batch of changes. The batch must be completed first.");

      if (!this._RedoStack.Contains(lastChangeToRedo))
        throw new InvalidOperationException("The specified change does not exist in the list of redoable changes. Perhaps it has already been redone.");

      //System.Diagnostics.Debug.WriteLine("Starting REDO: " + lastChangeToRedo.Description);

      bool done = false;
      this._IsUndoingOrRedoing = true;
      try
      {
        do
        {
          var changeSet = this._RedoStack.Pop();
          this.OnRedoStackChanged();

          if (changeSet == lastChangeToRedo || this._RedoStack.Count == 0)
          {
            done = true;
          }

          changeSet.Redo();

          this._UndoStack.Push(changeSet);
          this.OnUndoStackChanged();
        }
        while (!done);
      }
      finally
      {
        this._IsUndoingOrRedoing = false;
      }
    }

    /// <summary>
    /// Add a change to the Undo history. The change will be added to the existing batch, if in a batch. 
    /// Otherwise, a new ChangeSet will be created.
    /// </summary>
    /// <param name="change">The change to add to the history.</param>
    /// <param name="description">The description of this change.</param>
    public void AddChange(Change change, string description)
    {
      // We don't want to add additional changes representing the operations that happen when undoing or redoing a change.
      if (this._IsUndoingOrRedoing)
        return;

      // If batched, add to the current ChangeSet, otherwise add a new ChangeSet.
      if (this.IsInBatch)
      {
        this._CurrentBatchChangeSet.AddChange(change);
      }
      else
      {
        this._UndoStack.Push(new ChangeSet(this, description, change));
        this.OnUndoStackChanged();
      }

      // Prune the RedoStack
      this._RedoStack.Clear();
      this.OnRedoStackChanged();
    }

    /// <summary>
    /// Adds a new changeset to the undo history. The change set will be added to the existing batch, if in a batch. 
    /// </summary>
    /// <param name="changeSet">The ChangeSet to add.</param>
    public void AddChange(ChangeSet changeSet)
    {
      // System.Diagnostics.Debug.WriteLine("Starting AddChange: " + description);

      // We don't want to add additional changes representing the operations that happen when undoing or redoing a change.
      if (this._IsUndoingOrRedoing)
        return;

      //  If batched, add to the current ChangeSet, otherwise add a new ChangeSet.
      if (this.IsInBatch)
      {
        foreach (var chg in changeSet.Changes)
        {
          this._CurrentBatchChangeSet.AddChange(chg);
          //System.Diagnostics.Debug.WriteLine("AddChange: BATCHED " + description);
        }
      }
      else
      {
        this._UndoStack.Push(changeSet);
        this.OnUndoStackChanged();
        //System.Diagnostics.Debug.WriteLine("AddChange: " + description);
      }

      // Prune the RedoStack
      this._RedoStack.Clear();
      this.OnRedoStackChanged();
    }

    public void Clear()
    {
      if (this.IsInBatch || this._IsUndoingOrRedoing)
        throw new InvalidOperationException("Unable to clear the undo history because the system is collecting a batch of changes, or is in the process of undoing / redoing a change.");

      this._UndoStack.Clear();
      this._RedoStack.Clear();
      this.OnUndoStackChanged();
      this.OnRedoStackChanged();
    }

    #endregion

    #region Internal

    private void OnUndoStackChanged()
    {
      if (null != this.UndoStackChanged)
        this.UndoStackChanged(this, EventArgs.Empty);
    }

    private void OnRedoStackChanged()
    {
      if (null != this.RedoStackChanged)
        this.RedoStackChanged(this, EventArgs.Empty);
    }

    #endregion

  }

}
