using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Threading.Tasks;
using System.Windows.Media.Animation;

namespace XLY.XDD.Control
{
    /// <summary>
    /// 控件扩展对象
    /// </summary>
    public static class ControlExtension
    {
        /// <summary>
        /// 获取控件的视图模型
        /// </summary>
        /// <typeparam name="T">视图模型种类</typeparam>
        /// <param name="control">要获取视图模型的控件</param>
        /// <returns>视图模型</returns>
        public static T GetViewModel<T>(this System.Windows.Controls.Control control) where T : class
        {
            if (control == null)
                return null;
            return control.DataContext as T;
        }

        /// <summary>
        /// 异步更新界面
        /// </summary>
        /// <param name="ui">界面</param>
        /// <param name="action">更新方法</param>
        /// <param name="args">为该方法传入的参数</param>
        public static void BeginInvokeEx(this UIElement ui, Action<object> action, object args)
        {
            if (ui == null)
                return;
            ui.Dispatcher.BeginInvoke(new EventHandler<EventArgs>((o, e) =>
            {
                try
                {
                    object[] objs = o as object[];
                    Action<object> a = objs[0] as Action<object>;
                    object arg = objs[1];
                    a(arg);
                }
                catch (Exception ex)
                {

                }
            }), new object[] { action, args }, null);
        }

        /// <summary>
        /// 异步更新界面
        /// </summary>
        /// <param name="ui">界面</param>
        /// <param name="action">更新方法</param>
        public static void BeginInvokeEx(this UIElement ui, Action action)
        {
            BeginInvokeEx(ui, o => { (o as Action)(); }, action);
        }

        /// <summary>
        /// 同步更新界面
        /// </summary>
        /// <param name="ui">界面</param>
        /// <param name="action">更新方法</param>
        /// <param name="args">为该方法传入的参数</param>
        public static void InvokeEx(this UIElement ui, Action<object> action, object args)
        {
            if (ui == null)
                return;
            ui.Dispatcher.Invoke(new EventHandler<EventArgs>((o, e) =>
            {
                try
                {
                    object[] objs = o as object[];
                    Action<object> a = objs[0] as Action<object>;
                    object arg = objs[1];
                    a(arg);
                }
                catch (Exception ex)
                {

                }
            }), new object[] { action, args }, null);
        }

        /// <summary>
        /// 同步更新界面
        /// </summary>
        /// <param name="ui">界面</param>
        /// <param name="action">更新方法</param>
        public static void InvokeEx(this UIElement ui, Action action)
        {
            InvokeEx(ui, o => { (o as Action)(); }, action);
        }

        /// <summary>
        /// 异步执行界面更新
        /// </summary>
        /// <param name="action"></param>
        public static void BeginInvokeEx(Action action)
        {
            Application.Current.Dispatcher.BeginInvoke(new EventHandler<EventArgs>((o, e) =>
            {
                try
                {
                    (o as Action)();
                }
                catch (Exception ex)
                {

                }
            }), action, null);
        }

        /// <summary>
        /// 同步执行界面更新
        /// </summary>
        /// <param name="action"></param>
        public static void InvokeEx(Action action)
        {
            Application.Current.Dispatcher.Invoke(new EventHandler<EventArgs>((o, e) =>
            {
                try
                {
                    (o as Action)();
                }
                catch (Exception ex)
                {

                }
            }), action, null);
        }

        /// <summary>
        /// 查找父级控件
        /// </summary>
        /// <typeparam name="T">要查找的父级控件类型</typeparam>
        /// <param name="element">查找的开始控件</param>
        /// <returns>查找结果</returns>
        public static T FindParent<T>(this FrameworkElement element) where T : class
        {
            for (FrameworkElement e = element.TemplatedParent as FrameworkElement; e != null; e = e.TemplatedParent as FrameworkElement)
            {
                T local = e as T;
                if (local != null)
                    return local;
            }
            return default(T);
        }

        /// <summary>
        /// 使用可视化树查找父级控件
        /// </summary>
        /// <typeparam name="T">要查找的控件类型</typeparam>
        /// <param name="element">查找的开始控件</param>
        /// <returns>查找结果</returns>
        public static T GetAncestorByType<T>(this DependencyObject element) where T : class
        {
            if (element == null)
                return default(T);

            if (element is T)
                return element as T;

            return GetAncestorByType<T>(VisualTreeHelper.GetParent(element));
        }

        /// <summary>
        /// 遍历可视化树
        /// </summary>
        public static T FindChildrenFromVisualTree<T>(Visual visualObj)
            where T : Visual
        {
            if (visualObj is T)
                return visualObj as T;
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(visualObj); i++)
            {
                //接收特定索引的子元素
                Visual childVisual = (Visual)VisualTreeHelper.GetChild(visualObj, i);

                T t = FindChildrenFromVisualTree<T>(childVisual);
                if (t != null)
                    return t;
            }
            return null;
        }

        /// <summary>
        /// 遍历子节点，查找第一个符合要求的子元素。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="element"></param>
        /// <returns></returns>
        public static T FindChildren<T>(DependencyObject element) where T : class, new()
        {
            if (element is T)
                return element as T;

            for (int i = 0,childCount = VisualTreeHelper.GetChildrenCount(element); i < childCount; i++)
            {
                //接收特定索引的子元素
                Visual childVisual = (Visual)VisualTreeHelper.GetChild(element, i);

                T t = FindChildren<T>(childVisual);
                if (t != null)
                    return t;
            }
            return null;
        }

        /// <summary>
        /// 在视觉树中根据名称查找控件。
        /// </summary>
        /// <param name="ele"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static DependencyObject FindChildrenByName(DependencyObject ele, string name)
        {
            if (ele == null || string.IsNullOrWhiteSpace(name))
                return null;

            for (int i = 0, childCount = VisualTreeHelper.GetChildrenCount(ele); i < childCount; i++)
            {
                FrameworkElement childVisual = VisualTreeHelper.GetChild(ele, i) as FrameworkElement;
                if (childVisual == null)
                    continue;

                if (childVisual.Name == name)
                    return childVisual;

                FrameworkElement t = FindChildrenByName(childVisual, name) as FrameworkElement;
                if (t == null)
                    continue;

                return t;
            }

            return null;
        }

        /// <summary>  
        /// 截图控件
        /// </summary>  
        /// <param name="control">控件对象</param>  
        public static System.IO.MemoryStream Screenshot(this FrameworkElement control)
        {
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            RenderTargetBitmap bmp = new RenderTargetBitmap((int)control.ActualWidth, (int)control.ActualHeight, 96.0, 96.0, PixelFormats.Pbgra32);
            bmp.Render(control);
            PngBitmapEncoder encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(bmp));
            encoder.Save(ms);
            //-----------------------------
            //string temp = System.IO.Path.GetTempFileName();
            //using (System.IO.FileStream fs = new IO.FileStream(temp, IO.FileMode.Create))
            //{
            //    byte[] buffer = ms.GetBuffer();
            //    fs.Write(buffer, 0, buffer.Length);
            //}
            //Console.WriteLine(temp);
            //-----------------------------
            return ms;
        }

        [System.Runtime.InteropServices.DllImport("gdi32.dll")]
        public static extern bool DeleteObject(IntPtr hObject);

        /// <summary>
        /// 转化bitmap为Imagesource
        /// </summary>
        /// <param name="bitmap"></param>
        /// <returns></returns>
        public static ImageSource ChangeBitmapToImageSource(System.Drawing.Bitmap bitmap)
        {
            //Bitmap bitmap = icon.ToBitmap();
            IntPtr hBitmap = bitmap.GetHbitmap();
            ImageSource wpfBitmap = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                hBitmap,
                IntPtr.Zero,
                Int32Rect.Empty,
                BitmapSizeOptions.FromEmptyOptions());

            if (!DeleteObject(hBitmap))
            {
                throw new System.ComponentModel.Win32Exception();
            }
            return wpfBitmap;
        }

        #region Wait


        /// <summary>
        /// 等待控件处理
        /// </summary>
        /// <param name="control">控件</param>
        /// <param name="fun">等待结束条件</param>
        /// <param name="callback">等待结束后的回调函数</param>
        public static void Wait<T>(this FrameworkElement control, Func<T, bool> fun, Action<T> callback)
             where T : System.Windows.Controls.Control
        {
            Wait<T, object>(control, (c, o) => fun(c), null, (c, o) => { callback(c); });
        }

        /// <summary>
        /// 等待控件处理
        /// </summary>
        /// <param name="control">控件</param>
        /// <param name="fun">等待结束条件</param>
        /// <param name="wait">等待时间</param>
        /// <param name="callback">等待结束后的回调函数</param>
        public static void Wait<T>(this FrameworkElement control, Func<T, bool> fun, Action<T, bool> callback, TimeSpan wait)
            where T : System.Windows.Controls.Control
        {
            Wait<T, object>(control, (c, o) => fun(c), null, (c, o, b) => callback(c, b), wait);
        }

        /// <summary>
        /// 等待控件处理
        /// </summary>
        /// <param name="control">控件</param>
        /// <param name="fun">等待结束条件</param>
        /// <param name="callback">等待结束后的回调函数</param>
        public static void Wait<T, Targ>(this FrameworkElement control, Func<T, Targ, bool> fun, Targ arg, Action<T, Targ> callback)
            where T : System.Windows.Controls.Control
            where Targ : class
        {
            Task task = new Task(o =>
            {
                object[] args = o as object[];
                T c = args[0] as T;
                Func<T, Targ, bool> f = args[1] as Func<T, Targ, bool>;
                Targ r = args[2] as Targ;
                Action<T, Targ> a = args[3] as Action<T, Targ>;
                bool temp = false;
                while (true)
                {
                    c.InvokeEx(() => { temp = f(c, r); });
                    if (temp)
                    {
                        a(c, r);
                        return;
                    }
                    System.Threading.Thread.Sleep(500);
                }
            }, new object[] { control, fun, arg, callback });
            task.Start();
        }

        /// <summary>
        /// 等待控件处理
        /// </summary>
        /// <param name="control">控件</param>
        /// <param name="fun">等待结束条件</param>
        /// <param name="wait">等待时间</param>
        /// <param name="callback">等待结束后的回调函数</param>
        public static void Wait<T, Targ>(this FrameworkElement control, Func<T, Targ, bool> fun, object arg, Action<T, Targ, bool> callback, TimeSpan wait)
            where T : System.Windows.Controls.Control
            where Targ : class
        {
            Task task = new Task(o =>
            {
                object[] args = o as object[];
                T c = args[0] as T;
                Func<T, Targ, bool> f = args[1] as Func<T, Targ, bool>;
                Targ r = args[2] as Targ;
                Action<T, Targ, bool> a = args[3] as Action<T, Targ, bool>;
                TimeSpan w = (TimeSpan)args[4];
                double second = 0d;
                bool temp = false;
                while (true)
                {
                    c.InvokeEx(() => { temp = f(c, r); });
                    if (temp || TimeSpan.FromSeconds(second) >= w)
                    {
                        a(c, r, TimeSpan.FromSeconds(second) < w);
                        return;
                    }
                    System.Threading.Thread.Sleep(500);
                }
            }, new object[] { control, fun, arg, callback, wait });
            task.Start();
        }

        #endregion

        #region Storyboard

        /// <summary>
        /// 使用透明度和显示状态的改变来进行动画显示该控件
        /// </summary>
        /// <param name="element">要执行动画的控件</param>
        /// <param name="duration">持续时间</param>
        /// <param name="completed">执行完成后的行为</param>
        public static void BeginShowWithOpacityAndVisibility(this DependencyObject element, TimeSpan duration, Action completed)
        {
            Storyboard sb = new Storyboard();
            DoubleAnimationUsingKeyFrames daukf = new DoubleAnimationUsingKeyFrames();
            daukf.SetValue(Storyboard.TargetProperty, element);
            daukf.SetValue(Storyboard.TargetPropertyProperty, new PropertyPath("(FrameworkElement.Opacity)"));
            EasingDoubleKeyFrame f1 = new EasingDoubleKeyFrame(0, KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0)));
            EasingDoubleKeyFrame f2 = new EasingDoubleKeyFrame(1, KeyTime.FromTimeSpan(duration));
            daukf.KeyFrames.Add(f1);
            daukf.KeyFrames.Add(f2);
            ObjectAnimationUsingKeyFrames oaukf = new ObjectAnimationUsingKeyFrames();
            oaukf.SetValue(Storyboard.TargetProperty, element);
            oaukf.SetValue(Storyboard.TargetPropertyProperty, new PropertyPath("(UIElement.Visibility)"));
            DiscreteObjectKeyFrame f3 = new DiscreteObjectKeyFrame(Visibility.Visible, KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0)));
            oaukf.KeyFrames.Add(f3);
            sb.Children.Add(daukf);
            sb.Children.Add(oaukf);

            sb.Completed += (s, e) =>
            {
                if (completed != null)
                    completed();
            };

            sb.Begin();
        }

        /// <summary>
        /// 使用透明度和显示状态的改变来进行动画隐藏该控件
        /// </summary>
        /// <param name="element">要执行动画的控件</param>
        /// <param name="duration">持续时间</param>
        /// <param name="completed">执行完成后的行为</param>
        public static void BeginHideWithOpacityAndVisibility(this DependencyObject element, TimeSpan duration, Action completed)
        {
            Storyboard sb = new Storyboard();
            DoubleAnimationUsingKeyFrames daukf = new DoubleAnimationUsingKeyFrames();
            daukf.SetValue(Storyboard.TargetProperty, element);
            daukf.SetValue(Storyboard.TargetPropertyProperty, new PropertyPath("(FrameworkElement.Opacity)"));
            EasingDoubleKeyFrame f1 = new EasingDoubleKeyFrame(1, KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0)));
            EasingDoubleKeyFrame f2 = new EasingDoubleKeyFrame(0, KeyTime.FromTimeSpan(duration));
            daukf.KeyFrames.Add(f1);
            daukf.KeyFrames.Add(f2);
            ObjectAnimationUsingKeyFrames oaukf = new ObjectAnimationUsingKeyFrames();
            oaukf.SetValue(Storyboard.TargetProperty, element);
            oaukf.SetValue(Storyboard.TargetPropertyProperty, new PropertyPath("(UIElement.Visibility)"));
            DiscreteObjectKeyFrame f3 = new DiscreteObjectKeyFrame(Visibility.Visible, KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0)));
            DiscreteObjectKeyFrame f4 = new DiscreteObjectKeyFrame(Visibility.Collapsed, KeyTime.FromTimeSpan(duration));
            oaukf.KeyFrames.Add(f3);
            oaukf.KeyFrames.Add(f4);
            sb.Children.Add(daukf);
            sb.Children.Add(oaukf);

            sb.Completed += (s, e) =>
            {
                if (completed != null)
                    completed();
            };
            sb.Begin();
        }

        /// <summary>
        /// 使用宽度改变来进行动画
        /// </summary>
        /// <param name="element">要执行动画的控件</param>
        /// <param name="from">开始值</param>
        /// <param name="to">结束值</param>
        /// <param name="duration">持续时间</param>
        /// <param name="completed">执行完成后的行为</param>
        public static void BeginHeight(this UIElement element, double from, double to, TimeSpan duration, Action completed)
        {
            Storyboard sb = new Storyboard();
            DoubleAnimationUsingKeyFrames daukf = new DoubleAnimationUsingKeyFrames();
            daukf.SetValue(Storyboard.TargetProperty, element);
            daukf.SetValue(Storyboard.TargetPropertyProperty, new PropertyPath("(FrameworkElement.Height)"));
            EasingDoubleKeyFrame f1 = new EasingDoubleKeyFrame(from, KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0)));
            EasingDoubleKeyFrame f2 = new EasingDoubleKeyFrame(to, KeyTime.FromTimeSpan(duration));
            daukf.KeyFrames.Add(f1);
            daukf.KeyFrames.Add(f2);
            sb.Children.Add(daukf);

            sb.Completed += (s, e) =>
            {
                if (completed != null)
                    completed();
            };
            sb.Begin();
        }

        /// <summary>
        /// 使用宽度改变来进行动画
        /// </summary>
        /// <param name="element">要执行动画的控件</param>
        /// <param name="from">开始值</param>
        /// <param name="to">结束值</param>
        /// <param name="duration">持续时间</param>
        /// <param name="completed">执行完成后的行为</param>
        public static void BeginWidth(this UIElement element, double from, double to, TimeSpan duration, Action completed)
        {
            Storyboard sb = new Storyboard();
            DoubleAnimationUsingKeyFrames daukf = new DoubleAnimationUsingKeyFrames();
            daukf.SetValue(Storyboard.TargetProperty, element);
            daukf.SetValue(Storyboard.TargetPropertyProperty, new PropertyPath("(FrameworkElement.Width)"));
            EasingDoubleKeyFrame f1 = new EasingDoubleKeyFrame(from, KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0)));
            EasingDoubleKeyFrame f2 = new EasingDoubleKeyFrame(to, KeyTime.FromTimeSpan(duration));
            daukf.KeyFrames.Add(f1);
            daukf.KeyFrames.Add(f2);
            sb.Children.Add(daukf);

            sb.Completed += (s, e) =>
            {
                if (completed != null)
                    completed();
            };
            sb.Begin();
        }

        /// <summary>
        /// 使用位移创建一个动画
        /// </summary>
        /// <param name="element">要执行的控件</param>
        /// <param name="duration">持续时间</param>
        /// <param name="xTo">X的最终值</param>
        /// <param name="yTO">Y的最终值</param>
        /// <param name="completed">执行完成后的行为</param>
        public static void BeginTranslateTransform(this DependencyObject element, TimeSpan duration, double xTo, double yTO, Action completed)
        {
            BeginTranslateTransform(element, duration, xTo, yTO, completed, null, null);
        }

        /// <summary>
        /// 使用位移创建一个动画
        /// </summary>
        /// <param name="element">要执行的控件</param>
        /// <param name="duration">持续时间</param>
        /// <param name="xTo">X的最终值</param>
        /// <param name="yTO">Y的最终值</param>
        /// <param name="completed">执行完成后的行为</param>
        public static void BeginTranslateTransform(this DependencyObject element, TimeSpan duration, double xTo, double yTO, Action completed, IEasingFunction functionX, IEasingFunction functionY)
        {
            Storyboard sb = new Storyboard();

            DoubleAnimation da_x = new DoubleAnimation(xTo, duration);
            da_x.SetValue(Storyboard.TargetProperty, element);
            da_x.SetValue(Storyboard.TargetPropertyProperty, new PropertyPath("(UIElement.RenderTransform).(TransformGroup.Children)[3].(TranslateTransform.X)"));
            if (functionX != null)
                da_x.EasingFunction = functionX;
            DoubleAnimation da_y = new DoubleAnimation(yTO, duration);
            da_y.SetValue(Storyboard.TargetProperty, element);
            da_y.SetValue(Storyboard.TargetPropertyProperty, new PropertyPath("(UIElement.RenderTransform).(TransformGroup.Children)[3].(TranslateTransform.Y)"));
            if (functionY != null)
                da_y.EasingFunction = functionY;
            sb.Children.Add(da_x);
            sb.Children.Add(da_y);

            sb.Completed += (s, e) =>
            {
                if (completed != null)
                    completed();
            };

            sb.Begin();
        }


        #endregion

        /// <summary>
        /// 扩展控件在父级控件中是否用户可见。
        /// </summary>
        /// <param name="element">子元素</param>
        /// <param name="parent">父级元素</param>
        /// <returns>子元素在父级元素中是否用户可见</returns>
        public static bool IsUserVisible(this UIElement element, FrameworkElement parent)
        {
            if (!element.IsVisible)
                return false;

            //var container = VisualTreeHelper.GetParent(element) as FrameworkElement;
            if (parent == null)
                throw new ArgumentNullException("container");

            Rect bounds = element.TransformToAncestor(parent).TransformBounds(new Rect(0.0, 0.0, element.RenderSize.Width, element.RenderSize.Height));
            Rect rect = new Rect(0.0, 0.0, parent.ActualWidth, parent.ActualHeight);
            return rect.IntersectsWith(bounds);
        }
    }
}
