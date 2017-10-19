using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XLY.SF.Project.Domains;

namespace XLY.SF.Project.Domains
{
    /// <summary>
    /// 备忘录
    /// </summary>
    public class Memo : AbstractDataItem
    {
        [Display]
        public string Title { get; set; } //标题;

        [Display]
        public string Content { get; set; } //主要内容;

        [Display(Key = "Creation_Time")]
        public string CreationDate { get; set; } //创建时间;

        [Display]
        public string ModificationDate { get; set; } //重定义时间;

    }
}
