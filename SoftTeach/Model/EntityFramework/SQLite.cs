//using System;
//using System.Collections.Generic;
//using System.Data.Common;
//using System.Data.Entity;
//using System.Data.Entity.Core.EntityClient;
//using System.Data.Entity.Infrastructure;
//using System.Data.Entity.Infrastructure.DependencyResolution;
//using System.Data.SQLite;
//using System.Data.SQLite.EF6;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace SoftTeach.Model.EntityFramework
//{
//  public class SQLiteProviderInvariantName : IProviderInvariantName
//  {
//    public static readonly SQLiteProviderInvariantName Instance = new SQLiteProviderInvariantName();
//    private SQLiteProviderInvariantName() { }
//    public const string ProviderName = "System.Data.SQLite.EF6";
//    public string Name { get { return ProviderName; } }
//  }

//  class SQLiteDbDependencyResolver : IDbDependencyResolver
//  {
//    public object GetService(Type type, object key)
//    {
//      if (type == typeof(IProviderInvariantName)) return SQLiteProviderInvariantName.Instance;
//      if (type == typeof(DbProviderFactory)) return SQLiteProviderFactory.Instance;
//      if (type == typeof(IDbProviderFactoryResolver)) return SQLiteDbProviderFactoryResolver.Instance;

//      return SQLiteProviderFactory.Instance.GetService(type);
//    }

//    public IEnumerable<object> GetServices(Type type, object key)
//    {
//      var service = GetService(type, key);
//      if (service != null) yield return service;
//    }
//  }
//  class SQLiteDbProviderFactoryResolver : IDbProviderFactoryResolver
//  {
//    public static readonly SQLiteDbProviderFactoryResolver Instance = new SQLiteDbProviderFactoryResolver();
//    private SQLiteDbProviderFactoryResolver() { }
//    public DbProviderFactory ResolveProviderFactory(DbConnection connection)
//    {
//      if (connection is SQLiteConnection) return SQLiteProviderFactory.Instance;
//      if (connection is EntityConnection) return EntityProviderFactory.Instance;
//      return null;
//    }
//  }

//  class SQLiteDbConfiguration : DbConfiguration
//  {
//    public SQLiteDbConfiguration()
//    {
//      AddDependencyResolver(new SQLiteDbDependencyResolver());
//    }
//  }
//}
