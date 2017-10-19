// ***********************************************************************
// Assembly:XLY.SF.Project.Plugin.Adapter.FeatureMathch
// Author:Songbing
// Created:2017-04-12 09:58:36
// Description:验证文件是否存在
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
    /// 验证文件是否存在
    /// </summary>
    internal class FileExistPluginFeatureRule:IFeatureMathchRule
    {
        public string Id { get; set; }

        public string Success { get; set; }

        public string Failure { get; set; }

        public string Path { get; set; }

        public string FileName { get; set; }

        public string TryMathch(string sourcePath)
        {
            var arr = DirectoryHelper.GetDirectories(sourcePath, Path);
            if(arr.IsValid())
            {
                if(arr.Any((d) => DirectoryHelper.GetFiles(d, FileName).IsValid()))
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
