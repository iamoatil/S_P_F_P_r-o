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
    /// 进度条
    /// </summary>
    public class XlyProgressPanel : System.Windows.Controls.Control
    {
        static XlyProgressPanel()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(XlyProgressPanel), new FrameworkPropertyMetadata(typeof(XlyProgressPanel)));
        }

        #region YMaxValue -- Y轴最大值

        /// <summary>
        /// Y轴最大值
        /// </summary>
        public double YMaxValue
        {
            get { return (double)GetValue(YMaxValueProperty); }
            set { SetValue(YMaxValueProperty, value); }
        }

        // Using a DependencyProperty as the backing store for YMaxValue.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty YMaxValueProperty =
            DependencyProperty.Register("YMaxValue", typeof(double), typeof(XlyProgressPanel), new PropertyMetadata(100d, new PropertyChangedCallback((s, e) =>
            {
                XlyProgressPanel bar = s as XlyProgressPanel;
                if (e.NewValue == null)
                    return;
                bar.Update();
            })));

        #endregion

        #region XMaxValue -- X轴最大值

        /// <summary>
        /// X轴最大值
        /// </summary>
        public double XMaxValue
        {
            get { return (double)GetValue(XMaxValueProperty); }
            set { SetValue(XMaxValueProperty, value); }
        }

        // Using a DependencyProperty as the backing store for XMaxValue.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty XMaxValueProperty =
            DependencyProperty.Register("XMaxValue", typeof(double), typeof(XlyProgressPanel), new PropertyMetadata(100d, new PropertyChangedCallback((s, e) =>
            {
                XlyProgressPanel bar = s as XlyProgressPanel;
                if (e.NewValue == null)
                    return;
                bar.Update();
            })));

        #endregion

        #region Value -- 当前值

        /// <summary>
        /// 当前值
        /// </summary>
        public Point Value
        {
            get { return (Point)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Value.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(Point), typeof(XlyProgressPanel), new PropertyMetadata(new Point(), new PropertyChangedCallback((s, e) =>
            {
                XlyProgressPanel bar = s as XlyProgressPanel;
                if (e.NewValue == null)
                    return;
                lock (bar.list)
                {
                    Point p = (Point)e.NewValue;
                    bar.list.Add(p);
                }
                bar.Update();
            })));

        #endregion

        public List<Point> list = new List<Point>();

        public void Update()
        {
            this.InvalidateVisual();
        }

        /// <summary>
        /// 折线笔
        /// </summary>
        private Pen BrokenLinePen = new Pen(Brushes.Red, 1);

        /// <summary>
        /// 平均线笔
        /// </summary>
        private Pen AverageLinePen = new Pen(Brushes.SkyBlue, 1);

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);

            if (list.Count == 0)
                return;

            PathGeometry pathGeometry = new PathGeometry();
            int index = 0;
            PathFigure pathFigure = new PathFigure();
            pathFigure.StartPoint = this.getActualPoint(this.list[0]);
            pathFigure.IsClosed = true;
            pathGeometry.Figures.Add(pathFigure);

            foreach (Point p in this.list)
            {
                if (index == 0)
                {
                    ++index;
                    continue;
                }

                LineSegment l = new LineSegment(this.getActualPoint(p), false);
                pathFigure.Segments.Add(l);

                if (index == this.list.Count - 1)
                {
                    LineSegment l2 = new LineSegment(this.getActualPoint(new Point(p.X, 0)), false);
                    pathFigure.Segments.Add(l2);

                    LineSegment l3 = new LineSegment(this.getActualPoint(new Point(0, 0)), false);
                    pathFigure.Segments.Add(l3);
                }

                ++index;
            }

            drawingContext.DrawGeometry(Brushes.Red, this.BrokenLinePen, pathGeometry);

            double av_y = this.list.Average(p => p.Y);
            Point av_y_p = this.getActualPoint(new Point(0, av_y));
            drawingContext.DrawLine(this.AverageLinePen, av_y_p, new Point(this.ActualWidth, av_y_p.Y));

            Point temp = this.getActualPoint(this.Value);
            drawingContext.DrawLine(this.AverageLinePen, new Point(temp.X, 0), new Point(temp.X, this.ActualHeight));
        }

        /// <summary>
        /// 获取真实的坐标点
        /// </summary>
        /// <param name="p">逻辑坐标点</param>
        /// <returns>真实的坐标点</returns>
        private Point getActualPoint(Point p)
        {
            Point result = new Point();

            result.X = p.X * this.ActualWidth / this.XMaxValue;
            result.Y = this.ActualHeight - p.Y * this.ActualHeight / this.YMaxValue;

            return result;
        }
    }
}
