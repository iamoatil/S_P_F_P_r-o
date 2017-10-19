using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
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
using System.Reflection;
using System.Utility.Helper;
using DevExpress.Data.PLinq.Helpers;

namespace XLY.XDD.Control
{
    /// <summary>
    /// 详细信息激活控件
    /// </summary>
    [TemplatePart(Name = "PART_Button", Type = typeof(Button))]
    public class GridViewDetailActiveControl : System.Windows.Controls.Control
    {
        static GridViewDetailActiveControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(GridViewDetailActiveControl), new FrameworkPropertyMetadata(typeof(GridViewDetailActiveControl)));
        }

        #region IsFilterNullOrWhiteSpaceFild -- 是否过滤空列

        /// <summary>
        /// 是否过滤空列
        /// </summary>
        public bool IsFilterNullOrWhiteSpaceFild
        {
            get { return (bool)GetValue(IsFilterNullOrWhiteSpaceFildProperty); }
            set { SetValue(IsFilterNullOrWhiteSpaceFildProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsFilterNullOrWhiteSpaceFild.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsFilterNullOrWhiteSpaceFildProperty =
            DependencyProperty.Register("IsFilterNullOrWhiteSpaceFild", typeof(bool), typeof(GridViewDetailActiveControl), new UIPropertyMetadata(false));

        #endregion

        #region DateTimeStringFormat -- 时间日期字符格式化

        /// <summary>
        /// 时间日期字符格式化
        /// </summary>
        public string DateTimeStringFormat
        {
            get { return (string)GetValue(DateTimeStringFormatProperty); }
            set { SetValue(DateTimeStringFormatProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DateTimeStringFormat.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DateTimeStringFormatProperty =
            DependencyProperty.Register("DateTimeStringFormat", typeof(string), typeof(GridViewDetailActiveControl), new UIPropertyMetadata("yyyy-MM-dd HH:mm:ss"));

        #endregion

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            this.PART_Button = this.Template.FindName("PART_Button", this) as Button;
            this.PART_Button.Click -= new RoutedEventHandler(PART_DetailButton_Click);
            this.PART_Button.Click += new RoutedEventHandler(PART_DetailButton_Click);
        }

        /// <summary>
        /// 获取当前列属性
        /// </summary>
        /// <param name="type">要获取的对象的类型</param>
        /// <param name="filed">要获取的列名</param>
        /// <param name="objtarget">要获取的对象</param>
        /// <returns></returns>
        private string _GetValue(Type type, string filed, object objtarget)
        {
            string[] fileds_args = filed.Trim().Split('.');
            Type temp_type = type;
            object obj = objtarget;
            for (int i = 0; i < fileds_args.Length; ++i)
            {
                string f = fileds_args[i];

                PropertyInfo pinfo = temp_type.GetProperty(f);
                if (pinfo == null)
                    return string.Empty;
                if (pinfo.PropertyType == typeof(DateTime))
                {
                    DateTime _value = (DateTime)pinfo.GetValue(obj);
                    if (_value == DateTime.MinValue || _value == DateTime.MaxValue)
                        return string.Empty;
                    else
                        return _value.ToString(this.DateTimeStringFormat);
                }
                if (pinfo.PropertyType == typeof(DateTime?))
                {
                    DateTime? _value = (DateTime?)pinfo.GetValue(obj);
                    if (_value == null)
                        return string.Empty;
                    if (_value.Value == DateTime.MinValue || _value.Value == DateTime.MaxValue)
                        return string.Empty;
                    else
                        return _value.Value.ToString(this.DateTimeStringFormat);
                }
                if (pinfo.PropertyType == typeof(bool))
                {
                    bool _value = (bool)pinfo.GetValue(obj);
                    if (_value)
                        return "是";
                    else
                        return "否";
                }
                if (pinfo.PropertyType == typeof(bool?))
                {
                    bool? _value = (bool?)pinfo.GetValue(obj);
                    if (_value == true)
                    {
                        return "是";
                    }
                    else if (_value == false)
                    {
                        return "否";
                    }
                    else
                    {
                        return string.Empty;
                    }
                }
                if (pinfo.PropertyType == typeof (Dictionary<string, string>))
                {
                    //exif信息后面处理
                    return string.Empty;
                }
                obj = pinfo.GetValue(obj);
                if (i == fileds_args.Length - 1)
                {
                    return obj.ToSafeString();
                }
                temp_type = pinfo.PropertyType;
            }
            return string.Empty;
        }

        private void PART_DetailButton_Click(object sender, RoutedEventArgs e)
        {
            DevExpress.Xpf.Grid.EditGridCellData data = this.DataContext as DevExpress.Xpf.Grid.EditGridCellData;
            List<GridViewDetailInfo> list = new List<GridViewDetailInfo>();
            if (data.RowData.Row != null)
            {
                Type rowType = data.RowData.Row.GetType();
                bool isfoler = false;

                if (rowType.Name == "FileX" || rowType.Name == "FileEntity")
                {
                    string proName = string.Empty;
                    if (rowType.Name == "FileX")
                    {
                        proName = "Type";
                    }
                    else if (rowType.Name == "FileEntity")
                    {
                        proName = "ProxyFileX.Type";
                    }

                    string filetype = this._GetValue(rowType, proName, data.RowData.Row);
                    if (filetype == "Folder")
                    {
                        isfoler = true;
                    }
                }

                foreach (DevExpress.Xpf.Grid.GridColumnData c in data.RowData.CellData)
                {
                    if (this.IsFilterNullOrWhiteSpaceFild && c.Column.Header.ToSafeString().IsNullOrEmptyOrWhiteSpace())
                        continue;

                    GridViewDetailInfo detailInfo = new GridViewDetailInfo();
                    detailInfo.Label = c.Column.Header.ToSafeString();
                    detailInfo.Text = this._GetValue(rowType, c.Column.FieldName, data.RowData.Row);

                    if (detailInfo.Label == "物理大小" || detailInfo.Label == "逻辑大小")
                    {
                        if (isfoler)
                        {
                            detailInfo.Text = string.Empty;
                        }
                        else
                        {
                            long t;
                            bool b = long.TryParse(detailInfo.Text, out t);

                            if (b)
                            {
                                detailInfo.Text = System.Utility.Helper.File.GetFileSize(t);
                            }
                        }
                    }

                    list.Add(detailInfo);
                }

                var exifInfoDetails = ReadExifInfos(data.RowData.Row);
                if (exifInfoDetails.IsValid())
                {
                    list.AddRange(exifInfoDetails);
                }
            }
            else
            {
                foreach (DevExpress.Xpf.Grid.GridColumnData c in data.RowData.CellData)
                {
                    if (this.IsFilterNullOrWhiteSpaceFild && c.Column.Header.ToSafeString().IsNullOrEmptyOrWhiteSpace())
                        continue;
                    list.Add(new GridViewDetailInfo { Label = c.Column.Header.ToSafeString(), Text = string.Empty });
                }
            }

            Window window = null;
            if (GetDetailWindow != null)
                window = GetDetailWindow();
            else
                window = new Window();
            GridViewDetailControl detail = new GridViewDetailControl {ItemsSource = list};

            window.Content = detail;
            window.Owner = Window.GetWindow(data.View);
            window.ShowDialog();
        }

        /// <summary>
        /// 读取EXIF信息
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        private List<GridViewDetailInfo> ReadExifInfos(object file)
        {
            var result = new List<GridViewDetailInfo>();

            try
            {
                var t = file.GetType();
                var exifInfo = t.GetProperty("ExifInfoDic");
                if (exifInfo != null)
                {
                    var exifInfoDic = exifInfo.GetValue(file) as Dictionary<string, string>;
                    if (exifInfoDic != null)
                    {
                        foreach (var exifInfoData in exifInfoDic)
                        {
                            result.Add(new GridViewDetailInfo
                            {
                                Text = exifInfoData.Value,
                                Label = exifInfoData.Key
                            });
                        }
                    }
                }
            }
            catch
            {
            }
            


            return result;
        }

        /// <summary>
        /// 获取详细信息窗口
        /// </summary>
        public static Func<Window> GetDetailWindow;

        #region PART

        private Button PART_Button;

        #endregion
    }
}
