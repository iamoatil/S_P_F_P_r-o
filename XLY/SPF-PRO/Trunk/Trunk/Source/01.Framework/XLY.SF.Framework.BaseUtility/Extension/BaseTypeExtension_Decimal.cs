using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


/*************************************************
 * 创建人：Bob
 * 创建时间：2017/3/14 17:01:34
 * 类功能说明：
 *
 *************************************************/

namespace XLY.SF.Framework.BaseUtility
{
    public static partial class BaseTypeExtension
    {
        #region ToDecimal
        /// <summary>
        /// 转换为Decimal
        /// </summary>
        /// <param name="value">转换的值</param>
        /// <returns>失败返回default</returns>
        public static decimal ToDecimal(this string value)
        {
            decimal result;
            if (decimal.TryParse(value, out result))
            {
                return result;
            }
            return default(decimal);
        }
        #endregion

        #region ToSafeDecimal

        /// <summary>
        /// 转换为安全的Decimal
        /// </summary>
        public static Decimal ToSafeDecimal(this string value)
        {
            Decimal result;
            if (Decimal.TryParse(value, out result))
            {
                return result;
            }
            else
            {
                return 0;
            }
        }

        #endregion

        #region ToDouble
        /// <summary>
        /// 转换为double
        /// </summary>
        public static double ToDouble(this string value)
        {
            double result;
            if (double.TryParse(value, out result))
            {
                return result;
            }
            return default(double);
        }
        #endregion

        #region ToSafeDouble
        /// <summary>
        /// 转换为安全的Double
        /// </summary>
        public static double ToSafeDouble(this string value)
        {
            double result;
            if (double.TryParse(value, out result))
            {
                return result;
            }
            else
            {
                return 0;
            }
        }
        #endregion
    }
}
