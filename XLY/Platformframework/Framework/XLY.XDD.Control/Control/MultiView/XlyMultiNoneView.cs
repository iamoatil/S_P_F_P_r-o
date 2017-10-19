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

namespace XLY.XDD.Control
{
    /// <summary>
    /// 多视图空视图
    /// </summary>
    public class XlyMultiNoneView : System.Windows.Controls.Control, IXlyMultiView
    {
        static XlyMultiNoneView()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(XlyMultiNoneView), new FrameworkPropertyMetadata(typeof(XlyMultiNoneView)));
        }

        public XlyMultiView MultiViewOwner { get; set; }

        public IXlyMultiViewLinkedControl LinkedControl { get; set; }

        public object Doamin { get; set; }

        public object ItemsSourceOwner { get; set; }

        public System.Collections.IEnumerable ItemsSource { get; set; }

        public System.Collections.IEnumerable ColumnsSource { get; set; }

        public IEnumerable<XlyMultiViewInfo> ViewsSource { get; set; }

        public XlyMultiViewInfo CurrentViewInfo { get; set; }

        public bool IsSupport(Type type)
        {
            return false;
        }

        public void OnActivation()
        {

        }

        public void OnDeactivation()
        {

        }
    }
}
