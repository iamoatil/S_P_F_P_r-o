// ***********************************************************************
// Assembly:XLY.SF.Project.Plugin.Adapter.FeatureMathch
// Author:Songbing
// Created:2017-04-12 11:23:53
// Description:数据库辅助类
// ***********************************************************************
// Last Modified By:
// Last Modified On:
// ***********************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace XLY.SF.Project.Plugin.Adapter.FeatureMathch
{
    /// <summary>
    /// 数据库辅助类
    /// </summary>
    internal static class DbHelper
    {
        /// <summary>
        /// 判断表是否存在
        /// </summary>
        /// <param name="dbfile">数据库</param>
        /// <param name="tablename">表名</param>
        /// <returns></returns>
        public static bool IsExistTable(string dbfile, string tablename, bool decryted = false)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 判断表字段是否存在
        /// </summary>
        /// <param name="dbfile">数据库</param>
        /// <param name="tablename">表名</param>
        /// <param name="fieldName">字段名</param>
        /// <returns></returns>
        public static bool IsExistTableField(string dbfile, string tablename, string fieldName, bool decryted = false)
        {
            throw new NotImplementedException();
        }
    }
}
