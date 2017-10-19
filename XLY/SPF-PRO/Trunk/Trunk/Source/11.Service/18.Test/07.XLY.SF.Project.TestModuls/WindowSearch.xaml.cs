using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.SQLite;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using XLY.SF.Framework.BaseUtility;
using XLY.SF.Framework.Core.Base.MefIoc;
using XLY.SF.Project.DataView;
using XLY.SF.Project.DataView.Preview;
using XLY.SF.Project.Domains;
using XLY.SF.Project.Domains.Contract;
using XLY.SF.Project.Plugin.Adapter;

namespace _07.XLY.SF.Project.TestModuls
{
    /// <summary>
    /// WindowSearch.xaml 的交互逻辑
    /// </summary>
    public partial class WindowSearch : Window, INotifyPropertyChanged
    {
        public WindowSearch()
        {
            InitializeComponent();

            ResourceDictionary rd = new ResourceDictionary();
            rd.Source = new Uri("DisplayControls/DataGridStyle.xaml", UriKind.RelativeOrAbsolute);
            this.Resources.MergedDictionaries.Add(rd);

            IocManagerSingle.Instance.LoadParts(this.GetType().Assembly);
            PluginAdapter.Instance.Initialization(new DefaultAsyncProgress());

            _SearchTypes = Enum.GetValues(typeof(FilterEnum));
            this.DataContext = this;

            //TestSql();

        }

        #region 查询条件
        ObservableCollection<FilterArgs> _SearchConditions = new ObservableCollection<FilterArgs>();
        public ObservableCollection<FilterArgs> SearchConditions
        {
            get => _SearchConditions;
            set
            {
                _SearchConditions = value; OnPropertyChanged("SearchConditions");
            }
        }

        Array _SearchTypes = null;
        public Array SearchTypes
        {
            get => _SearchTypes;
            set
            {
                _SearchTypes = value; OnPropertyChanged("SearchTypes");
            }
        }

        FilterEnum _SearchTypeItem = FilterEnum.StringContains;
        public FilterEnum SearchTypeItem
        {
            get => _SearchTypeItem;
            set
            {
                _SearchTypeItem = value; OnPropertyChanged();
                SearchConditionItems.Clear();
                switch (value)
                {
                    case FilterEnum.BookmarkState:
                        SearchConditionItems.Add("标记1");
                        SearchConditionItems.Add("标记2");
                        SearchConditionItems.Add("标记3");
                        break;
                    case FilterEnum.Regex:
                        SearchConditionItems.Add("身份证");
                        SearchConditionItems.Add("网址");
                        SearchConditionItems.Add("手机号码");
                        break;
                    case FilterEnum.FilePath:
                        SearchConditionItems.Add("图片");
                        SearchConditionItems.Add("视频");
                        SearchConditionItems.Add("文档");
                        break;
                    case FilterEnum.EnumState:
                        SearchConditionItems.Add("正常");
                        SearchConditionItems.Add("删除");
                        break;
                }
            }
        }

        string _SearchText = "";
        public string SearchText
        {
            get => _SearchText;
            set
            {
                _SearchText = value; OnPropertyChanged();
            }
        }

        DateTime _StartDate = DateTime.MinValue;
        public DateTime StartDate
        {
            get => _StartDate;
            set
            {
                _StartDate = value; OnPropertyChanged();
            }
        }

        DateTime _EndDate = DateTime.MinValue;
        public DateTime EndDate
        {
            get => _EndDate;
            set
            {
                _EndDate = value; OnPropertyChanged();
            }
        }

        ObservableCollection<object> _SearchConditionItems = new ObservableCollection<object>();
        public ObservableCollection<object> SearchConditionItems
        {
            get => _SearchConditionItems;
            set
            {
                _SearchConditionItems = value; OnPropertyChanged();
            }
        }

        ObservableCollection<object> _SearchConditionItemSels = new ObservableCollection<object>();
        public ObservableCollection<object> SearchConditionItemSels
        {
            get => _SearchConditionItemSels;
            set
            {
                _SearchConditionItemSels = value; OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        /// <summary>
        /// 属性更新（不用给propertyName赋值）
        /// </summary>
        /// <param name="propertyName"></param>
        public void OnPropertyChanged([CallerMemberName]string propertyName = null)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        private void btnAddConditon_Click(object sender, RoutedEventArgs e)
        {
            switch (SearchTypeItem)
            {
                case FilterEnum.StringContains:
                    AddItem(new FilterByStringContainsArgs() { PatternText = SearchText });
                    break;
                case FilterEnum.Regex:
                    AddItem("身份证", FilterByRegexArgs.RegexIDCard);
                    AddItem("网址", FilterByRegexArgs.RegexUrl);
                    AddItem("手机号码", FilterByRegexArgs.RegexPhone);
                    break;
                case FilterEnum.BookmarkState:
                    AddItem("标记1", new FilterByBookmarkArgs() { BookmarkId = 1 });
                    AddItem("标记2", new FilterByBookmarkArgs() { BookmarkId = 2 });
                    AddItem("标记3", new FilterByBookmarkArgs() { BookmarkId = 3 });
                    break;
                case FilterEnum.FilePath:
                    break;
                case FilterEnum.DateTimeRange:
                    AddItem(new FilterByDateRangeArgs() { StartTime = StartDate, EndTime = EndDate });
                    break;
                case FilterEnum.Account:
                    AddItem(new FilterByAccountArgs() { AccountPattern = SearchText });
                    break;
                case FilterEnum.EnumState:
                    AddItem("正常", new FilterByEnumStateArgs() { State = EnumDataState.Normal });
                    AddItem("删除", new FilterByEnumStateArgs() { State = EnumDataState.Deleted });
                    break;
            }
        }

        private void AddItem(object selItem, FilterArgs arg)
        {
            if (lbox1.SelectedItems.Contains(selItem) && !SearchConditions.Any(s=>s.ToString() == arg.ToString()))
            {
                SearchConditions.Add(arg);
            }
        }

        private void AddItem(FilterArgs arg)
        {
            var s = SearchConditions.FirstOrDefault(i => i.FilterType == arg.FilterType);
            if(s != null)
            {
                SearchConditions.Remove(s);
            }
            SearchConditions.Add(arg);
        }

        private void BtnStartSearch_Click(object sender, RoutedEventArgs e)
        {
            Search();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            SearchConditions.Remove(lsboxFilter.SelectedItem as FilterArgs);
        }

        #endregion

        #region 数据展示
        TreeDataSource DataSource { get; set; }
        SimpleDataSource CallDataSource { get; set; }
        List<IDataSource> DataPool { get; set; } = new List<IDataSource>();
        const string DB_PATH = @"C:\Users\fhjun\Desktop\test1.db";
        const string DB_PATH2 = @"C:\Users\fhjun\Desktop\test2.db";
        const string JS_PATH = @"C:\Users\fhjun\Desktop\test";

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            CreateData();
            CreateView();
        }

        private void CreateData()
        {
            #region 生成树对象
            DataSource = new TreeDataSource();
            DataSource.TreeNodes = new List<TreeNode>();
            for (int i = 0; i < 2; i++)
            {
                TreeNode t = new TreeNode();
                t.Text = "账号" + i;
                DataSource.TreeNodes.Add(t);

                TreeNode accouts = new TreeNode();
                accouts.Text = "好友列表";
                accouts.IsHideChildren = true;
                t.TreeNodes.Add(accouts);
                accouts.Type = typeof(WeChatFriendShow);
                accouts.Items = new DataItems<WeChatFriendShow>(DB_PATH);
                for (int j = 0; j < 10; j++)
                {
                    accouts.Items.Add(new WeChatFriendShow() { Nick = "昵称" + j, WeChatId = "账号" + j});
                }

                TreeNode accouts2 = new TreeNode();
                accouts2.Text = "聊天记录";
                accouts2.IsHideChildren = i % 2 == 0;
                accouts2.Type = typeof(WeChatFriendShowX);
                accouts2.Items = new DataItems<WeChatFriendShowX>(DB_PATH);
                t.TreeNodes.Add(accouts2);
                for (int j = 0; j < 5; j += 2)
                {
                    accouts2.Items.Add(new WeChatFriendShowX() { Nick = "昵称" + j, WeChatId = "账号" + j });
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
            DataSource.BuildParent();
            DataPool.Add(DataSource);
            #endregion

            DateTime? t1 = DateTime.Now.AddDays(111);
            string s = t1.ToString();
            DateTime? t2 = null;
            string s2 = t2.ToString();
            string s23 = t2.GetValueOrDefault().ToString();

            #region SimpleDataSource
            CallDataSource = new SimpleDataSource();
            CallDataSource.Items = new DataItems<Call>(DB_PATH2);
            for(int i = 0;i< 10; i++)
            {
                CallDataSource.Items.Add(new Call() { DurationSecond = 10000, EndDate = DateTime.Now.AddDays(i + 0.1), Name = "张三_" + i, Number = "10086" });
            }
            
            CallDataSource.BuildParent();
            DataPool.Add(CallDataSource);
            #endregion

            #region 序列化

            //Serializer.SerializeToBinary(DataSource, JS_PATH);

            //var ds2 = Serializer.DeSerializeFromBinary<IDataSource>(JS_PATH);
            //var dsss = (ds2 as TreeDataSource).TreeNodes[0].TreeNodes[0].Items;
            //foreach (var i in dsss)
            //{

            //}

            //System.IO.File.WriteAllText(JS_PATH, Serializer.JsonSerilize(DataSource));
            //var ds2 = Serializer.JsonDeserilize<TreeDataSource>(System.IO.File.ReadAllText(JS_PATH));
            //ds2.BuildParent();
            //var dsss = ds2.TreeNodes[0].TreeNodes[0].Items;
            //foreach (var i in dsss)
            //{

            //}

            //int iddd = 1;
            //foreach (var d in DataPool)
            //{
            //    System.IO.File.WriteAllText($"{JS_PATH}{iddd++}.js", Serializer.JsonSerilize(d));
            //}

            //List<IDataSource> dss = new List<IDataSource>();
            //iddd = 1;
            //foreach (var d in DataPool)
            //{
            //    var a = Serializer.JsonDeserilize<IDataSource>(System.IO.File.ReadAllText($"{JS_PATH}{iddd++}.js"), false);
            //    a.BuildParent();
            //    dss.Add(a);
            //}

            //foreach (var d in dss)
            //{
            //    var items = d is TreeDataSource ? (d as TreeDataSource).TreeNodes[0].TreeNodes[0].Items : (d as SimpleDataSource).Items;
            //    foreach (var i in items)
            //    {

            //    }
            //}
            #endregion
        }
        private void CreateView()
        {
            if(rootViewDef.Content != null)
            {
                _treeSelectPath = (rootViewDef.Content as TreeView).SelectedValuePath;
                rootViewDef.Content = null;
            }
            
            TreeView tvDef = new TreeView();
            tvDef.ItemsSource = DataSource.TreeNodes;
            ResourceDictionary rd = new ResourceDictionary();
            rd.Source = new Uri("DisplayControls/TreeViewStyle.xaml", UriKind.RelativeOrAbsolute);
            tvDef.Resources.MergedDictionaries.Add(rd);
            rootViewDef.Content = tvDef;
            tvDef.SelectedItemChanged += TvDef_SelectedItemChanged;
            tvDef.SelectedValuePath = _treeSelectPath;
        }

        private string _treeSelectPath = string.Empty;

        private void TvDef_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            TreeView tv = (TreeView)sender;
            TreeNode node = tv.SelectedValue as TreeNode;
            V_SelectedDataChanged(node);
            var views = DataViewPluginAdapter.Instance.GetView("微信", node.Type);
            tabView1.Items.Clear();
            GC.Collect();
            DataViewPluginArgument arg = new DataViewPluginArgument() { DataSource = DataSource, CurrentNode = node };
            foreach (var v in views)
            {
                v.SelectedDataChanged += V_SelectedDataChanged;
                TabItem ti = new TabItem() { Header = v.PluginInfo.Name };
                ti.Content = v.GetControl(arg);
                tabView1.Items.Add(ti);
            }
            tabView1.SelectedIndex = views.Count() > 0 ? 0 : -1;
        }

        private void V_SelectedDataChanged(object obj)
        {
            tbPre.Items.Clear();
            GC.Collect();
            if (obj == null)
            {
                return;
            }
            var views = DataPreviewPluginAdapter.Instance.GetView("微信", obj.GetType());
            DataPreviewPluginArgument arg = new DataPreviewPluginArgument() { DataSource = DataSource, CurrentNode = null, Item = obj };
            foreach (var v in views)
            {
                TabItem ti = new TabItem() { Header = v.PluginInfo.Name };
                ti.Content = v.GetControl(arg);
                tbPre.Items.Add(ti);
            }
            tbPre.SelectedIndex = views.Count() > 0 ? 0 : -1;
        }
        #endregion

        #region 查询
        private void Search()
        {
            #region 使用abstractitem
            //var filter0 = _SearchConditions.FirstOrDefault(s => s.FilterType == FilterEnum.StringContains) as FilterByStringContainsArgs;
            //if (filter0 == null)
            //{
            //    return;
            //}
            //DataSource.Traverse(
            //    node => true,
            //    item =>
            //    {
            //        item.IsVisible = item.FilterByStringContains(filter0);
            //        return true;
            //    });
            #endregion

            #region 使用sql
            DataSource.Filter(_SearchConditions.ToArray());
            #endregion

            CreateView();
        }
        #endregion

        private void TestSql()
        {
            List<long> times = new List<long>();
            Stopwatch t = new Stopwatch();

            #region 生成数据
            t.Restart();
            int count = 10000;
            List<Call> lstcall = new List<Call>();
            for (int i = 0; i < count; i++)
            {
                lstcall.Add(new Call() { DurationSecond = 10000, EndDate = DateTime.Now.AddDays(i + 0.1), Name = "张三_" + i, Number = "10086" });
            }
            lstcall.ConvertAll(l => l.MD5);
            times.Add(t.ElapsedMilliseconds);
            #endregion

            #region 原插入算法

            //t.Restart();
            //using (var conn = new SQLiteConnection(@"Data Source='C:\Users\fhjun\Desktop\insert.db'"))
            //{
            //    conn.Open();
            //    StringBuilder sbc = new StringBuilder();
            //    sbc.AppendFormat("CREATE TABLE IF NOT EXISTS  {0}(", "t_call");
            //    sbc.AppendFormat("{0} CHAR(50) NOT NULL", "Key");
            //    var diss = DisplayAttributeHelper.FindDisplayAttributes(typeof(Call));
            //    foreach (var dis in diss)
            //    {
            //        sbc.AppendFormat(",{0} {1}", dis.Key, dis.DataType);
            //    }
            //    sbc.AppendFormat(",{0} TEXT", "XLY_Json");
            //    sbc.Append(");");
            //    using (var com = new SQLiteCommand(conn))
            //    {
            //        com.CommandText = sbc.ToString();
            //        com.ExecuteNonQuery();
            //    }

            //    var tras = conn.BeginTransaction();
            //    for (int i = 0; i < lstcall.Count; i++)
            //    {
            //        StringBuilder sb = new StringBuilder();
            //        sb.AppendFormat("INSERT INTO {0} VALUES('{1}'", "t_call", "11111111111111111111111111111");

            //        foreach (var dis in diss)
            //        {
            //            sb.AppendFormat(",'{0}'", dis.GetValue(lstcall[i]));
            //        }
            //        sb.AppendFormat(",'{0}'", Serializer.JsonSerilize(lstcall[i]));
            //        sb.Append(");");

            //        using (var com = new SQLiteCommand(conn))
            //        {
            //            com.CommandText = sb.ToString();
            //            com.ExecuteNonQuery();
            //        }
            //    }
            //    tras.Commit();
            //}
            //t.Stop();
            //times.Add(t.ElapsedMilliseconds);

            #endregion

            #region 使用SQLiteParameter插入
            t.Restart();
            using (var conn = new SQLiteConnection(@"Data Source='C:\Users\fhjun\Desktop\insert_param.db'"))
            {
                conn.Open();
                StringBuilder sbc = new StringBuilder();
                sbc.AppendFormat("CREATE TABLE IF NOT EXISTS {0}(", "t_call");
                sbc.AppendFormat("{0} CHAR(50) NOT NULL", "Key");
                var diss = DisplayAttributeHelper.FindDisplayAttributes(typeof(Call));
                foreach (var dis in diss)
                {
                    sbc.AppendFormat(",{0} {1}", dis.Key, dis.DataType);
                }
                sbc.AppendFormat(",{0} TEXT", "XLY_Json");
                sbc.Append(");");

                using (var com = new SQLiteCommand(conn))
                {
                    com.CommandText = sbc.ToString();
                    com.ExecuteNonQuery();
                }

                using (var com = new SQLiteCommand(conn))
                {
                    StringBuilder sql = new StringBuilder();
                    sql.AppendFormat("INSERT INTO {0} VALUES('{1}'", "t_call", "11111111111111111111111111111");
                    foreach (var dis in diss)
                    {
                        sql.AppendFormat(",@{0}", dis.Owner.Name);
                        com.Parameters.Add(new SQLiteParameter($"@{dis.Owner.Name}"));
                    }
                    sql.AppendFormat(",@XLY_Json");
                    com.Parameters.Add(new SQLiteParameter($"@XLY_Json"));
                    sql.Append(");");
                    com.CommandText = sql.ToSafeString();
                    com.Prepare();

                    var tras = conn.BeginTransaction();
                    for (int i = 0; i < lstcall.Count; i++)
                    {
                        foreach (var dis in diss)
                        {
                            com.Parameters[$"@{dis.Owner.Name}"].Value = dis.GetValue(lstcall[i]);
                        }
                        com.Parameters[$"@XLY_Json"].Value = Serializer.JsonSerilize(lstcall[i]);

                        com.ExecuteNonQuery();
                    }
                    tras.Commit();
                }
            }
            t.Stop();
            times.Add(t.ElapsedMilliseconds);
            #endregion

            MessageBox.Show(string.Join(",", times));
        }
    }
}
