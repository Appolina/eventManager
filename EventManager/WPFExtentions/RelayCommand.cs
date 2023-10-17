using System;
using System.Windows.Input;

namespace EventManager
{
    public class RelayCommand : ICommand
	{
		private readonly Action<object> execute;
		private Predicate<object> canExecute;

        public event EventHandler CanExecuteChanged;

        public RelayCommand(Action<object> execute, Predicate<object> canExecute)
        {
            if (execute == null)
                throw new ArgumentNullException("execute");

            this.execute = execute;
            this.canExecute = canExecute;           
        }

       

        #region ICommand Members
        public bool CanExecute(object parameters)
        {
            return canExecute == null ? true : canExecute(parameters);
        }

        //public event EventHandler CanExecuteChanged
        //{
        //    add { CommandManager.RequerySuggested += value; }
        //    remove { CommandManager.RequerySuggested -= value; }
        //}

        public void Execute(object parameters)
        {
            execute(parameters);
        }

		#endregion
	}
}
