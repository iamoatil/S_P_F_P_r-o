using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XLY.SF.Framework.BaseUtility;

namespace XLY.SF.Project.Services
{
    class FileExistPluginFeatureRule : IPluginFeatureRule
    {
        public string Id { get; set; }

        public string Success { get; set; }

        public string Failure { get; set; }

        public string Path { get; set; }

        public string FileName { get; set; }

        public string TryMathch(string taskSourcePath)
        {
            var arr = DirectoryHelper.GetDirectories(taskSourcePath, Path);
            if (arr.IsValid())
            {
                if (arr.Any((d) => DirectoryHelper.GetFiles(d, FileName).IsValid()))
                {
                    return Success;
                }
                else
                {
                    return Failure;
                }
            }
            else
            {
                return Failure;
            }
        }

    }
}
