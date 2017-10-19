using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace _07.XLY.SF.Project.TestModuls
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App:Application
    {

        protected override void OnStartup(StartupEventArgs e)
        {
            //Test tt = new Test();

            //tt.TestTT();

            //var uri = new Uri("/PresentationFramework.Classic, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35;component/themes/classic.xaml", UriKind.Relative);
            //App.Current.Resources.Source = uri;

            base.OnStartup(e);
        }

    }

    public class Test
    {
        public void TestTT()
        {
            //var buff = System.Text.Encoding.Unicode.GetBytes("E:\\USBMonitorService.exe");

            //var res = USBMonitorCoreDllHelper.USBMonitor_Service_Init(buff);

            //res = USBMonitorCoreDllHelper.USBMonitor_Start((status, vidpid) =>
            //    {
            //        Console.WriteLine("USBMonitor: " + vidpid + " " + status);

            //        return 0;
            //    });
        }
    }

}
