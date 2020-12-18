using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Client.Models.Common
{

	public class RelayCommand : ICommand
	{
		public event EventHandler CanExecuteChanged;
		private Action<object> executeMethod;
		private Predicate<object> canExecuteMethod;
		private Action saveLoadItem;

		public RelayCommand(Action<object> Execute, Predicate<object> CanExecute)
		{
			executeMethod = Execute;
			canExecuteMethod = CanExecute;
		}

		public RelayCommand(Action<object> Execute) : this(Execute, null)
		{ }

		public RelayCommand(Action saveLoadItem)
		{
			this.saveLoadItem = saveLoadItem;
		}

		public bool CanExecute(object parameter)
		{ return (canExecuteMethod == null) ? true : canExecuteMethod.Invoke(parameter); }

		public void Execute(object parameter)
		{ executeMethod.Invoke(parameter); }

		public void RaiseCanExecuteChanged()
		{ CanExecuteChanged?.Invoke(this, EventArgs.Empty); }
	}
}
