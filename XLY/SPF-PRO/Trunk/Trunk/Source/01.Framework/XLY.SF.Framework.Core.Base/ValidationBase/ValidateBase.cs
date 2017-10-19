using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using XLY.SF.Framework.Core.Base.ViewModel;


/*************************************************
 * 创建人：Bob
 * 创建时间：2017/3/16 13:29:03
 * 类功能说明：
 *
 *************************************************/

namespace XLY.SF.Framework.Core.Base.ValidationBase
{
    /// <summary>
    /// 验证基类（已继承至NotifyPropertyBase）
    /// </summary>
    public class ValidateBase : NotifyPropertyBase, IDataErrorInfo
    {
        #region 错误信息

        private string _errorStr;

        /// <summary>
        /// 错误信息（界面显示用）
        /// </summary>
        public string Error
        {
            get { return _errorStr; }
        }

        #endregion

        public string this[string columnName]
        {
            get
            {
                var typeInfo = this.GetType();
                var proItems = typeInfo.GetProperty(columnName);

                var attTmps = proItems.GetCustomAttributes(false);
                var proValue = proItems.GetValue(this);
                foreach (var attItem in attTmps)
                {
                    var valTmp = attItem as ValidationAttribute;
                    if (valTmp != null && !valTmp.IsValid(proValue))
                    {
                        _errorStr = valTmp.ErrorMessage;
                        return valTmp.ErrorMessage;
                    }
                }
                return string.Empty;
            }
        }

        /// <summary>
        /// 验证是否合法（只验证带有验证标记的属性）
        /// </summary>
        /// <returns></returns>
        public bool IsValidation()
        {
            var typeInfo = this.GetType();
            var proItems = typeInfo.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly);
            foreach (var item in proItems)
            {
                var attTmp = item.GetCustomAttributes(false);
                var value = item.GetValue(this);
                foreach (var attItem in attTmp)
                {
                    var valTmp = attItem as ValidationAttribute;
                    if (valTmp != null && !valTmp.IsValid(value))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        #region 选择标识

        private bool _isChecked;
        /// <summary>
        /// 是否选中
        /// </summary>
        public bool IsChecked
        {
            get { return _isChecked; }
            set
            {
                _isChecked = value;
                base.OnPropertyChanged();
            }
        }

        #endregion
    }
}
