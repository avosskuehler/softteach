﻿namespace Liduv.UndoRedo
{
  using System;


  /// <summary>
    /// Provides a simplified way to start and end a batch via a "using" block.
    /// When the UndoBatch is disposed (at the end of the using block) it will end the batch.
    /// NOTE: Nested blocks _are_ supported.
    /// </summary>
    public class UndoBatch : IDisposable
    {

        #region Member Variables

        private UndoRoot _UndoRoot;

        #endregion

        #region Constructors

        /// <summary>
        /// Starts an undo batch, which is ended when this instance is disposed. Designed for use in a using statement.
        /// </summary>
        /// <param name="root">An object that implements ISupportsUndo. The batch will call GetUndoRoot() to get the root.</param>
        /// <param name="description">The description of this batch of changes.</param>
        /// <param name="consolidateChangesForSameInstance">Should the batch consolidate changes.</param>
        public UndoBatch(Liduv.UndoRedo.ISupportsUndo instance, string description, bool consolidateChangesForSameInstance)
            : this(UndoService.Current[instance.GetUndoRoot()], description, consolidateChangesForSameInstance)
        { }

        /// <summary>
        /// Starts an undo batch, which is ended when this instance is disposed. Designed for use in a using statement.
        /// </summary>
        /// <param name="root">The UndoRoot related to this instance.</param>
        /// <param name="description">The description of this batch of changes.</param>
        /// <param name="consolidateChangesForSameInstance">Should the batch consolidate changes.</param>
        public UndoBatch(UndoRoot root, string description, bool consolidateChangesForSameInstance)
        {
            if (null == root)
                return;

            this._UndoRoot = root;
            root.BeginChangeSetBatch(description, consolidateChangesForSameInstance);
        }

        #endregion

        #region IDisposable Members

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (null != this._UndoRoot)
                    this._UndoRoot.EndChangeSetBatch();
            }
        }

        /// <summary>
        /// Disposing this instance will end the associated Undo batch.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion

    }

}
