using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using XLY.SF.Framework.BaseUtility;
using XLY.SF.Project.Domains;

/* ==============================================================================
* Assembly   ：	XLY.SF.Project.DataReport.DataSourceToBcpConverter
* Description：	  
* Author     ：	fhjun
* Create Date：	2017/9/28 11:38:33
* ==============================================================================*/

namespace XLY.SF.Project.DataReport
{
    /// <summary>
    /// 将DataSource转换为特定的BCP数据格式
    /// </summary>
    public class DataSourceToBcpConverter
    {
        #region 公共方法
        /// <summary>
        /// 将DataSource转换为Bcp格式的文件保存，返回保存的文件名(C:\data\137-673536061-510000-010000-1499824452-00001.zip)
        /// </summary>
        /// <param name="dataPool"></param>
        /// <param name="destPath">目标文件夹，比如C:\data\</param>
        public string ConverterToBcpFile(DataReportPluginArgument arg, string destPath)
        {
            List<BcpTable> lstBcpTable = ReadAndFilterBcpConfigure(arg.DataPool);
            DataSet ds = new DataSet();

            ds.Tables.AddRange(ConvertToBasicTable(arg));   //添加基本信息表

            foreach (var bcpTable in lstBcpTable)
            {
                IDataSource dataSource = arg.DataPool.FirstOrDefault(im => im.PluginInfo.Name.Trim() == bcpTable.AppName.Trim());

                try
                {
                    List<BcpNode> lstBcpNodes = new List<BcpNode>();
                    var xml = CreateXmlTree(dataSource, lstBcpNodes);     //创建xml树形结构，并将数据集转换为Datatable
                    DataTable dt = BulidBcpTable(bcpTable, lstBcpNodes, xml);
                    if (dt != null)
                    {
                        if (!ds.Tables.Contains(dt.TableName))
                        {
                            ds.Tables.Add(dt);
                        }
                        else
                        {
                            ds.Tables[dt.TableName].Merge(dt);      //如果已经存在该表则合并，比如将QQ的好友列表和微信的好友列表合并为一张表
                        }
                    }
                }
                catch (Exception ex)
                {
                }
            }

            return SaveBcpTables(ds, destPath);
        }
        #endregion

        #region 私有方法
        /// <summary>
        /// BCP配置信息，从配置文件或者从js插件中读取
        /// </summary>
        private static List<BcpTable> LstBcpTableConfigure = null;
        /// <summary>
        /// 转换接口
        /// </summary>
        private static BcpConverter _converter = new BcpConverter();
        /// <summary>
        /// 转换函数
        /// </summary>
        private static Dictionary<string, MethodInfo> _converterFuns = new Dictionary<string, MethodInfo>();
        /// <summary>
        /// 唯一采集目标编号
        /// </summary>
        private string _acquisitionTargetNumber = "";

        #region BCP导出配置
        private const string DATA_FROM = "137";
        private const string DATA_LOCATION = "510000";
        private const string END_CODE = "00001";
        private const string ORG_CODE = "673536061";
        private const string TT_CODE = "010000";
        private const string END_FILE_CODE = "00000";
        private const string LINE_SPLIT_DES = @"\n";
        private const string LINE_SPLIT = "\n";
        private const string COLUMN_SPLIT = "\t";
        private const string COLUMN_SPLIT_DES = @"\t";
        private string UtcTime = "";
        private string INDEX_FILE_NAME = "GAB_ZIP_INDEX.xml";
        #endregion

        #region BCP树生成

        /// <summary>
        /// 生成BCP包所需的采集信息表和设备信息表
        /// </summary>
        /// <param name="info"></param>
        /// <param name="device"></param>
        /// <returns></returns>
        private DataTable[] ConvertToBasicTable(DataReportPluginArgument arg)
        {
            string departNo = arg.CollectionInfo?.CollectLocationCode ?? "";
            string principalNo = arg.CollectionInfo?.CollectorCertificateCode.GetStrictLength(6, '0') ?? "";
            string acqtime = arg.CollectionInfo?.CollectTime ?? "";
            string manufacturerCode = arg.CollectionInfo?.ManufacturerCode ?? "";
            string imei = arg.DeviceInfo?.IMEI ?? "";

            _acquisitionTargetNumber = $"{departNo}{principalNo}{acqtime}{manufacturerCode}{imei}".GetStrictLength(57, '0');    //采集目标编号，唯一

            List<DataTable> tables = new List<DataTable>();
            if (arg.CollectionInfo != null)
            {
                var info = arg.CollectionInfo;
                DataTable infoTable = new DataTable();
                infoTable.TableName = "WA_MFORENSICS_010100";
                infoTable.Columns.Add("I050008");
                infoTable.Columns.Add("C010010");
                infoTable.Columns.Add("G020013");
                infoTable.Columns.Add("B010001");
                infoTable.Columns.Add("B030004");
                infoTable.Columns.Add("B030005");
                infoTable.Columns.Add("H010014");
                infoTable.Columns.Add("E020012");
                infoTable.Columns.Add("B010028");
                infoTable.Columns.Add("G020032");
                infoTable.Columns.Add("I050001");
                infoTable.Columns.Add("I050003");
                infoTable.Columns.Add("I050002");
                DataRow row = infoTable.NewRow();
                row["I050008"] = _acquisitionTargetNumber;
                row["C010010"] = info.ManufacturerName;
                row["G020013"] = info.ManufacturerCode;
                row["B010001"] = info.HolderName;
                row["B030004"] = info.HolderCertificateType;
                row["B030005"] = info.HolderCertificateCode;
                row["H010014"] = new BcpConverter().XlyUTC(info.CollectTime, null);
                row["E020012"] = info.CollectorName;
                row["B010028"] = info.CollectorCertificateCode;
                row["G020032"] = info.CollectLocationCode;
                row["I050001"] = info.CaseCode;
                row["I050003"] = info.CaseType;
                row["I050002"] = info.CaseName;
                infoTable.Rows.Add(row);
                tables.Add(infoTable);
            }

            if (arg.DeviceInfo != null)
            {
                var device = arg.DeviceInfo;
                DataTable deviceTable = new DataTable();
                deviceTable.TableName = "WA_MFORENSICS_010200";
                deviceTable.Columns.Add("I050008");
                deviceTable.Columns.Add("C010001");
                deviceTable.Columns.Add("C050001");
                deviceTable.Columns.Add("C040002");
                deviceTable.Columns.Add("C040006");
                deviceTable.Columns.Add("C010010");
                deviceTable.Columns.Add("C010003");
                deviceTable.Columns.Add("C010009");
                DataRow row1 = deviceTable.NewRow();
                row1["I050008"] = _acquisitionTargetNumber;
                row1["C010001"] = device.Name;
                row1["C050001"] = device.IMEI;
                row1["C040002"] = device.Mac;
                row1["C040006"] = device.BloothMac;
                row1["C010010"] = device.Manufacture;
                row1["C010003"] = device.Model;
                row1["C010009"] = device.FeatureDescription;
                deviceTable.Rows.Add(row1);
                tables.Add(deviceTable);
            }

            return tables.ToArray();
        }

        /// <summary>
        /// 读取BCP配置文件
        /// </summary>
        /// <returns></returns>
        private List<BcpTable> ReadAndFilterBcpConfigure(IEnumerable<IDataSource> dataSourceList)
        {
            if (LstBcpTableConfigure == null)
            {
                //读取配置文件中的配置
                LstBcpTableConfigure = Serializer.DeSerializeFromXML<List<BcpTable>>(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Config\\bcp.cfg.xml"));
            }

            //筛选配置
            List<BcpTable> lstFilter = new List<BcpTable>();
            foreach (var dataSource in dataSourceList)
            {
                DataParsePluginInfo pi = dataSource.PluginInfo as DataParsePluginInfo;
                var bcpDic =
                    LstBcpTableConfigure.Where(bcp => (bcp.Device.IsInvalid() || pi.DeviceOSType.ToString().IsInvalid() || bcp.Device.Contains(pi.DeviceOSType.ToString(), StringComparison.OrdinalIgnoreCase))
                        && bcp.AppName.Trim() == dataSource.PluginInfo.Name.Trim())   //根据系统类型和应用名称匹配，默认为Android
                        .GroupBy(bcp => bcp.GAWACode)
                        .ToDictionary(x => x.First().GAWACode,
                            y =>
                            {
                                List<BcpTable> tempList = new List<BcpTable>();
                                tempList.AddRange(y);
                                return tempList;
                            }); //匹配的所有配置列表，并根据Bcp表格的Code分组，1个Code可以对应多个版本的BCP配置

                if (bcpDic.Count == 0)
                {
                    continue;
                }

                foreach (var pair in bcpDic)
                {
                    List<BcpTable> bcp = new List<BcpTable>();

                    if (pi.VersionStr.IsInvalid() == false)
                    {
                        bcp = pair.Value.Where(b => b.AppVersion == pi.VersionStr).ToList();
                    }

                    if (bcp.IsInvalid())
                    {
                        var bv = pair.Value.Where(b => b.AppVersion.IsInvalid() || b.AppVersion == "*").ToList(); //如果版本号匹配失败，则使用通用的配置
                        if (bv != null)
                        {
                            bcp.AddRange(bv);
                        }
                    }

                    if (bcp.IsInvalid())
                    {
                        var bv = pair.Value.FirstOrDefault(); //如果未定义版本号，则使用配置中的第一个配置；如果定义了版本号但未配置成功，则匹配失败
                        if (bv != null)
                        {
                            bcp.Add(bv);
                        }
                    }

                    if (bcp.IsValid())
                    {
                        lstFilter.AddRange(bcp);
                    }
                    else
                    {

                    }
                }
            }

            return lstFilter;
        }

        private XmlDocument CreateXmlTree(IDataSource dataSource, List<BcpNode> lstBcpNodes)
        {
            XmlDocument xmldoc = new XmlDocument();
            XmlNode rootNode = xmldoc.CreateElement("root");
            xmldoc.AppendChild(rootNode);

            if (dataSource is TreeDataSource treeSource) //多级树节点数据
            {
                CreateXmlTreeNodeFromTreeSource(xmldoc, treeSource.TreeNodes, dataSource.PluginInfo as DataParsePluginInfo, rootNode, lstBcpNodes);
            }
            else   //普通数据
            {
                CreateXmlTreeNodeFromSampleSource(xmldoc, dataSource as SimpleDataSource, dataSource.PluginInfo as DataParsePluginInfo, rootNode, lstBcpNodes);
            }

            return xmldoc;
        }

        /// <summary>
        /// 根据树形节点生成xml
        /// </summary>
        /// <param name="xmldoc"></param>
        /// <param name="nodes"></param>
        /// <param name="current"></param>
        /// <param name="lstBcpNodes"></param>
        private void CreateXmlTreeNodeFromTreeSource(XmlDocument xmldoc, List<TreeNode> nodes, DataParsePluginInfo pluginInfo, XmlNode current, List<BcpNode> lstBcpNodes)
        {
            if (nodes.IsValid())
            {
                foreach (var n in nodes)
                {
                    XmlNode xmlnode = xmldoc.CreateElement("node");     //因为xml节点中不能包含特殊字符（如空格），所以节点定义格式为<node text="我的 好友">
                    XmlAttribute attr = xmldoc.CreateAttribute("text");
                    attr.Value = n.Text;
                    xmlnode.Attributes.Append(attr);
                    current.AppendChild(xmlnode);

                    DataTable dt = null;
                    if (n.Total > 0)
                    {
                        
                        if (n.Type is Type t)
                        {
                            var atts = DisplayAttributeHelper.FindDisplayAttributes(t);
                            if (!atts.IsValid())
                                continue;
                            dt = new DataTable();
                            dt.Columns.Add("I050008");
                            foreach (var a in atts)
                            {
                                dt.Columns.Add(a.Text);
                            }

                            foreach (var i in n.Items.View)
                            {
                                DataRow dr = dt.NewRow();
                                foreach (var a in atts)
                                {
                                    dr[a.Text] = a.GetValue(i);
                                }
                                dt.Rows.Add(dr);
                            }
                        }
                        else
                        {
                            //dt = new DataTable();
                            //dt.Columns.Add("I050008");
                            //Domains.DataView dv = pluginInfo.DataView[0];
                            //for (int i = 0; i < pluginInfo.DataView.Count; i++)
                            //{
                            //    if (pluginInfo.DataView[i].Type == n.Type.ToString())
                            //    {
                            //        dv = pluginInfo.DataView[i];
                            //    }
                            //}
                            //foreach (var model in n.Items.View)
                            //{
                            //    var info = model as DynamicEx;
                            //    foreach (var pair in info.Members)
                            //    {
                            //        string key = GetColumnNameFromDynamicX(dv, pair.Key);
                            //        if (!dt.Columns.Contains(key))
                            //            dt.Columns.Add(key);
                            //    }
                            //    DataRow dr = dt.NewRow();
                            //    foreach (var pair in info.Members)
                            //    {
                            //        string key = GetColumnNameFromDynamicX(dv, pair.Key);
                            //        dr[key] = pair.Value == null ? string.Empty : pair.Value.ToString();
                            //    }
                            //    dt.Rows.Add(dr);
                            //}
                        }
                    }

                    BcpNode bcpNode = new BcpNode() { DataTable = dt, TreeNode = n, XmlNode = xmlnode };
                    lstBcpNodes.Add(bcpNode);

                    if (n.TreeNodes.IsValid())
                    {
                        CreateXmlTreeNodeFromTreeSource(xmldoc, n.TreeNodes, pluginInfo, xmlnode, lstBcpNodes);
                    }
                }
            }
        }
        private string GetColumnNameFromDynamicX(Domains.DataView view, string keyName)
        {
            if (view == null)
            {
                return keyName;
            }
            for (int i = 0; i < view.Items.Count; i++)
            {
                var item = view.Items.FirstOrDefault(s => s.Code == keyName);
                if (item != null)
                {
                    return item.Name;
                }
            }
            return keyName;
        }
        ///// <summary>
        ///// 获取DynamicX属性名对应的列名
        ///// </summary>
        ///// <param name="view"></param>
        ///// <param name="keyName"></param>
        ///// <returns></returns>
        //private string GetColumnNameFromDynamicX(Domains.DataView view, string keyName)
        //{
        //    if (view == null)
        //    {
        //        return keyName;
        //    }
        //    for (int i = 0; i < view.DataViewCollections.Count; i++)
        //    {
        //        var item = view.DataViewCollections[i].FirstOrDefault(s => s.Code == keyName);
        //        if (item != null)
        //        {
        //            return item.Name;
        //        }
        //    }
        //    return keyName;
        //}
        /// <summary>
        /// 根据普通节点生成xml
        /// </summary>
        /// <param name="xmldoc"></param>
        /// <param name="dataList"></param>
        /// <param name="current"></param>
        /// <param name="lstBcpNodes"></param>
        private void CreateXmlTreeNodeFromSampleSource(XmlDocument xmldoc, SimpleDataSource dataSource, DataParsePluginInfo pluginInfo, XmlNode current, List<BcpNode> lstBcpNodes)
        {
            var dataList = dataSource.Items;
            if (dataList.Count == 0)
            {
                return;
            }

            XmlNode xmlnode = xmldoc.CreateElement("node");
            XmlAttribute attr = xmldoc.CreateAttribute("text");
            attr.Value = "root";
            xmlnode.Attributes.Append(attr);
            current.AppendChild(xmlnode);

            DataTable dt = null;

            var atts = DisplayAttributeHelper.FindDisplayAttributes((Type)dataSource.Type);
            if (atts.IsValid())
            {
                dt = new DataTable();
                dt.Columns.Add("I050008");
                foreach (var a in atts)
                {
                    dt.Columns.Add(a.Text);
                }

                foreach (var data in dataList.View)
                {
                    DataRow dr = dt.NewRow();
                    foreach (var a in atts)
                    {
                        dr[a.Text] = a.GetValue(data);
                    }
                    dt.Rows.Add(dr);
                }
            }
            else
            {
                //dt = new DataTable();
                //dt.Columns.Add("I050008");
                //foreach (var model in dataList.View)
                //{
                //    var info = model as DynamicEx;
                //    foreach (var pair in info.Members)
                //    {
                //        if (pair.Key != "XLYLogString")
                //        {
                //            string key = GetColumnNameFromDynamicX(view, pair.Key);
                //            if (!dt.Columns.Contains(key))
                //                dt.Columns.Add(key);
                //        }
                //    }
                //    DataRow dr = dt.NewRow();
                //    foreach (var pair in info.Members)
                //    {
                //        if (pair.Key != "XLYLogString")
                //        {
                //            string key = GetColumnNameFromDynamicX(view, pair.Key);
                //            dr[key] = pair.Value == null ? string.Empty : pair.Value.ToString();
                //        }
                //    }
                //    dt.Rows.Add(dr);
                //}
            }

            BcpNode bcpNode = new BcpNode() { DataTable = dt, TreeNode = null, XmlNode = xmlnode };
            lstBcpNodes.Add(bcpNode);
        }

        /// <summary>
        /// 生成BCP对应的Datatable
        /// </summary>
        /// <param name="bcpTable"></param>
        /// <param name="lstBcpNodes"></param>
        /// <param name="xmldoc"></param>
        /// <returns></returns>
        private DataTable BulidBcpTable(BcpTable bcpTable, List<BcpNode> lstBcpNodes, XmlDocument xmldoc)
        {
            DataTable dt = null;

            var tnodes = ToXmlNodeList(xmldoc, bcpTable.Node);  //获取该bcp表对应的节点
            if (tnodes == null)
                return dt;
            foreach (XmlNode tn in tnodes)
            {
                var sourceNode = lstBcpNodes.FirstOrDefault(bn => bn.XmlNode == tn);
                if (sourceNode != null && sourceNode.DataTable != null)
                {
                    DataTable tmpTable = sourceNode.DataTable.Copy();       //基本数据表

                    foreach (var col in bcpTable.Columns)           //遍历所有配置的列
                    {
                        if (col.FK == BcpForeignKey.None && tmpTable.Columns.Contains(col.Property))  //如果该列在当前表中，则只需要修改列名
                        {
                            int samePropertyCol =
                                bcpTable.Columns.Count(
                                    c => c.FK == BcpForeignKey.None && tmpTable.Columns.Contains(c.Property));
                            if (samePropertyCol <= 1)  //如果未出现相同的列，只需要修改列名
                            {
                                tmpTable.Columns[col.Property].ColumnName = col.GAWACode;
                            }
                            else     //有多个列，则需要复制列数据
                            {
                                tmpTable.Columns.Add(col.GAWACode);
                                foreach (DataRow dr in tmpTable.Rows)
                                {
                                    dr[col.GAWACode] = dr[col.Property];
                                }
                            }
                        }
                        else     //如果该列是外键关联数据，需要找到对应的关联节点获取
                        {
                            if (!col.FKNode.IsValid())   //未设置外键节点则表示为当前节点
                            {
                                col.FKNode = ".";
                            }
                            tmpTable.Columns.Add(col.GAWACode);

                            XmlNode fn = ToXmlNode(tn, col.FKNode);  //外键关联节点
                            if (fn != null)
                            {
                                var fkNode = lstBcpNodes.FirstOrDefault(bn => bn.XmlNode == fn);
                                if (fkNode != null)
                                {
                                    if (col.FK == BcpForeignKey.Merge) //如果没有定义当前表的关联属性，表示直接从关联节点中获取数据并添加
                                    {
                                        if (fkNode.DataTable.Columns.Contains(col.FKProperty))
                                        {
                                            string pk = fkNode.DataTable != null ? fkNode.DataTable.Rows[0][col.FKProperty].ToSafeString() : null;
                                            if (pk.IsValid())
                                            {
                                                foreach (DataRow dr in tmpTable.Rows)
                                                {
                                                    dr[col.GAWACode] = pk;
                                                }
                                            }
                                        }
                                    }
                                    else if (col.FK == BcpForeignKey.NodeText) //该列的数据为TreeNode节点显示的文本
                                    {
                                        string pk = fkNode.TreeNode.Text;
                                        if (pk.IsValid())
                                        {
                                            foreach (DataRow dr in tmpTable.Rows)
                                            {
                                                dr[col.GAWACode] = pk;
                                            }
                                        }
                                    }
                                    else if (col.FK == BcpForeignKey.Constant) //该列的数据为常量数据
                                    {
                                        string pk = GetConstant(bcpTable, lstBcpNodes, col, col.Property);
                                        if (pk.IsValid())
                                        {
                                            foreach (DataRow dr in tmpTable.Rows)
                                            {
                                                dr[col.GAWACode] = pk;
                                            }
                                        }
                                    }
                                    else if (col.FK == BcpForeignKey.Row) //该列的数据为行参数
                                    {
                                        int rowid = 0;
                                        foreach (DataRow dr in tmpTable.Rows)
                                        {
                                            if (col.Property == "rowid")    //rowid表示当前行号，从1开始
                                            {
                                                dr[col.GAWACode] = (++rowid).ToString("D32");
                                            }
                                            else if (tmpTable.Columns.Contains(col.FKProperty))   //其它列的数据复制
                                            {
                                                dr[col.GAWACode] = dr[col.FKProperty];
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        #region 数据转换
                        if (col.ConverterFunction.IsValid())
                        {
                            foreach (DataRow dr in tmpTable.Rows)
                            {
                                object[] param = GetConverterParams(bcpTable, lstBcpNodes, xmldoc, tn, col, tmpTable, dr, col.ConverterParam);
                                dr[col.GAWACode] = ConverterResult(col.ConverterFunction, param, dr[col.GAWACode]);
                            }
                        }
                        #endregion
                    }

                    if (dt == null)
                    {
                        dt = tmpTable;
                    }
                    else
                    {
                        dt.Merge(tmpTable);     //合并表格，比如qq有多个账户时，每次都合并1个账户的好友数据，结果就为全部账户的好友列表数据
                    }
                }
            }

            #region 对表进行后续处理，比如添加ID列，删除其他列等
            if (dt != null)
            {
                //对表数据进行筛选
                if (bcpTable.RowFilter.IsValid())
                {
                    DataRow[] drss = dt.Select(bcpTable.RowFilter);
                    if (!drss.IsValid())
                    {
                        return null;
                    }

                    DataTable tmp = drss[0].Table.Clone();
                    drss.ForEach(row =>
                    {
                        tmp.ImportRow(row);
                    });

                    dt = tmp;
                }

                dt.TableName = bcpTable.GAWACode;         //设置表名

                for (int i = dt.Columns.Count - 1; i >= 0; i--)         //移除其它非BCP列
                {
                    if (!bcpTable.Columns.Any(col => col.GAWACode == dt.Columns[i].ColumnName) && "I050008" != dt.Columns[i].ColumnName)
                    {
                        dt.Columns.RemoveAt(i);
                    }
                }

                foreach (DataRow dr in dt.Rows)  //添加并设置“手机取证采集目标编号”
                {
                    dr["I050008"] = _acquisitionTargetNumber;
                }
            }
            #endregion
            return dt;
        }

        #endregion

        #region BCP文件写入并压缩

        /// <summary>
        /// 将BCP文件保存到本地并压缩，返回文件路径
        /// </summary>
        private string SaveBcpTables(DataSet ds, string destPath)
        {
            UtcTime = ((int)Math.Round(DateTime.Now.ToUtcTime())).ToString();
            string tempPath = Path.Combine(destPath, Guid.NewGuid().ToString());
            if (!Directory.Exists(tempPath))
            {
                Directory.CreateDirectory(tempPath);
            }
            WriteIndexFile(ds, tempPath);
            string zipName = CompressBcpFile(destPath, tempPath);
            return Path.Combine(destPath, zipName);
        }

        private void WriteIndexFile(DataSet ds, string destPath)
        {
            XDocument doc = new XDocument(new XDeclaration("1.0", new UTF8Encoding(false).EncodingName, "yes"),
                new XElement("MESSAGE",
                    new XElement("DATASET",
                        new XAttribute("rmk", "数据文件索引信息"),
                        new XAttribute("ver", "1.0"),
                        new XAttribute("name", "WA_COMMON_010017"),
                        new XElement("DATA",
                            new XElement("DATASET",
                                new XAttribute("rmk", "数据文件索引信息"),
                                new XAttribute("name", "WA_COMMON_010013")))
                    )));
            XElement xele = doc.Element("MESSAGE").Element("DATASET").Element("DATA").Element("DATASET");
            for (int i = 0; i < ds.Tables.Count; i++)
            {
                string bcpFileName = WriteBcpFile(ds.Tables[i], destPath);
                WriteSingleIndexNode(xele, ds.Tables[i], bcpFileName);
            }

            string indexFileName = Path.Combine(destPath, INDEX_FILE_NAME);
            using (StreamWriter sw = new StreamWriter(indexFileName, false, new UTF8Encoding(false)))       //无BOM导出
            {
                doc.Save(sw);
            }
        }

        private void WriteSingleIndexNode(XElement xele, DataTable table, string bcpFileName)
        {
            XElement xedt = new XElement("DATA",
                    new XElement("ITEM", new XAttribute("rmk", "列分隔符"), new XAttribute("val", COLUMN_SPLIT_DES), new XAttribute("key", "I010032")),
                    new XElement("ITEM", new XAttribute("rmk", "行分隔符"), new XAttribute("val", LINE_SPLIT_DES), new XAttribute("key", "I010033")),
                    new XElement("ITEM", new XAttribute("rmk", "数据集代码"), new XAttribute("val", table.TableName), new XAttribute("key", "A010004")),
                    new XElement("ITEM", new XAttribute("rmk", "数据来源"), new XAttribute("val", DATA_FROM), new XAttribute("key", "B050016")),
                    new XElement("ITEM", new XAttribute("rmk", "网安专用产品厂家组织机构代码"), new XAttribute("val", ORG_CODE), new XAttribute("key", "G020013")),
                    new XElement("ITEM", new XAttribute("rmk", "数据采集地"), new XAttribute("val", DATA_LOCATION), new XAttribute("key", "F010008")),
                    new XElement("ITEM", new XAttribute("rmk", "数据起始行"), new XAttribute("val", "1"), new XAttribute("key", "I010038")),
                    new XElement("ITEM", new XAttribute("rmk", "BCP文件编码格式"), new XAttribute("val", "UTF-8无BOM"), new XAttribute("key", "I010039"))
                    );
            xele.Add(xedt);

            XElement xedsf = new XElement("DATASET",
                new XAttribute("rmk", "BCP数据文件信息"),
                new XAttribute("name", "WA_COMMON_010014"),
                new XElement("DATA",
                    new XElement("ITEM", new XAttribute("rmk", "文件路径"), new XAttribute("val", ""), new XAttribute("key", "H040003")),
                    new XElement("ITEM", new XAttribute("rmk", "文件名"), new XAttribute("val", bcpFileName), new XAttribute("key", "H010020")),
                    new XElement("ITEM", new XAttribute("rmk", "记录行数"), new XAttribute("val", "" + table.Rows.Count), new XAttribute("key", "I010034"))
                    )
                );
            xedt.Add(xedsf);
            XElement xedss = new XElement("DATASET",
                new XAttribute("rmk", "BCP文件数据结构"),
                new XAttribute("name", "WA_COMMON_010015"),
                new XAttribute("file", bcpFileName)
                );
            xedt.Add(xedss);
            XElement xedsd = new XElement("DATA");
            xedss.Add(xedsd);

            List<BcpStardardTableItem> Items = new List<BcpStardardTableItem>(BcpStandardFile.Instance[table.TableName].Items.ToArray());

            //此处修改为按照标准文件顺序输出列，因为平台就是按照该顺序读取. 2017/6/5,fhjun
            foreach (BcpStardardTableItem im in Items)
            {
                xedsd.Add(new XElement("ITEM",
                    new XAttribute("key", im.Key),
                    new XAttribute("chn", im.Name),
                    new XAttribute("eng", im.Description),
                    new XAttribute("rmk", im.Memo)
                ));
            }
        }

        /// <summary>
        /// 写入单个的BCP文件，返回该文件的文件名，如137-510000-1507947098-00000-WA_MFORENSICS_010600-0.bcp
        /// </summary>
        /// <param name="table"></param>
        /// <param name="destPath"></param>
        /// <returns></returns>
        private string WriteBcpFile(DataTable table, string destPath)
        {
            string bcpFileName = $"{DATA_FROM}-{DATA_LOCATION}-{UtcTime}-{END_FILE_CODE}-{table.TableName}-0.bcp";
            List<BcpStardardTableItem> Items = new List<BcpStardardTableItem>(BcpStandardFile.Instance[table.TableName].Items.ToArray());
            int col = 0;
            using (StreamWriter sw = new StreamWriter(Path.Combine(destPath, bcpFileName), false, new UTF8Encoding(false)))
            {
                foreach (System.Data.DataRow row in table.Rows)
                {
                    col = 0;
                    foreach (BcpStardardTableItem im in Items)
                    {
                        if (col != 0)       //添加列分隔符
                        {
                            sw.Write(COLUMN_SPLIT);
                        }
                        if (table.Columns.Contains(im.Key))
                        {
                            sw.Write(row[im.Key].ToSafeString().Replace(COLUMN_SPLIT, " ").Replace(LINE_SPLIT, " ").Replace("\r", " "));
                        }
                        col++;
                    }
                    sw.Write(LINE_SPLIT);
                }
            }
            return bcpFileName;
        }

        /// <summary>
        /// 压缩BCP文件，返回文件名，比如137-673536061-510000-010000-1507947098-00001.zip
        /// </summary>
        /// <param name="destPath"></param>
        /// <returns></returns>
        private string CompressBcpFile(string destPath, string tempPath)
        {
            string zipName = $"{DATA_FROM}-{ORG_CODE}-{DATA_LOCATION}-{TT_CODE}-{UtcTime}-{END_CODE}.zip";
            ZipFile.CreateFromDirectory(tempPath, Path.Combine(destPath, zipName));
            Directory.Delete(tempPath, true);
            return zipName;
        }

        #endregion

        #region 辅助类
        /// <summary>
        /// 将配置文件中的路径，转换为xml中的xpath路径
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private static string ToXPath(string path)
        {
            string[] paths = path.Split('/');
            StringBuilder sb = new StringBuilder();
            if (path.StartsWith("//"))
            {
                sb.Append("//");
            }
            else if (path.StartsWith("/"))
            {
                sb.Append("/");
            }
            for (int i = 0; i < paths.Length; i++)
            {
                if (paths[i] == "." || paths[i] == ".." || paths[i] == "*")
                {
                    sb.Append(paths[i]);
                }
                else
                {
                    if (paths[i].Contains("[") && paths[i].Contains("]"))
                    {
                        paths[i] = paths[i].Substring(0, paths[i].IndexOf("["));
                    }
                    sb.AppendFormat("node[@text=\"{0}\"]", paths[i]);
                }

                if (i != paths.Length - 1)
                {
                    sb.Append("/");
                }
            }

            return sb.ToString();
        }

        /// <summary>
        /// 根据配置文件中的路径，返回一个节点（一般为关联节点时调用此方法）
        /// </summary>
        /// <param name="currentNode"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        private static XmlNode ToXmlNode(XmlNode currentNode, string path)
        {
            if (!path.IsValid())
                return currentNode;
            XmlNode xml = currentNode is XmlDocument ? (currentNode as XmlDocument).DocumentElement : currentNode;
            try
            {
                return xml.SelectNodes(ToXPath(path))[0];
            }
            catch (Exception)
            {

            }
            try
            {
                return xml.SelectSingleNode(ToXPath(path));
            }
            catch (Exception)
            {

            }
            return currentNode;
        }

        /// <summary>
        /// 根据配置文件中的路径，返回多个节点（一般为数据节点时调用此方法）
        /// </summary>
        /// <param name="currentNode"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        private static XmlNodeList ToXmlNodeList(XmlNode currentNode, string path)
        {
            if (!path.IsValid())
                return null;
            if (currentNode is XmlDocument)
            {
                return (currentNode as XmlDocument).DocumentElement.SelectNodes(ToXPath(path));
            }
            else
            {
                return currentNode.SelectNodes(ToXPath(path));
            }
        }

        /// <summary>
        /// 转换函数的参数
        /// </summary>
        /// <param name="bcpTable"></param>
        /// <param name="lstBcpNodes"></param>
        /// <param name="xmldoc"></param>
        /// <param name="tn"></param>
        /// <param name="col"></param>
        /// <param name="tmpTable"></param>
        /// <param name="dr"></param>
        /// <param name="paramJson"></param>
        /// <returns></returns>
        private static object[] GetConverterParams(BcpTable bcpTable, List<BcpNode> lstBcpNodes, XmlDocument xmldoc, XmlNode tn, BcpColumn col, DataTable tmpTable, DataRow dr, string paramJson)
        {
            BcpConverterParam[] param = BcpConverterParam.ToConverterParams(paramJson);
            if (param.IsValid())
            {
                object[] result = new object[param.Length];
                int index = 0;
                foreach (BcpConverterParam p in param)
                {
                    object pk = null;
                    if (p.FK == BcpForeignKey.None && tmpTable.Columns.Contains(p.FKProperty)) //当前表中的列
                    {
                        pk = dr[p.FKProperty];
                    }
                    else
                    {
                        XmlNode fn = ToXmlNode(tn, p.FKNode);  //外键关联节点
                        if (fn != null)
                        {
                            var fkNode = lstBcpNodes.FirstOrDefault(bn => bn.XmlNode == fn);
                            if (fkNode != null)
                            {
                                if (p.FK == BcpForeignKey.Merge)
                                {
                                    pk = fkNode.DataTable != null ? fkNode.DataTable.Rows[0][p.FKProperty].ToSafeString() : null;
                                }
                                else if (p.FK == BcpForeignKey.NodeText) //该列的数据为TreeNode节点显示的文本
                                {
                                    pk = fkNode.TreeNode.Text;
                                }
                                else if (p.FK == BcpForeignKey.Constant) //该列的数据为常量数据
                                {
                                    pk = GetConstant(bcpTable, lstBcpNodes, col, p.FKProperty);
                                }
                                else if (p.FK == BcpForeignKey.Row) //该行的属性
                                {
                                    if (p.FKProperty == "rowid")
                                    {
                                        pk = (dr.Table.Rows.IndexOf(dr) + 1).ToString("D32");
                                    }
                                    else
                                    {
                                        if (tmpTable.Columns.Contains(p.FKProperty))
                                            pk = dr[p.FKProperty];
                                    }
                                }
                            }
                        }
                    }

                    result[index++] = pk;
                }

                return result;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 执行数据结果转换
        /// </summary>
        /// <param name="funName"></param>
        /// <param name="currentValue"></param>
        /// <returns></returns>
        private static object ConverterResult(string funName, object[] funParams, object currentValue)
        {
            if (_converterFuns.ContainsKey(funName))
            {
                return _converterFuns[funName].Invoke(_converter, new object[] { currentValue, funParams });
            }
            else
            {
                MethodInfo mi = _converter.GetType().GetMethod(funName);
                if (mi != null)
                {
                    _converterFuns[funName] = mi;
                    return _converterFuns[funName].Invoke(_converter, new object[] { currentValue, funParams });
                }
            }
            return currentValue;
        }

        private static string GetConstant(BcpTable bcpTable, List<BcpNode> lstBcpNodes, BcpColumn col, string key)
        {
            switch (key)
            {
                case "AppName":     //获取应用信息，如"QQ"
                    return bcpTable.AppName;
            }
            return key;
        }
        #endregion

        #endregion

    
    }
}
