using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Collections;
using DevExpress.Xpf.Grid;

namespace XLY.XDD.Control
{
    /// <summary>
    /// 资源管理器
    /// </summary>
    [TemplatePart(Name = "PART_Tree", Type = typeof(TreeView))]
    [TemplatePart(Name = "PART_Grid", Type = typeof(GridView))]
    [TemplatePart(Name = "PART_Up", Type = typeof(Button))]
    [TemplatePart(Name = "PART_Enter", Type = typeof(Button))]
    [TemplatePart(Name = "PART_Cancel", Type = typeof(Button))]
    [TemplatePart(Name = "PART_CreateFloder", Type = typeof(Button))]
    [TemplatePart(Name = "PART_TextBox", Type = typeof(TextBox))]
    [TemplatePart(Name = "PART_FileName", Type = typeof(TextBox))]
    public class ResourceBrowser : System.Windows.Controls.Control
    {
        static ResourceBrowser()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ResourceBrowser), new FrameworkPropertyMetadata(typeof(ResourceBrowser)));
        }

        public static Icon DriverIcon = new Icon(typeof(ResourceBrowser).Assembly.GetManifestResourceStream("XLY.XDD.Control.Icons.drive.ico"));

        #region PART

        private TreeView PART_Tree;
        private GridView PART_Grid;
        private Button PART_Up;
        private Button PART_Enter;
        private Button PART_Cancel;
        private TextBox PART_TextBox;
        private TextBox PART_FileName;
        private ComboBox PART_ComboBox;
        private Button PART_CreateFloder;

        #endregion

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();


            this.PART_Tree = this.Template.FindName("PART_Tree", this) as TreeView;
            this.PART_Grid = this.Template.FindName("PART_Grid", this) as GridView;
            this.PART_TextBox = this.Template.FindName("PART_TextBox", this) as TextBox;
            this.PART_Up = this.Template.FindName("PART_Up", this) as Button;
            this.PART_Enter = this.Template.FindName("PART_Enter", this) as Button;
            this.PART_Cancel = this.Template.FindName("PART_Cancel", this) as Button;
            this.PART_FileName = this.Template.FindName("PART_FileName", this) as TextBox;
            this.PART_ComboBox = this.Template.FindName("PART_ComboBox", this) as ComboBox;
            this.PART_CreateFloder = this.Template.FindName("PART_CreateFloder", this) as Button;
            //this.PART_FileName.IsReadOnly = false;
            if (this.PART_Tree != null)
            {
                this.PART_Tree.OnSelectedChanged -= new EventHandler<DevExpress.Xpf.Grid.SelectedItemChangedEventArgs>(PART_Tree_OnSelectedChanged);
                this.PART_Tree.OnSelectedChanged += new EventHandler<DevExpress.Xpf.Grid.SelectedItemChangedEventArgs>(PART_Tree_OnSelectedChanged);

                this.PART_Tree.OnNodeChanged -= new EventHandler<DevExpress.Xpf.Grid.TreeList.TreeListNodeChangedEventArgs>(PART_Tree_OnNodeChanged);
                this.PART_Tree.OnNodeChanged += new EventHandler<DevExpress.Xpf.Grid.TreeList.TreeListNodeChangedEventArgs>(PART_Tree_OnNodeChanged);
            }

            if (this.PART_Grid != null)
            {
                this.PART_Grid.OnSelectedChanged -= new EventHandler<DevExpress.Xpf.Grid.SelectedItemChangedEventArgs>(PART_Grid_OnSelectedChanged);
                this.PART_Grid.OnSelectedChanged += new EventHandler<DevExpress.Xpf.Grid.SelectedItemChangedEventArgs>(PART_Grid_OnSelectedChanged);

                this.PART_Grid.OnRowDoubleClick -= new EventHandler<DevExpress.Xpf.Grid.RowDoubleClickEventArgs>(PART_Grid_OnRowDoubleClick);
                this.PART_Grid.OnRowDoubleClick += new EventHandler<DevExpress.Xpf.Grid.RowDoubleClickEventArgs>(PART_Grid_OnRowDoubleClick);
            }

            if (this.PART_Up != null)
            {
                this.PART_Up.Click -= new RoutedEventHandler(PART_Up_Click);
                this.PART_Up.Click += new RoutedEventHandler(PART_Up_Click);
            }

            if (this.PART_Enter != null)
            {
                this.PART_Enter.Click -= new RoutedEventHandler(PART_Enter_Click);
                this.PART_Enter.Click += new RoutedEventHandler(PART_Enter_Click);

                if (this.ResourceBrowserType == Control.ResourceBrowserType.SaveFile)
                {
                    this.PART_Enter.Content = "保 存";
                }
            }

            if (this.PART_Cancel != null)
            {
                this.PART_Cancel.Click -= new RoutedEventHandler(PART_Cancel_Click);
                this.PART_Cancel.Click += new RoutedEventHandler(PART_Cancel_Click);
            }

            if (this.PART_TextBox != null)
            {
                this.PART_TextBox.KeyDown -= new KeyEventHandler(PART_TextBox_KeyDown);
                this.PART_TextBox.KeyDown += new KeyEventHandler(PART_TextBox_KeyDown);
            }

            if (this.PART_FileName != null)
            {
                this.PART_FileName.KeyDown -= new KeyEventHandler(PART_FileName_KeyDown);
                this.PART_FileName.KeyDown += new KeyEventHandler(PART_FileName_KeyDown);

                this.PART_FileName.TextChanged -= new TextChangedEventHandler(PART_FileName_TextChanged);
                this.PART_FileName.TextChanged += new TextChangedEventHandler(PART_FileName_TextChanged);

                if (this.ResourceBrowserType == Control.ResourceBrowserType.SaveFile)
                {
                    this.PART_FileName.Text = this.FileName;
                }
            }

            if (this.PART_ComboBox != null)
            {
                this.PART_ComboBox.SelectionChanged -= new SelectionChangedEventHandler(PART_ComboBox_SelectionChanged);
                this.PART_ComboBox.SelectionChanged += new SelectionChangedEventHandler(PART_ComboBox_SelectionChanged);
            }

            if (this.PART_CreateFloder != null)
            {
                this.PART_CreateFloder.Click -= new RoutedEventHandler(PART_CreateFloder_Click);
                this.PART_CreateFloder.Click += new RoutedEventHandler(PART_CreateFloder_Click);

                this.PART_CreateFloder.IsEnabled = false;
            }

            this.Init();
        }

        private void PART_Tree_OnNodeChanged(object sender, DevExpress.Xpf.Grid.TreeList.TreeListNodeChangedEventArgs e)
        {
            if (e.ChangeType == NodeChangeType.Add)
            {
                ResourceBrowserItem item = e.Node.Content as ResourceBrowserItem;
                if (item == null)
                    return;

                if (item.Items.IsInvalid())
                {
                    try
                    {
                        if (System.IO.Directory.GetDirectories(item.FullPath).Any())
                        {
                            e.Node.IsExpandButtonVisible = DevExpress.Utils.DefaultBoolean.True;
                        }
                        else
                        {
                            e.Node.IsExpandButtonVisible = DevExpress.Utils.DefaultBoolean.False;
                        }
                    }
                    catch
                    {
                        e.Node.IsExpandButtonVisible = DevExpress.Utils.DefaultBoolean.True;
                    }
                }
                else
                {
                    e.Node.IsExpandButtonVisible = DevExpress.Utils.DefaultBoolean.True;
                }
            }
        }

        /// <summary>
        /// 当打开时触发
        /// </summary>
        public event EventHandler<ResourceBrowserEventArgs> OnOpen;

        #region DirectoryPath -- 当前指向的目录路径

        /// <summary>
        /// 当前指向的目录路径
        /// </summary>
        public string DirectoryPath
        {
            get { return (string)GetValue(DirectoryPathProperty); }
            set { SetValue(DirectoryPathProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DirectoryPath.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DirectoryPathProperty =
            DependencyProperty.Register("DirectoryPath", typeof(string), typeof(ResourceBrowser), new UIPropertyMetadata(null, new PropertyChangedCallback((s, e) =>
            {
                try
                {
                    ResourceBrowser rb = s as ResourceBrowser;
                    string path = e.NewValue.ToSafeString();
                    if (path.IsNullOrEmptyOrWhiteSpace())
                    {
                        if (rb.PART_Grid != null)
                        {
                            rb.PART_Grid.ItemsSource = null;
                        }
                        return;
                    }
                    if (ResourceBrowserHelper.IsFile(path))
                    {
                        rb._DoOpen(path);
                    }
                    else
                    {
                        if (rb.ResourceBrowserType == ResourceBrowserType.OpenDirectory)
                            return;

                        rb.PART_Grid.ItemsSource = ResourceBrowserHelper.GetDirectorysAndFiles(path, rb._GetSelectFilter());

                        rb.PART_Grid.SetSelectedValue(null);
                    }
                }
                catch (Exception ex)
                {
                    XDD.Control.XlyMessageBox.ShowError(ex.Message);
                }
            })));

        #endregion

        #region DefaultDirectoryPath -- 默认目录路径

        /// <summary>
        /// 默认路径
        /// </summary>
        public IEnumerable<string> DefaultDirectoryPath
        {
            get { return (IEnumerable<string>)GetValue(DefaultDirectoryPathProperty); }
            set { SetValue(DefaultDirectoryPathProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DefaultDirectoryPath.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DefaultDirectoryPathProperty =
            DependencyProperty.Register("DefaultDirectoryPath", typeof(IEnumerable<string>), typeof(ResourceBrowser), new UIPropertyMetadata(null));

        #endregion

        #region IsUserLocalIcon -- 是否使用本地图标

        /// <summary>
        /// 是否使用本地图标
        /// </summary>
        public bool IsUserLocalIcon
        {
            get { return (bool)GetValue(IsUserLocalIconProperty); }
            set { SetValue(IsUserLocalIconProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsUserLocalIcon.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsUserLocalIconProperty =
            DependencyProperty.Register("IsUserLocalIcon", typeof(bool), typeof(ResourceBrowser), new UIPropertyMetadata(false));

        #endregion

        #region IsShieldSystemDrive -- 是否屏蔽系统盘

        /// <summary>
        /// 是否屏蔽系统盘
        /// </summary>
        public bool IsShieldSystemDrive
        {
            get { return (bool)GetValue(IsShieldSystemDriveProperty); }
            set { SetValue(IsShieldSystemDriveProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsShieldSystemDrive.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsShieldSystemDriveProperty =
            DependencyProperty.Register("IsShieldSystemDrive", typeof(bool), typeof(ResourceBrowser), new UIPropertyMetadata(false));

        #endregion

        #region IsShieldCurrentApplicationDrive -- 是否屏蔽当前系统启动的盘符

        /// <summary>
        /// 是否屏蔽当前系统启动的盘符
        /// </summary>
        public bool IsShieldCurrentApplicationDrive
        {
            get { return (bool)GetValue(IsShieldCurrentApplicationDriveProperty); }
            set { SetValue(IsShieldCurrentApplicationDriveProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsShieldCurrentApplicationDrive.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsShieldCurrentApplicationDriveProperty =
            DependencyProperty.Register("IsShieldCurrentApplicationDrive", typeof(bool), typeof(ResourceBrowser), new UIPropertyMetadata(false));

        #endregion

        #region IsShowDesktop -- 是否显示桌面

        /// <summary>
        /// 是否显示桌面
        /// </summary>
        public bool IsShowDesktop
        {
            get { return (bool)GetValue(IsShowDesktopProperty); }
            set { SetValue(IsShowDesktopProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsShowDesktop.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsShowDesktopProperty =
            DependencyProperty.Register("IsShowDesktop", typeof(bool), typeof(ResourceBrowser), new UIPropertyMetadata(true));

        #endregion

        #region ResourceBrowserType -- 资源项类型

        /// <summary>
        /// 资源项类型
        /// </summary>
        public ResourceBrowserType ResourceBrowserType
        {
            get { return (ResourceBrowserType)GetValue(ResourceBrowserTypeProperty); }
            set { SetValue(ResourceBrowserTypeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ResourceBrowserType.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ResourceBrowserTypeProperty =
            DependencyProperty.Register("ResourceBrowserType", typeof(ResourceBrowserType), typeof(ResourceBrowser), new UIPropertyMetadata(ResourceBrowserType.OpenFile));

        #endregion

        #region FileFilter -- 文件筛选

        /// <summary>
        /// 文件筛选
        /// </summary>
        public string FileFilter
        {
            get { return (string)GetValue(FileFilterProperty); }
            set { SetValue(FileFilterProperty, value); }
        }

        // Using a DependencyProperty as the backing store for FileFilter.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FileFilterProperty =
            DependencyProperty.Register("FileFilter", typeof(string), typeof(ResourceBrowser), new UIPropertyMetadata(null, new PropertyChangedCallback((s, e) =>
            {
                ResourceBrowser rb = s as ResourceBrowser;
                List<ResourceBrowserFilterItem> filters = ResourceBrowserHelper.GetFilterItem(e.NewValue.ToSafeString());
                rb.Filters = filters;
            })));

        /// <summary>
        /// 筛选项
        /// </summary>
        public List<ResourceBrowserFilterItem> Filters
        {
            get { return (List<ResourceBrowserFilterItem>)GetValue(FiltersProperty); }
            private set { SetValue(FiltersPropertyKey, value); }
        }

        // Using a DependencyProperty as the backing store for Filters.  This enables animation, styling, binding, etc...
        public static readonly DependencyPropertyKey FiltersPropertyKey =
            DependencyProperty.RegisterAttachedReadOnly("Filters", typeof(List<ResourceBrowserFilterItem>), typeof(ResourceBrowser), new UIPropertyMetadata(ResourceBrowserHelper.GetDefaultFilterItem()));

        public static readonly DependencyProperty FiltersProperty = FiltersPropertyKey.DependencyProperty;

        /// <summary>
        /// 获取选中的筛选项
        /// </summary>
        /// <returns></returns>
        private List<ResourceBrowserFilterItem> _GetSelectFilter()
        {
            List<ResourceBrowserFilterItem> result = new List<ResourceBrowserFilterItem>();
            if (this.PART_ComboBox != null)
            {
                ResourceBrowserFilterItem item = this.PART_ComboBox.SelectedValue as ResourceBrowserFilterItem;
                if (item != null)
                {
                    result.Add(item);
                    return result;
                }
                else
                {
                    return this.Filters;
                }
            }
            else
            {
                return this.Filters;
            }
        }

        #endregion

        #region IsMultipleSelect -- 是否支持多选，该属性在ResourceBrowserType 为 OpenFile 时生效

        /// <summary>
        /// 是否支持多选，该属性在ResourceBrowserType 为 OpenFile 时生效
        /// </summary>
        public bool IsMultipleSelect
        {
            get { return (bool)GetValue(IsMultipleSelectProperty); }
            set { SetValue(IsMultipleSelectProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsMultipleSelect.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsMultipleSelectProperty =
            DependencyProperty.Register("IsMultipleSelect", typeof(bool), typeof(ResourceBrowser), new UIPropertyMetadata());

        #endregion

        #region FileName -- 初始文件名，该属性在ResourceBrowserType 为 SaveFile 时生效

        /// <summary>
        /// 初始文件名，该属性在ResourceBrowserType 为 SaveFile 时生效
        /// </summary>
        public string FileName
        {
            get { return (string)GetValue(FileNameProperty); }
            set { SetValue(FileNameProperty, value); }
        }

        // Using a DependencyProperty as the backing store for FileName.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FileNameProperty =
            DependencyProperty.Register("FileName", typeof(string), typeof(ResourceBrowser), new UIPropertyMetadata(null, new PropertyChangedCallback((s, e) =>
            {
                ResourceBrowser rb = s as ResourceBrowser;
                if (rb.PART_FileName != null)
                {
                    rb.PART_FileName.Text = e.NewValue.ToSafeString();
                }
            })));

        #endregion

        /// <summary>
        /// 新建文件夹
        /// </summary>
        private void PART_CreateFloder_Click(object sender, RoutedEventArgs e)
        {
            if (this.PART_TextBox == null || this.PART_TextBox.Text.IsNullOrEmptyOrWhiteSpace() || !System.IO.Directory.Exists(this.PART_TextBox.Text))
                return;
            string result = string.Empty;
            if (!XDD.Control.XlyMessageBox.ShowInputEx("请输入文件夹名", "新建文件夹", ref result, so =>
                                                                                     {
                                                                                         var mbi =
                                                                                             so as XlyMessageBoxInput;
                                                                                         var s = mbi.InputText;


                                                                                         if (s.Length > mbi.MaxLength)
                                                                                         {
                                                                                             XDD.Control.XlyMessageBox.ShowInfo(string.Format("文件夹名长度不能大于{0}个字符", mbi.MaxLength));
                                                                                             return false;
                                                                                         }

                                                                                         if (s.IsNullOrEmptyOrWhiteSpace())
                                                                                         {
                                                                                             XDD.Control.XlyMessageBox.ShowInfo("请输入文件夹名");
                                                                                             return false;
                                                                                         }
                                                                                         bool isInvalid = (from c in s.ToCharArray()
                                                                                                           from t in System.IO.Path.GetInvalidFileNameChars()
                                                                                                           where c == t
                                                                                                           select c).Any();
                                                                                         if (isInvalid)
                                                                                         {
                                                                                             XDD.Control.XlyMessageBox.ShowInfo("文件夹名不能包含下列任何字符:/\\:*?\"<>|");
                                                                                             return false;
                                                                                         }
                                                                                         string path = System.IO.Path.Combine(this.PART_TextBox.Text, s);
                                                                                         if (System.IO.Directory.Exists(path))
                                                                                         {
                                                                                             XDD.Control.XlyMessageBox.ShowInfo("当前目录下文件夹 " + s + " 已经存在");
                                                                                             return false;
                                                                                         }
                                                                                         return true;
                                                                                     }))
                return;

            string floderPath = System.IO.Path.Combine(this.PART_TextBox.Text, result);
            System.IO.Directory.CreateDirectory(floderPath);
            ResourceBrowserItem selected = this.PART_Tree.GetSelectedValue() as ResourceBrowserItem;

            string temp_text = this.PART_TextBox.Text;

            if (this.IsShowDesktop || this.DefaultDirectoryPath.IsValid())
            {
                PART_CreateFloder_Click_Default(temp_text, floderPath, selected);
            }

            PART_CreateFloder_Click_LogicRoot(temp_text, floderPath, selected);

            this.Refresh_Grid();
        }

        /// <summary>
        /// 创建默认路径或桌面路径下的文件夹
        /// </summary>
        /// <param name="temp_text">要创建文件夹的父目录信息</param>
        /// <param name="floderPath">要创建的目录完整路径</param>
        /// <param name="selected">当前树选中的节点</param>
        private void PART_CreateFloder_Click_Default(string temp_text, string floderPath, ResourceBrowserItem selected)
        {
            foreach (ResourceBrowserItem i in this.PART_Tree.ItemsSource)
            {
                if (!i.IsLogicRoot)
                {
                    ResourceBrowserItem parent = ResourceBrowserHelper.FindItemEx(i, temp_text);

                    if (parent != null)
                    {
                        if (parent.Items == null)
                        {
                            this.PART_Tree.Expand(parent);
                            ResourceBrowserItem item = ResourceBrowserHelper.FindItemEx(i, floderPath);
                            if (ResourceBrowserHelper.IsParent(selected, item))
                            {
                                this.PART_Tree.SetSelectedValue(item);
                            }
                        }
                        else
                        {
                            ResourceBrowserItem item = new ResourceBrowserItem(floderPath, false);
                            this.PART_Tree.Expand(parent);
                            parent.Items.Add(item);
                            item.Parent = parent;
                            if (ResourceBrowserHelper.IsParent(selected, item))
                            {
                                this.PART_Tree.SetSelectedValue(item);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 创建逻辑跟路径下的文件夹
        /// </summary>
        /// <param name="temp_text">要创建文件夹的父目录信息</param>
        /// <param name="floderPath">要创建的目录完整路径</param>
        /// <param name="selected">当前树选中的节点</param>
        private void PART_CreateFloder_Click_LogicRoot(string temp_text, string floderPath, ResourceBrowserItem selected)
        {
            ResourceBrowserItem parent = ResourceBrowserHelper.FindItem(this.PART_Tree.ItemsSource, temp_text);

            if (parent != null)
            {
                if (parent.Items == null)
                {
                    this.PART_Tree.Expand(parent);
                    ResourceBrowserItem item = ResourceBrowserHelper.FindItem(this.PART_Tree.ItemsSource, floderPath);
                    if (ResourceBrowserHelper.IsParent(selected, item))
                    {
                        this.PART_Tree.SetSelectedValue(item);
                    }
                }
                else
                {
                    ResourceBrowserItem item = new ResourceBrowserItem(floderPath, false);
                    parent.Items.Add(item);
                    this.PART_Tree.Expand(parent);
                    item.Parent = parent;
                    if (ResourceBrowserHelper.IsParent(selected, item))
                    {
                        this.PART_Tree.SetSelectedValue(item);
                    }
                }
            }
        }


        /// <summary>
        /// 刷新列表
        /// </summary>
        internal void Refresh_Grid()
        {
            string temp = this.DirectoryPath;
            this.DirectoryPath = null;
            this.DirectoryPath = temp;
        }

        /// <summary>
        /// 筛选条件改变
        /// </summary>
        private void PART_ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ResourceBrowserFilterItem filter = this.PART_ComboBox.SelectedValue as ResourceBrowserFilterItem;
            this.Refresh_Grid();
        }

        /// <summary>
        /// 列表双击
        /// </summary>
        private void PART_Grid_OnRowDoubleClick(object sender, DevExpress.Xpf.Grid.RowDoubleClickEventArgs e)
        {
            this._DoEnter();
        }

        /// <summary>
        /// 树选择改变
        /// </summary>
        private void PART_Tree_OnSelectedChanged(object sender, DevExpress.Xpf.Grid.SelectedItemChangedEventArgs e)
        {
            ResourceBrowserItem item = this.PART_Tree.GetSelectedValue() as ResourceBrowserItem;
            if (item == null)
            {
                this.PART_CreateFloder.IsEnabled = false;
                return;
            }

            this.PART_CreateFloder.IsEnabled = true;

            this.DirectoryPath = item.FullPath;
            if (this.PART_TextBox != null)
            {
                this.PART_TextBox.Text = item.FullPath;
            }
        }

        /// <summary>
        /// 输入框输入内容是触发
        /// </summary>
        private void PART_TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                this.DirectoryPath = this.PART_TextBox.Text;
            }
        }

        /// <summary>
        /// 文件名输入
        /// </summary>
        private void PART_FileName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && !this.PART_FileName.Text.IsNullOrEmptyOrWhiteSpace())
            {
                this._DoEnter();
            }
        }

        /// <summary>
        /// 文件名内容改变时触发
        /// </summary>
        private void PART_FileName_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (this.isMissFileNameTextChanged)
                return;
            this.PART_Grid.SetSelectedValue(null);
        }

        private bool isMissFileNameTextChanged = false;

        private bool isSaveFile = false;

        /// <summary>
        /// 列表选中改变
        /// </summary>
        private void PART_Grid_OnSelectedChanged(object sender, DevExpress.Xpf.Grid.SelectedItemChangedEventArgs e)
        {
            ResourceBrowserItem item = this.PART_Grid.GetSelectedValue() as ResourceBrowserItem;

            if (this.ResourceBrowserType == Control.ResourceBrowserType.SaveFile)
            {
                //this.isSaveFile = true;

                //if (item != null && !item.IsFile)
                //{

                //}

                if (item == null || item.IsFile)
                {
                    this.isSaveFile = true;
                }
                else
                {
                    this.PART_TextBox.Text = item.FullPath;
                    this.isSaveFile = false;
                }

            }

            if (item != null && item.IsFile)
            {
                this.isMissFileNameTextChanged = true;
                this.PART_FileName.Text = item.Token;
                this.isMissFileNameTextChanged = false;
            }
        }

        /// <summary>
        /// 点击取消按钮
        /// </summary>
        private void PART_Cancel_Click(object sender, RoutedEventArgs e)
        {
            Window window = Window.GetWindow(this);
            if (window != null)
            {
                window.Close();
            }
        }

        /// <summary>
        /// 触发打开事件
        /// </summary>
        /// <param name="path">路径</param>
        private void _DoOpen(string path)
        {
            ResourceBrowserEventArgs args = new ResourceBrowserEventArgs();
            args.FullPaths.Add(path);
            this.OnOpen(this, args);
        }

        /// <summary>
        /// 触发打开事件
        /// </summary>
        /// <param name="paths">路径集合</param>
        private void _DoOpen(List<string> paths)
        {
            ResourceBrowserEventArgs args = new ResourceBrowserEventArgs();
            args.FullPaths.AddRange(paths);
            this.OnOpen(this, args);
        }

        /// <summary>
        /// 执行确定按钮
        /// </summary>
        private void _DoEnter()
        {
            if (this.ResourceBrowserType == Control.ResourceBrowserType.OpenDirectory)
            {
                if (this.OnOpen != null)
                {
                    ResourceBrowserItem item = this.PART_Tree.GetSelectedValue() as ResourceBrowserItem;
                    if (item == null)
                        return;
                    this._DoOpen(item.FullPath);
                }
            }
            else if (this.ResourceBrowserType == Control.ResourceBrowserType.OpenFile)
            {
                ResourceBrowserItem single = this.PART_Grid.GetSelectedValue() as ResourceBrowserItem;
                if (single == null)
                    return;
                if (!this.IsMultipleSelect)
                {
                    if (single.IsFile)
                    {
                        this._DoOpen(single.FullPath);
                        return;
                    }
                }
                IList il = this.PART_Grid.GetSelectedValues();
                if (il == null || il.Count == 0)
                    return;
                List<string> paths = new List<string>();
                foreach (ResourceBrowserItem item in il)
                {
                    if (item.IsFile)
                        paths.Add(item.FullPath);
                }
                if (paths.Count > 0)
                {
                    this._DoOpen(paths);
                    return;
                }
                this.DirectoryPath = single.FullPath;
                this.PART_TextBox.Text = single.FullPath;
            }
            else if (this.ResourceBrowserType == Control.ResourceBrowserType.SaveFile)
            {
                if (this.isSaveFile)
                {
                    if (this.PART_FileName.Text.IsNullOrEmptyOrWhiteSpace())
                        return;
                    var isInvlid = (from c in this.PART_FileName.Text.ToCharArray()
                                    from t in System.IO.Path.GetInvalidFileNameChars()
                                    where c == t
                                    select c).Any();
                    if (isInvlid)
                    {
                        XDD.Control.XlyMessageBox.ShowInfo("文件名不能包含下列任何字符:/\\:*?\"<>|");
                    }
                    string path = System.IO.Path.Combine(this.PART_TextBox.Text, this.PART_FileName.Text);
                    if (string.IsNullOrWhiteSpace(path))
                        return;

                    if (System.IO.File.Exists(path))
                    {
                        if (!XLY.XDD.Control.XlyMessageBox.ShowQuestion("文件已经存在，是否覆盖？", "是", "否"))
                        {
                            return;
                        }
                    }

                    this._DoOpen(path);
                }
                else
                {
                    ResourceBrowserItem single = this.PART_Grid.GetSelectedValue() as ResourceBrowserItem;
                    if (single == null)
                        return;
                    if (single.IsFile)
                    {
                        this._DoOpen(single.FullPath);
                    }
                    else
                    {
                        this.DirectoryPath = single.FullPath;
                        this.PART_TextBox.Text = single.FullPath;
                    }
                }

            }
        }

        /// <summary>
        /// 点击确定按钮
        /// </summary>
        private void PART_Enter_Click(object sender, RoutedEventArgs e)
        {
            if (ResourceBrowserType == ResourceBrowserType.SaveFile)
            {
                this.isSaveFile = true;
            }
            this._DoEnter();
        }

        /// <summary>
        /// 点击向上按钮
        /// </summary>
        private void PART_Up_Click(object sender, RoutedEventArgs e)
        {
            if (this.PART_TextBox.Text.IsNullOrEmptyOrWhiteSpace())
                return;

            string path = System.IO.Path.GetDirectoryName(this.PART_TextBox.Text);

            if (path.IsNullOrEmptyOrWhiteSpace())
                return;

            ResourceBrowserItem selected = this.PART_Tree.GetSelectedValue() as ResourceBrowserItem;

            if (selected == null)
                return;

            ResourceBrowserItem root = ResourceBrowserHelper.FindRoot(selected);

            ResourceBrowserItem item = ResourceBrowserHelper.FindItemEx(root, path);

            if (item == null)
            {
                this.DirectoryPath = path;
                this.PART_TextBox.Text = path;
            }
            else
            {
                this.PART_Tree.SetSelectedValue(item);
                this.PART_TextBox.Text = item.FullPath;
                this.DirectoryPath = item.FullPath;
            }
        }

        /// <summary>
        /// 系统盘路径
        /// </summary>
        private string system;
        /// <summary>
        /// 当前执行程序盘符
        /// </summary>
        private string currentExe;
        /// <summary>
        /// 桌面路径
        /// </summary>
        private string desktop;

        /// <summary>
        /// 初始化
        /// </summary>
        private void Init()
        {
            ObservableCollection<ResourceBrowserItem> items = new ObservableCollection<ResourceBrowserItem>();

            system = System.IO.Path.GetPathRoot(System.Environment.GetEnvironmentVariable("windir"));
            currentExe = System.IO.Path.GetPathRoot(AppDomain.CurrentDomain.BaseDirectory);
            desktop = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);

            if (this.DefaultDirectoryPath != null)
            {
                foreach (string path in this.DefaultDirectoryPath)
                {
                    ResourceBrowserItem i = new ResourceBrowserItem(path, false);
                    items.Add(i);
                }
            }

            if (this.IsShowDesktop)
            {
                ResourceBrowserItem i = new ResourceBrowserItem(desktop, false, null, "桌面");
                items.Add(i);
            }

            DriveInfo[] allDrives = DriveInfo.GetDrives();
            foreach (DriveInfo drive in allDrives)
            {
                if (drive.IsReady && (drive.DriveType == DriveType.Fixed || drive.DriveType == DriveType.Removable))
                {
                    if (this.IsShieldSystemDrive && drive.Name.Equals(system))
                        continue;
                    if (this.IsShieldCurrentApplicationDrive && drive.Name.Equals(currentExe))
                        continue;
                    ResourceBrowserItem i = new ResourceBrowserItem(drive.RootDirectory.FullName, false, ResourceBrowser.DriverIcon);
                    i.IsLogicRoot = true;
                    if (drive.VolumeLabel.IsNullOrEmptyOrWhiteSpace())
                    {
                        i.Name = "本地磁盘(" + drive.Name + ")";
                    }
                    else
                    {
                        i.Name = drive.VolumeLabel + "(" + drive.Name + ")";
                    }
                    items.Add(i);
                }
            }

            this.PART_Tree.ItemsSource = items;
        }

        #region Public Static Interface

        /// <summary>
        /// 获取窗口的方法
        /// </summary>
        public static Func<ResourceBrowserType, Window> GetWindow;

        /// <summary>
        /// 打开一个文件对话框
        /// </summary>
        /// <param name="selectPath">选择的路径</param>
        /// <param name="filter">筛选条件</param>
        /// <returns></returns>
        public static bool OpenFileDialog(out string selectPath, string filter = null)
        {
            ResourceBrowser browser = new ResourceBrowser();
            Window window = null;
            if (GetWindow != null)
            {
                window = GetWindow(ResourceBrowserType.OpenFile);
            }
            if (window == null)
            {
                window = new Window();
                window.WindowStyle = WindowStyle.None;
            }
            browser.ResourceBrowserType = ResourceBrowserType.OpenFile;
            browser.IsMultipleSelect = false;
            browser.FileFilter = filter;
            window.Content = browser;
            string result = null;
            browser.OnOpen += (s, e) =>
            {
                result = e.FullPaths.FirstOrDefault();
                window.Close();
            };
            window.ShowDialog();
            selectPath = result;
            return !selectPath.IsNullOrEmptyOrWhiteSpace();
        }

        /// <summary>
        /// 打开一个文件对话框
        /// </summary>
        /// <param name="selectPaths">选中的文件集合</param>
        /// <param name="filter">筛选条件</param>
        /// <returns></returns>
        public static bool OpenFileDialog(out IEnumerable<string> selectPaths, string filter = null)
        {
            ResourceBrowser browser = new ResourceBrowser();
            Window window = null;
            if (GetWindow != null)
            {
                window = GetWindow(ResourceBrowserType.OpenFile);
            }
            if (window == null)
            {
                window = new Window();
                window.WindowStyle = WindowStyle.None;
            }
            browser.ResourceBrowserType = ResourceBrowserType.OpenFile;
            browser.IsMultipleSelect = true;
            browser.FileFilter = filter;
            window.Content = browser;
            List<string> result = null;
            browser.OnOpen += (s, e) =>
            {
                result = e.FullPaths;
                window.Close();
            };
            window.ShowDialog();
            selectPaths = result;
            return !(result == null || result.Count == 0);
        }

        /// <summary>
        /// 打开一个目录选择对话框
        /// </summary>
        /// <param name="selectPath">选择的路径</param>
        /// <returns></returns>
        public static bool OpenDirectoryDialog(out string selectPath)
        {
            ResourceBrowser browser = new ResourceBrowser();
            Window window = null;
            if (GetWindow != null)
            {
                window = GetWindow(ResourceBrowserType.OpenDirectory);
            }
            if (window == null)
            {
                window = new Window();
                window.WindowStyle = WindowStyle.None;
            }
            browser.ResourceBrowserType = ResourceBrowserType.OpenDirectory;
            window.Content = browser;
            string result = null;
            browser.OnOpen += (s, e) =>
            {
                result = e.FullPaths.FirstOrDefault();
                
                window.Close();
            };
            window.ShowDialog();
            selectPath = result;
            
            return !selectPath.IsNullOrEmptyOrWhiteSpace();
        }

        /// <summary>
        /// 打开一个保存文件的对话框
        /// </summary>
        /// <param name="selectPath">选择的路径</param>
        /// <param name="fileName">初始化文件名</param>
        /// <param name="filter">筛选条件</param>
        /// <returns></returns>
        public static bool SaveFileDialog(out string selectPath, string fileName = null, string filter = null)
        {
            ResourceBrowser browser = new ResourceBrowser();
            Window window = null;
            if (GetWindow != null)
            {
                window = GetWindow(ResourceBrowserType.SaveFile);
            }
            if (window == null)
            {
                window = new Window();
                window.WindowStyle = WindowStyle.None;
            }
            browser.ResourceBrowserType = ResourceBrowserType.SaveFile;
            browser.FileFilter = filter;
            browser.FileName = fileName;
            window.Content = browser;
            string result = null;
            browser.OnOpen += (s, e) =>
            {
                result = e.FullPaths.FirstOrDefault();
                if (!System.Utility.Helper.File.InputPathIsValid(result))
                {
                    result = null;
                    XDD.Control.XlyMessageBox.ShowError("请选择的保存路径。");
                    return;
                }
                window.Close();
            };
            window.ShowDialog();
            selectPath = result;
            return !selectPath.IsNullOrEmptyOrWhiteSpace();
        }

        #endregion
    }
}