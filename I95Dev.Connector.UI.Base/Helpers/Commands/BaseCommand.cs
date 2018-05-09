using System;
using System.Windows.Input;

namespace I95Dev.Connector.UI.Base.Helpers.Commands
{
    public class BaseCommand : ICommand
    {
        private readonly Func<bool> canExecute;
        private readonly Func<object, bool> canExecuteWithParameter;
        private readonly Action execute;
        private readonly Action<object> executeWithParameter;
        private readonly bool canExecuteVariable = true;

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseCommand"/> class.
        /// </summary>
        /// <param name="canExecute">The can execute.</param>
        /// <param name="execute">The execute.</param>
        public BaseCommand(Func<object, bool> canExecute, Action<object> execute)
        {
            canExecuteWithParameter = canExecute;
            executeWithParameter = execute;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseCommand"/> class.
        /// </summary>
        /// <param name="canExecute">The can execute.</param>
        /// <param name="execute">The execute.</param>
        public BaseCommand(Func<bool> canExecute, Action<object> execute)
        {
            this.canExecute = canExecute;
            executeWithParameter = execute;
        }

        public BaseCommand(Func<bool> canExecute, Action execute)
        {
            this.canExecute = canExecute;
            this.execute = execute;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseCommand"/> class.
        /// </summary>
        /// <param name="canExecute">if set to <c>true</c> [can execute].</param>
        /// <param name="execute">The execute.</param>
        public BaseCommand(bool canExecute, Action<object> execute)
        {
            canExecuteVariable = canExecute;
            executeWithParameter = execute;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseCommand"/> class.
        /// </summary>
        /// <param name="canExecute">if set to <c>true</c> [can execute].</param>
        /// <param name="execute">The execute.</param>
        public BaseCommand(bool canExecute, Action execute)
        {
            canExecuteVariable = canExecute;
            this.execute = execute;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseCommand"/> class.
        /// </summary>
        /// <param name="execute">The execute.</param>
        public BaseCommand(Action<object> execute)
        {
            executeWithParameter = execute;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseCommand"/> class.
        /// </summary>
        /// <param name="execute">The execute.</param>
        public BaseCommand(Action execute)
        {
            this.execute = execute;
        }

        /// <summary>
        /// Occurs when changes occur that affect whether or not the command should execute.
        /// </summary>
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        /// <summary>
        /// Defines the method that determines whether the command can execute in its current state.
        /// </summary>
        /// <param name="parameter">Data used by the command.  If the command does not require data to be passed, this object can be set to null.</param>
        /// <returns>
        /// true if this command can be executed; otherwise, false.
        /// </returns>
        public bool CanExecute(object parameter)
        {
            if (canExecute != null)
                return canExecute();
            return canExecuteWithParameter?.Invoke(parameter) ?? canExecuteVariable;
        }

        /// <summary>
        /// Defines the method to be called when the command is invoked.
        /// </summary>
        /// <param name="parameter">Data used by the command.  If the command does not require data to be passed, this object can be set to null.</param>
        public void Execute(object parameter)
        {
            execute?.Invoke();
            executeWithParameter?.Invoke(parameter);
        }
    }
}