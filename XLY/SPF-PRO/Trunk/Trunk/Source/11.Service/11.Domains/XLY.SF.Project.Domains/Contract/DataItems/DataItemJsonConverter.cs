using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/* ==============================================================================
* Assembly   ：	XLY.SF.Project.Domains.Contract.DataItems.DataItemJsonConverter
* Description：	  
* Author     ：	fhjun
* Create Date：	2017/9/14 15:27:16
* ==============================================================================*/

namespace XLY.SF.Project.Domains
{
    /// <summary>
    /// 自定义的IDataItems转JsonConverter
    /// </summary>
    public class DataItemJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType.IsAssignableFrom(typeof(IDataItems));
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            IDataItems item = null;
            Type type = typeof(IDataItems);
            Dictionary<string, object> dicProp = new Dictionary<string, object>();

            if(reader.TokenType == JsonToken.StartObject)       //如果是object对象
            {
                JObject jo = JObject.Load(reader);
                foreach(var j in jo.Properties())    //读取所有的json属性
                {
                    dicProp[j.Name] = j.Value.ToObject<string>();
                }
            }
            
            if (dicProp.ContainsKey("$type"))   //如果包含了$type，则创建实际的数据类型实例，并反射设置值
            {
                type = Type.GetType(dicProp["$type"].ToString());
                if (type.IsGenericType)
                {
                    item = Activator.CreateInstance(type, dicProp["DbFilePath"], false) as IDataItems;
                }
                else
                {
                    item = Activator.CreateInstance(type, dicProp["DbFilePath"], dicProp["DataColunms"], false) as IDataItems;
                }
                
                foreach (var p in dicProp)
                {
                    if (p.Key != "$type")
                    {
                        type.GetProperty(p.Key).SetValue(item, p.Value);
                    }
                }
            }

            return item;
        }

        /// <summary>
        /// 写入json，针对IDataItems对象，不写入数据列表，而是只写入数据库属性
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="value"></param>
        /// <param name="serializer"></param>
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if(value.GetType().IsGenericType)
            {
                JObject jo = new JObject();
                jo.AddFirst(new JProperty("Key", value.GetType().GetProperty("Key").GetValue(value)));
                jo.AddFirst(new JProperty("DbTableName", value.GetType().GetProperty("DbTableName").GetValue(value)));
                jo.AddFirst(new JProperty("DbFilePath", value.GetType().GetProperty("DbFilePath").GetValue(value)));
                jo.AddFirst(new JProperty("$type", value.GetType().FullName));
                jo.WriteTo(writer);
            }
            else
            {
                JObject jo = new JObject();
                jo.AddFirst(new JProperty("DataColunms", value.GetType().GetProperty("DataColunms").GetValue(value)));
                jo.AddFirst(new JProperty("Key", value.GetType().GetProperty("Key").GetValue(value)));
                jo.AddFirst(new JProperty("DbTableName", value.GetType().GetProperty("DbTableName").GetValue(value)));
                jo.AddFirst(new JProperty("DbFilePath", value.GetType().GetProperty("DbFilePath").GetValue(value)));
                jo.AddFirst(new JProperty("$type", value.GetType().FullName));
                jo.WriteTo(writer);
            }
        }
    }
}
