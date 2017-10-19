/* 
    ======================================================================== 
        File name：        DynamicJsonConverter.cs
        Module:            System.Utility
        Author：           ShiXing
        Create Time：      2016年12月13日15:21:24
        Modify By:        
        Modify Date:    
    ======================================================================== 
*/
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Dynamic;
using System.Linq;
using System.Web.Script.Serialization;

namespace System
{
    public class DynamicJsonConverter : JavaScriptConverter
    {
        public override object Deserialize(IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
        {
            if (dictionary == null)
                throw new ArgumentNullException("dictionary");

            if (type == typeof(object))
            {
                return new DynamicJsonObject(dictionary);
            }

            return null;
        }

        public override IDictionary<string, object> Serialize(object obj, JavaScriptSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<Type> SupportedTypes
        {
            get { return new ReadOnlyCollection<Type>(new List<Type>(new Type[] { typeof(object) })); }
        }
    }

    public class DynamicJsonObject : DynamicObject
    {
        private IDictionary<string, object> Dictionary { get; set; }

        public DynamicJsonObject(IDictionary<string, object> dictionary)
        {
            this.Dictionary = dictionary;
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            result = this.Dictionary[binder.Name];

            if (result is IDictionary<string, object>)
            {
                result = new DynamicJsonObject(result as IDictionary<string, object>);
            }
            else if (result is ArrayList && (result as ArrayList) is IDictionary<string, object>)
            {
                result = new List<DynamicJsonObject>((result as ArrayList).ToArray().Select(x => new DynamicJsonObject(x as IDictionary<string, object>)));
            }
            else if (result is ArrayList)
            {
                result = new List<object>((result as ArrayList).ToArray());
            }

            return this.Dictionary.ContainsKey(binder.Name);
        }

        #region 索引方法  Add By ShiXing 2016年12月27日16:03:40

        /// <summary>
        /// 扩展一个索引方法，使得方便访问。
        /// </summary>
        /// <param name="par">索引键。</param>
        /// <returns>索引值</returns>
        public object this[string par]
        {
            get
            {
                if (this.Dictionary.ContainsKey(par))
                    return this.Dictionary[par];

                return null;
            }

            set
            {
                if (this.Dictionary.ContainsKey(par))
                    this.Dictionary[par] = value;

                this.Dictionary.Add(par, value);
            }
        } 

        #endregion
    }
}
