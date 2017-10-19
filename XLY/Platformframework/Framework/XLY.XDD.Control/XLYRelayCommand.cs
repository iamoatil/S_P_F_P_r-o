/* 
 *  ======================================================================== 
 *      File name：        XLYRelayCommand.cs
 *		MachineName:	   DESKTOP-VUAS8PE
 *      Author：           Shi Xing
 *      Create Time：      2017/3/28 星期二 16:02:28
 *      Modify By:         
 *      Modify Date:       
 *  ======================================================================== 
*/
using System;
using System.Windows.Input;

namespace XLY.XDD.Control
{
    public class XLYRelayCommand : ICommand
    {
        private Action _action;
        private Action<object> _actionpar;
        public XLYRelayCommand(Action acton)
        {
            this._action = acton;
        }

        public XLYRelayCommand(Action<object> action)
        {
            _actionpar = action;
        }
        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            if (null != _action)
                _action();
            if (null != _actionpar)
                _actionpar(parameter);
        }
    }
}
