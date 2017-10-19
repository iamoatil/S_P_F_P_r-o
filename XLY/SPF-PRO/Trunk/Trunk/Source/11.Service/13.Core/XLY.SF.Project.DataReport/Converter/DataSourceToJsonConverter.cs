using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using XLY.SF.Framework.BaseUtility;
using XLY.SF.Project.Domains;

/* ==============================================================================
* Assembly   ：	XLY.SF.Project.DataReport.DataSourceConverter
* Description：	  
* Author     ：	fhjun
* Create Date：	2017/9/28 11:38:33
* ==============================================================================*/

namespace XLY.SF.Project.DataReport
{
    /// <summary>
    /// 将DataSource转换为特定的数据格式
    /// </summary>
    public class DataSourceToJsonConverter
    {
        /// <summary>
        /// 将DataSource转换为Json格式的文件保存
        /// </summary>
        /// <param name="dataPool"></param>
        /// <param name="destPath">目标文件夹，比如C:\data\</param>
        public void ConverterToJsonFile(DataReportPluginArgument arg, string destPath)
        {
            CreateDeviceInfo(arg.DeviceInfo, destPath);
            CreateCollectionInfo(arg.CollectionInfo, destPath);
            CreateJson(arg.DataPool, destPath);
        }

        private void CreateJson(IList<IDataSource> dataPool, string destPath)
        {
            string treePath = Path.Combine(destPath, @"tree.js");

            List<JsonExportTree> tree = new List<JsonExportTree>();
            foreach (IDataSource ds in dataPool)
            {
                if (ds == null)
                {
                    continue;
                }

                JsonExportTree t = new JsonExportTree() { text = ds.PluginInfo.Name, location = "", icon = ds.PluginInfo.Icon ?? "", tags = new string[] { ds.Total.ToString() } };

                if (ds is TreeDataSource td)
                {
                    CreateTreeNodeJson(td.TreeNodes, destPath, t);
                }
                else if (ds is SimpleDataSource sd)
                {
                    CreateItemJson(sd.Items, destPath, t);
                }
                tree.Add(t);
            }
            System.IO.File.WriteAllText(treePath, $"var __data = {Serializer.JsonFastSerilize(tree)};");
        }

        /// <summary>
        /// 保存设备信息
        /// </summary>
        /// <param name="deviceInfo"></param>
        /// <param name="destPath"></param>
        private void CreateDeviceInfo(ExportDeviceInfo deviceInfo, string destPath)
        {
            string path = Path.Combine(destPath, @"device.js");
            System.IO.File.WriteAllText(path, $"var __device = {Serializer.JsonFastSerilize(deviceInfo ?? new object())};");
        }

        /// <summary>
        /// 保存采集信息
        /// </summary>
        /// <param name="collectionInfo"></param>
        /// <param name="destPath"></param>
        private void CreateCollectionInfo(ExportCollectionInfo collectionInfo, string destPath)
        {
            string path = Path.Combine(destPath, @"collect.js");
            System.IO.File.WriteAllText(path, $"var __collect = {Serializer.JsonFastSerilize(collectionInfo ?? new object())};");
        }

        private void CreateTreeNodeJson(List<TreeNode> nodes, string path, JsonExportTree t)
        {
            if (nodes == null)
            {
                return;
            }
            if (nodes.Count > 0 && t.nodes == null)
            {
                t.nodes = new List<JsonExportTree>();
            }
            foreach (TreeNode n in nodes)
            {
                JsonExportTree t0 = (new JsonExportTree() { text = n.Text, location = "", icon = t.icon, tags = new string[] { n.Total.ToString() } });
                CreateItemJson(n.Items, path, t0, (Type)n.Type);
                CreateTreeNodeJson(n.TreeNodes, path, t0);
                t.nodes.Add(t0);
            }
        }

        private void CreateItemJson(IDataItems items, string dir, JsonExportTree t, Type itemType = null)
        {
            if (items == null)
            {
                return;
            }
            t.location = items.Key;
            string path = Path.Combine(dir, $"{items.Key}.js"); ;       // 文件：\data\3bd9a209-cdaf-42ab-b232-1aa4636f5a17.js
            using (StreamWriter sw = new StreamWriter(path, false, Encoding.UTF8))
            {
                #region 生成数据json
                sw.Write("var __data = [");
                int r = 0;
                //items.Filter();
                foreach (var c in items.View)
                {
                    if (r != 0)
                        sw.Write(",");
                    sw.Write(Serializer.JsonSerilize(c));
                    r++;
                }
                sw.Write("];");
                #endregion

                #region 生成列属性json
                if (itemType == null)            //如果没有传入类型，则根据泛型参数类型来获取
                {
                    if (items.GetType().IsGenericType)
                    {
                        itemType = items.GetType().GetGenericArguments()[0];
                    }
                    else
                    {
                        throw new Exception("暂时先不处理的类型问题");
                    }
                }
                sw.Write("var __columns = ");
                List<JsonExportColumn> cols = new List<JsonExportColumn>();
                foreach (var c in DisplayAttributeHelper.FindDisplayAttributes(itemType))
                {
                    if (c.Visibility != EnumDisplayVisibility.ShowInDatabase)
                        cols.Add(new JsonExportColumn() { field = c.Key, title = c.Text });
                }
                sw.Write(Serializer.JsonFastSerilize(cols));
                sw.Write(";");
                #endregion
            }
        }
    }
}
