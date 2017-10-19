using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using Newtonsoft.Json;

namespace XLY.SF.Framework.BaseUtility
{
    /// <summary>
    /// 解析PList文件
    /// </summary>
    internal class MacPListParse
    {
        /// <summary>
        /// 将plist数据转成json
        /// </summary>
        /// <returns></returns>
        public string ReadPlistToJson(string path)
        {
            string plistJson = string.Empty;
            var plistObject = ReadPlistToObject(path);
            if (plistObject != null)
            {
                plistJson = JsonConvert.SerializeObject(plistObject);
            }

            return plistJson;
        }

        public string ReadPlistToJson(byte[] data)
        {
            string plistJson = string.Empty;
            var plistObject = ReadPlistToObject(data);
            if (plistObject != null)
            {
                plistJson = JsonConvert.SerializeObject(plistObject);
            }

            return plistJson;
        }

        public object ReadPlistToObject(string path)
        {
            object plistObject = null;
            if (!File.Exists(path))
            {
                return null;
            }

            using (var fs = new FileStream(path, FileMode.Open))
            using (var br = new BinaryReader(fs))
            {
                if (TryParsePListData(br.ReadBytes((int)br.BaseStream.Length), out plistObject))
                {
                    return plistObject;
                }
            }

            return plistObject;
        }

        public object ReadPlistToObject(byte[] sourceData)
        {
            object plistObject = null;
            if (TryParsePListData(sourceData, out plistObject))
            {
                return plistObject;
            }

            return plistObject;
        }

        private bool TryParsePListData(byte[] sourceData, out object plistObject)
        {
            plistObject = null;
            _sourceBuffer = sourceData;

            if (IsXmlPListFile() && TryReadXmlPListData(out plistObject))
            {
                return true;
            }

            if (IsBPListFile() && TryParseBPListData(out plistObject))
            {
                return true;
            }

            return false;
        }

        #region BPList

        /// <summary>
        /// 源数据
        /// </summary>
        private byte[] _sourceBuffer;

        /// <summary>
        /// 偏移量表
        /// </summary>
        private List<int> _trailTable;

        /// <summary>
        /// 对象表整形字节长度
        /// </summary>
        private int _objectTableIntegerLength;

        /// <summary>
        /// 偏移表整形字节长度
        /// </summary>
        private byte _offsetTableIntegerLength;

        /// <summary>
        /// 根节点在偏移表中的位置
        /// </summary>
        private int _rootNodeInOffsetTable;

        public MacPListParse()
        {
            _trailTable = new List<int>();
        }

        private bool TryParseBPListData(out object bplistObject)
        {
            bplistObject = null;

            if (!IsSupportedVersion())
            {
                return false;
            }

            //解析尾部
            if (!TryParseTrail(_sourceBuffer, out _trailTable))
            {
                return false;
            }

            //解析对象表
            bplistObject = ParseObjectData(_trailTable[_rootNodeInOffsetTable]);

            return true;
        }

        /// <summary>
        /// 判断文件是否是二进制PList文件
        /// </summary>
        /// <returns></returns>
        private bool IsBPListFile()
        {
            byte[] header;
            if (TryCopy(_sourceBuffer, 0, 6, out header))
            {
                var strHeader = Encoding.UTF8.GetString(header);
                if (strHeader.Equals("bplist"))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 是否是支持的版本
        /// </summary>
        /// <returns></returns>
        private bool IsSupportedVersion()
        {
            return _sourceBuffer[6] == 0x30 && _sourceBuffer[7] == 0x30;
        }

        /// <summary>
        /// 解析尾部
        /// </summary>
        private bool TryParseTrail(byte[] sourceBuffer, out List<int> offsetTable)
        {
            offsetTable = new List<int>();
            byte[] trailData;
            if (TryCopy(sourceBuffer, sourceBuffer.Length - 32, 32, out trailData))
            {
                //备用数据 6byte

                //偏移表整形字节长度 1byte
                _offsetTableIntegerLength = trailData[6];

                //对象表整形字节长度 1byte
                _objectTableIntegerLength = trailData[7];

                //偏移表元素个数
                var offsetElementCount = (int)BitConverter.ToInt64(CopyLittleEndian(trailData, 8, 8), 0);

                //根节点在偏移表中的位置
                _rootNodeInOffsetTable = (int)BitConverter.ToInt64(CopyLittleEndian(trailData, 16, 8), 0);

                //偏移表起始位置
                var offsetTableStartIndex = (int)BitConverter.ToInt64(CopyLittleEndian(trailData, 24, 8), 0);

                byte[] offsetTableData;
                if (TryCopy(sourceBuffer, offsetTableStartIndex, _offsetTableIntegerLength * offsetElementCount,
                    out offsetTableData))
                {
                    //读取偏移表
                    offsetTable = ReadOffsetTable(_offsetTableIntegerLength, offsetElementCount,
                        sourceBuffer, offsetTableStartIndex);
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 读取偏移量表
        /// </summary>
        /// <param name="offsetTableIntegerLength">偏移表整形字节长度</param>
        /// <param name="offsetElementCount">偏移表元素个数</param>
        /// <param name="sourceBuffer">源数据</param>
        /// <param name="offsetTableStartIndex">偏移表起始位置</param>
        /// <returns></returns>
        private List<int> ReadOffsetTable(byte offsetTableIntegerLength, int offsetElementCount,
            byte[] sourceBuffer, int offsetTableStartIndex)
        {
            var offsetTable = new List<int>();
            if (offsetTableIntegerLength == 1)
            {
                for (int i = 0; i < offsetElementCount; i++)
                {
                    offsetTable.Add(sourceBuffer[offsetTableStartIndex]);
                    offsetTableStartIndex++;
                }
            }
            else if (offsetTableIntegerLength == 2)
            {
                for (int i = 0; i < offsetElementCount; i++)
                {
                    offsetTable.Add(TransferToInt(offsetTableStartIndex, 2));
                    offsetTableStartIndex += 2;
                }
            }
            else if (offsetTableIntegerLength == 4)
            {
                for (int i = 0; i < offsetElementCount; i++)
                {
                    offsetTable.Add(BitConverter.ToInt32(CopyLittleEndian(sourceBuffer, offsetTableStartIndex, 4), 0));
                    offsetTableStartIndex += 4;
                }
            }

            return offsetTable;
        }

        /// <summary>
        /// 解析对象表
        /// </summary>
        /// <param name="nodeIndex">节点偏移量</param>
        private object ParseObjectData(int nodeIndex)
        {
            object data = null;

            var dataType = _sourceBuffer[nodeIndex];
            switch (dataType & 0xF0)
            {
                case 0x00:
                    //单字节
                    data = ParseByte(dataType);
                    break;
                case 0x10:
                    //整型
                    data = ParseInteger(nodeIndex, dataType);
                    break;
                case 0x20:
                    //浮点数
                    data = ParseFloating(nodeIndex, dataType);
                    break;
                case 0x30:
                    //时间
                    data = ParseDateTime(nodeIndex, dataType);
                    break;
                case 0x40:
                    //字节数组
                    data = ParseByteArray(nodeIndex, dataType);
                    break;
                case 0x50:
                    //ASCII字符串
                    data = ParseAsciiString(nodeIndex, dataType);
                    break;
                case 0x60:
                    //UNICODE字符串
                    data = ParseUnicodeString(nodeIndex, dataType);
                    break;
                case 0x80:
                    //UID
                    data = ParseUID(nodeIndex, dataType);
                    break;
                case 0xA0:
                    //对象数据
                    data = ParseObjectArray(nodeIndex, dataType);
                    break;
                case 0xC0:
                    //集合数据
                    data = ParseObjectArray(nodeIndex, dataType);
                    break;
                case 0xD0:
                    //字典数据
                    data = ParseDictionary(nodeIndex, dataType);
                    break;
            }

            return data;
        }

        /// <summary>
        /// 解析单字节
        /// </summary>
        private object ParseByte(byte dataType)
        {
            object data = null;
            switch (dataType & 0xF)
            {
                case 8:
                    data = false;
                    break;
                case 9:
                    data = true;
                    break;
            }
            return data;
        }

        /// <summary>
        /// 解析整型
        /// </summary>
        private object ParseInteger(int nodeIndex, byte dataType)
        {
            int dataLength = (int)Math.Pow(2, dataType & 0xF);
            if (dataLength == 8)
            {
                return TransferToLong(nodeIndex + 1, dataLength);
            }
            else if (dataLength > 4)
            {
                dataLength = 4;
            }
            return TransferToInt(nodeIndex + 1, dataLength);
        }

        /// <summary>
        /// 解析浮点数
        /// </summary>
        private object ParseFloating(int nodeIndex, byte dataType)
        {
            object data = null;

            int dataLength = (int)Math.Pow(2, dataType & 0xF);
            if (dataLength == 4)
            {
                //单精度浮点数
                data = BitConverter.ToSingle(CopyLittleEndian(_sourceBuffer, nodeIndex + 1, 4), 0);
            }
            else if (dataLength == 8)
            {
                //双精度浮点数
                data = BitConverter.ToDouble(CopyLittleEndian(_sourceBuffer, nodeIndex + 1, 8), 0);
            }

            return data;
        }

        /// <summary>
        /// 解析时间
        /// </summary>
        private object ParseDateTime(int nodeIndex, byte dataType)
        {
            object data = null;
            int dataLength = (int)Math.Pow(2, dataType & 0xF);
            if (dataLength == 8)
            {
                data = ConvertFromAppleTimeStamp(BitConverter.ToDouble(CopyLittleEndian(_sourceBuffer, nodeIndex + 1, 8), 0));
            }

            return data;
        }

        /// <summary>
        /// 解析字节数组
        /// </summary>
        private object ParseByteArray(int nodeIndex, byte dataType)
        {
            int dataLength = CalCollectionDataLength(nodeIndex, dataType);
            int startIndex = 0;
            if (dataLength < 0xF)
            {
                startIndex = nodeIndex + 1;
            }
            else
            {
                startIndex = nodeIndex + 2 + (int)Math.Pow(2, _sourceBuffer[nodeIndex + 1] & 0xF);
            }

            return CopyBigEndian(startIndex, dataLength);
        }

        /// <summary>
        /// 解析ASCII字符串
        /// </summary>
        private object ParseAsciiString(int nodeIndex, byte dataType)
        {
            int len = CalCollectionDataLength(nodeIndex, dataType);
            if (len < 0xF)
            {
                return Encoding.ASCII.GetString(_sourceBuffer, nodeIndex + 1, len);
            }
            else
            {
                return Encoding.ASCII.GetString(_sourceBuffer, nodeIndex + 3, len);
            }
        }

        /// <summary>
        /// 解析UNICODE字符串
        /// </summary>
        private object ParseUnicodeString(int nodeIndex, byte dataType)
        {
            int len = CalCollectionDataLength(nodeIndex, dataType);
            int startIndex = 0;
            if (len < 0xF)
            {
                startIndex = nodeIndex + 1;
            }
            else
            {
                startIndex = nodeIndex + 2 + (int)Math.Pow(2, _sourceBuffer[nodeIndex + 1] & 0xF);
            }
            var data = CopyBigEndian(startIndex, len * 2);
            for (int i = 0; i < data.Length - 1; i += 2)
            {
                byte temp = data[i];
                data[i] = data[i + 1];
                data[i + 1] = temp;
            }

            return Encoding.Unicode.GetString(data, 0, data.Length);
        }

        /// <summary>
        /// 解析UID
        /// </summary>
        private object ParseUID(int nodeIndex, byte dataType)
        {
            int len = dataType & 0xF;
            return CopyBigEndian(nodeIndex + 1, len + 1);
        }

        /// <summary>
        /// 解析对象数组
        /// </summary>
        private object ParseObjectArray(int nodeIndex, byte dataType)
        {
            var data = new List<object>();

            int elementCount = CalCollectionDataLength(nodeIndex, dataType);
            int dataStartIndex = 0;
            if (elementCount < 0xF)
            {
                dataStartIndex = nodeIndex + 1;
            }
            else
            {
                dataStartIndex = nodeIndex + 2 + (int)Math.Pow(2, _sourceBuffer[nodeIndex + 1] & 0xF);
            }
            for (int i = 0; i < elementCount; i++)
            {
                var objectIndex = TransferToInt(dataStartIndex + i * _objectTableIntegerLength, _objectTableIntegerLength);
                data.Add(ParseObjectData(_trailTable[objectIndex]));
            }

            return data;
        }

        /// <summary>
        /// 解析字典数据
        /// </summary>
        private object ParseDictionary(int nodeIndex, byte dataType)
        {
            var data = new Dictionary<string, object>();

            int elementCount = CalCollectionDataLength(nodeIndex, dataType);
            int dataStartIndex = 0;
            if (elementCount < 0xF)
            {
                dataStartIndex = nodeIndex + 1;
            }
            else
            {
                dataStartIndex = nodeIndex + 2 + (int)Math.Pow(2, _sourceBuffer[nodeIndex + 1] & 0xF);
            }
            for (int i = 0; i < elementCount; i++)
            {
                var keyIndex = TransferToInt(dataStartIndex + i * _objectTableIntegerLength, _objectTableIntegerLength);
                var valueIndex = TransferToInt(dataStartIndex + (i + elementCount) * _objectTableIntegerLength,
                    _objectTableIntegerLength);
                data.Add(
                    ParseObjectData(_trailTable[keyIndex]).ToString(),
                    ParseObjectData(_trailTable[valueIndex]));
            }

            return data;
        }

        #region Utils

        /// <summary>
        /// 拷贝字节数据
        /// </summary>
        /// <param name="sourceData">源数据</param>
        /// <param name="startIndex">源数据起始位置</param>
        /// <param name="length">拷贝字节长度</param>
        /// <param name="data">返回数据</param>
        /// <returns></returns>
        private bool TryCopy(byte[] sourceData, int startIndex, int length, out byte[] data)
        {
            data = new byte[length];
            if (startIndex >= 0 && startIndex <= sourceData.Length - length)
            {
                Array.Copy(sourceData, startIndex, data, 0, length);
                return true;
            }

            return false;
        }

        /// <summary>
        /// 拷贝小端字节数据，未做校验
        /// </summary>
        /// <param name="sourceBuffer"></param>
        /// <param name="startIndex"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        private byte[] CopyLittleEndian(byte[] sourceBuffer, int startIndex, int length)
        {
            var data = new byte[length];
            Array.Copy(sourceBuffer, startIndex, data, 0, length);
            Array.Reverse(data);
            return data;
        }

        /// <summary>
        /// 拷贝大端字节数据，未做校验
        /// </summary>
        /// <param name="startIndex"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        private byte[] CopyBigEndian(int startIndex, int length)
        {
            var data = new byte[length];
            Array.Copy(_sourceBuffer, startIndex, data, 0, length);

            return data;
        }

        /// <summary>
        /// 字节数组转换为int
        /// </summary>
        private int TransferToInt(int startIndex, int length)
        {
            var data = new byte[4];
            Array.Copy(_sourceBuffer, startIndex, data, 4 - length, length);
            Array.Reverse(data);
            return BitConverter.ToInt32(data, 0);
        }

        /// <summary>
        /// 字节数组转换为long
        /// </summary>
        private long TransferToLong(int startIndex, int length)
        {
            var data = new byte[8];
            Array.Copy(_sourceBuffer, startIndex, data, 8 - length, length);
            Array.Reverse(data);
            return BitConverter.ToInt64(data, 0);
        }

        /// <summary>
        /// Apple浮点数时间戳
        /// </summary>
        /// <param name="timestamp"></param>
        /// <returns></returns>
        private DateTime ConvertFromAppleTimeStamp(double timestamp)
        {
            DateTime origin = new DateTime(2001, 1, 1, 0, 0, 0, 0);
            return origin.AddSeconds(timestamp);
        }

        /// <summary>
        /// 计算集合类数据长度
        /// </summary>
        private int CalCollectionDataLength(int nodeIndex, byte dataType)
        {
            int length = dataType & 0xF;
            if (length == 0xF)
            {
                //0xXF后面的一个字节进行2的取幂作为长度
                var extensionLength = (int)ParseInteger(nodeIndex + 1, _sourceBuffer[nodeIndex + 1]);
                return extensionLength;
            }
            else
            {
                return length;
            }
        }

        #endregion

        #region 归档类型解析

        /// <summary>
        /// 解析PList中的归档类型
        /// </summary>
        /// <param name="sourceData">解析出的PList对象</param>
        /// <param name="archiverData">NS归档类型对象</param>
        /// <returns></returns>
        public bool TryParseArchiver(object sourceData, out object archiverData)
        {
            archiverData = null;
            var data = sourceData as Dictionary<string, object>;
            if (data != null && data.ContainsKey("$archiver"))
            {
                try
                {
                    dynamic topData = data["$top"];
                    var rootObjectIndex = topData["root"][0];

                    dynamic objectsData = data["$objects"];
                    var rootObject = objectsData[rootObjectIndex];
                    archiverData = ParseObject(objectsData, rootObject);
                    return true;
                }
                catch
                {
                    return false;
                }
            }

            return false;
        }

        private object ParseObject(dynamic objectsData, dynamic nsObject)
        {
            object nsObjectData = nsObject;
            if (nsObject is IDictionary<string, object> && nsObject.ContainsKey("$class"))
            {
                string nsObjectBaseClass = GetNsObjectBaseClass(objectsData, nsObject);
                switch (nsObjectBaseClass)
                {
                    case "NSDictionary":
                        nsObjectData = NsDictionaryParse(objectsData, nsObject);
                        break;
                    case "NSArray":
                        nsObjectData = NsArrayParse(objectsData, nsObject);
                        break;
                    case "NSObject":
                        nsObjectData = NsObjectParse(objectsData, nsObject);
                        break;
                    case "NSUUID":
                        nsObjectData = NsUuidParse(nsObject);
                        break;
                    case "NSURL":
                        nsObjectData = NsUrlParse(objectsData, nsObject);
                        break;
                    case "NSString":
                        nsObjectData = NsStringParse(nsObject);
                        break;
                    case "NSDecimalNumber":
                        nsObjectData = NsDecimalNumberParse(nsObject);
                        break;
                    default:
                        //未解析类型
                        break;
                }
            }

            return nsObjectData;
        }

        private object NsDecimalNumberParse(dynamic nsObject)
        {
            int number = 0;
            byte[] mantissa = nsObject["NS.mantissa"];
            bool isNegative = nsObject["NS.negative"];
            int exponent = nsObject["NS.exponent"];

            return number;
        }

        /// <summary>
        /// 获取对象的基本数据类型
        /// </summary>
        private string GetNsObjectBaseClass(dynamic objectsData, dynamic nsObject)
        {
            string nsObjectBaseClass = objectsData[nsObject["$class"][0]]["$classes"][0];
            if (!IsNsBaseClass(nsObjectBaseClass))
            {
                nsObjectBaseClass = objectsData[nsObject["$class"][0]]["$classes"][1];
            }

            return nsObjectBaseClass;
        }

        /// <summary>
        /// 基础的NS数据类型
        /// </summary>
        private readonly List<string> _nsBaseClass = new List<string> { "NSDictionary", "NSArray", "NSObject", "NSUUID", "NSURL", "NSString" };

        private bool IsNsBaseClass(string rootObjectClass)
        {
            return _nsBaseClass.Contains(rootObjectClass);
        }

        /// <summary>
        /// NSString解析
        /// </summary>
        /// <param name="rootObject"></param>
        /// <returns></returns>
        private object NsStringParse(dynamic rootObject)
        {
            return rootObject["NS.string"];
        }

        /// <summary>
        /// NSUrl解析
        /// </summary>
        /// <param name="objectsData"></param>
        /// <param name="rootObject"></param>
        /// <returns></returns>
        private object NsUrlParse(dynamic objectsData, dynamic rootObject)
        {
            return objectsData[rootObject["NS.relative"][0]];
        }

        /// <summary>
        /// NSUuid解析
        /// </summary>
        /// <param name="rootObject"></param>
        /// <returns></returns>
        private object NsUuidParse(dynamic rootObject)
        {
            var result = string.Empty;
            foreach (byte b in rootObject["NS.uuidbytes"])
            {
                if (b < 16)
                {
                    result = string.Concat(result, "0", Convert.ToString(b, 16));
                }
                else
                {
                    result = string.Concat(result, Convert.ToString(b, 16));
                }

            }
            return Guid.Parse(result).ToString("B");
        }

        /// <summary>
        /// NSObject解析
        /// </summary>
        /// <param name="objectsData"></param>
        /// <param name="rootObject"></param>
        /// <returns></returns>
        private object NsObjectParse(dynamic objectsData, dynamic rootObject)
        {
            var result = new Dictionary<string, object>();
            foreach (var item in rootObject)
            {
                if (item.Key.Equals("$class"))
                {
                    continue;
                }
                if (item.Value is byte[])
                {
                    result.Add(item.Key, ParseObject(objectsData, objectsData[item.Value[0]]));
                }
                else
                {
                    result.Add(item.Key, item.Value);
                }
            }

            return result;
        }

        /// <summary>
        /// NSArray解析
        /// </summary>
        /// <param name="objectsData"></param>
        /// <param name="rootObject"></param>
        /// <returns></returns>
        private object NsArrayParse(dynamic objectsData, dynamic rootObject)
        {
            var result = new List<object>();
            dynamic objectsIndex = rootObject["NS.objects"];
            for (int i = 0; i < objectsIndex.Count; i++)
            {
                var objectData = ParseObject(objectsData, objectsData[objectsIndex[i][0]]);
                result.Add(objectData);
            }

            return result;
        }

        /// <summary>
        /// NSDictionary解析
        /// </summary>
        /// <param name="objectsData"></param>
        /// <param name="rootObject"></param>
        /// <returns></returns>
        private object NsDictionaryParse(dynamic objectsData, dynamic rootObject)
        {
            var result = new Dictionary<string, object>();
            dynamic keysIndex = rootObject["NS.keys"];
            dynamic objectsIndex = rootObject["NS.objects"];
            for (int i = 0; i < keysIndex.Count; i++)
            {
                var objectData = ParseObject(objectsData, objectsData[objectsIndex[i][0]]);
                result.Add(objectsData[keysIndex[i][0]], objectData);
            }

            return result;
        }

        #endregion

        #endregion

        #region XMLPList

        /// <summary>
        /// 判断是否是XMLplist文件
        /// </summary>
        /// <returns></returns>
        private bool IsXmlPListFile()
        {
            byte[] header;
            if (TryCopy(_sourceBuffer, 0, 5, out header))
            {
                var strHeader = Encoding.UTF8.GetString(header);
                if (strHeader.Equals("<?xml"))
                {
                    return true;
                }
            }
            return false;
        }

        private bool TryReadXmlPListData(out object xplistObject)
        {
            xplistObject = string.Empty;

            XmlDocument xmlDoc = new XmlDocument { XmlResolver = null };
            xmlDoc.Load(new MemoryStream(_sourceBuffer));

            if (xmlDoc.DocumentElement == null || !xmlDoc.DocumentElement.HasChildNodes)
            {
                return false;
            }

            xplistObject = ParseXmlNode(xmlDoc.DocumentElement.ChildNodes[0]);

            return true;
        }

        /// <summary>
        /// 解析数据
        /// </summary>
        private object ParseXmlNode(XmlNode node)
        {
            switch (node.Name)
            {
                case "dict":
                    return ParseXmlDictionary(node);
                case "array":
                    return ParseXmlArray(node);
                case "string":
                    return node.InnerText;
                case "integer":
                    try
                    {
                        return Convert.ToInt32(node.InnerText, System.Globalization.NumberFormatInfo.InvariantInfo);
                    }
                    catch
                    {
                        return Convert.ToInt64(node.InnerText, System.Globalization.NumberFormatInfo.InvariantInfo);
                    }
                case "real":
                    return Convert.ToDouble(node.InnerText, System.Globalization.NumberFormatInfo.InvariantInfo);
                case "false":
                    return false;
                case "true":
                    return true;
                case "null":
                    return null;
                case "date":
                    return XmlConvert.ToDateTime(node.InnerText, XmlDateTimeSerializationMode.Utc);
                case "data":
                    return Convert.FromBase64String(node.InnerText);
            }

            throw new ApplicationException(String.Format("Plist Node `{0}' is not supported", node.Name));
        }

        /// <summary>
        /// 解析字典
        /// </summary>
        private Dictionary<string, object> ParseXmlDictionary(XmlNode node)
        {
            XmlNodeList children = node.ChildNodes;
            if (children.Count % 2 != 0)
            {
                throw new DataMisalignedException("Dictionary elements must have an even number of child nodes");
            }

            Dictionary<string, object> dict = new Dictionary<string, object>();

            for (int i = 0; i < children.Count; i += 2)
            {
                XmlNode keyNode = children[i];
                XmlNode valueNode = children[i + 1];

                if (keyNode.Name != "key")
                {
                    throw new ApplicationException("expected a key node");
                }

                object data = ParseXmlNode(valueNode);

                if (data != null)
                {
                    dict.Add(keyNode.InnerText, data);
                }
            }

            return dict;
        }

        /// <summary>
        /// 解析数组
        /// </summary>
        private List<object> ParseXmlArray(XmlNode node)
        {
            List<object> array = new List<object>();

            foreach (XmlNode child in node.ChildNodes)
            {
                object data = ParseXmlNode(child);
                if (data != null)
                {
                    array.Add(data);
                }
            }

            return array;
        }

        #endregion
    }
}
