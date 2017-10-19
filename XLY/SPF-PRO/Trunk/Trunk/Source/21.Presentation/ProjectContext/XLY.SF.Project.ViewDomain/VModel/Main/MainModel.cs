using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XLY.SF.Framework.Core.Base.ViewModel;

namespace XLY.SF.Project.ViewDomain.VModel.Main
{
    public class MainModel : NotifyPropertyBase
    {
        #region 当前登录用户

        private string _curUserName;
        /// <summary>
        /// 当前显示的用户名
        /// </summary>
        public string CurUserName
        {
            get
            {
                return this._curUserName;
            }

            set
            {
                this._curUserName = value;
                base.OnPropertyChanged();
            }
        }

        #endregion

        #region 当前时间显示

        private DateTime _curSysTime;
        /// <summary>
        /// 当前系统时间
        /// </summary>
        public DateTime CurSysTime
        {
            get
            {
                return this._curSysTime;
            }

            set
            {
                this._curSysTime = value;
                base.OnPropertyChanged();
            }
        }

        #endregion
    }
}
