// -----------------------------------------------------------------------
// <copyright file="IContext.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Liduv.Model
{
  using System;
  using System.Collections.Generic;
  using System.Data;
  using System.Data.Common;
  using System.Data.Entity;
  using System.Linq;
  using System.Text;

  /// <summary>
  /// TODO: Update summary.
  /// </summary>
  public interface IContext : IDisposable
  {
    //bool IsAuditEnabled { get; set; }

    ISnapshotManager SnapshotManager { get; }

    IDbSet<T> GetEntitySet<T>() where T : class;
    void ChangeState<T>(T entity, EntityState state) where T : class;
    DbTransaction BeginTransaction();
    int Commit();
  }
}
