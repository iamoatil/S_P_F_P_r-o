using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/* ==============================================================================
* Assembly   ：	XLY.SF.Project.DataReport.JsonExportTree
* Description：	  
* Author     ：	fhjun
* Create Date：	2017/9/28 11:27:23
* ==============================================================================*/

namespace XLY.SF.Project.DataReport
{
    /// <summary>
    /// 将数据源导出为Json树，字段名称必须小写
    /// </summary>
    public class JsonExportTree
    {
        /// <summary>
        /// 节点文本
        /// </summary>
        public string text { get; set; }
        /// <summary>
        /// 节点数据对应位置；
        /// 如果是按节点进行数据分页，则为Items的Key值；
        /// 如果是按数据行数分页，则为页的编号及元素ID；
        /// </summary>
        public string location { get; set; }
        /// <summary>
        /// 数据量（比如["324"]）
        /// </summary>
        public string[] tags { get; set; }
        /// <summary>
        /// 节点图标位置
        /// </summary>
        public string icon { get; set; }
        /// <summary>
        /// 子节点列表
        /// </summary>
        public List<JsonExportTree> nodes { get; set; }
    }

    /// <summary>
    /// 将数据源导出为Json表格数据时的列定义，字段名称必须小写
    /// </summary>
    public class JsonExportColumn
    {
        /// <summary>
        /// 字段名称
        /// </summary>
        public string field { get; set; }
        /// <summary>
        /// 列显示标题
        /// </summary>
        public string title { get; set; }
        /// <summary>
        /// 水平对齐方式， 'left', 'right', 'center'
        /// </summary>
        public string align { get; set; }
        /// <summary>
        /// 垂直对齐方式, 'top', 'middle', 'bottom' 
        /// </summary>
        public string valign { get; set; }
        /// <summary>
        /// 列宽，百分比或者像素
        /// </summary>
        public string width { get; set; }
        /// <summary>
        /// 是否可排序
        /// </summary>
        public bool sortable { get; set; } = true;
    }
}
