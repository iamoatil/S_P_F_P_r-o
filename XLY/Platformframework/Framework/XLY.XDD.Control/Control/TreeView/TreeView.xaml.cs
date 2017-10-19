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
using System.Collections;
using DevExpress.Xpf.Grid;
using System.IO;
using System.Windows.Markup;
using System.Utility.Logger;

namespace XLY.XDD.Control
{
    /// <summary>
    /// TreeView.xaml 的交互逻辑
    /// </summary>
    public partial class TreeView : UserControl
    {
        public TreeView()
        {
            InitializeComponent();

            this.OnCreateGridViewCellTemplateProvider();
        }

        #region Public Interface

        /// <summary>
        /// 刷新数据
        /// </summary>
        public void Refresh()
        {
            this.treeList.RefreshData();
        }

        /// <summary>
        /// 获取选中项
        /// </summary>
        /// <returns></returns>
        public object GetSelectedValue()
        {
            return this.treeList.SelectedItem;
        }

        /// <summary>
        /// 设置选中项
        /// </summary>
        /// <param name="obj">要选中的数据</param>
        public void SetSelectedValue(object obj)
        {
            this.treeList.SelectedItem = obj;
        }

        /// <summary>
        /// 展开某项
        /// </summary>
        /// <param name="obj">要展开的项</param>
        public void Expand(object obj)
        {
            TreeListNode node = this.treeView.GetNodeByContent(obj);
            if (node != null)
                node.IsExpanded = true;
        }

        /// <summary>
        /// 展开某项
        /// </summary>
        /// <param name="rowHandle">行号</param>
        public void Expand(int rowHandle)
        {
            TreeListNode node = this.treeView.GetNodeByRowHandle(rowHandle);
            if (node != null)
                node.IsExpanded = true;
        }

        /// <summary>
        /// 展开所有项
        /// </summary>
        public void ExpandAll()
        {
            this.treeView.ExpandAllNodes();
        }

        #endregion

        #region Domain -- 所属领域对象

        private object _Domain;
        /// <summary>
        /// 所属领域对象
        /// </summary>
        public object Domain
        {
            get { return _Domain; }
            set { _Domain = value; }
        }

        #endregion

        #region IsAutoExpandAllNodes -- 是否自动展开所有的节点

        /// <summary>
        /// 是否自动展开所有的节点
        /// </summary>
        public bool IsAutoExpandAllNodes
        {
            get { return (bool)GetValue(IsAutoExpandAllNodesProperty); }
            set { SetValue(IsAutoExpandAllNodesProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsAutoExpandAllNodes.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsAutoExpandAllNodesProperty =
            DependencyProperty.Register("IsAutoExpandAllNodes", typeof(bool), typeof(TreeView), new UIPropertyMetadata(false));

        #endregion

        #region TreeContextMenu -- 树右键菜单

        /// <summary>
        /// 树右键菜单
        /// </summary>
        public ContextMenu TreeContextMenu
        {
            get { return (ContextMenu)GetValue(TreeContextMenuProperty); }
            set { SetValue(TreeContextMenuProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TreeContextMenu.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TreeContextMenuProperty =
            DependencyProperty.Register("TreeContextMenu", typeof(ContextMenu), typeof(TreeView), new UIPropertyMetadata(null));

        #endregion

        #region ItemTemplate -- 数据项模版

        /// <summary>
        /// 数据项模版
        /// </summary>
        public DataTemplate ItemTemplate
        {
            get { return (DataTemplate)GetValue(ItemTemplateProperty); }
            set { SetValue(ItemTemplateProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ItemTemplate.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ItemTemplateProperty =
            DependencyProperty.Register("ItemTemplate", typeof(DataTemplate), typeof(TreeView), new UIPropertyMetadata(null, new PropertyChangedCallback((s, e) =>
            {
                TreeView tv = s as TreeView;
                DataTemplate dt = e.NewValue as DataTemplate;
                tv.treeList.Columns.Clear();
                TreeListColumn column = new TreeListColumn();
                column.CellTemplate = dt;
                tv.treeList.Columns.Add(column);
            })));

        #endregion

        #region CheckedTemplate -- 选中项的模版

        /// <summary>
        /// 选中项的模版
        /// </summary>
        public DataTemplate CheckedTemplate
        {
            get { return (DataTemplate)GetValue(CheckedTemplateProperty); }
            set { SetValue(CheckedTemplateProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CheckedTemplate.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CheckedTemplateProperty =
            DependencyProperty.Register("CheckedTemplate", typeof(DataTemplate), typeof(TreeView), new UIPropertyMetadata(null));

        #endregion

        #region ColumnsSource -- 列信息

        /// <summary>
        /// 列信息
        /// </summary>
        public IEnumerable ColumnsSource
        {
            get { return (IEnumerable)GetValue(ColumnsSourceProperty); }
            set { SetValue(ColumnsSourceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ColumnsSource.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ColumnsSourceProperty =
            DependencyProperty.Register("ColumnsSource", typeof(IEnumerable), typeof(TreeView), new UIPropertyMetadata(null, new PropertyChangedCallback((s, e) =>
            {
                TreeView treeView = s as TreeView;
                IEnumerable cloumns = e.NewValue as IEnumerable;
                treeView._ResetColumns(cloumns);
            })));

        #endregion

        #region IsAllowSorting -- 是否支持排序

        /// <summary>
        /// 是否支持排序
        /// </summary>
        public bool IsAllowSorting
        {
            get { return (bool)GetValue(IsAllowSortingProperty); }
            set { SetValue(IsAllowSortingProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsAllowSorting.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsAllowSortingProperty =
            DependencyProperty.Register("IsAllowSorting", typeof(bool), typeof(TreeView), new UIPropertyMetadata(false));

        #endregion

        #region IsAutoColumnWidth -- 是否让列自己判断宽度

        /// <summary>
        /// 是否让列自己判断宽度
        /// </summary>
        public bool IsAutoColumnWidth
        {
            get { return (bool)GetValue(IsAutoColumnWidthProperty); }
            set { SetValue(IsAutoColumnWidthProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsAutoColumnWidth.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsAutoColumnWidthProperty =
            DependencyProperty.Register("IsAutoColumnWidth", typeof(bool), typeof(TreeView), new UIPropertyMetadata(true));

        #endregion

        #region HighlightText -- 高亮内容

        /// <summary>
        /// 高亮内容
        /// </summary>
        public string HighlightText
        {
            get { return (string)GetValue(HighlightTextProperty); }
            set { SetValue(HighlightTextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for HighlightText.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HighlightTextProperty =
            DependencyProperty.Register("HighlightText", typeof(string), typeof(TreeView), new UIPropertyMetadata(null));

        #endregion

        #region RowIndent -- 行缩进（如果不设置值，将保留DevExpress 默认设置）

        /// <summary>
        /// 行缩进（如果不设置值，将保留DevExpress 默认设置）
        /// </summary>
        public double RowIndent
        {
            get { return this.treeView.RowIndent; }
            set { this.treeView.RowIndent = value; }
        }

        #endregion

        #region IsUseXlyRowIndent -- 是否使用自定义计算缩进

        /// <summary>
        /// 是否使用自定义计算缩进
        /// </summary>
        public bool IsUseXlyRowIndent
        {
            get { return (bool)GetValue(IsUseXlyRowIndentProperty); }
            set { SetValue(IsUseXlyRowIndentProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsUseXlyRowIndent.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsUseXlyRowIndentProperty =
            DependencyProperty.Register("IsUseXlyRowIndent", typeof(bool), typeof(TreeView), new UIPropertyMetadata(false));

        #endregion

        #region XlyRowIndent -- 用于计算的缩进值

        /// <summary>
        /// 用于计算的缩进值
        /// </summary>
        public double XlyRowIndent
        {
            get { return (double)GetValue(XlyRowIndentProperty); }
            set { SetValue(XlyRowIndentProperty, value); }
        }

        // Using a DependencyProperty as the backing store for XlyRowIndent.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty XlyRowIndentProperty =
            DependencyProperty.Register("XlyRowIndent", typeof(double), typeof(TreeView), new UIPropertyMetadata(20d));

        #endregion

        #region XlyRowPropertyName -- 用于计算缩进的属性名

        /// <summary>
        /// 用于计算缩进的属性名
        /// </summary>
        public string XlyRowPropertyName
        {
            get { return (string)GetValue(XlyRowPropertyNameProperty); }
            set { SetValue(XlyRowPropertyNameProperty, value); }
        }

        // Using a DependencyProperty as the backing store for XlyRowPropertyName.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty XlyRowPropertyNameProperty =
            DependencyProperty.Register("XlyRowPropertyName", typeof(string), typeof(TreeView), new UIPropertyMetadata(string.Empty));

        #endregion

        #region XlyRowIndentOffset -- 缩进修正值

        /// <summary>
        /// 缩进修正值
        /// </summary>
        public double XlyRowIndentOffset
        {
            get { return (double)GetValue(XlyRowIndentOffsetProperty); }
            set { SetValue(XlyRowIndentOffsetProperty, value); }
        }

        // Using a DependencyProperty as the backing store for XlyRowIndentOffset.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty XlyRowIndentOffsetProperty =
            DependencyProperty.Register("XlyRowIndentOffset", typeof(double), typeof(TreeView), new UIPropertyMetadata(0d));

        #endregion

        #region IsShowColumnHeaders -- 是否显示列头

        /// <summary>
        /// 是否显示列头
        /// </summary>
        public bool IsShowColumnHeaders
        {
            get { return (bool)GetValue(IsShowColumnHeadersProperty); }
            set { SetValue(IsShowColumnHeadersProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsShowColumnHeaders.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsShowColumnHeadersProperty =
            DependencyProperty.Register("IsShowColumnHeaders", typeof(bool), typeof(TreeView), new UIPropertyMetadata(true));

        #endregion

        #region IsShowHorizontalLines -- 是否显示水平分割线

        /// <summary>
        /// 是否显示水平分割线
        /// </summary>
        public bool IsShowHorizontalLines
        {
            get { return (bool)GetValue(IsShowHorizontalLinesProperty); }
            set { SetValue(IsShowHorizontalLinesProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsShowHorizontalLines.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsShowHorizontalLinesProperty =
            DependencyProperty.Register("IsShowHorizontalLines", typeof(bool), typeof(TreeView), new UIPropertyMetadata(true));

        #endregion

        #region IsShowVerticalLines -- 是否显示垂直分割线

        /// <summary>
        /// 是否显示垂直分割线
        /// </summary>
        public bool IsShowVerticalLines
        {
            get { return (bool)GetValue(IsShowVerticalLinesProperty); }
            set { SetValue(IsShowVerticalLinesProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsShowVerticalLines.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsShowVerticalLinesProperty =
            DependencyProperty.Register("IsShowVerticalLines", typeof(bool), typeof(TreeView), new UIPropertyMetadata(false));

        #endregion

        #region IsHideCustormExpandWhenHasNoetChildren -- 是否在没有子项的时候隐藏展开按钮

        /// <summary>
        /// 是否在没有子项的时候隐藏展开按钮
        /// </summary>
        public bool IsHideCustormExpandWhenHasNoetChildren
        {
            get { return (bool)GetValue(IsHideCustormExpandWhenHasNoetChildrenProperty); }
            set { SetValue(IsHideCustormExpandWhenHasNoetChildrenProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsHideCustormExpandWhenHasNoetChildren.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsHideCustormExpandWhenHasNoetChildrenProperty =
            DependencyProperty.Register("IsHideCustormExpandWhenHasNoetChildren", typeof(bool), typeof(TreeView), new UIPropertyMetadata(true));

        #endregion

        #region Internal Logic

        private DevExpress.Xpf.Grid.Hierarchy.HierarchyPanel _HierarchyPanel;
        /// <summary>
        /// 可视数据容器
        /// </summary>
        internal DevExpress.Xpf.Grid.Hierarchy.HierarchyPanel HierarchyPanel
        {
            get
            {
                if (_HierarchyPanel == null)
                {
                    Visual datapresenter = XDD.Control.ControlExtension.FindChildrenFromVisualTree<DevExpress.Xpf.Grid.DataPresenter>(this);
                    Visual datapanel = XDD.Control.ControlExtension.FindChildrenFromVisualTree<DevExpress.Xpf.Grid.Hierarchy.HierarchyPanel>(datapresenter);
                    _HierarchyPanel = datapanel as DevExpress.Xpf.Grid.Hierarchy.HierarchyPanel;
                }
                return _HierarchyPanel;
            }
        }

        private void updateWidth()
        {
            if (!this.IsUseXlyRowIndent)
                return;
            this.Dispatcher.BeginInvoke(new EventHandler<EventArgs>((o, a) =>
            {
                try
                {
                    double max_width = 0;
                    foreach (DevExpress.Xpf.Grid.RowControl row in this.HierarchyPanel.Children)
                    {
                        if (!row.IsVisible)
                            continue;
                        DevExpress.Xpf.Grid.TreeList.TreeListRowData rowData = row.DataContext as DevExpress.Xpf.Grid.TreeList.TreeListRowData;
                        object value = rowData.Row.GetType().GetProperty(this.XlyRowPropertyName).GetValue(rowData.Row);
                        FormattedText ft = new FormattedText(value.ToSafeString(),
                                                     System.Globalization.CultureInfo.CurrentUICulture,
                                                     System.Windows.FlowDirection.LeftToRight,
                                                     new Typeface((FontFamily)TextElement.GetFontFamily(this),
                                                                  (FontStyle)TextElement.GetFontStyle(this),
                                                                  (FontWeight)TextElement.GetFontWeight(this),
                                                                  (FontStretch)TextElement.GetFontStretch(this)),
                                                     (double)TextElement.GetFontSize(this),
                                                     (Brush)TextElement.GetForeground(this));
                        double current_width = ft.Width + this.XlyRowIndent * rowData.Level + this.XlyRowIndentOffset;
                        if (max_width < current_width)
                            max_width = current_width;
                    }
                    this.treeList.Columns[0].Width = max_width;
                }
                catch (Exception ex)
                {
                    LogHelper.Error("树展开时出错", ex);
                }
            }), System.Windows.Threading.DispatcherPriority.ApplicationIdle, new object[] { null, null });
        }

        private void treeView_NodeExpanded(object sender, DevExpress.Xpf.Grid.TreeList.TreeListNodeEventArgs e)
        {
            this.updateWidth();
        }

        private void treeView_NodeCollapsed(object sender, DevExpress.Xpf.Grid.TreeList.TreeListNodeEventArgs e)
        {
            this.updateWidth();
        }

        #endregion

        /// <summary>
        /// 单元格模版生成器
        /// </summary>
        private List<IGridViewCellTemplateProvider> _providers = new List<IGridViewCellTemplateProvider>();

        /// <summary>
        /// 创建单元格模版生成器
        /// </summary>
        public virtual void OnCreateGridViewCellTemplateProvider()
        {
            this._providers.Add(new GridViewCellTemplateProvider(this));
            this._providers.Add(new GridViewHighlitCellTemplateProvider(this));
        }

        // 重置列信息
        private void _ResetColumns(IEnumerable cloumns)
        {
            this.treeList.Columns.Clear();
            if (cloumns == null)
                return;
            DataTemplate template = null;
            foreach (IGridViewColumn c in cloumns)
            {
                DevExpress.Xpf.Grid.TreeListColumn column = new DevExpress.Xpf.Grid.TreeListColumn();
                column.Header = c.Header;
                column.Width = c.Width;
                if (c.IsSort && !c.FieldName.IsNullOrEmptyOrWhiteSpace())
                {
                    column.FieldName = c.FieldName;
                }
                DataTemplate temp = null;
                IGridViewCellTemplateProvider _proverder = null;
                foreach (IGridViewCellTemplateProvider proverder in this._providers)
                {
                    temp = proverder.CreateCellTemplate(c);
                    if (temp != null)
                    {
                        template = temp;
                        _proverder = proverder;
                    }
                }
                if (_proverder == null || template == null)
                {
                    throw new Exception("No suitable proverder.");
                }
                column.CellTemplate = template;
                column.Tag = c;
                c.Column = column;
                Binding binding = new Binding();
                binding.Source = c;
                binding.Path = new PropertyPath("IsVisible");
                column.SetBinding(GridColumn.VisibleProperty, binding);
                this.treeList.Columns.Add(column);
            }
        }

        #region ChildNodesSelector -- 树子节点选择器

        /// <summary>
        /// 树子节点选择器
        /// </summary>
        public IChildNodesSelector ChildNodesSelector
        {
            get { return (IChildNodesSelector)GetValue(ChildNodesSelectorProperty); }
            set { SetValue(ChildNodesSelectorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ChildNodesSelector.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ChildNodesSelectorProperty =
            DependencyProperty.Register("ChildNodesSelector", typeof(IChildNodesSelector), typeof(TreeView), new UIPropertyMetadata(null));

        #endregion

        #region ItemsSource -- 数据源

        /// <summary>
        /// 数据源
        /// </summary>
        public IEnumerable ItemsSource
        {
            get { return (IEnumerable)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ItemsSource.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register("ItemsSource", typeof(IEnumerable), typeof(TreeView), new UIPropertyMetadata((d, e) =>
            {

            }));

        #endregion

        #region SelectedItem -- 选中项

        /// <summary>
        /// 选中项
        /// </summary>
        public object SelectedItem
        {
            get { return (object)GetValue(SelectedItemProperty); }
            set { SetValue(SelectedItemProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SelectedItem.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectedItemProperty =
            DependencyProperty.Register("SelectedItem", typeof(object), typeof(TreeView), new UIPropertyMetadata(null));

        #endregion

        #region Event

        /// <summary>
        /// 当选中项发生改变时触发
        /// </summary>
        public event EventHandler<DevExpress.Xpf.Grid.SelectedItemChangedEventArgs> OnSelectedChanged;

        private void treeList_SelectedItemChanged(object sender, DevExpress.Xpf.Grid.SelectedItemChangedEventArgs e)
        {
            if (this.OnSelectedChanged != null)
            {
                this.OnSelectedChanged(this, e);
            }
        }

        /// <summary>
        /// 当节点选择改变时触发
        /// </summary>
        public event EventHandler<DevExpress.Xpf.Grid.TreeList.TreeListNodeChangedEventArgs> OnNodeChanged;

        /// <summary>
        /// 节点改变时触发
        /// </summary>
        private void view_NodeChanged(object sender, DevExpress.Xpf.Grid.TreeList.TreeListNodeChangedEventArgs e)
        {
            if (All_OnNodeChanged != null)
                All_OnNodeChanged(sender, e);
            if (this.OnNodeChanged != null)
                this.OnNodeChanged(sender, e);
        }

        /// <summary>
        /// 全局的节点改变触发事件
        /// </summary>
        public static EventHandler<DevExpress.Xpf.Grid.TreeList.TreeListNodeChangedEventArgs> All_OnNodeChanged;

        /// <summary>
        /// 当双击行时触发
        /// </summary>
        public event EventHandler<RowDoubleClickEventArgs> OnRowDoubleClick;

        private void treeView_RowDoubleClick(object sender, RowDoubleClickEventArgs e)
        {
            if (this.OnRowDoubleClick != null)
            {
                this.OnRowDoubleClick(this, e);
            }

            //而这里，就是处理双击fuck自己的代码
            //元芳，你怎么看
            var n = treeView.GetNodeByRowHandle(e.HitInfo.RowHandle);
            if (n.IsExpanded)
                this.treeView.CollapseNode(n.RowHandle);
            else this.treeView.ExpandNode(n.RowHandle);
        }

        #endregion

        public void ClearTreeView()
        {
            this.treeView.Nodes.Clear();
        }
    }
}
