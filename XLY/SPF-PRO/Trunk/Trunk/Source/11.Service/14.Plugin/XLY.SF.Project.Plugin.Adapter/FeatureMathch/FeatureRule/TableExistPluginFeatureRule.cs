// ***********************************************************************
// Assembly:XLY.SF.Project.Plugin.Adapter.FeatureMathch
// Author:Songbing
// Created:2017-04-12 10:00:37
// Description:验证数据库表是否存在
// ***********************************************************************
// Last Modified By:
// Last Modified On:
// ***********************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XLY.SF.Framework.BaseUtility;


namespace XLY.SF.Project.Plugin.Adapter.FeatureMathch
{
    /// <summary>
    /// 验证数据库表是否存在
    /// </summary>
    internal class TableExistPluginFeatureRule:IFeatureMathchRule
    {
        public string Id { get; set; }

        public string Success { get; set; }

        public string Failure { get; set; }

        public string Path { get; set; }

        public string DbFileName { get; set; }

        public string TableName { get; set; }

        public bool Decryted { get; set; }

        public string TryMathch(string sourcePath)
        {
            var arr = DirectoryHelper.GetDirectories(sourcePath, Path);
            if(arr.IsValid())
            {
                List<string> files = new List<string>();
                foreach(var d in arr)
                {
                    files.AddRange(DirectoryHelper.GetFiles(d, DbFileName));
                }

                if(files.Any((f) => DbHelper.IsExistTable(f, TableName, Decryted)))
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
