using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq;
using System.Text.RegularExpressions;
using XLY.SF.Framework.BaseUtility;
using XLY.SF.Project.Persistable.Primitive;

namespace XLY.SF.Project.ScriptEngine.Engine
{
    /// <summary>
    /// Sqlite数据库操作
    /// </summary>
    public class Sqlite
    {

        /// <summary>
        /// 数据分析恢复：指定源数据文件、特征库，及表名称，表名称多个以逗号','隔开。
        /// 恢复成功返回新的db路径
        /// 恢复失败则返回原来的路径
        /// </summary>
        public string DataRecovery(string sourcedb, string charatorPath, string tableNames, bool isScanDebris)
        {
            return SqliteRecoveryHelper.DataRecovery(sourcedb, charatorPath, tableNames, isScanDebris);
        }

        /// <summary>
        /// 数据分析恢复：指定源数据文件、特征库，及表名称，表名称多个以逗号','隔开。
        /// 恢复成功返回新的db路径
        /// 恢复失败则返回原来的路径
        /// </summary>
        public string DataRecovery(string sourcedb, string charatorPath, string tableNames)
        {
            return SqliteRecoveryHelper.DataRecovery(sourcedb, charatorPath, tableNames);
        }


        /// <summary>
        /// 查询sqlite数据库：参数1：文件路径；参数2:sql语句
        /// columns为列名，多个逗号隔开；encodestr为编码格式，如UTF-8，Unicode，GBK
        /// </summary>
        public string Find(string file, string sql, string columns, string encodestr)
        {
            try
            {
                var list = this._Find(file, sql);
                this.Encode(list, columns, encodestr);
                return JsonConvert.SerializeObject(list);
            }
            catch (Exception ex)
            {
                var str = string.Format("SQL查询失败", file, ex.Message);
                Console.WriteLine(str);
                return string.Empty;
            }
        }

        public string Find(string file, string sql)
        {
            return this.Find(file, sql, string.Empty, string.Empty);
        }

        #region BigFind

        private List<dynamic> list = new List<dynamic>();

        /// <summary>
        /// 执行sql，获取动态类型数据集合
        /// </summary>
        public void BigFindInit(string file, string sql)
        {
            try
            {
                list = this._Find(file, sql).ToList();
                this.Encode(list, string.Empty, string.Empty);
            }
            catch (Exception ex)
            {
                var str = string.Format("SQL查询失败", file, ex.Message);
                Console.WriteLine(str);
            }
        }

        public string BigFind()
        {
            List<dynamic> items = new List<dynamic>();
            if (list.Any())
            {
                items = list.Take(10000).ToList();

                if (list.Count() >= 10000)
                    list.RemoveRange(0, 10000);
                else
                    list.RemoveRange(0, list.Count());

                var result = from u in items
                             select new
                             {
                                 Id = u.fid,
                                 Name = u.file_name,
                                 MD5 = u.file_md5,
                                 Path = u.server_path,
                                 Size = u.file_size + " bytes",
                                 Time = (new Convert()).LinuxToDateTime(u.server_ctime),
                                 DataState = (new Convert()).ToDataState(u.XLY_DataType)
                             };
                return JsonConvert.SerializeObject(result).Replace("[{", "{").Replace("}]", "}");
            }
            else
            {
                return "";
            }
        }

        #endregion

        public string FindAndSort(string file, string sql, string sortKeys = "")
        {
            try
            {
                if (!System.IO.File.Exists(file))
                {
                    Console.WriteLine(string.Format("文件不存在", file));
                    return null;
                }
                SqliteContext context = new SqliteContext(file);
                var list = context.FindDataTableAndSort(sql, sortKeys);
                this.Encode(list, string.Empty, string.Empty);
                return JsonConvert.SerializeObject(list);
            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Format("SQL查询失败", file, ex.Message));
                throw;
            }
        }

        /// <summary>
        /// 查询sqlite数据库：参数1：文件路径；参数2:表名称
        /// columns为列名，多个逗号隔开；encodestr为编码格式，如UTF-8，Unicode，GBK
        /// </summary>
        public string FindByName(string file, string name, string columns, string encodestr)
        {
            try
            {
                var sql = string.Format("Select * from {0}", name);
                var list = this._Find(file, sql);
                this.Encode(list, columns, encodestr);
                return list == null ? "[]" : JsonConvert.SerializeObject(list);
            }
            catch (Exception ex)
            {
                var str = string.Format("SQL查询失败", file, ex.Message);
                Console.WriteLine(str);
                return string.Empty;
            }
        }

        public string FindByName(string file, string name)
        {
            return this.FindByName(file, name, string.Empty, string.Empty);
        }

        private IEnumerable<dynamic> _Find(string file, string sql)
        {
            try
            {
                if (!System.IO.File.Exists(file))
                {
                    Console.WriteLine(string.Format("文件不存在", file));
                    return null;
                }
                using(SqliteContext context = new SqliteContext(file))
                {
                    var res = context.Find(sql);
                    return res;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Format("SQL查询失败", file, ex.Message));
                throw;
            }
        }

        /// <summary>
        /// 对byte数据进行转码 。
        /// columns为列名，多个逗号隔开；encodestr为编码格式，如UTF-8，Unicode，GBK
        /// </summary>
        private void Encode(IEnumerable<dynamic> items, string columns, string encodestr)
        {
            if (String.IsNullOrEmpty(columns) || String.IsNullOrEmpty(encodestr)) return;
            if (items==null) return;
            var cs = columns.Replace("，", ",").Split(',');
            var encode = System.Text.Encoding.GetEncoding(encodestr);
            foreach (var item in items)
            {
                try
                {
                    DynamicEx obj = (DynamicEx)item;
                    foreach (var c in cs)
                    {
                        var value = obj.Get(c) as byte[];
                        obj.Set(c, encode.GetString(value).Replace("\0", ""));
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("转码异常" + e.Message);
                }

            }
        }

        /// <summary>
        /// 检查表是否存在
        /// </summary>
        /// <param name="file"></param>
        /// <param name="tablename"></param>
        /// <returns></returns>
        public bool TableIsExist(string file, string tablename)
        {
            try
            {
                SqliteContext context = new SqliteContext(file);
                return context.Exist(tablename);
            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Format("SQL查询失败", file, ex.Message));
                throw;
            }
        }

        /// <summary>
        /// 查询数据库，同时通过正则表达式过滤数据
        /// </summary>
        /// <param name="file">db文件</param>
        /// <param name="sql">SQL</param>
        /// <param name="colNames">过滤的列，和regex一一对应</param>
        /// <param name="regex">过滤的列对应的正则表达式，和regex一一对应</param>
        /// <returns></returns>
        public string FindByRegex(string file, string sql, string[] colNames, string[] regex)
        {
            if (colNames!=null || regex!=null)
            {
                return Find(file, sql);
            }

            try
            {
                if (colNames.Length != regex.Length)
                {
                    throw new Exception("the length of columns is not equal the length of regex");
                }
                
                //var list = this._Find(file, sql);
                if (!System.IO.File.Exists(file))
                {
                    Console.WriteLine(string.Format("文件不存在", file));
                    return null;
                }
                SqliteContext context = new SqliteContext(file);
                var dt = context.FindDataTable(sql);

                Dictionary<string, Regex> dicRegexes = new Dictionary<string, Regex>();
                for (int j = 0; j < colNames.Length; j++)
                {
                    dicRegexes[colNames[j]] = new Regex(regex[j]);
                }

                for (int i = dt.Rows.Count - 1; i >= 0; i--)
                {
                    if (dicRegexes.Any(r => !r.Value.IsMatch(dt.Rows[i][r.Key].ToString())))        //如果某列值不满足正则匹配，则删除该行数据
                    {
                        dt.Rows.RemoveAt(i);
                    }
                }

                var list = ToDynamicCollection(dt);
                this.Encode(list, string.Empty, string.Empty);
                return JsonConvert.SerializeObject(list);
            }
            catch (Exception ex)
            {
                var str = string.Format("SQL查询失败", file, ex.Message);
                Console.WriteLine(str);
                return string.Empty;
            }
        }
        public List<dynamic> ToDynamicCollection(DataTable dt)
        {
            if (dt == null || dt.Rows.Count <= 0 || dt.Columns.Count <= 0)
            {
                return new List<dynamic>();
            }
            var items = new List<dynamic>();
            foreach (DataRow row in dt.Rows)
            {
                dynamic dyn = new ExpandoObject();
                foreach (DataColumn column in dt.Columns)
                {
                    var dic = (IDictionary<string, object>)dyn;
                    dic[column.ColumnName] = row[column];
                }
                items.Add(dyn);
            }
            return items;
        }
    }
}
