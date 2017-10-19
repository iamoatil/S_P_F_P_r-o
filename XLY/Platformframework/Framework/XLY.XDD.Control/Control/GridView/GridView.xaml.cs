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
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows.Markup;
using System.IO;
using System.Threading;
using System.Windows.Controls.Primitives;
using DevExpress.Xpf.Grid;
using System.Collections.Concurrent;

namespace XLY.XDD.Control
{
    /// <summary>
    /// 列表视图
    /// </summary>
    public partial class GridView : UserControl
    {
        public GridView()
        {
            InitializeComponent();

            this.OnCreateGridViewCellTemplateProvider();

            this.SelectedItems = this.grid.SelectedItems;
        }

        #region Public Interface

        /// <summary>
        /// 清理排序
        /// </summary>
        public void ClearSorting()
        {
            this.grid.ClearSorting();
        }

        /// <summary>
        /// 刷新数据
        /// </summary>
        public void Refresh()
        {
            try
            {
                this.grid.RefreshData();
            }
            catch
            {
            }

        }

        /// <summary>
        /// 获取选中的项
        /// </summary>
        /// <returns></returns>
        public object GetSelectedValue()
        {
            return this.grid.SelectedItem;
        }

        /// <summary>
        /// 获取选中的项集合
        /// </summary>
        /// <returns></returns>
        public IList GetSelectedValues()
        {
            return this.grid.SelectedItems;
        }

        /// <summary>
        /// 设置选中项
        /// </summary>
        /// <param name="obj">要选中的数据</param>
        public void SetSelectedValue(object obj)
        {
            this.grid.SelectedItem = obj;
        }

        /// <summary>
        /// 获取当前控件的Dev列表控件
        /// </summary>
        /// <returns></returns>
        public GridControl GetGridControl()
        {
            return this.grid;
        }

        #endregion

        /// <summary>
        /// 控件GridControl
        /// </summary>
        public GridControl GridControl { get { return this.grid; } }

        #region GridContextMenu -- 列表右键菜单

        /// <summary>
        /// 列表右键菜单
        /// </summary>
        public ContextMenu GridContextMenu
        {
            get { return (ContextMenu)GetValue(GridContextMenuProperty); }
            set { SetValue(GridContextMenuProperty, value); }
        }

        // Using a DependencyProperty as the backing store for GridContextMenu.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty GridContextMenuProperty =
            DependencyProperty.Register("GridContextMenu", typeof(ContextMenu), typeof(GridView), new UIPropertyMetadata(null));

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
            DependencyProperty.Register("SelectedItem", typeof(object), typeof(GridView), new UIPropertyMetadata(null));

        #endregion

        #region SelectedItems -- 选中项的集合

        /// <summary>
        /// 选中项的集合
        /// </summary>
        public IList SelectedItems
        {
            get { return (IList)GetValue(SelectedItemsProperty); }
            private set { SetValue(SelectedItemsPropertyKey, value); }
        }

        // Using a DependencyProperty as the backing store for SelectedItems.  This enables animation, styling, binding, etc...
        public static readonly DependencyPropertyKey SelectedItemsPropertyKey =
            DependencyProperty.RegisterReadOnly("SelectedItems", typeof(IList), typeof(GridView), new UIPropertyMetadata(null));

        public static readonly DependencyProperty SelectedItemsProperty = SelectedItemsPropertyKey.DependencyProperty;

        #endregion

        #region ItemsSource -- 数据源
        private IEnumerable _itemsSource;

        public void ClearPreSearchData()
        {
            _itemsSource = null;
            if (ColumnsSource != null)
                (ColumnsSource as IList).Clear();
        }

        public bool _isSearchSource = false;
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
            DependencyProperty.Register("ItemsSource", typeof(IEnumerable), typeof(GridView), new UIPropertyMetadata(null, new PropertyChangedCallback((s, e) =>
            {
                GridView grid = s as GridView;
                var newImtes = (e.NewValue as IEnumerable<object>);
                if (newImtes == null) return;
                var first = newImtes.FirstOrDefault();
                if (!grid._isSearchSource)
                {
                    if (first == null || (first != null && IsSubClassOfTypeString(first.GetType(), "XLY.DF.Domain.DFTreeNode")))
                        grid._itemsSource = newImtes;
                }


                grid.distinct = null;

            }), new CoerceValueCallback((s, e) =>
            {
                GridView grid = s as GridView;

                if (e is IEnumerable)
                {
                    grid.isMissSelectionChanged = true;

                    return e;
                }
                return null;
            })));

        /// <summary>
        /// 判断某个类是否是<see cref="baseTypeString"/>类型的子类
        /// </summary>
        /// <param name="type">要判断的类型</param>
        /// <param name="baseTypeString">父类类型全名称字符串</param>
        /// <returns>是否是<see cref="baseTypeString"/>的子类</returns>
        private static bool IsSubClassOfTypeString(Type type, string baseTypeString)
        {
            if (type == null)
                return false;

            if (type.FullName.ToLower() == baseTypeString.ToLower())
                return true;

            Type baseType = type.BaseType;
            if (null != baseType)
                return IsSubClassOfTypeString(baseType, baseTypeString);

            return false;
        }

        #endregion

        #region IsSelectedFirstWhenItemsSourceChanged -- 当数据源改变时是否自动选中第一项（尚不完善）

        /// <summary>
        /// 当数据源改变时是否自动选中第一项（尚不完善）
        /// </summary>
        public bool IsSelectedFirstWhenItemsSourceChanged
        {
            get { return (bool)GetValue(IsSelectedFirstWhenItemsSourceChangedProperty); }
            set { SetValue(IsSelectedFirstWhenItemsSourceChangedProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsSelectedFirstWhenItemsSourceChanged.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsSelectedFirstWhenItemsSourceChangedProperty =
            DependencyProperty.Register("IsSelectedFirstWhenItemsSourceChanged", typeof(bool), typeof(GridView), new UIPropertyMetadata(true));

        /// <summary>
        /// 控制是否错过选择改变
        /// </summary>
        private bool isMissSelectionChanged;

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
            DependencyProperty.Register("ColumnsSource", typeof(IEnumerable), typeof(GridView), new UIPropertyMetadata(null, new PropertyChangedCallback((s, e) =>
            {
                GridView gridView = s as GridView;
                IEnumerable cloumns = e.NewValue as IEnumerable;
                gridView._ResetColumns(cloumns);
            })));

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
            DependencyProperty.Register("HighlightText", typeof(string), typeof(GridView), new UIPropertyMetadata(null));

        #endregion

        #region SearchText -- 搜索关键字

        /// <summary>
        /// 搜索关键字
        /// </summary>
        public string SearchText
        {
            get { return (string)GetValue(SearchTextProperty); }
            set { SetValue(SearchTextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SearchText.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SearchTextProperty =
            DependencyProperty.Register("SearchText", typeof(string), typeof(GridView), new UIPropertyMetadata(null, new PropertyChangedCallback(OnSearchTextChanged)));


        public static void OnSearchTextChanged(DependencyObject s, DependencyPropertyChangedEventArgs e)
        {
            string searchStr = e.NewValue.ToString().ToLower();
            GridView grid = s as GridView;
            if (string.IsNullOrEmpty(searchStr))
            {
                grid._isSearchSource = false;
                grid.ItemsSource = grid._itemsSource;
                return;
            }
            System.Windows.Window loading = XDD.Control.XlyMessageBox.ShowLoading("正在搜索，请稍等...");
            //grid.Loading.IsShow = true;
            //Thread.Sleep(1000);
            System.Threading.Tasks.Task.Factory.StartNew(gridv =>
            {
                var datas = gridv as IEnumerable<object>;
                ConcurrentBag<object> result = new ConcurrentBag<object>();
                if (datas != null && datas.Any())
                {
                    object tempp = null;
                    try
                    {
                        Type type = datas.First().GetType();
                        var propertys = type.GetProperties().AsParallel();
                        System.Threading.Tasks.Parallel.ForEach(datas, new System.Threading.Tasks.ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount }, n =>
                        {
                            foreach (var item in propertys)
                            {
                                try
                                {
                                    tempp = item.GetValue(n);
                                    if (tempp != null && tempp.ToString().ToLower().Contains(searchStr))
                                    {
                                        result.Add(n);
                                        break;
                                    }
                                }
                                catch (Exception ex)
                                {
                                }
                            }
                        });
                    }
                    catch (AggregateException ae)
                    {
                    }

                }
                grid.Dispatcher.Invoke((Action<GridView, ConcurrentBag<object>>)((gd, re) =>
                {
                    loading.Close();
                    gd._isSearchSource = true;
                    //var gdr = (gd.ItemsSource as IList);
                    //gdr.Clear();
                    //foreach (var item in re)
                    //{
                    //    gdr.Add(item);
                    //}
                    gd.ItemsSource = re;
                }), grid, result);
            }, grid._itemsSource, System.Threading.Tasks.TaskCreationOptions.PreferFairness);
            /**/
            //ThreadPool.QueueUserWorkItem(t =>
            //{
            //GridView grid = s as GridView;
            //grid.tableView.SearchString = string.Empty;
            //grid.tableView.SearchString = e.NewValue.ToString();
            //grid.HighlightText = e.NewValue.ToString();
            //}, null);
            //grid.Loading.IsShow = false;
        }
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
            DependencyProperty.Register("IsAutoColumnWidth", typeof(bool), typeof(GridView), new UIPropertyMetadata(false));

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
            DependencyProperty.Register("IsShowColumnHeaders", typeof(bool), typeof(GridView), new UIPropertyMetadata(true));

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
            DependencyProperty.Register("IsShowHorizontalLines", typeof(bool), typeof(GridView), new UIPropertyMetadata(true));

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
            DependencyProperty.Register("IsShowVerticalLines", typeof(bool), typeof(GridView), new UIPropertyMetadata(false));

        #endregion

        #region IsRowDistinct -- 是否需要消除重复的项，该属性适用于数据量较小时使用，如果数据量较大，那么请自行实现数据去重

        /// <summary>
        /// 是否需要消除重复的项，该属性适用于数据量较小时使用，如果数据量较大，那么请自行实现数据去重
        /// </summary>
        public bool IsRowDistinct
        {
            get { return (bool)GetValue(IsRowDistinctProperty); }
            set { SetValue(IsRowDistinctProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsRowDistinct.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsRowDistinctProperty =
            DependencyProperty.Register("IsRowDistinct", typeof(bool), typeof(GridView), new UIPropertyMetadata(false, new PropertyChangedCallback((s, e) =>
            {
                GridView gridView = s as GridView;
                gridView.Refresh();
            })));

        #endregion

        /// <summary>
        /// 单元格模版生成器
        /// </summary>
        protected List<IGridViewCellTemplateProvider> _providers = new List<IGridViewCellTemplateProvider>();

        /// <summary>
        /// 创建单元格模版生成器
        /// </summary>
        public virtual void OnCreateGridViewCellTemplateProvider()
        {
            this._providers.Add(new GridViewCellTemplateProvider(this));
            this._providers.Add(new GridViewHighlitCellTemplateProvider(this));
        }

        /// <summary>
        /// 添加单元格模版生成器
        /// </summary>
        /// <param name="provider">列样式生成器</param>
        public void AddCellTemplateProvider(IGridViewCellTemplateProvider provider)
        {
            this._providers.Add(provider);
        }

        /// <summary>
        /// 移除单元格模版生成器
        /// </summary>
        /// <param name="provider">列样式生成器</param>
        public void RemoveCellTemplateProvider(IGridViewCellTemplateProvider provider)
        {
            this._providers.Remove(provider);
        }

        /// <summary>
        /// 清理单元格模版生成器
        /// </summary>
        public void ClearCellTemplateProvider()
        {
            this._providers.Clear();
        }

        // 重置列信息
        private void _ResetColumns(IEnumerable cloumns)
        {
            this.grid.Columns.Clear();
            if (cloumns == null)
                return;
            DataTemplate template = null;
            foreach (IGridViewColumn c in cloumns)
            {
                DevExpress.Xpf.Grid.GridColumn column = new DevExpress.Xpf.Grid.GridColumn();
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
                this.grid.Columns.Add(column);
            }
        }

        #region Event

        /// <summary>
        /// 当列表项选中改变时触发
        /// </summary>
        public event EventHandler<DevExpress.Xpf.Grid.SelectedItemChangedEventArgs> OnSelectedChanged;

        private void grid_SelectedItemChanged(object sender, DevExpress.Xpf.Grid.SelectedItemChangedEventArgs e)
        {
            if (this.OnSelectedChanged != null)
            {
                if (this.isMissSelectionChanged && !this.IsSelectedFirstWhenItemsSourceChanged)
                {
                    this.isMissSelectionChanged = false;
                    return;
                }
                this.OnSelectedChanged(this, e);
            }
        }

        /// <summary>
        /// 当列表双击行时触发双击行
        /// </summary>
        public event EventHandler<DevExpress.Xpf.Grid.RowDoubleClickEventArgs> OnRowDoubleClick;

        private void tableView_RowDoubleClick(object sender, DevExpress.Xpf.Grid.RowDoubleClickEventArgs e)
        {
            if (this.OnRowDoubleClick != null)
            {
                this.OnRowDoubleClick(this, e);
            }
        }

        /// <summary>
        /// 当数据源改变时触发
        /// </summary>
        public event EventHandler<ItemsSourceChangedEventArgs> OnItemsSourceChanged;

        private void grid_ItemsSourceChanged(object sender, ItemsSourceChangedEventArgs e)
        {
            if (this.OnItemsSourceChanged != null)
            {
                this.OnItemsSourceChanged(sender, e);
            }
        }

        /// <summary>
        /// 当进行行筛选时触发
        /// </summary>
        public event EventHandler<GridViewRowEventArgs> OnCustomRowFilter;

        #endregion

        private List<object> distinct;

        /// <summary>
        /// 行筛选器
        /// </summary>
        private void grid_CustomRowFilter(object sender, RowFilterEventArgs e)
        {
            if (this.IsRowDistinct)
            {
                if (this.distinct == null && this.ItemsSource is IEnumerable<object>)
                {
                    GridViewRowDataEqualityComparer comparer = new GridViewRowDataEqualityComparer();
                    comparer.Columns = this.ColumnsSource as IEnumerable<IGridViewColumn>;
                    IEnumerable<object> ienumerable = this.ItemsSource as IEnumerable<object>;
                    this.distinct = ienumerable.Distinct(comparer).ToList();
                }

                object x_value = this.grid.GetRow(e.ListSourceRowIndex);

                if (this.distinct.Contains(x_value))
                    e.Visible = true;
                else
                    e.Visible = false;
            }
            if (this.OnCustomRowFilter != null)
            {
                GridViewRowEventArgs args = new GridViewRowEventArgs();
                args.GridView = this;
                args.RowData = this.grid.GetRow(e.ListSourceRowIndex);
                this.OnCustomRowFilter(this, args);
            }
        }
    }
}
