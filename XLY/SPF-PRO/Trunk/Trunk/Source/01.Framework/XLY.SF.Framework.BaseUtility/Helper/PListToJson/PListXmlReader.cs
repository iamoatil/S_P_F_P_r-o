using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace XLY.SF.Framework.BaseUtility.Plist
{
    internal class PListXmlReader
    {
        public List<IPlistXmlItem> Items { get; set; }

        public JToken LoadXml(XElement xmlDoc)
        {
            var topNode = xmlDoc.Element("dict").Element("array");

            this.Items = new List<IPlistXmlItem>();

            IPlistXmlItem tempItem = null;

            foreach(var node in topNode.Elements())
            {
                tempItem = LoadXmlNode(node);
                if(null != tempItem)
                {
                    tempItem.Uid = this.Items.Count;
                    this.Items.Add(tempItem);
                }
                else
                {
                    tempItem = new PListXmlItemString();
                    tempItem.Uid = this.Items.Count;
                    this.Items.Add(tempItem);
                }
            }

            PListXmlItemDict root = this.Items[1] as PListXmlItemDict;

            return GetJObject(root);
        }

        private JToken GetJObject(PListXmlItemDict dict)
        {
            if(dict.Value.Count == 2 && dict.Value.Keys.Contains("NS.objects") && dict.Value.Keys.Contains("$class"))
            {
                return GetJToken(dict.Value["NS.objects"]);
            }
            else if (dict.Value.Count == 3 && dict.Value.Keys.Contains("NS.objects") && dict.Value.Keys.Contains("NS.keys") && dict.Value.Keys.Contains("$class")
                && dict.Value["NS.objects"] is PListXmlItemArray && dict.Value["NS.keys"] is PListXmlItemArray)
            {
                return GetJToken(dict.Value["NS.keys"] as PListXmlItemArray, dict.Value["NS.objects"] as PListXmlItemArray);
            }
            else
            {
                JObject jRoot = new JObject();

                foreach(var item in dict.Value)
                {
                    jRoot.Add(item.Key, GetJToken(item.Value));
                }

                return jRoot;
            }
        }

        private JArray GetJArray(PListXmlItemArray array)
        {
            JArray jArr = new JArray();

            foreach(var item in array.Value)
            {
                jArr.Add(GetJToken(item));
            }

            return jArr;
        }

        private JToken GetJToken(IPlistXmlItem item)
        {
            if(item is PListXmlItemUid)
            {
                int uid = (item as PListXmlItemUid).Value;

                return GetJToken(this.Items[uid]);
            }
            else if(item is PListXmlItemDict)
            {
                return GetJObject((item as PListXmlItemDict));
            }
            else if(item is PListXmlItemInteger)
            {
                return (item as PListXmlItemInteger).Value;
            }
            else if(item is PListXmlItemString)
            {
                return (item as PListXmlItemString).Value;
            }
            else if(item is PListXmlItemArray)
            {
                return GetJArray(item as PListXmlItemArray);
            }
            else if(item is PListXmlItemFalse)
            {
                return "false";
            }
            else if(item is PListXmlItemTrue)
            {
                return "true";
            }

            return string.Empty;
        }

        private JToken GetJToken(PListXmlItemArray keys, PListXmlItemArray values)
        {
            if (keys.Value.Count != values.Value.Count)
            {
                return string.Empty;
            }

            JObject res = new JObject();
            for (int pos = 0; pos < keys.Value.Count; pos++)
            {
                res.Add((this.Items[(keys.Value[pos] as PListXmlItemUid).Value] as PListXmlItemString).Value,
                        GetJToken(values.Value[pos]));
            }

            return res;
        }

        public static IPlistXmlItem LoadXmlNode(XElement node)
        {
            IPlistXmlItem tempItem = null;
            switch(node.Name.LocalName)
            {
                case "string":
                case "ustring":
                    tempItem = new PListXmlItemString();
                    break;
                case "dict":
                    tempItem = new PListXmlItemDict();
                    break;
                case "integer":
                    tempItem = new PListXmlItemInteger();
                    break;
                case "array":
                    tempItem = new PListXmlItemArray();
                    break;
                case "uid":
                    tempItem = new PListXmlItemUid();
                    break;
                case "false":
                    tempItem = new PListXmlItemFalse();
                    break;
                case "true":
                    tempItem = new PListXmlItemTrue();
                    break;
                default:
                    tempItem = null;
                    break;
            }
            if(null != tempItem)
            {
                tempItem.Load(node);
            }

            return tempItem;
        }

    }
}
