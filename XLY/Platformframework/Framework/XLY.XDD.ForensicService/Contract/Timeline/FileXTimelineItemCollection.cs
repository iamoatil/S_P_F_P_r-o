using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace XLY.XDD.ForensicService
{
    /// <summary>
    /// FileX的时间轴项 集合。通过构造函数可直接把数据集转换为时间轴集合
    /// </summary>
    public class FileXTimelineItemCollection : List<FileXTimelineItem>
    {
        #region FileXTimelineItemCollection-构造函数（初始化）

        /// <summary>
        ///  FileXTimelineItemCollection-构造函数（初始化）
        /// </summary>
        public FileXTimelineItemCollection(IEnumerable<IFileX> files)
        {
            // min data
            var min1 = files.Min(s => s.CreationDate);
            var min2 = files.Min(s => s.LastAccessDate);
            var min3 = files.Min(s => s.LastWriteDate);
            var min = new Collection<DateTime> { min1, min2, min3 }.Min();
            //max data
            var max1 = files.Max(s => s.CreationDate);
            var max2 = files.Max(s => s.LastAccessDate);
            var max3 = files.Max(s => s.LastWriteDate);
            var max = new Collection<DateTime> { max1, max2, max3 }.Max();

            //generate items
            for (int i = min.Year; i <= max.Year; i++)
            {
                var item = new FileXTimelineItem(i, EnumDateTime.Year);
                item.Text = string.Concat(i, item.Type.GetDescription());
                this.Add(item);
            }
            //analysis files
            foreach (var obj in files)
            {
                var yitems = this.Where(s => s.Contain(obj));
                foreach (var yitem in yitems)
                {
                    yitem.Add(obj);
                }
            }
            //remove empty year
            this.RemoveAll(s => s.LastAccessTotal == 0 && s.CreationTotoal == 0 && s.LastWriteTotal == 0);
        }

        #endregion
    }
}