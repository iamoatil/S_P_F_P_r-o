// ***********************************************************************
// Assembly:XLY.SF.Project.Plugin.Adapter.FeatureMathch
// Author:Songbing
// Created:2017-04-12 09:59:43
// Description:验证文件夹是否存在
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
    /// 验证文件夹是否存在
    /// </summary>
    internal class PathExistPluginFeatureRule:IFeatureMathchRule
    {
        public string Id { get; set; }

        public string Success { get; set; }

        public string Failure { get; set; }

        public string Path { get; set; }

        public string TryMathch(string sourcePath)
        {
            if(DirectoryHelper.FindDirectories(sourcePath, Path))
            {
                return this.Success;
            }
            else
            {
                return this.Failure;
            }
        }
    }
}
