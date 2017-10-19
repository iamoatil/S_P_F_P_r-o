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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ComponentModel.Composition;
using XLY.SF.Framework.Core.Base.ViewModel;
using XLY.SF.Project.ViewDomain.MefKeys;
using ProjectExtend.Context;
using System.Windows.Threading;
using System.Windows.Media.Animation;

namespace XLY.SF.Project.Views.Main
{
    /// <summary>
    /// MainView.xaml 的交互逻辑
    /// </summary>
    [Export(ExportKeys.ModuleMainUcView, typeof(UcViewBase))]
    [PartCreationPolicy(System.ComponentModel.Composition.CreationPolicy.NonShared)]
    public partial class MainView : UcViewBase
    {
        /// <summary>
        /// 界面时间更新
        /// </summary>
        private DispatcherTimer updateTime;
        /// <summary>
        /// 展开创建案例动画
        /// </summary>
        private Storyboard _sbExpandCreateCase;
        /// <summary>
        /// 折叠创建案例动画
        /// </summary>
        private Storyboard _sbOnExpandCreateCaseBack;

        public MainView()
        {
            InitializeComponent();
        }

        [Import(ExportKeys.ModuleMainViewModel, typeof(ViewModelBase))]
        public override ViewModelBase DataSource
        {
            get
            {
                return this.DataContext as ViewModelBase;
            }
            set
            {
                value.SetViewContainer(this);
                this.DataContext = value;
            }
        }

        #region 成功加载

        private void UcViewBase_Loaded(object sender, RoutedEventArgs e)
        {
            _sbExpandCreateCase = this.Resources["OnExpandCreateCase"] as Storyboard;
            _sbExpandCreateCase.Completed += _sbExpandCreateCase_Completed;

            _sbOnExpandCreateCaseBack = this.Resources["OnExpandCreateCaseBack"] as Storyboard;
            _sbOnExpandCreateCaseBack.Completed += _sbOnExpandCreateCaseBack_Completed;

            StartUpdateTimer();
        }

        private void StartUpdateTimer()
        {
            updateTime = new DispatcherTimer();
            updateTime.Interval = TimeSpan.FromSeconds(1);
            updateTime.Tick += UpdateTime_Tick;
            updateTime.Start();
        }

        #endregion

        #region 时间更新

        private void UpdateTime_Tick(object sender, EventArgs e)
        {
            updateTime.Dispatcher.Invoke(() =>
            {
                tb_CurTime.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            });
        }

        #endregion

        #region 界面操作

        //最小化
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Window.GetWindow(this).WindowState = WindowState.Minimized;
        }

        //执行展开创建案例动画
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            //禁用展开按钮
            btn_Expand.IsEnabled = false;
            //重置创建案例界面大小
            if (btn_Expand.IsChecked.HasValue && btn_Expand.IsChecked.Value)
            {
                br_CreateCaseView.Height = gd_ViewContent.ActualHeight;
                _sbExpandCreateCase.Begin();
            }
            else
                _sbOnExpandCreateCaseBack.Begin();
            //使窗体无法改变大小
            //Window.GetWindow(this).ResizeMode = ResizeMode.NoResize;
        }

        //动画执行完毕【展开创建案例】
        private void _sbExpandCreateCase_Completed(object sender, EventArgs e)
        {
            btn_Expand.IsEnabled = true;
        }

        private void _sbOnExpandCreateCaseBack_Completed(object sender, EventArgs e)
        {
            btn_Expand.IsEnabled = true;
        }

        //界面大小改变【改变案例创建View大小】
        private void UcViewBase_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            br_CreateCaseView.Height = gd_ViewContent.ActualHeight;
        }

        #endregion
    }
}
