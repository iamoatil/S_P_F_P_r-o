using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using XLY.SF.Project.Themes.CustromControl.TabControl;

namespace XLY.SF.Project.Themes
{
    /// <summary>
    /// 按照步骤 1a 或 1b 操作，然后执行步骤 2 以在 XAML 文件中使用此自定义控件。
    ///
    /// 步骤 1a) 在当前项目中存在的 XAML 文件中使用该自定义控件。
    /// 将此 XmlNamespace 特性添加到要使用该特性的标记文件的根 
    /// 元素中: 
    ///
    ///     xmlns:MyNamespace="clr-namespace:XLY.SF.Project.Themes"
    ///
    ///
    /// 步骤 1b) 在其他项目中存在的 XAML 文件中使用该自定义控件。
    /// 将此 XmlNamespace 特性添加到要使用该特性的标记文件的根 
    /// 元素中: 
    ///
    ///     xmlns:MyNamespace="clr-namespace:XLY.SF.Project.Themes;assembly=XLY.SF.Project.Themes"
    ///
    /// 您还需要添加一个从 XAML 文件所在的项目到此项目的项目引用，
    /// 并重新生成以避免编译错误: 
    ///
    ///     在解决方案资源管理器中右击目标项目，然后依次单击
    ///     “添加引用”->“项目”->[浏览查找并选择此项目]
    ///
    ///
    /// 步骤 2)
    /// 继续操作并在 XAML 文件中使用控件。
    ///
    ///     <MyNamespace:PopTabControl/>
    ///
    /// </summary>
    public class PopTabControl : TabControl
    {
        #region 拖动操作

        /// <summary>
        /// 拖入框位置元素
        /// </summary>
        private TabPanelSizeElement _tabPanelEmt;
        /// <summary>
        /// 当前正在移动的窗口
        /// </summary>
        private DockingWindowElement _curMoveWindow;

        #endregion

        private bool moveNewWindow;
        private bool _isBtnDown;
        private TabPanel _tbPanel;
        private Window parentWin;

        static PopTabControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(PopTabControl), new FrameworkPropertyMetadata(typeof(PopTabControl)));
        }

        #region 内部重载

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            //获取控件
            this._tbPanel = this.Template.FindName("HeaderPanel", this) as TabPanel;
            parentWin = Window.GetWindow(this);

            this.AddHandler(TabItem.PreviewMouseLeftButtonDownEvent, new MouseButtonEventHandler(DockingTabControl_PreviewMouseLeftButtonDown));
            this.AddHandler(TabItem.PreviewMouseLeftButtonUpEvent, new MouseButtonEventHandler(DockingTabControl_PreviewMouseLeftButtonUp));
            this.AddHandler(TabItem.MouseMoveEvent, new MouseEventHandler(DockingTabControl_MouseMove));
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);
            if (parentWin != null)
            {
                //得到_tbPanel的坐标
                var a = _tbPanel.TransformToAncestor(parentWin).Transform(new Point(0, 0));
                _tabPanelEmt = new TabPanelSizeElement(a);
            }
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);
            if (_tabPanelEmt != null)
            {
                //得到_tbPanel的区域
                _tabPanelEmt.SetLocation(_tbPanel.ActualWidth, _tbPanel.ActualHeight);
            }
        }

        #endregion

        #region 拖出

        void DockingTabControl_MouseMove(object sender, MouseEventArgs e)
        {
            var a = e.Source as TabItem;
            var b = e.GetPosition(a);
            var c = e.GetPosition(this);
            var d = PointToScreen(c);

            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if (moveNewWindow)
                {
                    //更新窗口位置
                    _curMoveWindow.UpdateWindowLocation(d);

                    //判断是否拖入TabPanel
                    if ((c.X >= _tabPanelEmt.TabPanelStartPoint.X && c.X <= _tabPanelEmt.TabPanelStartPoint.X + _tabPanelEmt.TabPanelSize.Width) &&
                        (c.Y >= _tabPanelEmt.TabPanelStartPoint.Y && c.Y <= _tabPanelEmt.TabPanelStartPoint.Y + _tabPanelEmt.TabPanelSize.Height))
                    {
                        if (!this.Items.Contains(_curMoveWindow.DragTargetHeader))
                        {
                            //拖进来
                            this.Items.Add(_curMoveWindow.DragTargetHeader);
                        }
                    }
                    else
                    {
                        //拖出去
                        if (this.Items.Contains(_curMoveWindow.DragTargetHeader))
                            this.Items.Remove(_curMoveWindow.DragTargetHeader);
                    }
                }
                else
                {
                    if (_isBtnDown && a != null)
                    {
                        if (_curMoveWindow != null)
                        {
                            //更新窗口位置
                            _curMoveWindow.UpdateWindowLocation(d);
                        }
                        if ((b.X < 0 || b.Y < 0 || b.X > a.ActualWidth || b.Y > a.ActualHeight) && _curMoveWindow == null)
                        {
                            _curMoveWindow = new DockingWindowElement(a, a.Content, parentWin, d);
                            _curMoveWindow.CreateDragWindow(a.Header.ToString(), true, parentWin);
                            _curMoveWindow.DragWindow.Evt_MoveWindow += DragWindow_Evt_MoveWindow;
                            _curMoveWindow.DragWindow.MouseLeave += DragWindow_MouseLeave;
                            a.Opacity = 0.4;
                        }
                    }
                }
            }
        }

        void DragWindow_Evt_MoveWindow(object obj, object header, Point startPoint)
        {
            _curMoveWindow = new DockingWindowElement(header, obj, parentWin, startPoint);

            //修改弹出框名称
            _curMoveWindow.CreateDragWindow("测试", false, parentWin);
            _curMoveWindow.DragWindow.MouseLeave += DragWindow_MouseLeave;
            _curMoveWindow.DragWindow.Evt_MoveWindow += DragWindow_Evt_MoveWindow;
            _tbPanel.CaptureMouse();
            moveNewWindow = true;
        }

        void DragWindow_MouseLeave(object sender, MouseEventArgs e)
        {
            //鼠标离开窗体，目前已知是窗口切换Alt+Tab
            moveNewWindow = false;
            _isBtnDown = false;
            //_curMoveWindow = null;
        }

        void DockingTabControl_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            //判断是否拖入其中
            if (moveNewWindow)
            {
                var c = e.GetPosition(this);
                if ((c.X >= _tabPanelEmt.TabPanelStartPoint.X && c.X <= _tabPanelEmt.TabPanelStartPoint.X + _tabPanelEmt.TabPanelSize.Width) &&
                    (c.Y >= _tabPanelEmt.TabPanelStartPoint.Y && c.Y <= _tabPanelEmt.TabPanelStartPoint.Y + _tabPanelEmt.TabPanelSize.Height))
                {
                    if (this.Items.Contains(_curMoveWindow.DragTargetHeader))
                    {
                        var newTabItem = _curMoveWindow.DragTargetHeader as TabItem;
                        if (newTabItem != null)
                            newTabItem.Opacity = 1;
                        _curMoveWindow.DragWindow.Close();
                    }
                    else
                    {
                        if (this.Items.Contains(_curMoveWindow.DragTargetHeader))
                            this.Items.Remove(_curMoveWindow.DragTargetHeader);
                    }
                }
                else
                    _curMoveWindow.DragWindow.Topmost = false;
                moveNewWindow = false;
                _tbPanel.ReleaseMouseCapture();
                _curMoveWindow = null;
            }
            else
            {
                _isBtnDown = false;
                var a = e.Source as TabItem;
                if (a != null)
                    a.ReleaseMouseCapture();
                if (_curMoveWindow != null)
                {
                    this.Items.Remove(_curMoveWindow.DragTargetHeader);
                    _curMoveWindow.DragWindow.Topmost = false;
                    _curMoveWindow = null;
                }
            }
        }

        void DockingTabControl_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _isBtnDown = true;
            var a = e.Source as TabItem;
            if (a != null)
                a.CaptureMouse();
        }

        #endregion
    }

    #region 拖动的窗体元素

    /// <summary>
    /// 拖动的窗体元素
    /// </summary>
    public class DockingWindowElement
    {
        /// <summary>
        /// 拖动的窗体
        /// </summary>
        public PopTabWindow DragWindow { get; set; }
        /// <summary>
        /// 拖动的TabItemHeader
        /// </summary>
        public object DragTargetHeader { get; private set; }
        /// <summary>
        /// 拖动的内容
        /// </summary>
        public object DragContent { get; private set; }

        /// <summary>
        /// 起始位置
        /// </summary>
        private Point _startPoint;
        /// <summary>
        /// 父窗体
        /// </summary>
        private Window ownerWindow;

        public DockingWindowElement(object dragTargetHeader, object dragContent, Window owner, Point startPoint)
        {
            this.DragTargetHeader = dragTargetHeader;
            this.DragContent = dragContent;
            this.ownerWindow = owner;
            this._startPoint = startPoint;
        }

        public void CreateDragWindow(string header, bool showAnimation, Window parentWindow)
        {
            DragWindow = new PopTabWindow(parentWindow, showAnimation);
            DragWindow.Title = "测试";
            DragWindow.Content = this.DragContent;
            DragWindow.Topmost = true;
            DragWindow.TabItemHeader = this.DragTargetHeader;
            DragWindow.Top = this._startPoint.Y - 15;
            DragWindow.Left = this._startPoint.X - 60;
            DragWindow.Show();
        }

        public void UpdateWindowLocation(Point curPoint)
        {
            if (DragWindow != null)
            {
                DragWindow.Top = curPoint.Y - 15;
                DragWindow.Left = curPoint.X - 60;
            }
        }
    }

    #endregion

    #region 可拖入的容器元素
    /// <summary>
    /// 可拖入的容器元素
    /// </summary>
    public class TabPanelSizeElement
    {
        public TabPanelSizeElement(Point startPoint)
        {
            TabPanelStartPoint = startPoint;
            _tabPanelSize = new Size();
        }

        /// <summary>
        /// 起始坐标
        /// </summary>
        public Point TabPanelStartPoint { get; private set; }

        private Size _tabPanelSize;

        /// <summary>
        /// 大小
        /// </summary>
        public Size TabPanelSize
        {
            get
            {
                return _tabPanelSize;
            }
        }

        /// <summary>
        /// 设置位置
        /// </summary>
        public void SetLocation(double width, double height)
        {
            this._tabPanelSize.Width = width;
            this._tabPanelSize.Height = height;
        }
    }

    #endregion
}
