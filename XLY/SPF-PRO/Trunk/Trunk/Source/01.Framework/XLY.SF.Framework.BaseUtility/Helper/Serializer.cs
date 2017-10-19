using Newtonsoft.Json;
using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Xml.Serialization;

namespace XLY.SF.Framework.BaseUtility
{
    /// <summary>
    /// 提供各种序列化方法
    /// </summary>
    public static class Serializer
    {
        #region 将对象序列化到XML文件中
        /// <summary>
        /// 将对象序列化到XML文件中
        /// </summary>
        /// <typeparam name="T">要序列化的类，即instance的类名</typeparam>
        /// <param name="instance">要序列化的对象</param>
        /// <param name="xmlFile">Xml文件名，表示保存序列化数据的位置</param>
        public static void SerializeToXML<T>(T instance, string xmlFile)
        {
            //创建XML序列化对象
            var serializer = new XmlSerializer(typeof(T));

            //创建文件流
            using (FileStream fs = new FileStream(xmlFile, FileMode.Create))
            {
                //开始序列化对象
                serializer.Serialize(fs, instance);
            }
        }
        #endregion

        #region 将XML文件反序列化为对象
        /// <summary>
        /// 将XML文件反序列化为对象
        /// </summary>
        /// <typeparam name="T">要获取的类</typeparam>
        /// <param name="xmlFile">Xml文件名，即保存序列化数据的位置</param>        
        public static T DeSerializeFromXML<T>(string xmlFile) where T : class
        {
            //创建XML序列化对象
            var serializer = new XmlSerializer(typeof(T));

            //创建文件流
            using (FileStream fs = new FileStream(xmlFile, FileMode.Open))
            {
                //开始反序列化对象
                return serializer.Deserialize(fs) as T;
            }
        }

        /// <summary>
        /// 将XML文件反序列化为对象
        /// </summary>
        /// <typeparam name="T">要获取的类</typeparam>
        /// <param name="xmlFile">Xml文件名，即保存序列化数据的位置</param>        
        public static object DeSerializeFromXML(string xmlFile, Type t)
        {
            //创建XML序列化对象
            var serializer = new XmlSerializer(t);

            //创建文件流
            using (FileStream fs = new FileStream(xmlFile, FileMode.Open))
            {
                //开始反序列化对象
                return serializer.Deserialize(fs);
            }
        }
        #endregion

        #region 将对象序列化到二进制文件中
        /// <summary>
        /// 将对象序列化到二进制文件中
        /// </summary>
        /// <param name="instance">要序列化的对象</param>
        /// <param name="fileName">文件名，保存二进制序列化数据的位置,后缀一般为.bin</param>
        public static void SerializeToBinary(object instance, string fileName)
        {
            //创建二进制序列化对象
            BinaryFormatter serializer = new BinaryFormatter();

            //创建文件流
            using (FileStream fs = new FileStream(fileName, FileMode.Create))
            {
                //开始序列化对象
                serializer.Serialize(fs, instance);
            }
        }

        /// <summary>
        /// 将对象序列化为二进制数据
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        public static byte[] SerializeToBinary(object instance)
        {
            //创建二进制序列化对象
            BinaryFormatter serializer = new BinaryFormatter();

            //创建文件流
            using (MemoryStream _memory = new MemoryStream())
            {
                //开始序列化对象
                serializer.Serialize(_memory, instance);

                _memory.Position = 0;
                byte[] read = new byte[_memory.Length];
                _memory.Read(read, 0, read.Length);
                _memory.Close();
                return read;
            }
        }

        #endregion

        #region 将二进制文件反序列化为对象
        /// <summary>
        /// 将二进制文件反序列化为对象
        /// </summary> <typeparam name="T">要获取的类</typeparam>
        /// <param name="fileName">文件名，保存二进制序列化数据的位置</param>        
        public static T DeSerializeFromBinary<T>(string fileName) where T : class
        {
            //创建二进制序列化对象
            BinaryFormatter serializer = new BinaryFormatter();

            //创建文件流
            using (FileStream fs = new FileStream(fileName, FileMode.Open))
            {
                //开始反序列化对象-
                return serializer.Deserialize(fs) as T;
            }
        }

        /// <summary>
        /// 将二进制数据反序列化为对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="buffer"></param>
        /// <returns></returns>
        public static T DeSerializeFromBinary<T>(byte[] buffer) where T : class
        {
            //创建二进制序列化对象
            BinaryFormatter serializer = new BinaryFormatter();

            //创建文件流
            using (MemoryStream ms = new MemoryStream(buffer))
            {
                ms.Position = 0;

                //开始反序列化对象-
                return serializer.Deserialize(ms) as T;
            }
        }

        #endregion

        #region 将对象序列化成字符串
        /// <summary>
        /// 将对象序列化
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        public static string SerializeToXML<T>(T instance)
        {
            using (var sw = new StringWriter())
            {
                var xs = new XmlSerializer(instance.GetType());
                xs.Serialize(sw, instance);
                return sw.ToString();
            }
        }
        #endregion

        #region 将字符串范序列化成对象
        /// <summary>
        /// 将字符串范序列化成对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="s"></param>
        /// <returns></returns>
        public static T DeSerializeToObject<T>(string s) where T : class
        {
            using (var sr = new StringReader(s))
            {
                var xz = new XmlSerializer(typeof(T));
                return (T)xz.Deserialize(sr);
            }
        }
        #endregion

        #region 将对象json序列化为字符串
        /// <summary>
        /// 将对象json序列化为字符串，保留类型说明（$type="Message")
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string JsonSerilize(object obj)
        {
            //var binder = new JsonTypeNameSerializationBinder();
            var jsondata = JsonConvert.SerializeObject(obj, Formatting.Indented, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,       //忽略循环引用
                //Binder = binder
            });
            return jsondata;
        }

        /// <summary>
        /// JSON序列化(二进制方式，实体类使用[Serializable])
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <returns></returns>
        public static string JsonSerializerIO<T>(T t)
        {
            DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(T));
            using (MemoryStream ms = new MemoryStream())
            {
                ser.WriteObject(ms, t);
                string jsonString = Encoding.UTF8.GetString(ms.ToArray());
                ms.Close();
                return jsonString;
            }
        }

        /// <summary>
        /// JSON序列化 使用fastJson
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string JsonFastSerilize(object obj)
        {
            return fastJSON.JSON.ToJSON(obj);
        }

        #endregion

        #region 将json字符串反序列化为对象

        /// <summary>
        /// 将json字符串（包含$type="Message"）转换为对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="json"></param>
        /// <returns></returns>
        public static T JsonDeserilize<T>(string json, bool isAbsoluteType = true)
        {
            //if (isAbsoluteType)
            //{
            //    var data = JsonConvert.DeserializeObject<T>(json, new JsonSerializerSettings
            //    {
            //        TypeNameHandling = TypeNameHandling.All,
            //    });
            //    return data;
            //}
            //else
            //{
            //    int index0 = json.IndexOf("\"$type\": \"");
            //    if(index0 < 0)
            //    {
            //        return JsonDeserilize<T>(json, true);
            //    }
            //    try
            //    {
            //        string typeString = json.Substring(index0 + 10, json.IndexOf("\"", index0 + 10) - index0 - 10);
            //        Type realType = Type.GetType(typeString);
            //        return (T)JsonConvert.DeserializeObject(json, realType, new JsonSerializerSettings
            //        {
            //            TypeNameHandling = TypeNameHandling.All,
            //        });
            //    }
            //    catch(Exception ex)
            //    {
            //        return JsonDeserilize<T>(json, true);
            //    }
            //}

            //var binder = new JsonTypeNameSerializationBinder();
            var data = JsonConvert.DeserializeObject<T>(json, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All,
                //Binder = binder
            });
            return data;
        }


        /// <summary>
        /// JSON反序列化(二进制方法，实体类使用[Serializable])
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="jsonString"></param>
        /// <returns></returns>
        public static T JsonDeserializeIO<T>(string jsonString)
        {
            DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(T));
            using (MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(jsonString)))
            {
                T obj = (T)ser.ReadObject(ms);
                return obj;
            }
        }

        /// <summary>
        /// JSON反序列化 使用fastJson
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="json"></param>
        /// <returns></returns>
        public static T JsonFastDeserilize<T>(string json)
        {
            return fastJSON.JSON.ToObject<T>(json);
        }

        #endregion

    }

    /// <summary>
    /// json序列化时，用于转换类型的名称和程序集名称
    /// </summary>
    public class JsonTypeNameSerializationBinder : SerializationBinder
    {
        public override void BindToName(Type serializedType, out string assemblyName, out string typeName)
        {
            assemblyName = null;
            typeName = serializedType.Name;
        }
        public override Type BindToType(string assemblyName, string typeName)
        {
            if (EmitCreator.DicEmitType.ContainsKey(typeName))
            {
                return EmitCreator.DicEmitType[typeName];
            }
            var format = "{0}.{1}, {0}";
            return Type.GetType(string.Format(format, EmitCreator.DefaultAssemblyName, typeName), true);
        }
    }
}
