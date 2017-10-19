using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using XLY.SF.Framework.Log4NetService;
using XLY.SF.Framework.BaseUtility;
using X64Service;

namespace XLY.SF.Project.Persistable.Primitive
{
    /// <summary>
    /// Sqlite数据库恢复助手。
    /// </summary>
    public class SqliteRecoveryHelper
    {
        #region  全局字段

        //删除内容
        private const int DeleteData = 1;

        //正常内容
        private const int NormalData = 2;

        //扫描碎片数据
        private const int ScanData = 4;

        /// <summary>
        /// 数据模型
        /// </summary>
        private byte _DataMode;

        private const string NewColumnName = "XLY_DataType";

        private string licenseFile = "sqliteKey.dat";

        //sqlite 回调
        //private  SqliteGeneralCallBack _CallBack;

        private List<List<SqliteColumnObject>> _AllNewRowData;

        private Encoding _CurrentEncoding = Encoding.UTF8;

        #endregion

        private SqliteRecoveryHelper()
        {

        }

        /// <summary>
        /// 数据恢复。
        /// </summary>
        /// <param name="sourcedb">源数据库db文件路径。</param>
        /// <param name="charatorPath">特征库文件（可空）</param>
        /// <param name="tableNames">多个表之间请按照","分隔，如 t1,t2,t3</param>
        /// <param name="isScanDebris">是否扫描碎片数据</param>
        /// <returns>恢复成功，则返回新数据库db文件。(极端)失败则返回源来的数据库db文件。</returns>
        public static string DataRecovery(string sourcedb, string charatorPath, string tableNames, bool isScanDebris = false)
        {
            return new SqliteRecoveryHelper()._DataRecovery(sourcedb, charatorPath, tableNames, null, null, isScanDebris, null);
        }

        /// <summary>
        /// 数据恢复（SPF版本，添加了特征扫描功能） 
        /// </summary>
        /// <param name="sourcedb">源数据库db文件路径。</param>
        /// <param name="charatorPath">特征库文件（可空）</param>
        /// <param name="tableNames">多个表之间请按照","分隔，如 t1,t2,t3</param>
        /// <param name="baseDbPath">基础数据库</param>
        /// <param name="baseCharatorPath">特征扫描特征库</param>
        /// <param name="isScanDebris">是否扫描碎片数据</param>
        /// <param name="index">索引列, 表名--索引列名</param>
        /// <returns>恢复成功，则返回新数据库db文件。(极端)失败则返回源来的数据库db文件。</returns>
        public static string DataRecovery(string sourcedb, string charatorPath, string tableNames,
            string baseDbPath, string baseCharatorPath, bool isScanDebris = false, Dictionary<string, string> index = null)
        {
            return new SqliteRecoveryHelper()._DataRecovery(sourcedb, charatorPath, tableNames, baseDbPath, baseCharatorPath, isScanDebris, index);
        }

        /// <summary>
        /// 调用底层方法，获取所有表名
        /// </summary>
        /// <param name="sourceDb">源数据库路径</param>
        /// <returns></returns>
        public static List<string> ButtomGetAllTables(string sourceDb)
        {
            return new SqliteRecoveryHelper()._ButtomGetAllTables(sourceDb, null);
        }

        /// <summary>
        /// 调用底层方法，获取表的所有列名
        /// </summary>
        /// <param name="sourceDb">源数据库路径</param>
        /// <param name="tableName">表名</param>
        /// <returns></returns>
        public static List<string> ButtomGetTableDefin(string sourceDb, string tableName)
        {
            return new SqliteRecoveryHelper()._ButtomGetTableDefin(sourceDb, tableName);
        }

        #region 私有方法

        /// <summary>
        /// 数据恢复。
        /// </summary>
        /// <param name="sourcedb">源数据库db文件路径。</param>
        /// <param name="charatorPath">特征库文件（可空）</param>
        /// <param name="tableNames">多个表之间请按照","分隔，如 t1,t2,t3</param>
        /// <param name="isScanDebris">是否扫描碎片数据</param>
        /// <returns>恢复成功，则返回新数据库db文件。(极端)失败则返回源来的数据库db文件。</returns>
        private string _DataRecovery(string sourcedb, string charatorPath, string tableNames, bool isScanDebris = false)
        {
            if ((String.IsNullOrWhiteSpace(sourcedb)) || (!File.Exists(sourcedb)))
            {
                LoggerManagerSingle.Instance.Error(string.Format("Sqlite恢复文件【{0}】不存在，或者文件大小为0，无法处理。", sourcedb));
                return sourcedb;
            }

            if (isScanDebris)
            {
                _DataMode = NormalData + DeleteData + ScanData;
            }
            else
            {
                _DataMode = NormalData + DeleteData;
            }

            try
            {
                var name = Path.GetFileName(sourcedb);
                var path = Path.GetDirectoryName(sourcedb);
                var ext = Path.GetExtension(sourcedb);
                var newfile = Path.Combine(path, string.Format("{0}_recovery.{1}", name.TrimEnd(ext.ToArray()).TrimEnd('.'), ext.TrimStart('.'))).TrimEnd('.');

                if (File.Exists(newfile))
                {
                    try
                    {
                        File.Delete(newfile);
                    }
                    catch
                    {
                        newfile = Path.Combine(path, string.Format("{0}_recovery_{1}.{2}", name.TrimEnd(ext.ToArray()).TrimEnd('.'), DateTime.Now.Second, ext)).TrimEnd('.');
                    }
                }

                if (charatorPath.IsValid())
                {
                    charatorPath = Path.GetFullPath(charatorPath);
                }

                //表列表
                var tableArray = tableNames.Replace('，', ',').TrimEnd(new[] { ',', ' ' }).Split(',');

                //底层C++处理
                var res = ButtomDataRecovery(sourcedb, charatorPath, newfile, tableArray);

                //若底层处理不成功，采用上层数据处理。
                if (!res.IsSucess)
                {
                    LoggerManagerSingle.Instance.Info(string.Format("Sqlite数据库恢复底层C++恢复文件【{0}】失败，系统进入上层C#（正常数据）恢复流程。", sourcedb));
                    return TopDataRecovery(sourcedb, newfile, tableArray);
                }

                return newfile;
            }
            catch (Exception ex)
            {
                LoggerManagerSingle.Instance.Error(ex, string.Format("文件[{0}]执行数据恢复出错：{1}", sourcedb, ex.AllMessage()));
                return sourcedb;
            }
        }

        /// <summary>
        /// 数据恢复（SPF版本，添加了特征扫描功能） 2016/10/20 songbing ADD
        /// </summary>
        /// <param name="sourcedb">源数据库db文件路径。</param>
        /// <param name="charatorPath">特征库文件（可空）</param>
        /// <param name="tableNames">多个表之间请按照","分隔，如 t1,t2,t3</param>
        /// <param name="baseDbPath">基础数据库</param>
        /// <param name="baseCharatorPath">特征扫描特征库</param>
        /// <param name="isScanDebris">是否扫描碎片数据</param>
        /// <param name="index">索引列, 表名--索引列名</param>
        /// <returns>恢复成功，则返回新数据库db文件。(极端)失败则返回源来的数据库db文件。</returns>
        private string _DataRecovery(string sourcedb, string charatorPath, string tableNames,
            string baseDbPath, string baseCharatorPath, bool isScanDebris = false, Dictionary<string, string> index = null)
        {
            if ((String.IsNullOrWhiteSpace(sourcedb)) || (!File.Exists(sourcedb)))
            {
                LoggerManagerSingle.Instance.Error(string.Format("Sqlite恢复文件【{0}】不存在，或者文件大小为0，无法处理。", sourcedb));
                return sourcedb;
            }

            if (isScanDebris)
            {
                _DataMode = NormalData + DeleteData + ScanData;
            }
            else
            {
                _DataMode = NormalData + DeleteData;
            }

            try
            {
                var name = Path.GetFileName(sourcedb);
                var path = Path.GetDirectoryName(sourcedb);
                var ext = Path.GetExtension(sourcedb);
                var newfile = Path.Combine(path, string.Format("{0}_recovery.{1}", name.TrimEnd(ext.ToArray()).TrimEnd('.'), ext.TrimStart('.'))).TrimEnd('.');

                if (File.Exists(newfile))
                {
                    try
                    {
                        File.Delete(newfile);
                    }
                    catch
                    {
                        newfile = Path.Combine(path, string.Format("{0}_recovery_{1}.{2}", name.TrimEnd(ext.ToArray()).TrimEnd('.'), DateTime.Now.Second, ext)).TrimEnd('.');
                    }
                }

                if (charatorPath.IsValid())
                {
                    charatorPath = Path.GetFullPath(charatorPath);
                }

                //表列表
                var tableArray = tableNames.Replace('，', ',').TrimEnd(new[] { ',', ' ' }).Split(',');

                //底层C++处理
                var res = ButtomDataRecoveryForSPF(sourcedb, charatorPath, newfile, tableArray, baseDbPath, baseCharatorPath, index);

                //若底层处理不成功，采用上层数据处理。
                if (!res.IsSucess)
                {
                    LoggerManagerSingle.Instance.Info(string.Format("Sqlite数据库恢复底层C++恢复文件【{0}】失败，系统进入上层C#（正常数据）恢复流程。", sourcedb));
                    return TopDataRecovery(sourcedb, newfile, tableArray);
                }

                return newfile;
            }
            catch (Exception ex)
            {
                LoggerManagerSingle.Instance.Error(ex, string.Format("文件[{0}]执行数据恢复出错：{1}", sourcedb, ex.AllMessage()));
                return sourcedb;
            }
        }

        private void DeepScanMirror(string mirrorPath, string baseDbFile, string baseCharatorPath, string tableNames, string newDbPath)
        {
            IntPtr tempDbBase = IntPtr.Zero;
            if (InitDb(baseDbFile, baseCharatorPath, ref tempDbBase))
            {
                foreach (var tableName in tableNames.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries))
                {
                    _AllNewRowData = new List<List<SqliteColumnObject>>();
                    var res = SqliteCoreDll.getContentFromFile(tempDbBase, Sqlite3_General_OtherScanCallBack, mirrorPath, SqliteCallBack, tableName);
                    if (0 == res)
                    {
                        //获取表定义
                        GetTableDefin(tempDbBase, tableName, out allColumnNames, out allColumnTypes);
                        if (0 == allColumnNames.Count)
                        {
                            LoggerManagerSingle.Instance.Error(string.Format("特征扫描后获取表定义失败,SOURCE=[{0}] [{1}]", baseDbFile, tableName));
                        }

                        var context = new SqliteContext(newDbPath);
                        curContext = context;

                        if (CreateTable(context, tableName, allColumnNames, allColumnTypes))
                        {
                            #region 插入数据
                            //若底层获取了数据，则把底层的数据写入副本对应的表中。
                            //高效批量插入多条数据（采用事务机制）
                            // 高效事务批量插入数据库。
                            // 由于SQLite特殊性，它是文件存储的，每一次插入都是一次IO操作
                            //为了高效插入，引入事务机制，先在内存中插入，最后一次性提交到数据库。
                            try
                            {
                                context.UsingSafeTransaction(command =>
                                {
                                    //获取插入表的SQL(多条)语句。
                                    var notIdColumns = new StringBuilder();
                                    allColumnNames.Skip(1).ForEach(name => notIdColumns.Append(name + ","));
                                    notIdColumns.Append(NewColumnName);

                                    var hasIdColumns = new StringBuilder();
                                    // 过滤字段中特殊符号(`);
                                    for (int i = 0; i < allColumnNames.Count; i++)
                                    {
                                        allColumnNames[i] = allColumnNames[i].Replace("`", "");
                                    }
                                    allColumnNames.ForEach(name => hasIdColumns.Append(name + ","));

                                    hasIdColumns.Append(NewColumnName);

                                    //对数据进行分组合并，按正常、删除、碎片顺序存储。
                                    var allRowData = GetAllRowData();

                                    foreach (var row in allRowData)
                                    {
                                        var parameters = new List<SQLiteParameter>();
                                        var valuesHead = string.Empty;
                                        StringBuilder columns;

                                        if (row[0].Value.ToString().IsNullOrEmpty())
                                        {//主键为空，一般为Integer的数字
                                            columns = notIdColumns;
                                        }
                                        else
                                        {//主键为存在
                                            columns = hasIdColumns;
                                            string primaryKeyId = "@" + allColumnNames[0];
                                            var parameter = new SQLiteParameter(primaryKeyId, DbType.String);
                                            if (row[0].Type == ColumnType.BLOB)
                                            {
                                                parameter.DbType = DbType.Binary;
                                            }

                                            parameter.Value = row[0].Value;
                                            valuesHead = primaryKeyId + ",";
                                            parameters.Add(parameter);
                                        }

                                        var valuesBody = new StringBuilder();
                                        valuesBody.Append(valuesHead);

                                        var formTwoDatas = row.Skip(1);

                                        int columnIndex = 1;
                                        foreach (var cell in formTwoDatas)
                                        {
                                            string paramName = "@" + allColumnNames[columnIndex];
                                            var parameter = new SQLiteParameter(paramName, DbType.String);
                                            if (cell.Type == ColumnType.BLOB)
                                            {
                                                parameter.DbType = DbType.Binary;
                                            }
                                            parameter.Value = cell.Value;
                                            parameters.Add(parameter);
                                            valuesBody.Append(paramName + ",");
                                            columnIndex++;
                                        }

                                        valuesBody.Append(row[0].DataState);
                                        var insertSql = string.Format("insert into \"{0}\"({1}) values({2})", tableName, columns, valuesBody);
                                        command.CommandText = insertSql;
                                        command.Parameters.AddRange(parameters.ToArray());

                                        try
                                        {
                                            command.ExecuteNonQuery();
                                        }
                                        catch (Exception ex)
                                        {
                                            var msg = string.Format("Sqlite插入表【{0}】发生异常 \n SQL语句为： {1}", tableName, insertSql);
                                            LoggerManagerSingle.Instance.Warn(ex, msg);
                                        }
                                    }
                                });
                            }
                            catch (Exception ex)
                            {
                                LoggerManagerSingle.Instance.Error(string.Format("特征扫描插入数据出错"), ex);
                                break;
                            }
                            #endregion

                            continue;
                        }
                    }
                    else
                    {
                        LoggerManagerSingle.Instance.Error(string.Format("特征扫描失败,SOURCE=[{0}] [{1}],错误码 {2}", baseDbFile, tableName, res));
                    }
                }
            }
        }

        /// <summary>
        /// 是否存在某个表。
        /// </summary>
        /// <param name="context">SQLite上下文对象。</param>
        /// <param name="tableName">表名字（注意大小写）。</param>
        /// <returns>存在返回True，反之返回false。</returns>
        private bool IsExistTable(SqliteContext context, string tableName)
        {
            if (tableName.IsInvalid())
            {
                return false;
            }
            return context.Exist(tableName);
        }

        /// <summary>
        /// 获取所有用户信息表
        /// </summary>
        /// <returns></returns>
        private List<string> GetAllTables(SqliteContext context)
        {
            try
            {
                var tables = context.Find(new SQLiteString("select * from sqlite_master where type = 'table'"));
                return tables.Select(table => DynamicConvert.ToSafeString(table.name)).Cast<string>().ToList();
            }
            catch
            {
                LoggerManagerSingle.Instance.Error("上层获取数据库表信息失败. 转底层获取!");
                return ButtomGetAllTables(context.DataSource);
            }
        }

        private Encoding GetSqliteEcoding(string sourceDb)
        {
            IntPtr dbBase = IntPtr.Zero;
            InitDb(sourceDb, string.Empty, ref dbBase);
            //清理资源
            DisposeSource(dbBase);
            return _CurrentEncoding;
        }

        /// <summary>
        /// 调用底层方法，获取所有表名
        /// </summary>
        /// <param name="sourceDb">源数据库路径</param>
        /// <param name="charatorPath">特征库文件路径</param>
        /// <returns></returns>
        private List<string> _ButtomGetAllTables(string sourceDb, string charatorPath)
        {
            var tableNames = new List<string>();
            IntPtr dbBase = IntPtr.Zero;
            var stackMsgBuilder = new StringBuilder();
            bool isSuccess = true;
            IntPtr tableArr = IntPtr.Zero;
            int tableCount = 0;

            //判断是否初始化，若初始化底层失败，则返回。
            if (!InitDb(sourceDb, charatorPath, ref dbBase))
            {
                stackMsgBuilder.AppendLine("SQLite底层DLl初始化失败。可能原因");
                stackMsgBuilder.AppendLine("1：程序未使用管理员权限运行。");
                stackMsgBuilder.AppendLine("2：底层DLL缺少必要的Key文件。");
                isSuccess = false;
            }

            if (isSuccess)
            {
                SqliteCoreDll.GetAllTableName(dbBase, ref tableArr, ref tableCount);
                tableNames = ConvertToArray(tableArr, tableCount);
                SqliteCoreDll.FreeALLTableName(dbBase, tableArr, tableCount);
            }
            else
            {
                LoggerManagerSingle.Instance.Error(stackMsgBuilder.ToSafeString());
            }

            SqliteCoreDll.CloseSqliteHandle(dbBase);

            return tableNames;
        }

        /// <summary>
        /// 调用底层方法，获取表的所有列名
        /// </summary>
        /// <param name="sourceDb">源数据库路径</param>
        /// <param name="tableName">表名</param>
        /// <returns></returns>
        private List<string> _ButtomGetTableDefin(string sourceDb, string tableName)
        {
            var stackMsgBuilder = new StringBuilder();
            IntPtr dbBase = IntPtr.Zero;
            string charatorPath = "";
            bool isSuccess = true;

            //判断是否初始化，若初始化底层失败，则返回。
            if (!InitDb(sourceDb, charatorPath, ref dbBase))
            {
                stackMsgBuilder.AppendLine("SQLite底层DLl初始化失败。可能原因");
                stackMsgBuilder.AppendLine("1：程序未使用管理员权限运行。");
                stackMsgBuilder.AppendLine("2：底层DLL缺少必要的Key文件。");
                isSuccess = false;
            }

            IList<string> allColumnNames = null;
            IList<string> allColumnTypes = null;

            if (isSuccess)
            {
                GetTableDefin(dbBase, tableName, out allColumnNames, out allColumnTypes);
            }
            else
            {
                LoggerManagerSingle.Instance.Error(stackMsgBuilder.ToSafeString());
            }

            SqliteCoreDll.CloseSqliteHandle(dbBase);

            return allColumnNames.IsValid() ? allColumnNames.ToList() : new List<string>();
        }

        /// <summary>
        /// 设置授权文件路径全路径
        /// 如果需要使用自己的指定授权文件，需要在数据恢复之前调用该方法
        /// </summary>
        /// <param name="path">license文件全路径</param>
        private void SetLicenseFile(string path)
        {
            licenseFile = path;
        }

        #endregion

        #region 核心处理

        /// <summary>
        /// 上层C#数据处理。
        /// 上传恢复的数据只有删除的数据。
        /// </summary>
        /// <param name="sourcedb">数据源db。</param>
        /// <param name="newfile">新备份文件db。</param>
        /// <param name="tableArray">表列表。</param>
        /// <returns>返回新的备份文件db。</returns>
        private string TopDataRecovery(string sourcedb, string newfile, IEnumerable<string> tableArray)
        {
            var oldcontext = new SqliteContext(sourcedb);
            System.IO.File.Copy(sourcedb, newfile, true);
            var newcontext = new SqliteContext(newfile);
            foreach (var tableName in tableArray)
            {
                //判断原库中是否含有表，若存在该表则处理，否则不处理。
                if (!IsExistTable(oldcontext, tableName))
                {
                    LoggerManagerSingle.Instance.Warn(string.Format("Sqlite数据库恢复-上层C#恢复库【{0}】的表【{1}】失败，可能是表名拼写不准确（如大小写或表名存在特殊字符等）", sourcedb, tableName));
                    continue;
                }

                //添加新的一列
                string alterTablesql = string.Format("ALTER TABLE {0} ADD COLUMN {1} INTEGER default 2", tableName, NewColumnName);
                newcontext.ExecuteNonQuery(alterTablesql);
            }

            newcontext.Dispose();
            return newfile;
        }

        /// <summary>
        /// 底层c++处理数据库。
        /// 数据库处理后，请使用新数据库进行查找。
        /// </summary>
        /// <param name="sourceDb">源数据库路径。</param>
        /// <param name="charatorPath">特征库文件路径。</param>
        /// <param name="newDbPath">临时数据库路径。</param>
        /// <param name="tableNames">表名（特别注意表名拼写正确）集合,多个表之间请按照","分隔，如 t1,t2,t3</param>
        /// <returns>处理结果，成功则SqliteReturn对象的IsSucess = true，StackMsg为空。
        /// 注意若是多个表进行同时处理，只要有一个表处理成功，IsSucess= true。
        /// 其他异常错误信息，可从StackMsg属性中获取概要信息；出错时查看日志文件，可得到更多栈信息。</returns>
        private SqliteReturn ButtomDataRecovery(string sourceDb, string charatorPath, string newDbPath, string[] tableNames)
        {
            var sqliteReturn = new SqliteReturn { IsSucess = false, StackMsg = string.Empty };
            if (string.IsNullOrEmpty(sourceDb) || string.IsNullOrEmpty(newDbPath) || tableNames.Length <= 0)
            {
                sqliteReturn.StackMsg = "传入参数【源数据库路径，备份路径，表列表】不能存在空值。";
                return sqliteReturn;
            }

            var context = new SqliteContext(newDbPath);
            bool isInit = false;
            IntPtr dbBase = IntPtr.Zero;
            var stackMsgBuilder = new StringBuilder();
            bool isSuccess = true;

            foreach (var tableName in tableNames)
            {
                //判断是否含有表,则直接跳过处理。
                if (IsExistTable(context, tableName))
                {
                    stackMsgBuilder.AppendLine(string.Format("副本数据库 【{0}】中已经存在表【{1}】，系统未对该表进行处理。", newDbPath, tableName));
                    continue;
                }

                //判断是否初始化，若初始化底层失败，则返回。
                if (isInit == false)
                {
                    if (InitDb(sourceDb, charatorPath, ref dbBase))
                    {
                        isInit = true;
                    }
                    else
                    {
                        stackMsgBuilder.AppendLine("SQLite底层DLl初始化失败。可能原因");
                        stackMsgBuilder.AppendLine("1：程序未使用管理员权限运行。");
                        stackMsgBuilder.AppendLine("2：底层DLL缺少必要的Key文件。");
                        isSuccess = false;
                        break;
                    }
                }

                //获取表定义
                IList<string> allColumnNames;
                IList<string> allColumnTypes;
                GetTableDefin(dbBase, tableName, out allColumnNames, out allColumnTypes);
                if (allColumnNames.Count == 0)
                {
                    isSuccess = false;
                    stackMsgBuilder.AppendLine(string.Format("无法获取表【{0}】的定义,可能原因：", tableName));
                    stackMsgBuilder.AppendLine(string.Format("1：原数据库中不存在表【{0}】，请注意表名大小写（用SQLite工具查看核实）。", tableName));
                    stackMsgBuilder.AppendLine("2：c++底层DLL存在错误。");
                    break;
                }

                #region 创建表，插入数据

                //先从底层获取当前表的数据
                GetTableAllData(dbBase, tableName);

                #region 修正字段类型

                if (_AllNewRowData.Count > 0)
                {
                    var tempData = _AllNewRowData[0];
                    if (tempData.Count == allColumnTypes.Count)
                    {
                        for (int pos = 0; pos < tempData.Count; pos++)
                        {
                            switch (tempData[pos].Type)
                            {
                                case ColumnType.TEXT://文本
                                    allColumnTypes[pos] = "TEXT";
                                    break;
                                case ColumnType.INTEGER://整数
                                    allColumnTypes[pos] = "INTEGER";
                                    break;
                                case ColumnType.BLOB://二进制
                                    allColumnTypes[pos] = "BLOB";
                                    break;
                                case ColumnType.DOUBLE://浮点数
                                    allColumnTypes[pos] = "REAL";
                                    break;
                            }
                        }
                    }
                }

                #endregion

                //创建表架构
                if (CreateTable(context, tableName, allColumnNames, allColumnTypes))
                {
                    //若底层获取了数据，则把底层的数据写入副本对应的表中。
                    if (_AllNewRowData.Count > 0)
                    {
                        //高效批量插入多条数据（采用事务机制）
                        // 高效事务批量插入数据库。
                        // 由于SQLite特殊性，它是文件存储的，每一次插入都是一次IO操作
                        //为了高效插入，引入事务机制，先在内存中插入，最后一次性提交到数据库。
                        try
                        {
                            context.UsingSafeTransaction(command =>
                            {
                                //获取插入表的SQL(多条)语句。
                                var notIdColumns = new StringBuilder();
                                allColumnNames.Skip(1).ForEach(name => notIdColumns.Append(name + ","));
                                notIdColumns.Append(NewColumnName);

                                var hasIdColumns = new StringBuilder();
                                // 过滤字段中特殊符号(`);
                                for (int i = 0; i < allColumnNames.Count; i++)
                                {
                                    allColumnNames[i] = allColumnNames[i].Replace("`", "");
                                }
                                allColumnNames.ForEach(name => hasIdColumns.Append(name + ","));

                                hasIdColumns.Append(NewColumnName);

                                //对数据进行分组合并，按正常、删除、碎片顺序存储。
                                var allRowData = GetAllRowData();

                                foreach (var row in allRowData)
                                {
                                    var parameters = new List<SQLiteParameter>();
                                    var valuesHead = string.Empty;
                                    StringBuilder columns;

                                    if (row[0].Value.ToString().IsNullOrEmpty())
                                    {//主键为空，一般为Integer的数字
                                        columns = notIdColumns;
                                    }
                                    else
                                    {//主键为存在
                                        columns = hasIdColumns;
                                        string primaryKeyId = "@" + allColumnNames[0];
                                        var parameter = new SQLiteParameter(primaryKeyId, DbType.String);
                                        if (row[0].Type == ColumnType.BLOB)
                                        {
                                            parameter.DbType = DbType.Binary;
                                        }

                                        parameter.Value = row[0].Value;
                                        valuesHead = primaryKeyId + ",";
                                        parameters.Add(parameter);
                                    }

                                    var valuesBody = new StringBuilder();
                                    valuesBody.Append(valuesHead);

                                    var formTwoDatas = row.Skip(1);

                                    int columnIndex = 1;
                                    foreach (var cell in formTwoDatas)
                                    {
                                        string paramName = "@" + allColumnNames[columnIndex];
                                        var parameter = new SQLiteParameter(paramName, DbType.String);
                                        if (cell.Type == ColumnType.BLOB)
                                        {
                                            parameter.DbType = DbType.Binary;
                                        }
                                        parameter.Value = cell.Value;
                                        parameters.Add(parameter);
                                        valuesBody.Append(paramName + ",");
                                        columnIndex++;
                                    }

                                    valuesBody.Append(row[0].DataState);
                                    var insertSql = string.Format("insert into \"{0}\" ({1}) values({2})", tableName, columns, valuesBody);
                                    command.CommandText = insertSql;
                                    command.Parameters.AddRange(parameters.ToArray());

                                    try
                                    {
                                        command.ExecuteNonQuery();
                                    }
                                    catch (Exception ex)
                                    {
                                        var msg = string.Format("Sqlite插入表【{0}】发生异常 \n SQL语句为： {1}", tableName, insertSql);
                                        LoggerManagerSingle.Instance.Warn(ex, msg);
                                    }
                                }
                            });
                        }
                        catch (Exception ex)
                        {
                            var msg = string.Format("Sqlite数据库恢复-Sqlite插入表【{0}】发生异常", tableName);
                            stackMsgBuilder.AppendLine(msg);
                            LoggerManagerSingle.Instance.Warn(ex, msg);
                            isSuccess = false;
                            break;
                        }
                    }
                }
                else
                {
                    isSuccess = false;
                    stackMsgBuilder.AppendLine(string.Format("无法在备份库中创建表【{0}】，详细信息可在日志文件中查看。", tableName));
                    break;
                }

                #endregion
            }

            //清理资源
            DisposeSource(dbBase);

            sqliteReturn.IsSucess = isSuccess;
            sqliteReturn.StackMsg = stackMsgBuilder.ToString();
            return sqliteReturn;
        }

        /// <summary>
        /// 底层c++处理数据库(SPF版本，添加特征扫描)  2016/10/20 songbing ADD
        /// 数据库处理后，请使用新数据库进行查找。
        /// </summary>
        /// <param name="sourceDb">源数据库路径。</param>
        /// <param name="charatorPath">特征库文件路径。</param>
        /// <param name="newDbPath">临时数据库路径。</param>
        /// <param name="tableNames">表名（特别注意表名拼写正确）集合,多个表之间请按照","分隔，如 t1,t2,t3</param>
        /// <param name="baseDbPath">基础数据库</param>
        /// <param name="baseCharatorPath">特征扫描特征库</param>
        /// <param name="index">索引列, 表名--索引列名</param>
        /// <returns>处理结果，成功则SqliteReturn对象的IsSucess = true，StackMsg为空。
        /// 注意若是多个表进行同时处理，只要有一个表处理成功，IsSucess= true。
        /// 其他异常错误信息，可从StackMsg属性中获取概要信息；出错时查看日志文件，可得到更多栈信息。</returns>
        private SqliteReturn ButtomDataRecoveryForSPF(string sourceDb, string charatorPath, string newDbPath, string[] tableNames, string baseDbPath, string baseCharatorPath, Dictionary<string, string> index = null)
        {
            var sqliteReturn = new SqliteReturn { IsSucess = false, StackMsg = string.Empty };
            if (sourceDb.IsNullOrEmpty() || newDbPath.IsNullOrEmpty() || tableNames.Length <= 0)
            {
                sqliteReturn.StackMsg = "传入参数【源数据库路径，备份路径，表列表】不能存在空值。";
                return sqliteReturn;
            }

            var context = new SqliteContext(newDbPath);

            curContext = context;

            bool isInit = false;
            IntPtr dbBase = IntPtr.Zero;
            var stackMsgBuilder = new StringBuilder();
            bool isSuccess = true;

            foreach (var tableName in tableNames)
            {
                //判断是否含有表,则直接跳过处理。
                if (IsExistTable(context, tableName))
                {
                    stackMsgBuilder.AppendLine(string.Format("副本数据库 【{0}】中已经存在表【{1}】，系统未对该表进行处理。", newDbPath, tableName));
                    continue;
                }

                //判断是否初始化，若初始化底层失败，则返回。
                if (isInit == false)
                {
                    if (InitDb(sourceDb, charatorPath, ref dbBase))
                    {
                        isInit = true;
                    }
                    else
                    {
                        stackMsgBuilder.AppendLine("SQLite底层DLl初始化失败。可能原因");
                        stackMsgBuilder.AppendLine("1：程序未使用管理员权限运行。");
                        stackMsgBuilder.AppendLine("2：底层DLL缺少必要的Key文件。");
                        isSuccess = false;
                        break;
                    }
                }

                //清理对象
                curCommand = null;
                bCreateTable = false;
                sCurTableName = "";
                sCurIndexName = "";
                allColumnNames = null;
                allColumnTypes = null;

                //索引列
                string indexName = "";
                if (index.IsValid() && index.Keys.Contains(tableName))
                {
                    indexName = index[tableName];
                }

                //获取表定义
                GetTableDefin(dbBase, tableName, out allColumnNames, out allColumnTypes);

                if (allColumnNames.Count == 0)
                {
                    #region 文件扫描

                    if (baseDbPath.IsValid() && baseCharatorPath.IsValid())
                    {
                        //做文件扫描
                        LoggerManagerSingle.Instance.Info(string.Format("开始特征扫描,SOURCE=[{0}] [{1}]", sourceDb, tableName));

                        IntPtr tempDbBase = IntPtr.Zero;
                        if (InitDb(baseDbPath, baseCharatorPath, ref tempDbBase))
                        {
                            //获取表的正常和删除数据。
                            _AllNewRowData = new List<List<SqliteColumnObject>>();

                            var res = SqliteCoreDll.getContentFromFile(tempDbBase, Sqlite3_General_OtherScanCallBack, sourceDb, SqliteCallBack, tableName);
                            if (0 == res)
                            {
                                //获取表定义
                                GetTableDefin(tempDbBase, tableName, out allColumnNames, out allColumnTypes);
                                if (0 == allColumnNames.Count)
                                {
                                    LoggerManagerSingle.Instance.Error(string.Format("特征扫描后获取表定义失败,SOURCE=[{0}] [{1}]", sourceDb, tableName));
                                }
                                if (CreateTable(context, tableName, allColumnNames, allColumnTypes, indexName))
                                {
                                    #region 插入数据
                                    //若底层获取了数据，则把底层的数据写入副本对应的表中。
                                    //高效批量插入多条数据（采用事务机制）
                                    // 高效事务批量插入数据库。
                                    // 由于SQLite特殊性，它是文件存储的，每一次插入都是一次IO操作
                                    //为了高效插入，引入事务机制，先在内存中插入，最后一次性提交到数据库。
                                    try
                                    {
                                        context.UsingSafeTransaction(command =>
                                        {
                                            //获取插入表的SQL(多条)语句。
                                            var notIdColumns = new StringBuilder();
                                            allColumnNames.Skip(1).ForEach(name => notIdColumns.Append(name + ","));
                                            notIdColumns.Append(NewColumnName);

                                            var hasIdColumns = new StringBuilder();
                                            // 过滤字段中特殊符号(`);
                                            for (int i = 0; i < allColumnNames.Count; i++)
                                            {
                                                allColumnNames[i] = allColumnNames[i].Replace("`", "");
                                            }
                                            allColumnNames.ForEach(name => hasIdColumns.Append(name + ","));

                                            hasIdColumns.Append(NewColumnName);

                                            //对数据进行分组合并，按正常、删除、碎片顺序存储。
                                            var allRowData = GetAllRowData();

                                            foreach (var row in allRowData)
                                            {
                                                var parameters = new List<SQLiteParameter>();
                                                var valuesHead = string.Empty;
                                                StringBuilder columns;

                                                if (row[0].Value.ToString().IsNullOrEmpty())
                                                {//主键为空，一般为Integer的数字
                                                    columns = notIdColumns;
                                                }
                                                else
                                                {//主键为存在
                                                    columns = hasIdColumns;
                                                    string primaryKeyId = "@" + allColumnNames[0];
                                                    var parameter = new SQLiteParameter(primaryKeyId, DbType.String);
                                                    if (row[0].Type == ColumnType.BLOB)
                                                    {
                                                        parameter.DbType = DbType.Binary;
                                                    }

                                                    parameter.Value = row[0].Value;
                                                    valuesHead = primaryKeyId + ",";
                                                    parameters.Add(parameter);
                                                }

                                                var valuesBody = new StringBuilder();
                                                valuesBody.Append(valuesHead);

                                                var formTwoDatas = row.Skip(1);

                                                int columnIndex = 1;
                                                foreach (var cell in formTwoDatas)
                                                {
                                                    string paramName = "@" + allColumnNames[columnIndex];
                                                    var parameter = new SQLiteParameter(paramName, DbType.String);
                                                    if (cell.Type == ColumnType.BLOB)
                                                    {
                                                        parameter.DbType = DbType.Binary;
                                                    }
                                                    parameter.Value = cell.Value;
                                                    parameters.Add(parameter);
                                                    valuesBody.Append(paramName + ",");
                                                    columnIndex++;
                                                }

                                                valuesBody.Append(row[0].DataState);
                                                var insertSql = string.Format("insert into \"{0}\"({1}) values({2})", tableName, columns, valuesBody);
                                                command.CommandText = insertSql;
                                                command.Parameters.AddRange(parameters.ToArray());

                                                try
                                                {
                                                    command.ExecuteNonQuery();
                                                }
                                                catch (Exception ex)
                                                {
                                                    var msg = string.Format("Sqlite插入表【{0}】发生异常 \n SQL语句为： {1}", tableName, insertSql);
                                                    LoggerManagerSingle.Instance.Warn(ex, msg);
                                                }
                                            }
                                        });
                                    }
                                    catch (Exception ex)
                                    {
                                        var msg = string.Format("Sqlite数据库恢复-Sqlite插入表【{0}】发生异常", tableName);
                                        stackMsgBuilder.AppendLine(msg);
                                        LoggerManagerSingle.Instance.Warn(ex, msg);
                                        isSuccess = false;
                                        break;
                                    }
                                    #endregion

                                    continue;
                                }
                            }
                            else
                            {
                                LoggerManagerSingle.Instance.Error(string.Format("特征扫描失败,SOURCE=[{0}] [{1}],错误码 {2}", sourceDb, tableName, res));
                            }
                        }
                    }
                    else
                    {
                        isSuccess = false;
                        stackMsgBuilder.AppendLine(string.Format("无法获取表【{0}】的定义,可能原因：", tableName));
                        stackMsgBuilder.AppendLine(string.Format("1：原数据库中不存在表【{0}】，请注意表名大小写（用SQLite工具查看核实）。", tableName));
                        stackMsgBuilder.AppendLine("2：c++底层DLL存在错误。");
                        break;
                    }

                    #endregion
                }
                else
                {
                    //获取表的正常和删除数据。
                    _AllNewRowData = new List<List<SqliteColumnObject>>();

                    bCreateTable = true;
                    sCurTableName = tableName;
                    sCurIndexName = indexName;

                    curContext.UsingSafeTransaction(command =>
                    {
                        curCommand = command;

                        int getCotentCode = SqliteCoreDll.getTableContentGenearal(dbBase, SqliteCallBackTTTT, tableName, _DataMode);
                        if (getCotentCode != 0)
                        {
                            LoggerManagerSingle.Instance.Error(string.Format("Sqlite数据库恢复-Sqlite底层读取表[{0}]记录发生错误，错误码：{1}", tableName, getCotentCode));
                        }

                        if (bCreateTable)
                        {
                            CreateTable(context, tableName, allColumnNames, allColumnTypes, indexName);
                        }
                    });
                }
            }

            //清理资源
            DisposeSource(dbBase);

            sqliteReturn.IsSucess = isSuccess;
            sqliteReturn.StackMsg = stackMsgBuilder.ToString();
            return sqliteReturn;
        }

        private SqliteContext curContext = null;
        private SQLiteCommand curCommand = null;
        private bool bCreateTable = false;
        private string sCurTableName = "";
        private string sCurIndexName = "";
        private IList<string> allColumnNames;
        private IList<string> allColumnTypes;

        /// <summary>
        /// Sqilte获取表数据的回调函数。
        /// </summary>
        /// <param name="count">返回表的行数。</param>
        /// <param name="objectPointer">列数组指针。</param>
        /// <param name="dataType">每一行的数据类型（正常，删除，扫描）</param>
        /// <returns>默认返回0。</returns>
        private int SqliteCallBackTTTT(int count, IntPtr objectPointer, byte dataType)
        {
            var cellObjects = new List<SqliteColumnObject>();
            for (int i = 0; i < count; i++)
            {
                var columnObject = objectPointer.ToStruct<ColumnObject>();

                var sqliteColumn = new SqliteColumnObject();
                cellObjects.Add(sqliteColumn);

                sqliteColumn.DataState = dataType;
                sqliteColumn.Length = columnObject.Length;
                if (columnObject.Length == 0)
                {
                    sqliteColumn.Value = "";
                    objectPointer = objectPointer.Increment<ColumnObject>();
                    continue;
                }

                if (columnObject.ColumnType == 1)
                {//Integer
                    byte[] bytes = GetIntAndDoubleBytes(columnObject.Value, columnObject.Length);
                    sqliteColumn.Value = BitConverter.ToInt64(bytes, 0);
                    sqliteColumn.Type = ColumnType.INTEGER;
                }
                else if (columnObject.ColumnType == 2)
                {//Double
                    byte[] bytes = GetIntAndDoubleBytes(columnObject.Value, columnObject.Length);
                    sqliteColumn.Value = BitConverter.ToDouble(bytes, 0);
                    sqliteColumn.Type = ColumnType.DOUBLE;
                }
                else if (columnObject.ColumnType == 3)
                {//Text
                    var bytes = new byte[columnObject.Length];
                    Marshal.Copy(columnObject.Value, bytes, 0, columnObject.Length);
                    //string tempValue = Encoding.Default.GetString(Encoding.Convert(_CurrentEncoding, Encoding.Default, bytes));
                    //Modify：chenjing 为了民族团结，为了少数民族文化传承，for the horde
                    string tempValue = Encoding.UTF8.GetString(bytes);

                    if (bytes[columnObject.Length - 1] != 0)
                    {
                        tempValue = tempValue.TrimEnd("\0");
                    }

                    sqliteColumn.Value = tempValue;
                    sqliteColumn.Type = ColumnType.TEXT;
                }
                else if (columnObject.ColumnType == 4)
                {//Blob
                    var bytes = new byte[columnObject.Length];
                    Marshal.Copy(columnObject.Value, bytes, 0, columnObject.Length);

                    sqliteColumn.Value = bytes;
                    sqliteColumn.Type = ColumnType.BLOB;
                }
                else
                {//未知
                    sqliteColumn.Value = "";
                    sqliteColumn.Type = ColumnType.NONE;
                }


                objectPointer = objectPointer.Increment<ColumnObject>();

            }

            if (bCreateTable)
            {
                var tempData = cellObjects;
                if (tempData.Count == allColumnTypes.Count)
                {
                    for (int pos = 0; pos < tempData.Count; pos++)
                    {
                        switch (tempData[pos].Type)
                        {
                            case ColumnType.TEXT://文本
                                allColumnTypes[pos] = "TEXT";
                                break;
                            case ColumnType.INTEGER://整数
                                allColumnTypes[pos] = "INTEGER";
                                break;
                            case ColumnType.BLOB://二进制
                                allColumnTypes[pos] = "BLOB";
                                break;
                            case ColumnType.DOUBLE://浮点数
                                allColumnTypes[pos] = "REAL";
                                break;
                        }
                    }
                }

                CreateTable(curContext, sCurTableName, allColumnNames, allColumnTypes, sCurIndexName);

                bCreateTable = false;
            }

            //获取插入表的SQL(多条)语句。
            var notIdColumns = new StringBuilder();
            allColumnNames.Skip(1).ForEach(name => notIdColumns.Append(name + ","));
            notIdColumns.Append(NewColumnName);

            var hasIdColumns = new StringBuilder();
            // 过滤字段中特殊符号(`);
            for (int i = 0; i < allColumnNames.Count; i++)
            {
                allColumnNames[i] = allColumnNames[i].Replace("`", "");
            }
            allColumnNames.ForEach(name => hasIdColumns.Append(name + ","));

            hasIdColumns.Append(NewColumnName);

            //对数据进行分组合并，按正常、删除、碎片顺序存储。
            var allRowData = GetAllRowData();

            //foreach(var row in allRowData)
            var row = cellObjects;
            {
                var parameters = new List<SQLiteParameter>();
                var valuesHead = string.Empty;
                StringBuilder columns;

                if (row[0].Value.ToString().IsNullOrEmpty())
                {//主键为空，一般为Integer的数字
                    columns = notIdColumns;
                }
                else
                {//主键为存在
                    columns = hasIdColumns;
                    string primaryKeyId = "@" + allColumnNames[0];
                    var parameter = new SQLiteParameter(primaryKeyId, DbType.String);
                    if (row[0].Type == ColumnType.BLOB)
                    {
                        parameter.DbType = DbType.Binary;
                    }

                    parameter.Value = row[0].Value;
                    valuesHead = primaryKeyId + ",";
                    parameters.Add(parameter);
                }

                var valuesBody = new StringBuilder();
                valuesBody.Append(valuesHead);

                var formTwoDatas = row.Skip(1);

                int columnIndex = 1;
                foreach (var cell in formTwoDatas)
                {
                    string paramName = "@" + allColumnNames[columnIndex];
                    var parameter = new SQLiteParameter(paramName, DbType.String);
                    if (cell.Type == ColumnType.BLOB)
                    {
                        parameter.DbType = DbType.Binary;
                    }
                    parameter.Value = cell.Value;
                    parameters.Add(parameter);
                    valuesBody.Append(paramName + ",");
                    columnIndex++;
                }

                valuesBody.Append(row[0].DataState);
                var insertSql = string.Format("insert into \"{0}\"({1}) values({2})", sCurTableName, columns, valuesBody);
                curCommand.CommandText = insertSql;
                curCommand.Parameters.AddRange(parameters.ToArray());

                try
                {
                    curCommand.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    var msg = string.Format("Sqlite插入表【{0}】发生异常 \n SQL语句为： {1}", sCurTableName, insertSql);
                    LoggerManagerSingle.Instance.Warn(ex, msg);
                }
            }

            return 0;
        }

        #endregion

        #region 底层流程处理逻辑

        /// <summary>
        /// 获取表定义。
        /// 获取表列集合。
        /// 获取列数据类型集合。
        /// </summary>
        private void GetTableDefin(IntPtr dbBase, string tableName, out IList<string> allColumnNames, out IList<string> allColumnTypes)
        {
            //获取表定义(包含列名和列类型)
            IntPtr columnNames = IntPtr.Zero;
            IntPtr columnTypes = IntPtr.Zero;
            int columnCount = 0;
            int gettabledefineCode = SqliteCoreDll.GetTableDefine(dbBase, tableName, ref columnNames, ref columnTypes, ref columnCount);
            if (gettabledefineCode != 0)
            {
                LoggerManagerSingle.Instance.Error(string.Format("获取表【{0}】定义失败，错误码:{1}，可能原因：表名字错误", tableName, gettabledefineCode));
                allColumnNames = new List<string>();
                allColumnTypes = new List<string>();
                return;
            }

            allColumnNames = ConvertToArray(columnNames, columnCount);
            allColumnTypes = ConvertToArray(columnTypes, columnCount);
            int freeTableDefineCode = SqliteCoreDll.FreeTableDefine(dbBase, ref columnNames, ref columnTypes, ref columnCount);
            if (freeTableDefineCode != 0)
            {
                LoggerManagerSingle.Instance.Warn(string.Format("Sqlite数据库恢复-释放表【{0}】定义失败，错误码:{1}", tableName, freeTableDefineCode));
            }
        }

        /// <summary>
        /// 分组存储数据。
        /// 由于返回的是错乱的结构，必须把正常的数据放在前面，因为他们有主键值。
        /// </summary>
        /// <returns>按照正常、删除、碎片结构返回。</returns>
        private IEnumerable<List<SqliteColumnObject>> GetAllRowData()
        {
            var normalRowData = new List<List<SqliteColumnObject>>();
            var deleteRowData = new List<List<SqliteColumnObject>>();
            var scanRowData = new List<List<SqliteColumnObject>>();

            foreach (var rowData in _AllNewRowData)
            {
                int dataState = rowData[0].DataState;
                if (dataState == 2)
                {
                    normalRowData.Add(rowData);
                }
                else if (dataState == 1)
                {
                    deleteRowData.Add(rowData);
                }
                else
                {
                    scanRowData.Add(rowData);
                }
            }

            var allData = new List<List<SqliteColumnObject>>();
            allData.AddRange(normalRowData);
            allData.AddRange(deleteRowData);
            allData.AddRange(scanRowData);

            return allData;
        }

        /// <summary>
        /// 获取所有从Dll底层返回的数据。
        /// </summary>
        /// <param name="dbBase"></param>
        /// <param name="tableName"></param>
        private void GetTableAllData(IntPtr dbBase, string tableName)
        {
            //获取表的正常和删除数据。
            _AllNewRowData = new List<List<SqliteColumnObject>>();

            //注意：张工的Release版本DLL在这个接口添加了一个检测机制，如果发现没有指定的程序（SPF、SPA、DF等等）运行，将产生内存读写错误！！！DEBUG版本DLL无此检测。
            int getCotentCode = SqliteCoreDll.getTableContentGenearal(dbBase, SqliteCallBack, tableName, _DataMode);
            if (getCotentCode != 0)
            {
                LoggerManagerSingle.Instance.Error(string.Format("Sqlite数据库恢复-Sqlite底层读取表[{0}]记录发生错误，错误码：{1}", tableName, getCotentCode));
            }
        }

        /// <summary>
        /// 创建表构架。
        /// </summary>
        private bool CreateTable(SqliteContext context, string tableName, IList<string> allColumnNames, IList<string> allColumnTypes, string indexCol = null)
        {
            /* 2017/02/07 songbing 修改
             * 屏蔽表主键获取
             * 由于文件占用导致获取表主键将产生性能问题
             * 目前看来主键是不需要的
            */

            // 获取当前表中主键字段;
            //var primaryKeys = GetPrimaryKey(context, tableName);
            //拼接创建表的SQL语句
            var fieldBuilder = new StringBuilder();
            for (int i = 0; i < allColumnNames.Count; i++)
            {
                //if (i == 0 && (allColumnNames[0].ToUpper().Equals("_ID") || allColumnNames[0].ToUpper().Equals("ID")) && allColumnTypes[0].Contains("INT"))
                //if (primaryKeys.Contains(allColumnNames[0].ToUpper()))
                //{
                //    fieldBuilder.Append(allColumnNames[i] + " " + allColumnTypes[i] + " PRIMARY KEY,");
                //}
                //else
                //{
                fieldBuilder.Append(allColumnNames[i] + " " + allColumnTypes[i] + ",");
                //}
            }
            fieldBuilder.Append(NewColumnName + " INTEGER");

            //创建数据表
            string createTableSql = string.Format("create table \"{0}\" ({1})", tableName, fieldBuilder);

            try
            {
                if (null != curCommand)
                {
                    curCommand.CommandText = createTableSql;
                    curCommand.ExecuteNonQuery();

                    if (indexCol.IsValid())
                    {//建索引
                        curCommand.CommandText = string.Format("create index xlyIndex{0}{1} on {0}({1});", tableName, indexCol);
                        curCommand.ExecuteNonQuery();
                    }
                }
                else
                {
                    context.ExecuteNonQuery(createTableSql);

                    if (indexCol.IsValid())
                    {//建索引
                        context.ExecuteNonQuery(string.Format("create index xlyIndex{0}{1} on {0}({1});", tableName, indexCol));
                    }
                }
            }
            catch (Exception ex)
            {
                LoggerManagerSingle.Instance.Error(ex, string.Format("创建表{0}构造失败,创建表的SQL语句：\n{1}", tableName, createTableSql));
                return false;
            }

            return true;

        }

        /// <summary>
        /// 获取表中的主键信息;
        /// </summary>
        /// <param name="context"></param>
        /// <param name="tableName"></param>
        /// <returns></returns>
        private List<string> GetPrimaryKey(SqliteContext context, string tableName)
        {
            var keyAll = new List<string>();
            try
            {
                var sqlite = new SqliteContext("") { DataSource = context.DataSource.Replace("_recovery", "") };
                var data =
                    sqlite.Find(
                        new SQLiteString("select * from sqlite_master where type = 'table' and tbl_name = '" + tableName +
                                         "';"));
                if (data == null || data.Count() == 0)
                    return keyAll;
                var lists = Regex.Matches(DynamicConvert.ToSafeString(data.First().sql), @"\(.*\)");
                if (lists.Count == 0)
                    return keyAll;
                var columns = lists[0].Value.TrimStart('(').TrimEnd(')').Split(',');
                foreach (var column in columns)
                {
                    if (!column.Contains("PRIMARY KEY")) continue;
                    var result = Regex.Matches(column, "'.*'");
                    if (result.Count == 0 || result.Count > 1)
                        continue;
                    keyAll.Add(DynamicConvert.ToSafeString(result[0].Value).Replace("'", ""));
                }
            }
            catch (Exception ex)
            {
                LoggerManagerSingle.Instance.Error(ex, string.Format("获取表{0}主键信息失败", tableName));
            }
            return keyAll;
        }

        /// <summary>
        /// 初始化Dll底层
        /// 类似与Mount。
        /// </summary>
        /// <param name="sourceDb">源数据库路径。</param>
        /// <param name="charatorPath">特征库路径。</param>
        /// <param name="dbBase">数据库句柄。</param>
        /// <returns>返回是否初始化成功。</returns>
        private bool InitDb(string sourceDb, string charatorPath, ref IntPtr dbBase)
        {
            try
            {
                int initCode = SqliteCoreDll.Init(licenseFile);
                if (initCode != 0)
                {
                    LoggerManagerSingle.Instance.Error(string.Format("Sqlite数据库恢复-Sqlite底层初始化错误，错误码：{0}", initCode));
                    return false;
                }

                int openCode = SqliteCoreDll.OpenSqliteData(ref dbBase, sourceDb, charatorPath);
                if (openCode != 0)
                {
                    LoggerManagerSingle.Instance.Error(openCode == 9999
                                        ? string.Format("Sqlite数据库恢复-Sqlite 打开数据库失败,错误码：{0}.原因可能是没有注册或者没有管理员方式运行", openCode)
                                        : string.Format("Sqlite数据库恢复-Sqlite 打开数据库失败,错误码：{0}", openCode));

                    return false;
                }

                int formatCode = 0;
                int getCode = SqliteCoreDll.GetCodeFomart(dbBase, ref formatCode);
                if (getCode != 0)
                {
                    LoggerManagerSingle.Instance.Error(string.Format("-Sqlite数据库恢复Sqlite获取数据库【{0}】编码失败，错误码：{1}", sourceDb, getCode));
                }

                _CurrentEncoding = GetFormatString(formatCode);
                //_CallBack = SqliteCallBack;

                return true;
            }
            catch (Exception ex)
            {
                LoggerManagerSingle.Instance.Error(ex, "调用底层SQLite-dll发生异常");
                return false;
            }

        }

        /// <summary>
        /// 清理资源，释放数据库句柄。
        /// 把所有数据清空。
        /// </summary>
        /// <param name="dbBase"></param>
        private void DisposeSource(IntPtr dbBase)
        {
            if (dbBase != IntPtr.Zero)
            {
                int freeDataBase = SqliteCoreDll.CloseSqliteHandle(dbBase);
                if (freeDataBase != 0)
                {
                    LoggerManagerSingle.Instance.Error(string.Format("Sqlite数据库恢复-释放数据库句柄失败，错误码：{0}", freeDataBase));
                }
            }

            curContext = null;
            curCommand = null;
            bCreateTable = false;
            sCurTableName = "";
            allColumnNames = null;
            allColumnTypes = null;

            _AllNewRowData = null;
        }

        /// <summary>
        /// Sqilte获取表数据的回调函数。
        /// </summary>
        /// <param name="count">返回表的行数。</param>
        /// <param name="objectPointer">列数组指针。</param>
        /// <param name="dataType">每一行的数据类型（正常，删除，扫描）</param>
        /// <returns>默认返回0。</returns>
        private int SqliteCallBack(int count, IntPtr objectPointer, byte dataType)
        {
            var cellObjects = new List<SqliteColumnObject>();
            for (int i = 0; i < count; i++)
            {
                var columnObject = objectPointer.ToStruct<ColumnObject>();

                var sqliteColumn = new SqliteColumnObject();
                cellObjects.Add(sqliteColumn);

                sqliteColumn.DataState = dataType;
                sqliteColumn.Length = columnObject.Length;
                if (columnObject.Length == 0)
                {
                    sqliteColumn.Value = "";
                    objectPointer = objectPointer.Increment<ColumnObject>();
                    continue;
                }

                if (columnObject.ColumnType == 1)
                {//Integer
                    byte[] bytes = GetIntAndDoubleBytes(columnObject.Value, columnObject.Length);
                    sqliteColumn.Value = BitConverter.ToInt64(bytes, 0);
                    sqliteColumn.Type = ColumnType.INTEGER;
                }
                else if (columnObject.ColumnType == 2)
                {//Double
                    byte[] bytes = GetIntAndDoubleBytes(columnObject.Value, columnObject.Length);
                    sqliteColumn.Value = BitConverter.ToDouble(bytes, 0);
                    sqliteColumn.Type = ColumnType.DOUBLE;
                }
                else if (columnObject.ColumnType == 3)
                {//Text
                    var bytes = new byte[columnObject.Length];
                    Marshal.Copy(columnObject.Value, bytes, 0, columnObject.Length);
                    //string tempValue = Encoding.Default.GetString(Encoding.Convert(_CurrentEncoding, Encoding.Default, bytes));
                    //Modify：chenjing 为了民族团结，为了少数民族文化传承，for the horde
                    string tempValue = Encoding.UTF8.GetString(bytes);

                    if (bytes[columnObject.Length - 1] != 0)
                    {
                        tempValue = tempValue.TrimEnd("\0");
                    }

                    sqliteColumn.Value = tempValue;
                    sqliteColumn.Type = ColumnType.TEXT;
                }
                else if (columnObject.ColumnType == 4)
                {//Blob
                    var bytes = new byte[columnObject.Length];
                    Marshal.Copy(columnObject.Value, bytes, 0, columnObject.Length);

                    sqliteColumn.Value = bytes;
                    sqliteColumn.Type = ColumnType.BLOB;
                }
                else
                {//未知
                    sqliteColumn.Value = "";
                    sqliteColumn.Type = ColumnType.NONE;
                }


                objectPointer = objectPointer.Increment<ColumnObject>();

            }

            _AllNewRowData.Add(cellObjects);

            return 0;
        }

        /// <summary>
        /// 特征扫描回调函数
        /// </summary>
        /// <param name="pagesize"></param>
        /// <param name="curpage"></param>
        /// <param name="allPage"></param>
        /// <param name="scanedCount"></param>
        /// <param name="stop"></param>
        /// <returns></returns>
        private int Sqlite3_General_OtherScanCallBack(int pagesize, int curpage, int allPage, int scanedCount, ref int stop)
        {
            return 0;
        }

        #endregion

        #region 工具方法

        /// <summary>
        /// 获取数据库的编码。
        /// </summary>
        private Encoding GetFormatString(int code)
        {
            // 1：utf-8 2:utf-16le 3:utf-16be 
            switch (code)
            {
                case 1:
                    return Encoding.UTF8;
                case 2:
                    return Encoding.GetEncoding("UTF-16LE");
                case 3:
                    return Encoding.GetEncoding("UTF-16BE");
                default:
                    return Encoding.UTF8;
            }
        }

        /// <summary>
        /// 获取Int和Double类型
        /// 从底层获取的指针为高位存储，需要转换为Windows能识别的低位存储。
        /// </summary>
        private byte[] GetIntAndDoubleBytes(IntPtr intper, int count)
        {
            var bytes = new byte[8];
            Marshal.Copy(intper, bytes, 0, count);

            for (int i = 0; i < count / 2; i++)
            {
                byte temp = bytes[i];
                bytes[i] = bytes[count - 1 - i];
                bytes[count - 1 - i] = temp;
            }

            if ((bytes[count - 1] & 128) == 128)
            {
                for (int i = count; i < 8; i++)
                {
                    bytes[i] = 255;
                }
            }

            return bytes;
        }


        /// <summary>
        /// 转换数组指针为字符集合。
        /// </summary>
        /// <param name="stringPointer">数组指针</param>
        /// <param name="count">数组大小</param>
        /// <returns>泛型集合</returns>
        private List<string> ConvertToArray(IntPtr stringPointer, int count)
        {
            var tbs = new IntPtr[count];
            Marshal.Copy(stringPointer, tbs, 0, count);
            var resultList = new List<string>();
            Array.ForEach(tbs, name => resultList.Add(Marshal.PtrToStringAnsi(name)));
            return resultList;
        }

        private string GetKeyId()
        {
            IntPtr key = IntPtr.Zero;
            SqliteCoreDll.GetAuthorizeId(ref key);
            return key.ToAnsiString().Substring(0, 32);
        }

        #endregion
    }
}
