namespace SoftTeach.ViewModel.Helper
{
  using System;
  using System.Windows.Input;

  public class DelegateCommand<T> : ICommand
  {
    private readonly Action<T> _execute;
    private readonly Predicate<T> _canExecute;

    public DelegateCommand(Action<T> execute)
      : this(execute, x => true)
    {
    }

    public DelegateCommand(Action<T> execute, Predicate<T> canExecute)
    {
      _execute = execute ?? throw new ArgumentNullException(nameof(execute));
      _canExecute = canExecute ?? throw new ArgumentNullException(nameof(canExecute));
    }

    public void Execute(object parameter)
    {
      _execute((T)parameter);
    }

    public bool CanExecute(object parameter)
    {
      return _canExecute((T)parameter);
    }

    public event EventHandler CanExecuteChanged;

    public void RaiseCanExecuteChanged()
    {
      CanExecuteChanged.Raise(this);
    }
  }

  public class DelegateCommand : DelegateCommand<object>
  {
    public DelegateCommand(Action execute)
      : base(execute != null ? x => execute() : (Action<object>)null)
    {
    }

    public DelegateCommand(Action execute, Func<bool> canExecute)
      : base(execute != null ? x => execute() : (Action<object>)null,
              canExecute != null ? x => canExecute() : (Predicate<object>)null)
    {
    }

    public DelegateCommand(Action execute, bool canExecute)
      : base(execute != null ? x => execute() : (Action<object>)null, x => canExecute)
    {
    }
  }

  ///// <summary>
  ///// ICommand implementation based on delegates
  ///// </summary>
  //public class DelegateCommand : ICommand
  //{
  //  /// <summary>
  //  /// Action to be performed when this command is executed
  //  /// </summary>
  //  private readonly Action<object> executionAction;

  //  /// <summary>
  //  /// Predicate to determine if the command is valid for execution
  //  /// </summary>
  //  private readonly Predicate<object> canExecutePredicate;

  //  /// <summary>
  //  /// Initialisiert eine neue Instanz der <see cref="DelegateCommand"/> Klasse. 
  //  /// The command will always be valid for execution.
  //  /// </summary>
  //  /// <param name="execute">
  //  /// The delegate to call on execution
  //  /// </param>
  //  public DelegateCommand(Action<object> execute)
  //    : this(execute, null)
  //  {
  //  }

  //  /// <summary>
  //  /// Initialisiert eine neue Instanz der <see cref="DelegateCommand"/> Klasse. 
  //  /// </summary>
  //  /// <param name="execute">
  //  /// The delegate to call on execution
  //  /// </param>
  //  /// <param name="canExecute">
  //  /// The predicate to determine if command is valid for execution
  //  /// </param>
  //  public DelegateCommand(Action<object> execute, Predicate<object> canExecute)
  //  {
  //    if (execute == null)
  //    {
  //      throw new ArgumentNullException("execute");
  //    }

  //    this.executionAction = execute;
  //    this.canExecutePredicate = canExecute;
  //  }

  //  ///// <summary>
  //  ///// Raised when CanExecute is changed
  //  ///// </summary> 
  //  //public event EventHandler CanExecuteChanged;

  //  /// <summary>
  //  /// Raised when CanExecute is changed
  //  /// </summary>
  //  public event EventHandler CanExecuteChanged
  //  {
  //    add { CommandManager.RequerySuggested += value; }
  //    remove { CommandManager.RequerySuggested -= value; }
  //  }

  //  /// <summary>
  //  /// Executes the delegate backing this DelegateCommand
  //  /// </summary>
  //  /// <param name="parameter">parameter to pass to predicate</param>
  //  /// <returns>True if command is valid for execution</returns>
  //  public bool CanExecute(object parameter)
  //  {
  //    return this.canExecutePredicate == null || this.canExecutePredicate(parameter);
  //  }

  //  /// <summary>
  //  /// Executes the delegate backing this DelegateCommand
  //  /// </summary>
  //  /// <param name="parameter">parameter to pass to delegate</param>
  //  /// <exception cref="InvalidOperationException">Thrown if CanExecute returns false</exception>
  //  public void Execute(object parameter)
  //  {
  //    if (!this.CanExecute(parameter))
  //    {
  //      throw new InvalidOperationException("The command is not valid for execution, check the CanExecute method before attempting to execute.");
  //    }

  //    this.executionAction(parameter);
  //  }
  //}
}