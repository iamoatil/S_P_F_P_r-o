using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using XLY.SF.Project.Domains;
using System.Collections.Generic;
using XLY.SF.Project.Domains.Contract;
using System.Linq;
using System.Text.RegularExpressions;
using XLY.SF.Project.DataFilter.Views;

namespace XLY.SF.UnitTest
{
    [TestClass]
    public class DataFilterTest
    {
        [TestMethod]
        public void TestKeywordsFilter()
        {
            KeyValuePair<String, String>[] kvs = new KeyValuePair<String, String>[]
            {
                new KeyValuePair<String, String>("Content", "成功" ),
                new KeyValuePair<String, String>("Type", "检测" ),
            };
            var dataSource = CreateDataSource();
            IEnumerable<dynamic> result = dataSource[1].Items.FilterByKeywords<dynamic>(out Int32 count, kvs);
            Console.WriteLine($"----{count}----");
            foreach (var a in result)
            {
                Console.WriteLine(a);
            }
            ((IDisposable)result).Dispose();
        }

        [TestMethod]
        public void TestRegexFilter()
        {
            KeyValuePair<String, String>[] kvs = new KeyValuePair<String, String>[]
            {
                new KeyValuePair<String, String>("Content", "^请求更新*.$" ),
                new KeyValuePair<String, String>("Type", "^*.检测$" ),
            };
            var dataSource = CreateDataSource();
            IEnumerable<dynamic> result = dataSource[1].Items.FilterByRegex<dynamic>(out Int32 count, kvs);
            Console.WriteLine($"----{count}----");
            foreach (var a in result)
            {
                Console.WriteLine(a);
            }
            ((IDisposable)result).Dispose();
        }

        [TestMethod]
        public void TestAggregationFilter()
        {
            var dataSource = CreateDataSource();
            FilterByStringContainsArgs arg1 = new FilterByStringContainsArgs { PatternText = "9" };
            FilterByRegexArgs arg2 = new FilterByRegexArgs { Regex = new Regex(@"^(\d{3,4}-)?\d{6,8}$") };
            FilterByDateRangeArgs arg3 = new FilterByDateRangeArgs { StartTime = DateTime.Now, EndTime = DateTime.Now.AddDays(1) };
            FilterByEnumStateArgs arg4 = new FilterByEnumStateArgs { State = EnumDataState.Deleted };
            FilterByBookmarkArgs arg5 = new FilterByBookmarkArgs { BookmarkId = 10 };
            var result = dataSource[1].Filter<AbstractDataItem>(arg1, arg2);
            foreach (Call item in result.OfType<Call>())
            {
                Console.WriteLine($"{item.Name}--{item.Number}--{item.StartDate}--{item.DurationSecond}");
            }
        }


        private List<IDataSource> CreateDataSource()
        {
            string DB_PATH = @"F:\Temp\test1.db";
            List<IDataSource> dataPool = new List<IDataSource>();
            TreeDataSource treeDataSource = new TreeDataSource();
            SimpleDataSource callDataSource = new SimpleDataSource();

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
                    accouts.Items.Add(new WeChatFriendShow() { Nick = "昵称" + j, WeChatId = "账号" + j });
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
                        MessageCore msg = new MessageCore() { SenderName = friend.Text, SenderImage = "images/zds.png", Receiver = t.Text, Content = "消息内容" + k, MessageType = k % 4 == 0 ? "图片" : "文本", SendState = EnumSendState.Send };
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

            callDataSource.Items = new DataItems<Call>(DB_PATH);
            for (int i = 0; i < 10; i++)
            {
                callDataSource.Items.Add(new Call() { DurationSecond = 10000, EndDate = DateTime.Now.AddDays(i + 0.1), Name = "张三_" + i, Number = "10086" });
            }

            callDataSource.BuildParent();
            dataPool.Add(callDataSource);

            return dataPool;
        }
    }
}
