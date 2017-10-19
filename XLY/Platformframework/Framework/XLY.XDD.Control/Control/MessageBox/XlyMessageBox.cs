using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace XLY.XDD.Control
{
    /// <summary>
    /// Xly消息框
    /// </summary>
    public static class XlyMessageBox
    {
        public static void Init()
        {
            if (MessageBoxWindowStyle == null)
                MessageBoxWindowStyle = XDDGlobal.NoneWindowStyle;
            if (MirrorMessageBoxWindowStyle == null)
                MirrorMessageBoxWindowStyle = XDDGlobal.MirrorInfoWindowStyle;
            if (WindowPool == null)
                WindowPool = new List<Window>();
        }

        /// <summary>
        /// 消息框窗口
        /// </summary>
        public static Style MessageBoxWindowStyle { get; set; }

        /// <summary>
        /// 镜像消息框窗口
        /// </summary>
        public static Style MirrorMessageBoxWindowStyle { get; set; }

        /// <summary>
        /// 消息框窗口池
        /// </summary>
        private static List<Window> WindowPool { get; set; }

        /// <summary>
        /// 销毁所有窗口
        /// </summary>
        public static void Dispose()
        {
            ControlExtension.InvokeEx(() =>
            {
                lock (WindowPool)
                {
                    foreach (Window w in WindowPool)
                    {
                        w.Close();
                        WindowPool.Remove(w);
                    }
                }
            });
        }

        public static void CloseLastWin()
        {
            ControlExtension.InvokeEx(() =>
            {
                lock (WindowPool)
                {
                    if (WindowPool.IsValid())
                    {
                        var win = WindowPool.Last();
                        WindowPool.Remove(win);
                        win.Close();
                    }
                }
            });
        }

        private static void window_Closed(object sender, EventArgs e)
        {
            lock (WindowPool)
            {
                WindowPool.Remove(sender as Window);
            }
        }

        /// <summary>
        /// 显示等待信息
        /// </summary>
        /// <param name="message">提示信息</param>
        /// <param name="isMaximized">是否最大化窗口</param>
        /// <returns>等待窗口</returns>
        public static Window ShowLoading(string message, bool isMaximized = true)
        {
            Window window = null;
            ControlExtension.InvokeEx(() =>
            {
                Init();
                window = new Window();
                lock (WindowPool)
                    WindowPool.Add(window);
                window.Loaded += new RoutedEventHandler(window_Loaded);
                window.Closed += new EventHandler(window_Closed);
                window.SizeChanged += new SizeChangedEventHandler(size_Changed);
                window.Style = MessageBoxWindowStyle;
                XlyMessageBoxLoading loading = new XlyMessageBoxLoading();
                loading.Message = message;
                window.ShowInTaskbar = false;
                window.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                window.Content = loading;
                window.Owner = Application.Current.MainWindow;
                if (isMaximized)
                {
                    window.WindowState = WindowState.Maximized;
                }
                window.Show();
                if (isMaximized)
                {
                    loading.Width = window.ActualWidth;
                    loading.Height = window.ActualHeight;
                }
            });
            return window;
        }


        private static void size_Changed(object sender, SizeChangedEventArgs e)
        {
            Window window = (Window)sender;
            window.Width = Application.Current.MainWindow.Width;
            window.Height = Application.Current.MainWindow.Height;
            window.Top = Application.Current.MainWindow.Top;
            window.Left = Application.Current.MainWindow.Left;
        }

        private static void window_Loaded(object sender, RoutedEventArgs e)
        {
            Window window = sender as Window;
            window.Topmost = true;
            window.Topmost = false;
        }

        /// <summary>
        /// 显示提示信息
        /// </summary>
        /// <param name="message">提示信息</param>
        /// <param name="isShowDialog">是否是用ShowDialog方式弹出</param>
        /// <param name="isTopmost">是否置顶</param>
        public static void ShowInfo(string message, bool isShowDialog = true, bool isTopmost = true)
        {
            ControlExtension.InvokeEx(() =>
            {
                Init();
                Window window = new Window();
                lock (WindowPool)
                    WindowPool.Add(window);
                window.Closed += new EventHandler(window_Closed);
                window.Style = MessageBoxWindowStyle;
                XlyMessageBoxInfo info = new XlyMessageBoxInfo();
                info.Message = message;
                window.ShowInTaskbar = true;
                window.Topmost = isTopmost;
                window.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                window.Owner = Application.Current.MainWindow;
                window.Content = info;
                if (isShowDialog)
                    window.ShowDialog();
                else
                    window.Show();
            });
        }

        /// <summary>
        /// 显示镜像提示信息
        /// </summary>
        /// <param name="message">提示信息</param>
        /// <param name="isShowDialog">是否是用ShowDialog方式弹出</param>
        /// <param name="isTopmost">是否置顶</param>
        public static void ShowMirrorInfo(string message, bool isShowDialog = true, bool isTopmost = true)
        {
            ControlExtension.InvokeEx(() =>
            {
                Init();
                Window window = new Window();
                lock (WindowPool)
                    WindowPool.Add(window);
                window.Closed += new EventHandler(window_Closed);
                window.Style = MirrorMessageBoxWindowStyle;
                XlyMirrorMessageBoxInfo info = new XlyMirrorMessageBoxInfo();
                info.Message = message;
                window.ShowInTaskbar = true;
                window.Topmost = isTopmost;
                window.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                window.Owner = Application.Current.MainWindow;
                window.Content = info;
                if (isShowDialog)
                    window.ShowDialog();
                else
                    window.Show();
            });
        }

        /// <summary>
        /// 显示成功信息
        /// </summary>
        /// <param name="message">成功信息</param>
        /// <param name="isShowDialog">是否是用ShowDialog方式弹出</param>
        public static void ShowSuccess(string message, bool isShowDialog = true)
        {
            ControlExtension.InvokeEx(() =>
            {
                Init();
                Window window = new Window();
                lock (WindowPool)
                    WindowPool.Add(window);
                window.Closed += new EventHandler(window_Closed);
                window.Style = MessageBoxWindowStyle;
                XlyMessageBoxSuccess success = new XlyMessageBoxSuccess();
                success.Message = message;
                window.Topmost = true;
                window.ShowInTaskbar = true;
                window.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                window.Content = success;
                if (isShowDialog)
                    window.ShowDialog();
                else
                    window.Show();
            });
        }

        /// <summary>
        /// 显示警告信息
        /// </summary>
        /// <param name="message">警告信息</param>
        /// <param name="isShowDialog">是否是用ShowDialog方式弹出</param>
        public static void ShowWarning(string message, bool isShowDialog = true)
        {
            ControlExtension.InvokeEx(() =>
            {
                Init();
                Window window = new Window();
                lock (WindowPool)
                    WindowPool.Add(window);
                window.Closed += new EventHandler(window_Closed);
                window.Style = MessageBoxWindowStyle;
                XlyMessageBoxWarning warning = new XlyMessageBoxWarning();
                warning.Message = message;
                window.Topmost = true;
                window.ShowInTaskbar = true;
                window.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                window.Content = warning;
                if (isShowDialog)
                    window.ShowDialog();
                else
                    window.Show();
            });
        }

        /// <summary>
        /// 显示错误信息
        /// </summary>
        /// <param name="message">错误信息</param>
        /// <param name="isShowDialog">是否是用ShowDialog方式弹出</param>
        public static void ShowError(string message, bool isTopmost = true)
        {
            ControlExtension.InvokeEx(() =>
            {
                Init();
                Window window = new Window();
                lock (WindowPool)
                    WindowPool.Add(window);
                window.Closed += new EventHandler(window_Closed);
                window.Style = MessageBoxWindowStyle;
                XlyMessageBoxError error = new XlyMessageBoxError();
                error.Message = message;
                window.ShowInTaskbar = true;
                window.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                window.Owner = Application.Current.MainWindow;
                window.Topmost = isTopmost;
                window.Content = error;
                window.ShowDialog();
            });
        }

        /// <summary>
        /// 显示询问
        /// </summary>
        /// <param name="message">询问信息</param>
        /// <param name="yesLabel">确定按钮标签</param>
        /// <param name="cancelLabel">取消按钮标签</param>
        /// <returns></returns>
        public static bool ShowQuestion(string message, string yesLabel, string cancelLabel, bool isTopmost = true)
        {
            bool? result = false;
            ControlExtension.InvokeEx(() =>
            {
                Init();
                Window window = new Window();
                window.Style = MessageBoxWindowStyle;
                lock (WindowPool)
                    WindowPool.Add(window);
                window.Closed += new EventHandler(window_Closed);
                XlyMessageBoxQuestion question = new XlyMessageBoxQuestion();
                question.Message = message;
                question.YesButtonLabel = yesLabel;
                question.CancelButtonLabel = cancelLabel;
                question.IsThreeButton = false;
                window.ShowInTaskbar = true;
                window.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                window.Owner = Application.Current.MainWindow;
                window.Content = question;
                window.Topmost = isTopmost;
                window.ShowDialog();
                result = question.ResultValue;
            });
            return result ?? false;
        }


        /// <summary>
        /// 显示询问
        /// </summary>
        /// <param name="message">询问信息</param>
        /// <param name="yesLabel">确定按钮标签</param>
        /// <param name="cancelLabel">取消按钮标签</param>
        /// <returns></returns>
        public static bool ShowMirrorQuestion(string message, string yesLabel, string cancelLabel, bool isTopmost = true)
        {
            bool? result = false;
            ControlExtension.InvokeEx(() =>
            {
                Init();
                Window window = new Window();
                window.Style = MirrorMessageBoxWindowStyle;
                lock (WindowPool)
                    WindowPool.Add(window);
                window.Closed += new EventHandler(window_Closed);
                XlyMirrorMessageBoxQuestion question = new XlyMirrorMessageBoxQuestion();
                question.Message = message;
                question.YesButtonLabel = yesLabel;
                question.CancelButtonLabel = cancelLabel;
                question.IsThreeButton = false;
                window.ShowInTaskbar = true;
                window.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                window.Owner = Application.Current.MainWindow;
                window.Content = question;
                window.Topmost = isTopmost;
                window.ShowDialog();
                result = question.ResultValue;
            });
            return result ?? false;
        }

        /// <summary>
        /// 显示询问
        /// </summary>
        /// <param name="message">询问信息</param>
        /// <param name="yesLabel">确定按钮标签</param>
        /// <param name="noLabel">否定按钮标签</param>
        /// <param name="cancelLabel">取消按钮标签</param>
        /// <returns></returns>
        public static bool? ShowQuestion(string message, string yesLabel, string noLabel, string cancelLabel, bool isTopmost = true)
        {
            bool? result = false;
            ControlExtension.InvokeEx(() =>
            {
                Init();
                Window window = new Window();
                lock (WindowPool)
                    WindowPool.Add(window);
                window.Closed += new EventHandler(window_Closed);
                window.Style = MessageBoxWindowStyle;
                XlyMessageBoxQuestion question = new XlyMessageBoxQuestion();
                question.Message = message;
                question.YesButtonLabel = yesLabel;
                question.NoButtonLabel = noLabel;
                question.CancelButtonLabel = cancelLabel;
                question.IsThreeButton = true;
                window.ShowInTaskbar = true;
                window.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                window.Content = question;
                window.Topmost = isTopmost;
                window.ShowDialog();
                result = question.ResultValue;
            });
            return result;
        }

        /// <summary>
        /// 显示输入框
        /// </summary>
        /// <param name="message">标签</param>
        /// <param name="inputText">显示在输入框内的文本</param>
        /// <param name="result">输入字符串</param>
        /// <param name="verification">确定时的数据验证方法</param>
        /// <returns></returns>
        public static bool ShowInput(string message, string inputText, ref string result, Func<string, bool> verification)
        {
            bool r = false;
            string temp = null;
            ControlExtension.InvokeEx(() =>
            {
                Init();
                Window window = new Window();
                lock (WindowPool)
                    WindowPool.Add(window);
                window.Closed += new EventHandler(window_Closed);
                window.Style = MessageBoxWindowStyle;
                XlyMessageBoxInput input = new XlyMessageBoxInput();
                input.Message = message;
                input.InputText = inputText;
                input.OnEnterButtonClick += (s, e) =>
                {
                    if (!verification(input.InputText))
                    {
                        e.Handled = true;
                    }
                };
                window.ShowInTaskbar = true;
                window.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                window.Content = input;
                window.Topmost = true;
                window.ShowDialog();
                r = input.IsEnter;
                temp = input.InputText;
            });
            result = temp;
            return r;
        }

        /// <summary>
        /// 显示输入框
        /// </summary>
        /// <param name="message">标签</param>
        /// <param name="inputText">显示在输入框内的文本</param>
        /// <param name="result">输入字符串</param>
        /// <param name="verification">确定时的数据验证方法</param>
        /// <returns></returns>
        public static bool ShowInputEx(string message, string inputText, ref string result, Func<object, bool> verification)
        {
            bool r = false;
            string temp = null;
            ControlExtension.InvokeEx(() =>
            {
                Init();
                Window window = new Window();
                lock (WindowPool)
                    WindowPool.Add(window);
                window.Closed += new EventHandler(window_Closed);
                window.Style = MessageBoxWindowStyle;
                XlyMessageBoxInput input = new XlyMessageBoxInput();
                input.MaxLength = 20;
                input.Message = message;
                input.InputText = inputText;
                input.OnEnterButtonClick += (s, e) =>
                {
                    if (!verification(input))
                    {
                        e.Handled = true;
                    }
                };
                window.ShowInTaskbar = true;
                window.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                window.Content = input;
                window.Topmost = true;
                window.ShowDialog();
                r = input.IsEnter;
                temp = input.InputText;
            });
            result = temp;
            return r;
        }




    }
}
