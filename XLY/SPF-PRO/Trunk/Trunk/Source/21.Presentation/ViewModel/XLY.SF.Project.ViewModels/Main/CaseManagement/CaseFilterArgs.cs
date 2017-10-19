using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XLY.SF.Project.ViewModels.Main.CaseManagement
{
    public class CaseFilterArgs
    {
        public String Keyword { get; set; }

        public DateTime? Begin { get; set; }

        public DateTime? End { get; set; }

    }
}
