using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XLY.SF.Project.ScriptEngine.Engine
{
    /// <summary>
    /// 调试
    /// </summary>
    public class Debug
    {
        /// <summary>
        /// 输出字符
        /// </summary>
        public void Write(string message)
        {
            Console.Write(message);
        }

        /// <summary>
        /// 输出字符
        /// </summary>
        public void Write(object message)
        {
            if(message is string)
            {
                Console.Write(message);
            }
            else
                Console.Write(Newtonsoft.Json.JsonConvert.SerializeObject(message));
        }

        /// <summary>
        /// 换行并输出字符
        /// </summary>
        public void WriteLine(object message)
        {
            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(message));
        }

        /// <summary>
        /// 换行并输出字符
        /// </summary>
        public void WriteLine(string message)
        {
            Console.WriteLine(message);
        }

        /// <summary>
        /// 输出换行
        /// </summary>
        public void WriteNewLine()
        {
            Console.WriteLine();
        }

        /// <summary>
        /// 输出当前日期时间
        /// </summary>
        public void WriteDateTime()
        {
            Console.Write(DateTime.Now.ToString());
        }

        /// <summary>
        /// 获取随机数，指定最小数字start和最大数字end
        /// </summary>
        public string RandomInt(object start, object end)
        {
            var a = Int32.Parse(start.ToString());
            var b = Int32.Parse(end.ToString());
            Random ran = new Random();
            return ran.Next(a, b).ToString();
        }
    }
}