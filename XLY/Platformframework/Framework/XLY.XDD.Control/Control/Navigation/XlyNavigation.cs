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
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using System.Threading;

namespace XLY.XDD.Control
{
    /// <summary>
    /// 导航控件
    /// </summary>
    [TemplatePart(Name = "PART_Root", Type = typeof(Grid))]
    public class XlyNavigation : System.Windows.Controls.Control
    {
        static XlyNavigation()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(XlyNavigation), new FrameworkPropertyMetadata(typeof(XlyNavigation)));
        }

        public XlyNavigation()
        {
            this.Loaded += new RoutedEventHandler(XlyNavigation_Loaded);
        }

        void XlyNavigation_Loaded(object sender, RoutedEventArgs e)
        {
            if (this.CurrentConfig == null && this.NavigationConfigs != null && this.NavigationConfigs.Count > 0)
            {
                this.NavigationTo(this.NavigationConfigs.First().NavigationKey);
            }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            this.PART_Root = this.Template.FindName("PART_Root", this) as Grid;
        }

        #region PART

        private Grid PART_Root;

        #endregion

        /// <summary>
        /// 清理导航
        /// </summary>
        public void Clear()
        {
            if (this.NavigationConfigs != null)
                this.NavigationConfigs.Clear();
            if (this._Pool != null)
                this._Pool.Clear();
            if (this.PART_Root != null)
                this.PART_Root.Children.Clear();
        }

        /// <summary>
        /// 当前导航视图
        /// </summary>
        public XlyNavigationConfig CurrentConfig { get; set; }

        private List<XlyNavigationConfig> _NavigationConfigs = new List<XlyNavigationConfig>();
        /// <summary>
        /// 导航配置
        /// </summary>
        public List<XlyNavigationConfig> NavigationConfigs
        {
            get { return _NavigationConfigs; }
            set { _NavigationConfigs = value; }
        }

        /// <summary>
        /// 移除视图
        /// </summary>
        /// <param name="navigationKey">导航键值</param>
        public void RemoveConfig(string navigationKey)
        {
            XlyNavigationConfig config = this.GetNavigationConfig(navigationKey);
            if (config == null)
                return;

            if (this._Pool.ContainsKey(config))
            {
                this._Pool.Remove(config);
                this.PART_Root.Children.Remove(config.View);
            }
        }

        /// <summary>
        /// 获取导航配置信息
        /// </summary>
        /// <param name="navigationKey">导航键值</param>
        /// <returns></returns>
        public XlyNavigationConfig GetNavigationConfig(string navigationKey)
        {
            if (this.NavigationConfigs == null || navigationKey == null || navigationKey.Trim() == string.Empty)
                return null;
            return this.NavigationConfigs.Where(c => c.NavigationKey.Equals(navigationKey)).SingleOrDefault();
        }

        /// <summary>
        /// 获取导航配置信息
        /// </summary>
        /// <param name="view">视图</param>
        /// <returns></returns>
        public XlyNavigationConfig GetNavigationConfig(FrameworkElement view)
        {
            if (this.NavigationConfigs == null || view == null)
                return null;
            return this.NavigationConfigs.Where(c => c.View == view).SingleOrDefault();
        }

        /// <summary>
        /// 导航中是否包含某键值的视图
        /// </summary>
        /// <param name="navigationKey">导航键值</param>
        /// <returns></returns>
        public bool ContainsKey(string navigationKey)
        {
            if (this.NavigationConfigs == null || navigationKey == null || navigationKey.Trim().Equals(string.Empty))
                return false;
            return this.NavigationConfigs.Where(c => c.NavigationKey.Equals(navigationKey)).SingleOrDefault() != null;
        }

        private Dictionary<XlyNavigationConfig, FrameworkElement> _Pool = new Dictionary<XlyNavigationConfig, FrameworkElement>();

        /// <summary>
        /// 导航至某视图
        /// </summary>
        /// <param name="navigationKey">导航键值</param>
        /// <param name="load"></param>
        public void NavigationTo(string navigationKey)
        {
            XlyNavigationConfig config = this.GetNavigationConfig(navigationKey);
            if (config == null || config.ViewType == null && config.FunCreateView == null)
                return;
            if (this.PART_Root == null)
            {
                this.ApplyTemplate();
            }

            this.BuildConfig(config, null, null);

            foreach (var kv in this._Pool)
            {
                if (kv.Key == config)
                {
                    kv.Value.Visibility = System.Windows.Visibility.Visible;
                }
                else
                {
                    kv.Value.Visibility = System.Windows.Visibility.Collapsed;
                }
            }
            this.CurrentConfig = config;
        }

        /// <summary>
        /// 当构建导航配置时触发
        /// </summary>
        public event EventHandler<XlyNavigationEventArgs> OnBuildConfig;

        /// <summary>
        /// 构建导航配置
        /// </summary>
        /// <param name="config"></param>
        public void BuildConfig(XlyNavigationConfig config, Action<FrameworkElement> actionCreateView, Action<object> actionCreateViewModel)
        {
            if (config == null || config.ViewType == null && config.FunCreateView == null)
                return;
            if (this.PART_Root == null)
            {
                this.ApplyTemplate();
            }

            if (!this._Pool.ContainsKey(config))
            {
                if (config.View == null)
                {
                    if (config.FunCreateView != null)
                        config.View = config.FunCreateView(config);
                    else
                        config.View = config.ViewType.Assembly.CreateInstance(config.ViewType.FullName) as FrameworkElement;
                    if (actionCreateView != null)
                        actionCreateView(config.View);
                }
                if (config.ViewModelType != null || config.ViewModel != null || config.FunCreateViewModel != null)
                {
                    if (config.ViewModel == null)
                    {
                        if (config.FunCreateViewModel != null)
                            config.ViewModel = config.FunCreateViewModel(config);
                        else
                            config.ViewModel = config.ViewModelType.Assembly.CreateInstance(config.ViewModelType.FullName);
                        if (actionCreateViewModel != null)
                            actionCreateViewModel(config.ViewModel);
                    }
                    if (config.ViewModel is XlyNavigationViewModelBase)
                    {
                        XlyNavigationViewModelBase vm = config.ViewModel as XlyNavigationViewModelBase;
                        vm.Config = config;
                        vm.Navigation = this;
                    }
                    config.View.DataContext = config.ViewModel;
                }
                this._Pool.Add(config, config.View);
                this.PART_Root.Children.Add(config.View);
                if (this.OnBuildConfig != null)
                {
                    this.OnBuildConfig(this, new XlyNavigationEventArgs { Config = config });
                }
            }
        }

        /// <summary>
        /// 构建控件导航配置
        /// </summary>
        /// <param name="navigationKey">导航键值</param>
        public void BuildConfig(string navigationKey, Action<FrameworkElement> actionCreateView, Action<object> actionCreateViewModel)
        {
            XlyNavigationConfig config = this.GetNavigationConfig(navigationKey);
            this.BuildConfig(config, actionCreateView, actionCreateViewModel);
        }

        private void View_Loaded(object sender, RoutedEventArgs e)
        {
            FrameworkElement fe = sender as FrameworkElement;
            XlyNavigationConfig config = this.GetNavigationConfig(fe);
            if (config != null && config.OnLoad != null)
            {
                Action<FrameworkElement> action = config.OnLoad;
                config.OnLoad = null;
                config.View.Loaded -= this.View_Loaded;
                action(fe);
            }
        }
    }
}