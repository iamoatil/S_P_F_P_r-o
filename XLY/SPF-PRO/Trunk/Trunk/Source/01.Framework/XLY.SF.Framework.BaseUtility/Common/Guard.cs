using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XLY.SF.Framework.BaseUtility
{
    /// <summary>
    /// (n. 守卫)提供全局通用验证
    /// </summary>
    public static class Guard
    {
        #region ArgumentNotNull：验证参数是否为NULL
        /// <summary>
        /// 验证参数是否为NULL
        /// </summary>
        public static void ArgumentNotNull(object value, string argumentName)
        {
            if (value == null)
            {
                throw new ArgumentNullException(argumentName,
                    string.Format("parameter {0} can not be null.", argumentName));
            }
        }
        #endregion

        #region ArgumentNotNullOrEmpty：验证参数是否为NULL或空字符串
        /// <summary>
        /// 验证参数是否为NULL或空字符串
        /// </summary>
        public static void ArgumentNotNullOrEmpty(string value, string argumentName)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentNullException(argumentName,
                    string.Format("parameter {0} can not be null or empty.", argumentName));
            }
        }
        #endregion
    }
}
