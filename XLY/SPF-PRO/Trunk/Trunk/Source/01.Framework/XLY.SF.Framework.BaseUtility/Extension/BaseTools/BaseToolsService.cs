using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using XLY.SF.Framework.Log4NetService;


/*************************************************
 * 创建人：Bob
 * 创建时间：2017/3/15 13:30:43
 * 类功能说明：基础更具服务类
 * 1.提供基础类型的辅助方法
 *
 *************************************************/

namespace XLY.SF.Framework.BaseUtility.BaseTools
{
    public class BaseToolsService
    {
        /// <summary>
        /// 深拷贝引用对象
        /// </summary>
        /// <typeparam name="T">要拷贝的对象类型</typeparam>
        /// <param name="source">被拷贝对象实例</param>
        /// <returns>深拷贝副本</returns>
        public T DeepCopy<T>(T source)
            where T : class
        {
            T CloneObject = null;
            var Ttype = typeof(T);
            if (Ttype.IsDefined(typeof(SerializableAttribute), false) && source != null)
            {
                using (var ms = new MemoryStream())
                {
                    var bf = new BinaryFormatter(null, new StreamingContext(StreamingContextStates.Clone));
                    bf.Serialize(ms, source);
                    ms.Seek(0, SeekOrigin.Begin);
                    try
                    {
                        // 反序列化至另一个对象(即创建了一个原对象的深表副本) 
                        CloneObject = (T)bf.Deserialize(ms);
                    }
                    catch (Exception ex)
                    {
                        LoggerManagerSingle.Instance.Error(ex);
                    }
                }
            }
            return CloneObject;
        }
    }
}
