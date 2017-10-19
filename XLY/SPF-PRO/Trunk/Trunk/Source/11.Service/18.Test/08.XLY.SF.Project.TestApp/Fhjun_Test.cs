using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XLY.SF.Framework.BaseUtility;
using XLY.SF.Project.BaseUtility.Helper;
using XLY.SF.Project.Domains;

/* ==============================================================================
* Assembly   ：	XLY.SF.Project.TestApp.Fhjun_Test
* Description：	  
* Author     ：	fhjun
* Create Date：	2017/9/26 13:41:09
* ==============================================================================*/

namespace XLY.SF.Project.TestApp
{
    /// <summary>
    /// Fhjun_Test
    /// </summary>
    public class Fhjun_Test
    {
        #region 测试Html报表导出

        class ExportTree
        {
            public string text { get; set; }
            public string location { get; set; }
            public string[] tags { get; set; }
            public string icon { get; set; }
            public List<ExportTree> nodes { get; set; }
        }

        class ExportColumn
        {
            public string field { get; set; }
            public string title { get; set; }
            public bool sortable { get; set; } = true;
        }


        private List<IDataSource> CreateDataSource()
        {
            string DB_PATH = @"C:\Users\fhjun\Desktop\test1.db";
            List<IDataSource> dataPool = new List<IDataSource>();
            TreeDataSource treeDataSource = new TreeDataSource();
            SimpleDataSource callDataSource = new SimpleDataSource();

            treeDataSource.PluginInfo = new DataParsePluginInfo() { Name = "微信", Guid = "11FC356E-3EA6-481F-ACF6-D96925F80A4C" };
            treeDataSource.TreeNodes = new List<TreeNode>();
            for (int i = 0; i < 2; i++)
            {
                TreeNode t = new TreeNode();
                t.Text = "账号" + i;
                treeDataSource.TreeNodes.Add(t);

                TreeNode accouts = new TreeNode();
                accouts.Text = "好友列表";
                accouts.IsHideChildren = true;
                t.TreeNodes.Add(accouts);
                accouts.Type = typeof(WeChatFriendShow);
                accouts.Items = new DataItems<WeChatFriendShow>(DB_PATH);
                for (int j = 0; j < 10; j++)
                {
                    accouts.Items.Add(new WeChatFriendShow() { Nick = "昵称" + j, WeChatId = "账号" + j, Gender = EnumSex.Female });
                }

                TreeNode accouts2 = new TreeNode();
                accouts2.Text = "聊天记录";
                accouts2.IsHideChildren = i % 2 == 0;
                accouts2.Type = typeof(WeChatFriendShow);
                accouts2.Items = new DataItems<WeChatFriendShow>(DB_PATH);
                t.TreeNodes.Add(accouts2);
                for (int j = 0; j < 5; j += 2)
                {
                    accouts2.Items.Add(new WeChatFriendShow() { Nick = "昵称" + j, WeChatId = "账号" + j });
                    TreeNode friend = new TreeNode();
                    friend.Text = "昵称" + j;
                    friend.Type = typeof(MessageCore);
                    friend.Items = new DataItems<MessageCore>(DB_PATH);
                    accouts2.TreeNodes.Add(friend);

                    for (int k = 0; k < 100; k++)
                    {
                        MessageCore msg = new MessageCore() { SenderName = friend.Text, SenderImage = "images/zds.png", Receiver = t.Text, Content = "发送信息内容" + k, MessageType = k % 4 == 0 ? "图片" : "文本", SendState = EnumSendState.Send };
                        friend.Items.Add(msg);
                        MessageCore msg2 = new MessageCore() { Receiver = friend.Text, SenderImage = "images/zjq.png", SenderName = t.Text, Content = "返回消息内容" + k, MessageType = k % 5 == 0 ? "图片" : "文本", SendState = EnumSendState.Receive };
                        friend.Items.Add(msg2);
                    }
                }

                TreeNode accouts3 = new TreeNode();
                accouts3.Text = "群消息";
                accouts3.IsHideChildren = true;
                t.TreeNodes.Add(accouts3);

                TreeNode accouts4 = new TreeNode();
                accouts4.Text = "发现";
                accouts4.IsHideChildren = true;
                t.TreeNodes.Add(accouts4);
            }
            treeDataSource.BuildParent();
            dataPool.Add(treeDataSource);

            callDataSource.PluginInfo = new DataParsePluginInfo() { Name = "短信", Guid = "DDDC356E-3EA6-481F-ACF6-D96925F80EEE" };
            callDataSource.Items = new DataItems<Call>(DB_PATH);
            for (int i = 0; i < 10; i++)
            {
                callDataSource.Items.Add(new Call() { DurationSecond = 10000, EndDate = DateTime.Now.AddDays(i + 0.1), Name = "张三_" + i, Number = "10086" });
            }

            callDataSource.BuildParent();
            dataPool.Add(callDataSource);

            return dataPool;
        }

        private void CreateJson(List<IDataSource> dataPool)
        {
            string destPath = @"C:\Users\fhjun\Desktop\html\data\";
            string JS_PATH = destPath + "tree.js";

            List<ExportTree> tree = new List<ExportTree>();
            foreach (IDataSource ds in dataPool)
            {
                if(ds == null)
                {
                    continue;
                }

                ExportTree t = new ExportTree() { text = ds.PluginInfo.Name, location = "", icon = ds.PluginInfo.Icon??"", tags = new string[]{ "0" } };

                if (ds is TreeDataSource td)
                {
                    CreateJson(td.TreeNodes, destPath, t);
                }
                else if(ds is SimpleDataSource sd)
                {
                    CreateItemJson(sd.Items, destPath, t, typeof(Call));
                }
                tree.Add(t);
            }
            System.IO.File.WriteAllText(JS_PATH, $"var __data = {Serializer.JsonFastSerilize(tree)};");
        }

        private void CreateJson(List<TreeNode> nodes, string path, ExportTree t)
        {
            if(nodes == null)
            {
                return;
            }
            if(nodes.Count > 0 && t.nodes == null)
            {
                t.nodes = new List<ExportTree>();
            }
            foreach(TreeNode n in nodes)
            {
                ExportTree t0 = (new ExportTree() { text = n.Text, location = "", icon = t.icon, tags = new string[] { "0" } });
                CreateItemJson(n.Items, path, t0, (Type)n.Type);
                CreateJson(n.TreeNodes, path, t0);
                t.nodes.Add(t0);
            }
        }

        private void CreateItemJson(IDataItems items, string dir, ExportTree t, Type itemType = null)
        {
            if(items == null)
            {
                return;
            }
            t.location = items.Key;
            string path = $"{dir}{items.Key}.js";
            using (StreamWriter sw = new StreamWriter(path, false, Encoding.UTF8))
            {
                sw.Write("var __data = [");
                int r = 0;
                items.Filter();
                foreach (var c in items.View)
                {
                    if(r != 0)
                        sw.Write(",");
                    sw.Write(Serializer.JsonSerilize(c));
                    r++;
                }
                sw.Write("];");

                sw.Write("var __columns = ");
                List<ExportColumn> cols = new List<ExportColumn>();
                foreach(var c in DisplayAttributeHelper.FindDisplayAttributes(itemType))
                {
                    if(c.Visibility != EnumDisplayVisibility.ShowInDatabase)
                        cols.Add(new ExportColumn() { field = c.Key , title = c.Text });
                }
                sw.Write(Serializer.JsonFastSerilize(cols));
                sw.Write(";");
            }
        }

        public void TestHtmlReport()
        {
            Console.WriteLine("开始测试测试Html报表导出...");

            FileHelper.CopyDirectory(@"D:\Report\任务-2016-06-20-18-05-53_6", @"C:\Users\fhjun\Desktop\html22");
            var dataPool = CreateDataSource();
            CreateJson(dataPool);

            Console.WriteLine("json序列化完成...");
        }
        #endregion
    }
}
