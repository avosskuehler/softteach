namespace Liduv.ViewModel.Helper
{
  using System;
  using System.Collections.Generic;
  using System.Collections.Specialized;
  using System.ComponentModel;
  using System.Globalization;
  using System.Linq;
  using System.Linq.Expressions;
  using System.Reflection;
  using System.Windows;
  using System.Windows.Controls;
  using System.Windows.Markup;

  using Liduv.UndoRedo;

  /// <summary>
  /// Die Basisklasse für alle ViewModels.
  /// Implementiert INotifyPropertyChanged
  /// </summary>
  public class ViewModelBase : DependencyObject, INotifyPropertyChanged, ISupportsUndo
  {
    ///// <summary>
    ///// Prefix für Execute methoden von DelegateCommands.
    ///// </summary>
    //private const string ExecutePrefix = "Execute";

    ///// <summary>
    ///// Prefix für CanExecute methoden von DelegateCommands.
    ///// </summary>
    //private const string CanExecutePrefix = "CanExecute";

    /// <summary>
    /// Keine Ahnung was das ist.
    /// </summary>
    private static bool cultureBinding = false;

    /// <summary>
    /// Gibt an, ob das ViewModel im DesingMode benutzt wird (für Blend und Designer)
    /// </summary>
    private static bool isInDesignMode = false;

    /// <summary>
    /// Speichert die Werte des ViewModels.
    /// </summary>
    private readonly Dictionary<string, object> values;

    /// <summary>
    /// Speichert die Properties des ViewModels.
    /// </summary>
    private readonly IDictionary<string, List<string>> propertyMap;

    ///// <summary>
    ///// Speichert die Methoden des ViewModels, die nicht CanExecute sind.
    ///// </summary>
    //private readonly IDictionary<string, List<string>> methodMap;

    ///// <summary>
    ///// Speichert die Methoden des ViewModels, die CanExecute sind.
    ///// </summary>
    //private readonly IDictionary<string, List<string>> commandMap;

    /// <summary>
    /// Initialisiert eine neue Instanz der <see cref="ViewModelBase"/> Klasse.
    /// </summary>
    public ViewModelBase()
    {
      this.values = new Dictionary<string, object>();
      this.propertyMap = MapDependencies<DependsUponAttribute>(() => GetType().GetProperties());
      //this.methodMap = MapDependencies<DependsUponAttribute>(() => GetType().GetMethods().Cast<MemberInfo>().Where(method => !method.Name.StartsWith(CanExecutePrefix)));
      //this.commandMap = MapDependencies<DependsUponAttribute>(() => GetType().GetMethods().Cast<MemberInfo>().Where(method => method.Name.StartsWith(CanExecutePrefix)));

      //this.CreateCommands();
      this.VerifyDependencies();
    }

    /// <summary>
    /// Das Event was ausgelöst wird, wenn sich eine Property ändert.
    /// </summary>
    public event PropertyChangedEventHandler PropertyChanged;

    /// <summary>
    /// Das Event was ausgelöst wird, wenn sich eine Property ändert und
    /// dabei noch den alten Wert transportiert.
    /// </summary>
    public event PropertyChangingEventHandler PropertyChanging;

    /// <summary>
    /// Holt einen Wert, der angibt, ob the control is in design mode, running inside Visual Studio or Blend.
    /// </summary>
    protected bool IsInDesignMode
    {
      get
      {
        return isInDesignMode;
      }
    }

    /// <summary>
    /// Holt eine Beschreibung des ViewModels für die Curriculasortierung
    /// </summary>
    public string ViewModelType
    {
      get
      {
        var type = this.GetType().ToString();
        if (type.Contains("TagesplanViewModel"))
        {
          return "Tagespläne des Jahresplans";
        }

        if (type.Contains("SequenzViewModel"))
        {
          return "C-Sequenzen des Curriculums";
        }

        if (type.Contains("ReiheViewModel"))
        {
          return "B-Reihe";
        }

        if (type.Contains("SchulwocheViewModel"))
        {
          return "A-Schulwoche";
        }

        if (type.Contains("StundeViewModel"))
        {
          return "Stunde";
        }

        return type;
      }
    }

    /// <summary>
    /// Implementation von ISupportUndo
    /// </summary>
    /// <returns> The <see cref="object"/>. </returns>
    public object GetUndoRoot()
    {
      return App.MainViewModel;
    }

    /// <summary>
    /// Initialize the instance.
    /// </summary>
    protected virtual void OnInitialize()
    {
      isInDesignMode = DesignerProperties.GetIsInDesignMode(new Button())
          || Application.Current == null || Application.Current.GetType() == typeof(Application);

      if (!isInDesignMode)
      {
        var designMode = DesignerProperties.IsInDesignModeProperty;
        ViewModelBase.isInDesignMode = (bool)DependencyPropertyDescriptor.FromProperty(designMode, typeof(FrameworkElement)).Metadata.DefaultValue;
      }

      if (isInDesignMode)
      {
        this.DesignData();
      }

      SetOsCultureBinding();
    }

    /// <summary>
    /// With this method, we can inject design time data into the view so that we can
    /// create a more Blendable application.
    /// </summary>
    protected virtual void DesignData()
    {
    }

    /// <summary>
    /// Call this when a property changes that should be tracked for undo.
    /// </summary>
    /// <param name="viewModel">Das ViewModel dessen Property geändert wurde</param>
    /// <param name="propertyName">The name of the property.</param>
    /// <param name="oldValue">The previous value.</param>
    /// <param name="newValue">The new value.</param>
    /// <param name="isDataContextRelevant">Gibt an, ob diese Änderung in der Datenbank mitgeschrieben werden soll.</param>
    protected void UndoablePropertyChanging(object viewModel, string propertyName, object oldValue, object newValue, bool isDataContextRelevant)
    {
      ChangeFactory.Current.OnChanging(viewModel, propertyName, oldValue, newValue, isDataContextRelevant);
      this.RaisePropertyChanging(propertyName, oldValue, newValue);
    }

    /// <summary>
    /// Call this when a property changes that should be tracked for undo
    /// and you wish to have then change always be transmitted to database context.
    /// </summary>
    /// <param name="viewModel">Das ViewModel dessen Property geändert wurde</param>
    /// <param name="propertyName">The name of the property.</param>
    /// <param name="oldValue">The previous value.</param>
    /// <param name="newValue">The new value.</param>
    protected void UndoablePropertyChanging(object viewModel, string propertyName, object oldValue, object newValue)
    {
      ChangeFactory.Current.OnChanging(viewModel, propertyName, oldValue, newValue, true);
      this.RaisePropertyChanging(propertyName, oldValue, newValue);
    }

    /// <summary>
    /// Call this when the ViewModel has an ObservableCollection that changed.
    /// The method will attempt to create an undo change item for it.
    /// </summary>
    /// <param name="viewModel"> Das ViewModel dessen collection geändert wurde </param>
    /// <param name="propertyName"> The name of the property that holds the collection. </param>
    /// <param name="collection"> The collection instance. </param>
    /// <param name="e"> The event args raised by the INotifyCollectionChanged.CollectionChanged event. </param>
    /// <param name="isDataContextRelevant">Gibt an, ob diese Änderung in der Datenbank mitgeschrieben werden soll.</param>
    /// <param name="descriptionOfChange"> The description Of Change. </param>
    protected void UndoableCollectionChanged(object viewModel, string propertyName, object collection, NotifyCollectionChangedEventArgs e, bool isDataContextRelevant, string descriptionOfChange)
    {
      ChangeFactory.Current.OnCollectionChanged(viewModel, propertyName, collection, e, isDataContextRelevant, descriptionOfChange);
    }

    /// <summary>
    /// Call this when the ViewModel has an ObservableCollection that changed.
    /// The method will attempt to create an undo change item for it.
    /// Additionally this method ensures, that the collection change is transmitted to the underlying 
    /// database context.
    /// </summary>
    /// <param name="viewModel"> Das ViewModel dessen collection geändert wurde </param>
    /// <param name="propertyName"> The name of the property that holds the collection. </param>
    /// <param name="collection"> The collection instance. </param>
    /// <param name="e"> The event args raised by the INotifyCollectionChanged.CollectionChanged event. </param>
    /// <param name="descriptionOfChange"> The description Of Change. </param>
    protected void UndoableCollectionChanged(object viewModel, string propertyName, object collection, NotifyCollectionChangedEventArgs e, string descriptionOfChange)
    {
      ChangeFactory.Current.OnCollectionChanged(viewModel, propertyName, collection, e, true, descriptionOfChange);
    }

    /// <summary>
    /// Dieses Attribut sorgt für korrektes Updaten von 
    /// abhängigen Properties und Methoden.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    protected class DependsUponAttribute : Attribute
    {
      /// <summary>
      /// Initialisiert eine neue Instanz der <see cref="DependsUponAttribute"/> Klasse.
      /// </summary>
      /// <param name="propertyName"> The property name. </param>
      public DependsUponAttribute(string propertyName)
      {
        this.DependencyName = propertyName;
      }

      /// <summary>
      /// Holt den Namen für die Abhängigkeit.
      /// </summary>
      public string DependencyName { get; private set; }

      /// <summary>
      /// Holt oder setzt einen Wert, der angibt, ob die Property existieren muss,
      /// </summary>
      public bool VerifyStaticExistence { get; set; }
    }

    protected T Get<T>(string name)
    {
      return Get(name, default(T));
    }

    protected T Get<T>(string name, T defaultValue)
    {
      if (this.values.ContainsKey(name))
      {
        return (T)this.values[name];
      }

      return defaultValue;
    }

    protected T Get<T>(string name, Func<T> initialValue)
    {
      if (this.values.ContainsKey(name))
      {
        return (T)this.values[name];
      }

      Set(name, initialValue);
      return Get<T>(name);
    }

    protected T Get<T>(Expression<Func<T>> expression)
    {
      return Get<T>(PropertyName(expression));
    }

    protected T Get<T>(Expression<Func<T>> expression, T defaultValue)
    {
      return Get(PropertyName(expression), defaultValue);
    }

    protected T Get<T>(Expression<Func<T>> expression, Func<T> initialValue)
    {
      return Get(PropertyName(expression), initialValue);
    }

    public void Set<T>(string name, T value)
    {
      if (this.values.ContainsKey(name))
      {
        if (this.values[name] == null && value == null)
          return;

        if (this.values[name] != null && this.values[name].Equals(value))
          return;

        this.values[name] = value;
      }
      else
      {
        this.values.Add(name, value);
      }

      this.RaisePropertyChanged(name);
    }

    /// <summary>
    /// This method raises all property changed events
    /// along with all dependant properties and methods.
    /// </summary>
    /// <param name="name">Der Name der Property.</param>
    protected void RaisePropertyChanged(string name)
    {
      this.PropertyChanged.Raise(this, name);

      if (this.propertyMap.ContainsKey(name))
      {
        this.propertyMap[name].Each(this.RaisePropertyChanged);
      }

      //this.ExecuteDependentMethods(name);
      //this.FireChangesOnDependentCommands(name);
    }

    protected void RaisePropertyChanging(string name, object oldValue, object newValue)
    {
      if (this.PropertyChanging != null)
      {
        this.PropertyChanging(this, new PropertyChangingEventArgs(name, oldValue, newValue));
      }
      //PropertyChanged.Raise(this, name);

      //if (propertyMap.ContainsKey(name))
      //  propertyMap[name].Each(this.RaisePropertyChanged);

      //ExecuteDependentMethods(name);
      //FireChangesOnDependentCommands(name);
    }

    /// <summary>
    /// Set the current culture binding based on the OS culture.
    /// </summary>
    private static void SetOsCultureBinding()
    {
      if (cultureBinding || isInDesignMode)
      {
        return;
      }

      FrameworkElement.LanguageProperty.OverrideMetadata(
        typeof(FrameworkElement),
        new FrameworkPropertyMetadata(XmlLanguage.GetLanguage(CultureInfo.CurrentCulture.IetfLanguageTag)));
      cultureBinding = true;
    }

    //private void ExecuteDependentMethods(string name)
    //{
    //  if (this.methodMap.ContainsKey(name))
    //  {
    //    this.methodMap[name].Each(ExecuteMethod);
    //  }
    //}

    //private void FireChangesOnDependentCommands(string name)
    //{
    //  if (this.commandMap.ContainsKey(name))
    //    this.commandMap[name].Each(RaiseCanExecuteChangedEvent);
    //}

    //protected void Set<T>(Expression<Func<T>> expression, T value)
    //{
    //  Set(PropertyName(expression), value);
    //}

    private static string PropertyName<T>(Expression<Func<T>> expression)
    {
      var memberExpression = expression.Body as MemberExpression;

      if (memberExpression == null)
      {
        throw new ArgumentException("expression must be a property expression");
      }

      return memberExpression.Member.Name;
    }

    //public override bool TryGetMember(GetMemberBinder binder, out object result)
    //{
    //  result = Get<object>(binder.Name);

    //  if (result != null)
    //  {
    //    return true;
    //  }

    //  return base.TryGetMember(binder, out result);
    //}

    //public override bool TrySetMember(SetMemberBinder binder, object value)
    //{
    //  var result = base.TrySetMember(binder, value);
    //  if (result)
    //  {
    //    return true;
    //  }

    //  this.Set(binder.Name, value);
    //  return true;
    //}

    //private void CreateCommands()
    //{
    //  CommandNames.Each(name => Set(name, new DelegateCommand<object>(x => ExecuteCommand(name, x), x => CanExecuteCommand(name, x))));
    //}


    //private IEnumerable<string> CommandNames
    //{
    //  get
    //  {
    //    return from method in GetType().GetMethods()
    //           where method.Name.StartsWith(ExecutePrefix)
    //           select method.Name.StripLeft(ExecutePrefix.Length);
    //  }
    //}

    //private void ExecuteCommand(string name, object parameter)
    //{
    //  var methodInfo = GetType().GetMethod(ExecutePrefix + name);
    //  if (methodInfo == null) return;

    //  methodInfo.Invoke(this, methodInfo.GetParameters().Length == 1 ? new[] { parameter } : null);
    //}

    //private bool CanExecuteCommand(string name, object parameter)
    //{
    //  var methodInfo = GetType().GetMethod(CanExecutePrefix + name);
    //  if (methodInfo == null) return true;

    //  return (bool)methodInfo.Invoke(this, methodInfo.GetParameters().Length == 1 ? new[] { parameter } : null);
    //}

    //protected void RaiseCanExecuteChangedEvent(string canExecute_name)
    //{
    //  var commandName = canExecute_name.StripLeft(CanExecutePrefix.Length);
    //  var command = Get<DelegateCommand<object>>(commandName);
    //  if (command == null)
    //    return;

    //  command.RaiseCanExecuteChanged();
    //}

    private static IDictionary<string, List<string>> MapDependencies<T>(Func<IEnumerable<MemberInfo>> getInfo) where T : DependsUponAttribute
    {
      var dependencyMap = getInfo().Where(p => p.GetCustomAttributes(typeof(T), true).Any()).ToDictionary(
                  p => p.Name,
                  p => p.GetCustomAttributes(typeof(T), true)
                        .Cast<T>()
                        .Select(a => a.DependencyName)
                        .ToList());

      return Invert(dependencyMap);
    }

    private static IDictionary<string, List<string>> Invert(IDictionary<string, List<string>> map)
    {
      var flattened = from key in map.Keys
                      from value in map[key]
                      select new { Key = key, Value = value };

      var uniqueValues = flattened.Select(x => x.Value).Distinct();

      return uniqueValues.ToDictionary(
                  x => x,
                  x => (from item in flattened
                        where item.Value == x
                        select item.Key).ToList());
    }

    //private void ExecuteMethod(string name)
    //{
    //  var memberInfo = GetType().GetMethod(name);
    //  if (memberInfo == null)
    //    return;

    //  memberInfo.Invoke(this, null);
    //}

    private void VerifyDependencies()
    {
      //var methods = GetType().GetMethods().Cast<MemberInfo>();
      var properties = GetType().GetProperties();

      //var propertyNames = methods.Union(properties)
      var propertyNames = properties
          .SelectMany(method => method.GetCustomAttributes(typeof(DependsUponAttribute), true).Cast<DependsUponAttribute>())
          .Where(attribute => attribute.VerifyStaticExistence)
          .Select(attribute => attribute.DependencyName);

      propertyNames.Each(this.VerifyDependancy);
    }

    private void VerifyDependancy(string propertyName)
    {
      var property = GetType().GetProperty(propertyName);
      if (property == null)
      {
        throw new ArgumentException("DependsUpon Property Does Not Exist: " + propertyName);
      }
    }
  }
}
