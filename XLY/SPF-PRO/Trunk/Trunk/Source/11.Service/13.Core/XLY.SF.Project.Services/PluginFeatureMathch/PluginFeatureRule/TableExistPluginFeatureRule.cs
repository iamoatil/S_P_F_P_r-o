using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XLY.SF.Framework.BaseUtility;

namespace XLY.SF.Project.Services
{
    class TableExistPluginFeatureRule : IPluginFeatureRule
    {
        public string Id { get; set; }

        public string Success { get; set; }

        public string Failure { get; set; }

        public string Path { get; set; }

        public string DbFileName { get; set; }

        public string TableName { get; set; }

        public bool Decryted { get; set; }

        public string TryMathch(string taskSourcePath)
        {
            var arr = DirectoryHelper.GetDirectories(taskSourcePath, Path);
            if (arr.IsValid())
            {
                List<string> files = new List<string>();
                foreach (var d in arr)
                {
                    files.AddRange(DirectoryHelper.GetFiles(d, DbFileName));
                }

                if (files.Any((f) => DbHelper.IsExistTable(f, TableName, Decryted)))
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
