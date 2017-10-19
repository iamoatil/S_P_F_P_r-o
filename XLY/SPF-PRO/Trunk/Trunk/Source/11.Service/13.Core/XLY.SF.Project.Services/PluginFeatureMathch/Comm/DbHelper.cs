using System.IO;
using System.Linq;
using XLY.SF.Project.Persistable.Primitive;

namespace XLY.SF.Project.Services
{
    public static class DbHelper
    {

        private static bool ButtomIsExistTable(string dbfile, string tablename)
        {

            var tbs = SqliteRecoveryHelper.ButtomGetAllTables(dbfile);

            return tbs.Contains(tablename);
        }

        private static bool ButtomIsExistTableField(string dbfile, string tablename, string fieldName)
        {
            var fds = SqliteRecoveryHelper.ButtomGetTableDefin(dbfile, tablename);

            return fds.Contains(fieldName);
        }

        /// <summary>
        /// 判断数据库表字段是否存在
        /// </summary>
        /// <param name="dbfile"></param>
        /// <param name="tablename"></param>
        /// <param name="fieldName"></param>
        /// <param name="decryted"></param>
        /// <returns></returns>
        public static bool IsExistTableField(string dbfile, string tablename, string fieldName, bool decryted = false)
        {
            bool res = false;

            if (decryted)
            {
                FileInfo fi = new FileInfo(dbfile);
                string temp = System.IO.Path.Combine(fi.DirectoryName, "temp.db");

                DirectoryHelper.TryDeleteFile(temp);

                if (0 == DeCryptedDBHelper.DecryptAndroidWeChatSqlite(dbfile, temp))
                {
                    res = ButtomIsExistTableField(temp, tablename, fieldName);

                    DirectoryHelper.TryDeleteFile(temp);
                }
                else
                {
                    res = ButtomIsExistTableField(dbfile, tablename, fieldName);
                }
            }
            else
            {
                res = ButtomIsExistTableField(dbfile, tablename, fieldName);
            }

            return res;
        }

        /// <summary>
        /// 判断数据库表是否存在
        /// </summary>
        /// <param name="dbfile"></param>
        /// <param name="tablename"></param>
        /// <param name="decryted"></param>
        /// <returns></returns>
        public static bool IsExistTable(string dbfile, string tablename, bool decryted = false)
        {
            bool res = false;

            if (decryted)
            {
                FileInfo fi = new FileInfo(dbfile);
                string temp = System.IO.Path.Combine(fi.DirectoryName, "temp.db");

                DirectoryHelper.TryDeleteFile(temp);

                if (0 == DeCryptedDBHelper.DecryptAndroidWeChatSqlite(dbfile, temp))
                {
                    res = ButtomIsExistTable(temp, tablename);

                    DirectoryHelper.TryDeleteFile(temp);
                }
                else
                {
                    res = ButtomIsExistTable(dbfile, dbfile);
                }
            }
            else
            {
                res = ButtomIsExistTable(dbfile, tablename);
            }

            return res;
        }
    }
}
