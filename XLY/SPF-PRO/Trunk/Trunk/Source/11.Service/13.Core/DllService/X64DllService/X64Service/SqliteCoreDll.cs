/***************************************************************************
 * 
 * author :  Songbing
 * time: 2017/8/15 11:27:47 
 * explain :  
 *
*****************************************************************************/

using System;
using System.IO;
using System.Runtime.InteropServices;
using XLY.SF.Framework.Log4NetService;

namespace X64Service
{
    /// <summary>
    /// Sqlite相关底层接口
    /// </summary>
    public static class SqliteCoreDll
    {

        private const string _SqliteDllName = @"bin\SqliteInterface.dll";

        public static int Init(string licensePath)
        {
            try
            {
                return InitPath("", licensePath);
            }
            catch (Exception ex)
            {
                string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, _SqliteDllName);
                LoggerManagerSingle.Instance.Error(ex, string.Format("Sqlite数据库恢复底层DLL初始化( InitPath)失败，当前路径：{0}", path));
                return -1;
            }
        }

        //int __stdcall  fun10(char *pdllPath,char *pAuthorisedFilePath)//设置dll路径与授权文件路径
        [DllImport(_SqliteDllName, EntryPoint = "fun10")]
        public static extern int InitPath(string dllPath, string licensePath);

        //int __stdcall  fun2(HANDLE  &pdataBase,char *pPath, char *pChractorPath)//打开文件得到句柄pdataBase，pPath为文件路径，pChractorPath特征数据库路径
        [DllImport(_SqliteDllName, EntryPoint = "fun2")]
        public static extern int OpenSqliteData(ref IntPtr dataBase, string dbPath, string chratorPath);

        //打开sqlite数据pSqliteBuff得到句柄pdataBase,length 数据长度，pChractorPath为特征文件路径
        //int __stdcall fun1(HANDLE  &pdataBase,char *pSqliteBuff,UINT64 length, char *pChractorPath);
        [DllImport(_SqliteDllName, EntryPoint = "fun1")]
        public static extern int OpenSqliteBuff(ref IntPtr dataBase, string buffer, ulong length, string chratorPath);

        //int  __stdcall fun11(HANDLE pDatabase,int &codeType)   //  获取编码类型 1：utf-8 2:utf-16le 3:utf-16be 
        [DllImport(_SqliteDllName, EntryPoint = "fun11")]
        public static extern int GetCodeFomart(IntPtr dataBase, ref int codeType);

        //int   __stdcall   fun4( HANDLE pDatabase,char ** &ptableNameGroup,int &tableCount) 
        [DllImport(_SqliteDllName, EntryPoint = "fun4")]
        public static extern int GetAllTableName(IntPtr dataBase, ref IntPtr arr, ref int tableCount);

        //int  __stdcall   fun5(HANDLE pDatabase,char ** ptabaleNameGroup, int tableCount)
        [DllImport(_SqliteDllName, EntryPoint = "fun5")]
        public static extern int FreeALLTableName(IntPtr dataBase, IntPtr tableNames, int tableCount);

        //int  __stdcall  fun6(HANDLE pDatabase,const char * pTableName,char ** &pColumeName,char ** &pColumeType,int &columnCount)
        [DllImport(_SqliteDllName, EntryPoint = "fun6")]
        public static extern int GetTableDefine(IntPtr dataBase, string tableName, ref IntPtr columnNames, ref IntPtr columnTypes, ref int columnCount);

        //int   __stdcall  fun7(HANDLE pDatabase,char ** &pColumeName,char ** &pColumeType,int &columnCount)
        [DllImport(_SqliteDllName, EntryPoint = "fun7")]
        public static extern int FreeTableDefine(IntPtr dataBase, ref IntPtr columnNames, ref IntPtr columnTypes, ref int columnCount);

        [DllImport(_SqliteDllName, EntryPoint = "fun8")]
        public static extern int getTableContentGenearal(IntPtr dataBase, SqliteGeneralCallBack callBack, string tableName, byte dataMode);

        [DllImport(_SqliteDllName, EntryPoint = "fun3")]
        public static extern int CloseSqliteHandle(IntPtr dataBase);

        //pAuthorizeID 获取到32位值，该id不同电脑不同
        //int   __stdcall  fun9( char * &pAuthorizeID) //没有授权情况下，请使用该接口获取授权号，然后索取授权文件
        [DllImport(_SqliteDllName, EntryPoint = "fun9")]
        public static extern int GetAuthorizeId(ref IntPtr authorizeId);

        //int __stdcall   fun12(HANDLE pDatabase ,Sqlite3_General_OtherScanCallBack pPogressCallback,char *pohterfileName,Sqlite3_General_CallBack pcallback,const char *tableName)
        //特征扫描
        [DllImport(_SqliteDllName, EntryPoint = "fun12")]
        public static extern int getContentFromFile(IntPtr dataBase, Sqlite3_General_OtherScanCallBack scanCallBack, string filename, SqliteGeneralCallBack dataCallBack, string tableName);

    }

    public delegate int SqliteGeneralCallBack(int count, IntPtr columnObject, byte dataType);
    public delegate int Sqlite3_General_OtherScanCallBack(int pagesize, int curpage, int allPage, int scanedCount, ref int stop);

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct ColumnObject
    {
        public int Length;

        public byte ColumnType;

        public IntPtr Value;
    }

    public enum ColumnType
    {
        NONE = 0,
        INTEGER = 1,
        DOUBLE = 2,
        TEXT = 3,
        BLOB = 4
    }

    public class SqliteColumnObject
    {
        public byte DataState { get; set; }

        public ColumnType Type { get; set; }

        public int Length { get; set; }

        public object Value { get; set; }

        public SqliteColumnObject()
        {
            Type = ColumnType.NONE;
            DataState = 2;
        }
    }
}
