using System;
using System.Windows.Input;

namespace RevitTaskExample
{
    public class UiCommand : ICommand
    {
        private readonly Action _act;

        public UiCommand(Action act)
        {
            _act = act;
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            _act();
        }
    }
}
