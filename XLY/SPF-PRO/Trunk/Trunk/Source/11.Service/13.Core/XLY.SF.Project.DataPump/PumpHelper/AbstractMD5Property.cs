using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace XLY.SF.Project.DataPump.PumpHelper
{
    [Serializable]
    public abstract class AbstractMD5Property
    {
        /// <summary>
        /// 实体反射后的对象
        /// </summary>
        public static Dictionary<string, PropertyInfo[]> modelList = new Dictionary<string, PropertyInfo[]>();

        /// <summary>
        /// 需要添加文件计算MD5的实体及字段,Key为实体类类名,Value为字段名(该字段一般存的是文件路径)
        /// </summary>
        public static Dictionary<string, string> excludeModel = new Dictionary<string, string>()
            .TryAdd("MessageCore", "")
            .TryAdd("SinaMicroBlogContent", "PicPath")
            .TryAdd("SinaMicroBlogLetter", "")
            .TryAdd("TencentMicroBlog", "")
            .TryAdd("TencentLetters", "");


        private string _md5String;
        /// <summary>
        /// MD5串
        /// </summary>
        public string MD5String
        {
            get
            {
                if (string.IsNullOrEmpty(_md5String))
                {
                    _md5String = BuildMD5String();
                }
                return _md5String;
            }
            set { _md5String = value; }
        }

        /// <summary>
        /// 生成MD5字符串
        /// 注：把该实体所有Public属性的值串联后生成MD5字符串
        /// </summary>
        /// <returns></returns>
        public virtual string BuildMD5String()
        {
            try
            {
                string result = "";
                StringBuilder context = new StringBuilder();
                string modelName = this.GetType().Name;
                //这儿还要过滤有文件记录的实体，有文件记录的实体应该是由该实体所有值串联后的Byte与文件的Byte合并在一起生成MD5
                PropertyInfo[] pis = AbstractMD5Property.GetModelProperty(this, modelName);
                if (pis == null)
                {
                    return result;
                }
                int len = pis == null ? 0 : pis.Length;
                for (int i = 0; i < len; i++)
                {
                    if (!string.Equals(pis[i].Name, "MD5String"))
                    {
                        var item = pis[i].GetValue(this, null);
                        context.Append(item);
                    }
                }
                if (!AbstractMD5Property.excludeModel.ContainsKey(modelName))
                {
                    result = Cryptography.MD5FromString(context.ToString());
                }
                else
                {
                    string fieldName = AbstractMD5Property.excludeModel.GetValue(modelName, "");
                    string filePath = "";
                    for (int i = 0; i < len; i++)
                    {
                        if (string.Equals(pis[i].Name, fieldName))
                        {
                            filePath = pis[i].GetValue(this, null).ToString();
                            break;
                        }
                    }
                    result = Cryptography.MD5FromStringAndFile(context.ToString(), filePath);

                    //System.Utility.Logger.LogHelper.Error(string.Format("MD5FromStringAndFile Md5:{0}", result));
                }
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 生成MD5字符串
        /// </summary>
        /// <param name="excludes">要排除的属性</param>
        /// <returns></returns>
        public virtual string BuildMD5String(params string[] excludes)
        {
            try
            {
                string result = "";
                StringBuilder context = new StringBuilder();
                string modelName = this.GetType().Name;
                //这儿还要过滤有文件记录的实体，有文件记录的实体应该是由该实体所有值串联后的Byte与文件的Byte合并在一起生成MD5
                PropertyInfo[] pis = AbstractMD5Property.GetModelProperty(this, modelName);
                if (pis == null)
                {
                    return result;
                }
                int len = pis == null ? 0 : pis.Length;
                for (int i = 0; i < len; i++)
                {
                    if (!string.Equals(pis[i].Name, "MD5String") && excludes.Contains(pis[i].Name))
                    {
                        var item = pis[i].GetValue(this, null);
                        context.Append(item);
                    }
                }
                if (!AbstractMD5Property.excludeModel.ContainsKey(modelName))
                {
                    result = Cryptography.MD5FromString(context.ToString());
                }
                else
                {
                    string fieldName = AbstractMD5Property.excludeModel.GetValue(modelName, "");
                    string filePath = "";
                    for (int i = 0; i < len; i++)
                    {
                        if (string.Equals(pis[i].Name, fieldName))
                        {
                            filePath = pis[i].GetValue(this, null).ToString();
                            break;
                        }
                    }
                    result = Cryptography.MD5FromStringAndFile(context.ToString(), filePath);

                    //System.Utility.Logger.LogHelper.Error(string.Format("MD5FromStringAndFile Md5:{0}", result));
                }
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 获取实体反射后的对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <returns></returns>
        public static PropertyInfo[] GetModelProperty<T>(T t, string modelName)
        {
            PropertyInfo[] result = null;
            if (!AbstractMD5Property.modelList.ContainsKey(modelName))
            {
                List<string> modelPrypertyList = new List<string>();
                result = t.GetType().GetProperties().ToArray();
                AbstractMD5Property.modelList.Add(t.GetType().Name, result);
            }
            else
            {
                result = AbstractMD5Property.modelList.GetSafeValue<string, PropertyInfo[]>(modelName, null);
            }
            return result;
        }
    }
    public static class DictionaryExtensionMethodClass
    {
        /// <summary>
        /// 尝试将键和值添加到字典中：如果不存在，才添加；存在，不添加也不抛导常
        /// </summary>
        public static Dictionary<TKey, TValue> TryAdd<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key, TValue value)
        {
            if (dict.ContainsKey(key) == false)
                dict.Add(key, value);
            return dict;
        }

        /// <summary>
        /// 获取与指定的键相关联的值，如果没有则返回输入的默认值
        /// </summary>
        public static TValue GetValue<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key, TValue defaultValue)
        {
            return dict.ContainsKey(key) ? dict[key] : defaultValue;
        }

        public static TValue GetSafeValue<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key, TValue defalut = default(TValue))
        {
            if (dictionary==null)
            {
                return defalut;
            }

            if (dictionary.ContainsKey(key))
            {
                return dictionary[key];
            }

            return defalut;
        }
    }
}
