namespace SoftTeach.UndoRedo
{
  using System;
  using System.Collections;
  using System.Collections.Generic;
  using System.Collections.Specialized;

  using SoftTeach.UndoRedo.ChangeTypes;

  public class ChangeFactory
  {
    /// <summary>
    /// Initialisiert statische Member der <see cref="ChangeFactory"/> Klasse.
    /// </summary>
    static ChangeFactory()
    {
      Current = new ChangeFactory();
    }

    /// <summary>
    /// Initialisiert eine neue Instanz der <see cref="ChangeFactory"/> Klasse.
    /// </summary>
    public ChangeFactory()
    {
      this.ThrowExceptionOnCollectionResets = true;
    }

    /// <summary>
    /// Holt die statische ChangeFactory Instanz
    /// </summary>
    public static ChangeFactory Current { get; private set; }

    /// <summary>
    /// Holt oder setzt einen Wert, der angibt, ob das Undo/Redo Modul aktiviert sein soll.
    /// Wenn True, werden alle Änderungen am System aufgezeichnet.
    /// </summary>
    public bool IsTracking { get; set; }

    /// <summary>
    /// Holt oder setzt einen Wert, der angibt, ob eine Warnung ausgegeben werden soll, wenn 
    /// collections resetted werden.
    /// </summary>
    public bool ThrowExceptionOnCollectionResets { get; set; }

    /// <summary>
    /// Construct a Change instance with actions for undo / redo.
    /// </summary>
    /// <param name="instance">The instance that changed.</param>
    /// <param name="propertyName">The property name that changed. (Case sensitive, used by reflection.)</param>
    /// <param name="oldValue">The old value of the property.</param>
    /// <param name="newValue">The new value of the property.</param>
    /// <param name="isDataContextRelevant">The is Data Context Relevant. </param>
    /// <param name="descriptionOfChange">A description of this change. </param>
    /// <returns>A Change that can be added to the UndoRoot's undo stack.</returns>
    public virtual Change GetChange(object instance, string propertyName, object oldValue, object newValue, bool isDataContextRelevant, string descriptionOfChange)
    {
      var undoMetadata = instance as IUndoMetadata;
      if (null != undoMetadata)
      {
        if (!undoMetadata.CanUndoProperty(propertyName, oldValue, newValue))
          return null;
      }

      var change = new PropertyChange(instance, propertyName, oldValue, newValue, isDataContextRelevant, descriptionOfChange);

      return change;
    }

    /// <summary>
    /// Construct a Change instance with actions for undo / redo.
    /// </summary>
    /// <param name="instance"> The instance that changed. </param>
    /// <param name="propertyName"> The property name that changed. (Case sensitive, used by reflection.) </param>
    /// <param name="oldValue"> The old value of the property. </param>
    /// <param name="newValue"> The new value of the property. </param>
    /// <param name="isDataContextRelevant">The is Data Context Relevant. </param>
    public virtual void OnChanging(object instance, string propertyName, object oldValue, object newValue, bool isDataContextRelevant)
    {
      if (!this.IsTracking)
      {
        return;
      }

      var description = string.Format("{0} geändert von {1} nach {2}", propertyName, oldValue, newValue);
      this.OnChanging(instance, propertyName, oldValue, newValue, isDataContextRelevant, description);
    }

    /// <summary>
    /// Construct a Change instance with actions for undo / redo.
    /// </summary>
    /// <param name="instance"> The instance that changed. </param>
    /// <param name="propertyName"> The property name that changed. (Case sensitive, used by reflection.) </param>
    /// <param name="oldValue">The old value of the property. </param>
    /// <param name="newValue"> The new value of the property. </param>
    /// <param name="isDataContextRelevant">Gibt an, ob diese Änderung in der Datenbank mitgeschrieben werden soll.</param>
    /// <param name="descriptionOfChange">A description of this change. </param>
    public virtual void OnChanging(object instance, string propertyName, object oldValue, object newValue, bool isDataContextRelevant, string descriptionOfChange)
    {
      if (!this.IsTracking)
      {
        return;
      }

      var supportsUndo = instance as ISupportsUndo;
      if (null == supportsUndo)
        return;

      var root = supportsUndo.GetUndoRoot();
      if (null == root)
        return;

      var change = this.GetChange(instance, propertyName, oldValue, newValue, isDataContextRelevant, descriptionOfChange);

      UndoService.Current[root].AddChange(change, descriptionOfChange);
    }

    /// <summary>
    /// Construct a Change instance with actions for undo / redo.
    /// </summary>
    /// <param name="instance">The instance that changed.</param>
    /// <param name="propertyName">The property name that exposes the collection that changed. (Case sensitive, used by reflection.)</param>
    /// <param name="collection">The collection that had an item added / removed.</param>
    /// <param name="e">The NotifyCollectionChangedEventArgs event args parameter, with info about the collection change.</param>
    /// <param name="isDataContextRelevant">Gibt an, ob diese Änderung in der Datenbank mitgeschrieben werden soll.</param>
    /// <returns>A Change that can be added to the UndoRoot's undo stack.</returns>
    public virtual IList<Change> GetCollectionChange(object instance, string propertyName, object collection, NotifyCollectionChangedEventArgs e, bool isDataContextRelevant)
    {
      var undoMetadata = instance as IUndoMetadata;
      if (null != undoMetadata)
      {
        if (!undoMetadata.CanUndoCollectionChange(propertyName, collection, e))
          return null;
      }

      var ret = new List<Change>();

      switch (e.Action)
      {
        case NotifyCollectionChangedAction.Add:
          foreach (var item in e.NewItems)
          {
            var des = string.Format("Neues Element {0} zu {1} hinzugefügt.", item, propertyName);
            var change = new CollectionAddChange(
              instance, propertyName, (IList)collection, e.NewStartingIndex, item, isDataContextRelevant, des);

            ret.Add(change);
          }

          break;

        case NotifyCollectionChangedAction.Remove:
          foreach (var item in e.OldItems)
          {
            var des = string.Format("Element {0} aus {1} gelöscht.", item, propertyName);
            var change = new CollectionRemoveChange(
              instance, propertyName, (IList)collection, e.OldStartingIndex, item, isDataContextRelevant, des);

            ret.Add(change);
          }

          break;

        case NotifyCollectionChangedAction.Replace:
          // FIXME handle multi-item replace event
          var desR = string.Format("Element {0} in {1} durch {2} ersetzt.", e.OldItems[0], propertyName, e.NewItems[0]);
          var replaceChange = new CollectionReplaceChange(
            instance, propertyName, (IList)collection, e.NewStartingIndex, e.OldItems[0], e.NewItems[0], isDataContextRelevant, desR);
          ret.Add(replaceChange);
          break;
        case NotifyCollectionChangedAction.Move:
          var desM = string.Format("Element {0} in {1} von {2} auf {3} verschoben.", e.OldItems[0], propertyName, e.OldStartingIndex, e.NewStartingIndex);
          var moveChange = new CollectionMoveChange(
             instance, propertyName, (IList)collection, e.NewStartingIndex, e.OldStartingIndex, isDataContextRelevant, desM);
          ret.Add(moveChange);
          break;
        case NotifyCollectionChangedAction.Reset:
          break;
        //if (this.ThrowExceptionOnCollectionResets) var ok=true;
        //  // throw new NotSupportedException("Undoing collection resets is not supported via the CollectionChanged event. The collection is already null, so the Undo system has no way to capture the set
        //  // of elements that were previously in the collection.");
        //else
        //  break;

        //IList collectionClone = collection.GetType().GetConstructor(new Type[] { collection.GetType() }).Invoke(new object[] { collection }) as IList;

        //var resetChange = new DelegateChange(
        //                        instance,
        //                        () =>
        //                        {
        //                            for (int i = 0; i < collectionClone.Count; i++) //for instead foreach to preserve the order
        //                                ((IList)collection).Add(collectionClone[i]);
        //                        },
        //                        () => collection.GetType().GetMethod("Clear").Invoke(collection, null),
        //                        new ChangeKey<object, string, object>(instance, propertyName, collectionClone)
        //                    );
        //ret.Add(resetChange);
        //break;

        default:
          throw new NotSupportedException();
      }

      return ret;
    }

    /// <summary>
    /// Construct a Change instance with actions for undo / redo.
    /// </summary>
    /// <param name="instance">The instance that changed.</param>
    /// <param name="propertyName">The property name that exposes the collection that changed. (Case sensitive, used by reflection.)</param>
    /// <param name="collection">The collection that had an item added / removed.</param>
    /// <param name="e">The NotifyCollectionChangedEventArgs event args parameter, with info about the collection change.</param>
    /// <param name="isDataContextRelevant">Gibt an, ob diese Änderung in der Datenbank mitgeschrieben werden soll.</param>
    public virtual void OnCollectionChanged(object instance, string propertyName, object collection, NotifyCollectionChangedEventArgs e, bool isDataContextRelevant)
    {
      if (!this.IsTracking)
      {
        return;
      }

      this.OnCollectionChanged(instance, propertyName, collection, e, isDataContextRelevant, propertyName);
    }

    /// <summary>
    /// Construct a Change instance with actions for undo / redo.
    /// </summary>
    /// <param name="instance"> The instance that changed. </param>
    /// <param name="propertyName"> The property name that exposes the collection that changed. (Case sensitive, used by reflection.) </param>
    /// <param name="collection"> The collection that had an item added / removed. </param>
    /// <param name="e"> The NotifyCollectionChangedEventArgs event args parameter, with info about the collection change. </param>
    /// <param name="isDataContextRelevant"> Gibt an, ob diese Änderung in der Datenbank mitgeschrieben werden soll. </param>
    /// <param name="descriptionOfChange">A description of the change. </param>
    public virtual void OnCollectionChanged(object instance, string propertyName, object collection, NotifyCollectionChangedEventArgs e, bool isDataContextRelevant, string descriptionOfChange)
    {
      if (!this.IsTracking)
      {
        return;
      }

      var supportsUndo = instance as ISupportsUndo;
      if (null == supportsUndo)
        return;

      var root = supportsUndo.GetUndoRoot();
      if (null == root)
        return;

      // Create the Change instances.
      var changes = this.GetCollectionChange(instance, propertyName, collection, e, isDataContextRelevant);
      if (null == changes)
        return;

      // Add the changes to the UndoRoot
      var undoRoot = UndoService.Current[root];
      foreach (var change in changes)
      {
        undoRoot.AddChange(change, descriptionOfChange);
      }
    }
  }
}