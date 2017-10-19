using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows;

namespace XLY.XDD.Control
{
    /// <summary>
    /// 列表详细信息按钮可见性转化器
    /// </summary>
    public class GridViewDetailActiveControlIsOpenConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            try
            {
                bool isMouseOver = (bool)values[0];
                if (isMouseOver == false)
                    return false;
                FrameworkElement element = values[1] as FrameworkElement;
                if (element == null)
                    return false;
                object selected_obj = values[2];
                DevExpress.Xpf.Grid.EditGridCellData cell_data = element.DataContext as DevExpress.Xpf.Grid.EditGridCellData;
                if (cell_data == null)
                    return false;
                object target_obj = cell_data.RowData.Row;
                return isMouseOver && (selected_obj == target_obj);
            }
            catch
            {
                return false;
            }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
