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

namespace XLY.XDD.Control
{
    /// <summary>
    /// 多视图预览
    /// </summary>
    [TemplatePart(Name = "PART_ListBox", Type = typeof(ListBox))]
    [TemplatePart(Name = "PART_Navigation", Type = typeof(XlyNavigation))]
    public class XlyMultiView : System.Windows.Controls.Control, IXlyMultiView
    {
        static XlyMultiView()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(XlyMultiView), new FrameworkPropertyMetadata(typeof(XlyMultiView)));
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            this.PART_ListBox = this.Template.FindName("PART_ListBox", this) as ListBox;
            this.PART_Navigation = this.Template.FindName("PART_Navigation", this) as XlyNavigation;

            this.PART_ListBox.SelectionChanged -= new SelectionChangedEventHandler(PART_ListBox_SelectionChanged);
            this.PART_ListBox.SelectionChanged += new SelectionChangedEventHandler(PART_ListBox_SelectionChanged);
        }

        /// <summary>
        /// 列表选项改变时触发
        /// </summary>
        private void PART_ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.PART_Navigation == null)
                this.ApplyTemplate();
            if (this.PART_Navigation == null)
                return;

            XlyMultiViewInfo info = this.PART_ListBox.SelectedValue as XlyMultiViewInfo;
            if (info == null)
                return;
            XlyNavigationConfig lastConfig = this.PART_Navigation.CurrentConfig;
            if (string.IsNullOrWhiteSpace(info.NavigationKey))
            {
                info.NavigationKey = Guid.NewGuid().ToString();
                Type viewType = info.ViewType;
                if (viewType == null)
                    viewType = typeof(XlyMultiNoneView);
                XlyNavigationConfig config_temp = new XlyNavigationConfig();
                config_temp.ViewType = viewType;
                config_temp.NavigationKey = info.NavigationKey;
                this.PART_Navigation.NavigationConfigs.Add(config_temp);
            }
            XlyNavigationConfig config = this.PART_Navigation.GetNavigationConfig(info.NavigationKey);
            this.PART_Navigation.NavigationTo(info.NavigationKey);
            if (config.View is IXlyMultiView)
            {
                IXlyMultiView imv = config.View as IXlyMultiView;
                imv.MultiViewOwner = this;
                imv.LinkedControl = this.LinkedControl;
                imv.Doamin = this.Doamin;
                imv.ItemsSourceOwner = this.ItemsSourceOwner;
                imv.ColumnsSource = this.ColumnsSource;
                imv.ViewsSource = this.ViewsSource;
                imv.CurrentViewInfo = info;
                if (imv.IsSupport(null))
                {
                    imv.ItemsSource = this.ItemsSource;
                }
                imv.OnActivation();
                if (lastConfig != null && lastConfig.View is IXlyMultiView)
                {
                    ((IXlyMultiView)lastConfig.View).OnDeactivation();
                }
            }
        }

        #region PART

        private ListBox PART_ListBox;
        private XlyNavigation PART_Navigation;

        #endregion

        #region LinkedControl -- 关联控件

        /// <summary>
        /// 关联控件
        /// </summary>
        public IXlyMultiViewLinkedControl LinkedControl
        {
            get { return (IXlyMultiViewLinkedControl)GetValue(LinkedControlProperty); }
            set { SetValue(LinkedControlProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LinkedControl.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LinkedControlProperty =
            DependencyProperty.Register("LinkedControl", typeof(IXlyMultiViewLinkedControl), typeof(XlyMultiView), new UIPropertyMetadata(null, new PropertyChangedCallback((s, e) =>
            {
                XlyMultiView xmv = s as XlyMultiView;
                if (xmv.PART_Navigation == null)
                    xmv.ApplyTemplate();
                if (xmv.PART_Navigation == null)
                    return;
                IXlyMultiViewLinkedControl linked = e.NewValue as IXlyMultiViewLinkedControl;
                foreach (XlyNavigationConfig c in xmv.PART_Navigation.NavigationConfigs)
                {
                    if (c.View != null && c.View is IXlyMultiView)
                    {
                        IXlyMultiView ixmv = c.View as IXlyMultiView;
                        ixmv.LinkedControl = linked;
                    }
                }
            })));

        #endregion

        #region Doamin -- 所属领域模型

        /// <summary>
        /// 所属领域模型
        /// </summary>
        public object Doamin
        {
            get { return (object)GetValue(DoaminProperty); }
            set { SetValue(DoaminProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Doamin.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DoaminProperty =
            DependencyProperty.Register("Doamin", typeof(object), typeof(XlyMultiView), new UIPropertyMetadata(null, new PropertyChangedCallback((s, e) =>
            {
                XlyMultiView xmv = s as XlyMultiView;
                if (xmv.PART_Navigation == null)
                    xmv.ApplyTemplate();
                if (xmv.PART_Navigation == null)
                    return;
                object domain = e.NewValue as object;
                foreach (XlyNavigationConfig c in xmv.PART_Navigation.NavigationConfigs)
                {
                    if (c.View != null && c.View is IXlyMultiView)
                    {
                        IXlyMultiView ixmv = c.View as IXlyMultiView;
                        ixmv.Doamin = domain;
                    }
                }
            })));

        #endregion

        #region ItemsSourceOwner -- 数据源所有者

        /// <summary>
        /// 数据源所有者
        /// </summary>
        public object ItemsSourceOwner
        {
            get { return (object)GetValue(ItemsSourceOwnerProperty); }
            set { SetValue(ItemsSourceOwnerProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ItemsSourceOwner.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ItemsSourceOwnerProperty =
            DependencyProperty.Register("ItemsSourceOwner", typeof(object), typeof(XlyMultiView), new UIPropertyMetadata(null, new PropertyChangedCallback((s, e) => 
            {
                XlyMultiView xmv = s as XlyMultiView;
                if (xmv.PART_Navigation == null)
                    xmv.ApplyTemplate();
                if (xmv.PART_Navigation == null)
                    return;
                object itemsOwner = e.NewValue as object;
                foreach (XlyNavigationConfig c in xmv.PART_Navigation.NavigationConfigs)
                {
                    if (c.View != null && c.View is IXlyMultiView)
                    {
                        IXlyMultiView ixmv = c.View as IXlyMultiView;
                        ixmv.ItemsSourceOwner = itemsOwner;
                    }
                }
            })));

        #endregion

        #region MultiViewOwner -- 所属多视图

        /// <summary>
        /// 所属多视图
        /// </summary>
        public XlyMultiView MultiViewOwner
        {
            get { return (XlyMultiView)GetValue(MultiViewOwnerProperty); }
            set { SetValue(MultiViewOwnerProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MultiViewOwner.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MultiViewOwnerProperty =
            DependencyProperty.Register("MultiViewOwner", typeof(XlyMultiView), typeof(XlyMultiView), new UIPropertyMetadata(null));

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
            DependencyProperty.Register("ItemsSource", typeof(IEnumerable), typeof(XlyMultiView), new UIPropertyMetadata(null, new PropertyChangedCallback((s, e) =>
            {
                XlyMultiView xmv = s as XlyMultiView;
                if (xmv.PART_Navigation == null)
                    xmv.ApplyTemplate();
                if (xmv.PART_Navigation == null)
                    return;
                IEnumerable items = e.NewValue as IEnumerable;
                foreach (XlyNavigationConfig c in xmv.PART_Navigation.NavigationConfigs)
                {
                    if (c.View != null && c.View is IXlyMultiView)
                    {
                        IXlyMultiView ixmv = c.View as IXlyMultiView;
                        if (ixmv.IsSupport(items.GetFirstItemType()))
                        {
                            ixmv.ItemsSource = items;
                        }
                        else
                        {
                            ixmv.ItemsSource = null;
                        }
                    }
                }
            })));

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
            DependencyProperty.Register("ColumnsSource", typeof(IEnumerable), typeof(XlyMultiView), new UIPropertyMetadata(null, new PropertyChangedCallback((s, e) =>
            {
                XlyMultiView xmv = s as XlyMultiView;
                if (xmv.PART_Navigation == null)
                    xmv.ApplyTemplate();
                if (xmv.PART_Navigation == null)
                    return;
                IEnumerable items = e.NewValue as IEnumerable;
                foreach (XlyNavigationConfig c in xmv.PART_Navigation.NavigationConfigs)
                {
                    if (c.View != null && c.View is IXlyMultiView)
                    {
                        IXlyMultiView ixmv = c.View as IXlyMultiView;
                        ixmv.ColumnsSource = items;
                    }
                }
            })));

        #endregion

        #region ViewsSource -- 多视图信息

        /// <summary>
        /// 多视图信息
        /// </summary>
        public IEnumerable<XlyMultiViewInfo> ViewsSource
        {
            get { return (IEnumerable<XlyMultiViewInfo>)GetValue(ViewsSourceProperty); }
            set { SetValue(ViewsSourceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ViewsSource.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ViewsSourceProperty =
            DependencyProperty.Register("ViewsSource", typeof(IEnumerable<XlyMultiViewInfo>), typeof(XlyMultiView), new UIPropertyMetadata(null, new PropertyChangedCallback((s, e) =>
            {
                XlyMultiView xmv = s as XlyMultiView;
                if (xmv.PART_Navigation == null)
                    xmv.ApplyTemplate();
                if (xmv.PART_Navigation == null)
                    return;
                IEnumerable<XlyMultiViewInfo> items = e.NewValue as IEnumerable<XlyMultiViewInfo>;
                foreach (XlyNavigationConfig c in xmv.PART_Navigation.NavigationConfigs)
                {
                    if (c.View != null && c.View is IXlyMultiView)
                    {
                        IXlyMultiView ixmv = c.View as IXlyMultiView; 
                        ixmv.ViewsSource = items;
                    }
                }
            })));

        #endregion

        #region CurrentViewInfo -- 当前视图信息

        /// <summary>
        /// 当前视图信息
        /// </summary>
        public XlyMultiViewInfo CurrentViewInfo
        {
            get { return (XlyMultiViewInfo)GetValue(CurrentViewInfoProperty); }
            set { SetValue(CurrentViewInfoProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CurrentViewInfo.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CurrentViewInfoProperty =
            DependencyProperty.Register("CurrentViewInfo", typeof(XlyMultiViewInfo), typeof(XlyMultiView), new UIPropertyMetadata(null));

        #endregion

        /// <summary>
        /// 是否支持某种数据源
        /// </summary>
        /// <param name="type">数据类型</param>
        /// <returns></returns>
        public bool IsSupport(Type type)
        {
            return true;
        }

        /// <summary>
        /// 被激活时执行
        /// </summary>
        public void OnActivation()
        {
            if (this.PART_Navigation == null)
                this.ApplyTemplate();
            if (this.PART_Navigation == null)
                return;
            if (this.PART_Navigation.CurrentConfig != null && this.PART_Navigation.CurrentConfig != null && this.PART_Navigation.CurrentConfig.View is IXlyMultiView)
            {
                ((IXlyMultiView)this.PART_Navigation.CurrentConfig.View).OnActivation();
            }
        }

        /// <summary>
        /// 反激活时执行
        /// </summary>
        public void OnDeactivation()
        {
            if (this.PART_Navigation == null)
                this.ApplyTemplate();
            if (this.PART_Navigation == null)
                return;
            if (this.PART_Navigation.CurrentConfig != null && this.PART_Navigation.CurrentConfig != null && this.PART_Navigation.CurrentConfig.View is IXlyMultiView)
            {
                ((IXlyMultiView)this.PART_Navigation.CurrentConfig.View).OnDeactivation();
            }
        }
    }
}
