using System;
using System.Collections.Generic;
using System.Linq;
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
using XLY.SF.Framework.Core.Base.CoreInterface;
using XLY.SF.Framework.Core.Base.MefIoc;
using XLY.SF.Project.DataView;
using XLY.SF.Project.DataView.Preview;
using XLY.SF.Project.Domains;
using XLY.SF.Project.Plugin.Adapter;

namespace _07.XLY.SF.Project.TestModuls
{
    /// <summary>
    /// WindowDisplay.xaml 的交互逻辑
    /// </summary>
    public partial class WindowDisplay : Window
    {
        public WindowDisplay()
        {
            InitializeComponent();

            IocManagerSingle.Instance.LoadParts(this.GetType().Assembly);
            PluginAdapter.Instance.Initialization(new DefaultAsyncProgress());


            var plugin = IocManagerSingle.Instance.GetMetaParts<IPlugin, IMetaPluginType>(PluginExportKeys.PluginScriptKey);
            foreach (var loader in plugin)
            {
                
            }
        }

        TreeDataSource DataSource { get; set; }
        string DB_PATH = @"C:\Users\fhjun\Desktop\test.db";

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            CreateData();
            CreateView();
        }

        private void CreateData()
        {
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
                    accouts.Items.Add(new WeChatFriendShow() { Nick = "昵称" + j, WeChatId = "账号" + j });
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
                        MessageCore msg = new MessageCore() { SenderName = friend.Text, SenderImage= "images/zds.png", Receiver = t.Text, Content = "消息内容" + k, MessageType = k % 4 == 0 ? "图片" : "文本", SendState = EnumSendState.Send };
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
        }
        private void CreateView()
        {
            TreeView tvDef = new TreeView();
            tvDef.ItemsSource = DataSource.TreeNodes;
            ResourceDictionary rd = new ResourceDictionary();
            rd.Source = new Uri("DisplayControls/TreeViewStyle.xaml", UriKind.RelativeOrAbsolute);
            tvDef.Resources.MergedDictionaries.Add(rd);
            rootViewDef.Content = tvDef;
            tvDef.SelectedItemChanged += TvDef_SelectedItemChanged;
        }

        private void TvDef_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            TreeView tv = (TreeView)sender;
            TreeNode node = tv.SelectedValue as TreeNode;
            V_SelectedDataChanged(node);
            var views = DataViewPluginAdapter.Instance.GetView("微信", node.Type);
            tabView1.Items.Clear();
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
            if(obj == null)
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
    }

    public class WeChatFriendShowX : WeChatFriendShow { }

}
