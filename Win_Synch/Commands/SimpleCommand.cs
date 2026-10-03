using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;



namespace Win_Synch.Commands
{
    public class SimpleCommand:System.Windows.Input.ICommand
    {
        private readonly Action<object?> Exec;
        private readonly Func<object?, bool>? CanExec;

        public SimpleCommand(Action<object?>? exec, Func<object?, bool>? canExec)
        {
            Exec = exec?? throw new ArgumentNullException("Команда должна что-то делать! Задайте нормальный дегат исполнения!");
            CanExec = canExec;
        }

        public bool CanExecute(object? parameter) {
            return CanExec?.Invoke(parameter) ?? true;
        }

        public void Execute(object? parameter) {
             Exec.Invoke(parameter);
        }

        public event EventHandler? CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        public void RaiseCanExecuteChanged() =>
            CommandManager.InvalidateRequerySuggested();

    }

}
