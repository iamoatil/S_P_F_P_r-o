using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XLY.SF.Framework.BaseUtility;

namespace XLY.SF.Project.Services
{
    class PathExistPluginFeatureRule : IPluginFeatureRule
    {
        public string Id { get; set; }

        public string Success { get; set; }

        public string Failure { get; set; }

        public string Path { get; set; }

        public string TryMathch(string taskSourcePath)
        {
            var arr = DirectoryHelper.GetDirectories(taskSourcePath, Path);

            if (arr.IsValid())
            {
                return Success;
            }
            else
            {
                return Failure;
            }
        }
    }
}
