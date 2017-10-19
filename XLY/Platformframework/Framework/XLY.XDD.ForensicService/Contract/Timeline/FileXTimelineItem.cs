using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace XLY.XDD.ForensicService
{
    /// <summary>
    /// 基于文件对象的时间轴项
    /// </summary>
    public class FileXTimelineItem : BaseTimelineItem
    {
        /// <summary>
        /// 创建文件总数
        /// </summary>
        public int CreationTotoal { get { return this.CreationFiles.Count; } }
        /// <summary>
        /// 最后修改文件总数
        /// </summary>
        public int LastWriteTotal { get { return this.LastWriteFiles.Count; } }
        /// <summary>
        /// 最后访问时间总数
        /// </summary>
        public int LastAccessTotal { get { return this.LastAccessFiles.Count; } }

        public List<IFileX> CreationFiles { get; set; }
        public List<IFileX> LastAccessFiles { get; set; }
        public List<IFileX> LastWriteFiles { get; set; }

        #region FileXTimelineItem-构造函数（初始化）

        /// <summary>
        ///  FileXTimelineItem-构造函数（初始化）
        /// </summary>
        public FileXTimelineItem(int value, EnumDateTime type)
        {
            this.CreationFiles = new List<IFileX>();
            this.LastAccessFiles = new List<IFileX>();
            this.LastWriteFiles = new List<IFileX>();
            this.Type = type;
            this.Value = value;
        }

        #endregion

        /// <summary>
        /// 添加文件
        /// </summary>
        /// <param name="file"></param>
        public virtual void Add(IFileX file)
        {
            //this.add
            if (this.ContainCreationTime(file))
            {
                this.CreationFiles.Add(file);
                this.AddChildItem(file.CreationDate, file, DateType.Creation);
            }
            if (this.ContainLastAccessTime(file))
            {
                this.LastAccessFiles.Add(file);
                this.AddChildItem(file.LastAccessDate, file, DateType.Access);
            }
            if (this.ContainLastWriteTime(file))
            {
                this.LastWriteFiles.Add(file);
                this.AddChildItem(file.LastWriteDate, file, DateType.Write);
            }
        }

        private void Add(IFileX file, DateType dtype)
        {
            //this.add
            if (dtype == DateType.Creation && this.ContainCreationTime(file))
            {
                this.CreationFiles.Add(file);
                this.AddChildItem(file.CreationDate, file, dtype);
            }
            if (dtype == DateType.Access && this.ContainLastAccessTime(file))
            {
                this.LastAccessFiles.Add(file);
                this.AddChildItem(file.LastAccessDate, file, dtype);
            }
            if (dtype == DateType.Write && this.ContainLastWriteTime(file))
            {
                this.LastWriteFiles.Add(file);
                this.AddChildItem(file.LastWriteDate, file, dtype);
            }
        }

        private void AddChildItem(DateTime dt, IFileX file, DateType dtype)
        {
            switch (this.Type)
            {
                case EnumDateTime.Year:
                    var mitme = this.AddChildItem(dt.Year * 100 + dt.Month, EnumDateTime.Month);
                    mitme.Text = dt.Month + mitme.Type.GetDescription();
                    mitme.Add(file, dtype);
                    break;
                case EnumDateTime.Month:
                    var ditme = this.AddChildItem(dt.Year * 10000 + dt.Month * 100 + dt.Day, EnumDateTime.Day);
                    ditme.Text = dt.Day.ToString();
                    ditme.Add(file, dtype);
                    break;
            }
        }

        private FileXTimelineItem AddChildItem(int value, EnumDateTime type)
        {
            if (this.Items.IsInvalid()) this.Items = new List<BaseTimelineItem>();
            var citem = this.Items.FirstOrDefault(s => s.Value == value);
            if (citem == null)
            {
                citem = new FileXTimelineItem(value, type);
                this.Items.Add(citem);
                this.Items = this.Items.OrderBy(s => s.Value).ToList();
            }
            return citem as FileXTimelineItem;
        }

        /// <summary>
        /// 验证该项的时间值是否匹配指定文件
        /// </summary>
        public virtual bool Contain(IFileX file)
        {
            return this.ContainCreationTime(file) || this.ContainLastAccessTime(file) || this.ContainLastWriteTime(file);
        }

        /// <summary>
        /// 验证创建时间是否匹配指定文件
        /// </summary>
        public bool ContainCreationTime(IFileX file)
        {
            return this.Contain(file.CreationDate);
        }
        /// <summary>
        /// 验证最后访问时间是否匹配指定文件
        /// </summary>
        public bool ContainLastAccessTime(IFileX file)
        {
            return this.Contain(file.LastAccessDate);
        }
        /// <summary>
        /// 验证最后修改时间是否匹配指定文件
        /// </summary>
        public bool ContainLastWriteTime(IFileX file)
        {
            return this.Contain(file.LastWriteDate);
        }

        /// <summary>
        /// 验证时间是否可以包含在该时间轴内.
        /// true表示可以包含在内
        /// </summary>
        public bool Contain(DateTime dt)
        {
            switch (this.Type)
            {
                case EnumDateTime.Year:
                    return this.Value == dt.Year;
                case EnumDateTime.Month:
                    return this.Value == dt.Year * 100 + dt.Month;
                case EnumDateTime.Day:
                    return this.Value == dt.Year * 10000 + dt.Month * 100 + dt.Day;
            }
            return false;
        }

        /// <summary>
        /// 文件日期类型
        /// </summary>
        internal enum DateType
        {
            Creation,
            Access,
            Write,
        }
    }
}