using System;
using System.Collections.Generic;
using System.IO;
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
using XLY.SF.Framework.BaseUtility;

namespace XLY.SF.Project.DataView
{
    /// <summary>
    /// HtmlViewControl.xaml 的交互逻辑
    /// </summary>
    public partial class HtmlViewControl : UserControl
    {
        public HtmlViewControl(string url, object source)
        {
            InitializeComponent();

            this.Url = url;     //@"C:\Users\fhjun\Desktop\htmp\323\index.html"
            this.DataSource = source;
            SaveDataSource();
            _operator.OnSelectedDataChanged += OnSelectedDataChanged;

            web1.Navigate(new Uri(this.Url, UriKind.RelativeOrAbsolute));
            web1.ObjectForScripting = _operator;
        }

        private WebOperator _operator = new WebOperator();

        public event DelgateDataViewSelectedItemChanged OnSelectedDataChanged;

        public string Url { get; set; }
        public object DataSource { get; set; }

        private void SaveDataSource()
        {
            if(DataSource == null)
            {
                return;
            }
            FileInfo fi = new FileInfo(Url);
            if (!fi.Exists)
            {
                return;
            }
            string fileName = System.IO.Path.Combine(fi.DirectoryName, "__data.js");
            File.WriteAllText(fileName, $"var __data = {Serializer.JsonSerilize(DataSource)}", Encoding.UTF8);
        }
    }

    [System.Runtime.InteropServices.ComVisibleAttribute(true)]//将该类设置为com可访问  
    public class WebOperator
    {
        public event DelgateDataViewSelectedItemChanged OnSelectedDataChanged;
        public void SelectItem(object obj)
        {
            OnSelectedDataChanged?.Invoke(obj);
            //MessageBox.Show($"you click {obj}");
        }
    }
}
