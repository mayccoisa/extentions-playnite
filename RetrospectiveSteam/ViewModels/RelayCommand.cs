using System;
using System.Windows.Input;

namespace RetrospectiveSteam.ViewModels
{
    public class RelayCommand : ICommand
    {
        private readonly Action _execute;
        private readonly Func<bool> _canExecute;

        public RelayCommand(Action execute, Func<bool> canExecute = null)
        {
            if (execute == null) throw new ArgumentNullException("execute");
            _execute = execute;
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute == null || _canExecute();
        }

        public void Execute(object parameter)
        {
            _execute();
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
    }

    public class RelayCommand<T> : ICommand
    {
        private readonly Action<T> _execute;
        private readonly Func<T, bool> _canExecute;

        public RelayCommand(Action<T> execute, Func<T, bool> canExecute = null)
        {
            if (execute == null) throw new ArgumentNullException("execute");
            _execute = execute;
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
<<<<<<< HEAD
            if (_canExecute == null) return true;
            if (parameter == null || parameter == System.Windows.DependencyProperty.UnsetValue)
                return _canExecute(default(T));
            
            if (parameter is T) return _canExecute((T)parameter);
            return _canExecute(default(T));
        }
    
        public void Execute(object parameter)
        {
            if (parameter == null || parameter == System.Windows.DependencyProperty.UnsetValue)
            {
                _execute(default(T));
                return;
            }
            
            if (parameter is T) _execute((T)parameter);
            else _execute(default(T));
=======
            return _canExecute == null || _canExecute((T)parameter);
        }

        public void Execute(object parameter)
        {
            _execute((T)parameter);
>>>>>>> origin/main
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
    }
}
