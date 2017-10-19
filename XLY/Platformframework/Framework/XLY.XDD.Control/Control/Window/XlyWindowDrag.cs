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
    /// 窗口拖拽条
    /// </summary>
    public class XlyWindowDrag : System.Windows.Controls.Control
    {
        static XlyWindowDrag()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(XlyWindowDrag), new FrameworkPropertyMetadata(typeof(XlyWindowDrag)));
        }

        public XlyWindowDrag()
        {
            this.PreviewMouseMove += new MouseEventHandler(XlyWindowDrag_PreviewMouseMove);
        }

        private void XlyWindowDrag_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            Window window = Window.GetWindow(this);
            if (window != null && e.LeftButton == MouseButtonState.Pressed)
            {
                window.DragMove();
            }
        }
    }
}
