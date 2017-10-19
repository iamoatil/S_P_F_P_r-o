using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using Color = System.Windows.Media.Color;

namespace XLY.SF.Framework.BaseUtility
{
    /// <summary>
    /// 颜色操作辅助类
    /// </summary>
    public class ColorHelper
    {
        /// <summary>
        /// 获取指定颜色的浅色值
        /// </summary>
        public static Color GetLowerColor(Color color)
        {
            try
            {
                //起始颜色
                Int16[] s = new Int16[]
                {
                    Int16.Parse(color.R.ToString()),
                    Int16.Parse(color.G.ToString()),
                    Int16.Parse(color.B.ToString())
                };
                Color end = Color.FromRgb(250, 250, 250);
                Int16[] e = new Int16[]
                {
                    Int16.Parse(end.R.ToString()),
                    Int16.Parse(end.G.ToString()),
                    Int16.Parse(end.B.ToString()),
                };
                //区间
                int step = 2;
                var n = 1;
                //渐变色
                Int32[] c = new Int32[3];
                for (int j = 0; j < 3; j++)
                {
                    c[j] = e[j] + (s[j] - e[j]) / step * n;
                }
                var res = Color.FromRgb(BitConverter.GetBytes(c[0])[0],
                    BitConverter.GetBytes(c[1])[0], BitConverter.GetBytes(c[2])[0]);
                return res;
            }
            catch
            {
                return Colors.DarkGray;
            }

        }

        /// <summary>
        /// 获取指定颜色的深色值
        /// </summary>
        public static Color GetHeightColor(Color color)
        {
            try
            {
                //起始颜色
                Int16[] s = new Int16[]
                            {
                                Int16.Parse(color.R.ToString()),
                                Int16.Parse(color.G.ToString()),
                                Int16.Parse(color.B.ToString())
                            };
                Color end = Color.FromRgb(11, 12, 12);
                Int16[] e = new Int16[]
                            {
                                Int16.Parse(end.R.ToString()),
                                Int16.Parse(end.G.ToString()),
                                Int16.Parse(end.B.ToString()),
                            };
                //区间
                int step = 2;
                var n = 1;
                //渐变色
                Int32[] c = new Int32[3];
                for (int j = 0; j < 3; j++)
                {
                    c[j] = e[j] + (s[j] - e[j]) / step * n;
                }
                var res = Color.FromRgb(BitConverter.GetBytes(c[0])[0],
                    BitConverter.GetBytes(c[1])[0], BitConverter.GetBytes(c[2])[0]);
                return res;
            }
            catch
            {
                return Colors.DarkGray;
            }
        }
    }
}
