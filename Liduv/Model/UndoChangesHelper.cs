///****************************** Module Header ******************************\
//Module Name:  UndoChangesHelper.cs
//Project:      CSEFUndoChanges
//Copyright (c) Microsoft Corporation.

//This sample demonstrates how to undo the changes in Entity Framework.
//This file contains the methods that undo the changes in different levels 
//using UnitOfWork or ObjectcContext.

//This source is subject to the Microsoft Public License.
//See http://www.microsoft.com/en-us/openness/licenses.aspx#MPL.
//All other rights reserved.

//THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
//EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
//WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
//\***************************************************************************/

//namespace Liduv.Model
//{
//  using System;
//  using System.Collections.Generic;
//  using System.Data;
//  using System.Data.Entity;
//  using System.Data.Entity.Infrastructure;
//  using System.Data.Objects;
//  using System.Data.Objects.DataClasses;
//  using System.Linq;
//  using System.Reflection;

//  public static class UndoChangesHelper
//  {
//    /// <summary>
//    /// This method undoes the changes in Context level using UnitOfWork.
//    /// </summary>
//    /// <param name="context">Undo the changes in this context</param>
//    public static void UndoDbContext(this DbContext context)
//    {
//      if (context == null)
//      {
//        throw new ArgumentException();
//      }

//      // Undo the changes of the all entries.
//      foreach (DbEntityEntry entry in context.ChangeTracker.Entries())
//      {
//        switch (entry.State)
//        {
//          // Under the covers, changing the state of an entity from 
//          // Modified to Unchanged first sets the values of all 
//          // properties to the original values that were read from 
//          // the database when it was queried, and then marks the 
//          // entity as Unchanged. This will also reject changes to 
//          // FK relationships since the original value of the FK 
//          // will be restored.
//          case EntityState.Modified:
//            entry.State = EntityState.Unchanged;
//            break;
//          case EntityState.Added:
//            entry.State = EntityState.Detached;
//            break;

//          // If the EntityState is the Deleted, reload the date from the database.  
//          case EntityState.Deleted:
//            entry.Reload();
//            break;
//          default: break;
//        }
//      }
//    }

//    /// <summary>
//    /// This method undoes the changes in DbEntities level using UnitOfWork.
//    /// </summary>
//    /// <typeparam name="T">Type of the DbEntities</typeparam>
//    /// <param name="context">Undo the changes in this context</param>
//    public static void UndoDbEntities<T>(this DbContext context) where T : class
//    {
//      if (context == null)
//      {
//        throw new ArgumentException();
//      }

//      // Undo the changes of the T type entries.
//      foreach (DbEntityEntry<T> entry in context.ChangeTracker.Entries<T>())
//      {
//        switch (entry.State)
//        {
//          case EntityState.Modified:
//            entry.State = EntityState.Unchanged;
//            break;
//          case EntityState.Deleted:
//            entry.Reload();
//            break;
//          case EntityState.Added:
//            entry.State = EntityState.Detached;
//            break;
//          default: break;

//        }
//      }
//    }

//    /// <summary>
//    /// This method undoes the changes in DbEntity level using UnitOfWork.
//    /// </summary>
//    /// <param name="context">Undo the changes in this context</param>
//    /// <param name="entity">Undo the changes of the Entity</param>
//    public static void UndoDbEntity(this DbContext context, object entity)
//    {
//      if (context == null || entity == null)
//      {
//        throw new ArgumentException();
//      }

//      // Get the entry of the entity, and then undo the changes.
//      DbEntityEntry entry = context.Entry(entity);
//      if (entry != null)
//      {
//        switch (entry.State)
//        {
//          case EntityState.Modified:
//            entry.State = EntityState.Unchanged;
//            break;
//          case EntityState.Deleted:
//            entry.Reload();
//            break;
//          case EntityState.Added:
//            entry.State = EntityState.Detached;
//            break;
//          default: break;
//        }
//      }
//    }

//    /// <summary>
//    /// This method undoes the changes in DbEntity Property level using UnitOfWork.
//    /// </summary>
//    /// <param name="context">Undo the changes in this context</param>
//    /// <param name="entity">Undo the changes in the Entity</param>
//    /// <param name="propertyName">Undo the changes of the Property</param>
//    public static void UndoDbEntityProperty(this DbContext context, object entity, string propertyName)
//    {
//      if (context == null || entity == null || propertyName == null)
//      {
//        throw new ArgumentException();
//      }

//      try
//      {
//        DbEntityEntry entry = context.Entry(entity);
//        if (entry.State == EntityState.Added || entry.State == EntityState.Detached)
//        {
//          return;
//        }

//        // Get and Set the Property value by the Property Name.
//        var propertyValue = entry.OriginalValues.GetValue<object>(propertyName);
//        entry.Property(propertyName).CurrentValue = entry.Property(propertyName).OriginalValue;
//      }
//      catch
//      {
//        throw;
//      }
//    }
//  }
//}
