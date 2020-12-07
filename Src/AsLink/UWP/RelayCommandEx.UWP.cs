using System;
using System.Diagnostics;
using System.Windows.Input;

namespace MVVM.Common
{
    public class RelayCommand : ICommand
    {
        readonly Action<object> _execute;
        readonly Predicate<object> _canExecute;

        public RelayCommand(Action<object> execute) : this(execute, null) { }
        public RelayCommand(Action<object> execute, Predicate<object> canExecute)
        {
            _execute = execute ?? throw new ArgumentNullException("Not good: Action is not set");
            _canExecute = canExecute;
        }

        [DebuggerStepThrough] public bool CanExecute(object parameter) { return _canExecute == null ? true : _canExecute(parameter); }
        public void Execute(object parameter) { _execute(parameter); }

        public event EventHandler CanExecuteChanged;
        public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }
}