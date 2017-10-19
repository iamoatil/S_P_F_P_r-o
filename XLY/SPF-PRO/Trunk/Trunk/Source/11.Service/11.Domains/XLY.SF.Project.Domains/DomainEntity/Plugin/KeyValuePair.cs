using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XLY.SF.Project.Domains
{
    /// <summary>
    /// 字符串键值项
    /// </summary>
    [Serializable]
    public class KeyValueItem : AbstractDataItem, IComparer<KeyValueItem>
    {
        [Display]
        public string Key { get; set; }

        [Display]
        public string Value { get; set; }

        public KeyValueItem()
        { 
        }

        public KeyValueItem(string key, string value)
        {
            this.Key = key;
            this.Value = value;
        }

        public int Compare(KeyValueItem x, KeyValueItem y)
        {
            var k1 = x.Key.GetHashCode();
            var k2 = y.Key.GetHashCode();
            if (k1 == k2) return 0;
            return k1 > k2 ? 1 : -1;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("{0},{1}", Key, Value);
            return sb.ToString();
        }
    }

    /// <summary>
    /// 字符串键值集合
    /// </summary>
    public class KeyValueItems : List<KeyValueItem>
    {
        /// <summary>
        /// 添加键值项
        /// </summary>
        public void AddItem(string key, string value)
        {
            this.Add(new KeyValueItem(key, value));
        }

        /// <summary>
        /// 添加key唯一的键值项
        /// </summary>
        public void AddUnqueItems(List<string> keys)
        {
            if (keys==null)
            {
                return;
            }
            foreach (var key in keys)
            {
                this.AddUnqueItem(key, string.Empty);
            }
        }

        /// <summary>
        /// 添加key唯一的键值项组
        /// </summary>
        public void AddUnqueItem(string key, string value)
        {
            var res = this.Find(s => s.Key == key);
            if (res == null)
            {
                this.AddItem(key, value);
            }
        }

        /// <summary>
        /// 获取键对应的值，不存在返回string.Empty
        /// </summary>
        public string TryGetValue(string key)
        {
            var res = this.Find(s => s.Key == key);
            if (res == null)
            {
                return string.Empty;
            }
            return res.Value;
        }

        public string this[string key]
        {
            get { return this.TryGetValue(key); }
        }
    }
}
