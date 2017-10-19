/***************************************************************************
 * 
 * author :  Songbing
 * time: 2017/9/14 11:15:29 
 * explain :  
 *
*****************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace X86DllServer
{
    /// <summary>
    /// 读取进度回调函数
    /// </summary>
    /// <param name="curp"></param>
    /// <param name="allProcess"></param>
    /// <param name="stop"></param>
    /// <returns></returns>
    public delegate int ReadProgress(int curP, int allProgress, ref int stop);

    /// <summary>
    /// SIM卡调用底层DLL处理
    /// </summary>
    public static class SIMCoreAPI
    {
        private const string SIMdll = "simDataRead.dll";

        /// <summary>
        /// 扫描com口
        /// </summary>
        /// <param name="pcomArrayStr">用来保存COM端口</param>
        /// <param name="comNum">扫描到的端口数量</param>
        /// <returns>comNum</returns>
        [DllImport(SIMdll, EntryPoint = "SimCard_scanCom")]
        public static extern int SimCard_scanCom(ref IntPtr pcomArrayStr, ref int comNum);

        /// <summary>
        /// 初始化函数 
        /// </summary>
        /// <param name="handle">装载的当前com端口句柄(底层赋值)</param>
        /// <param name="comstr">端口号</param>
        /// <param name="process">在读取通话记录，短信 ，通讯薄上的所有内容都会回调该进度</param>
        /// <returns></returns>
        [DllImport(SIMdll, EntryPoint = "SimCard_Mount")]
        public static extern int SimCard_Mount(ref IntPtr handle, string comstr, ReadProgress process);

        /// <summary>
        /// 读取通讯录内容
        /// </summary>
        /// <param name="shandle">当前COM端口句柄</param>
        /// <param name="lpsimtb">通讯录结构体数组</param>
        /// <param name="recordNum">条数</param>
        /// <returns></returns>
        [DllImport(SIMdll, EntryPoint = "SimCard_readAddressbook")]
        public static extern int SimCard_readAddressbook(IntPtr shandle, ref IntPtr lpsimtb, ref int recordNum);

        /// <summary>
        /// 读取短信内容
        /// </summary>
        /// <param name="shandle">当前COM端口句柄</param>
        /// <param name="lpsimtb">短信结构体数组</param>
        /// <param name="recordNum">条数</param>
        /// <returns></returns>
        [DllImport(SIMdll, EntryPoint = "SimCard_readSMS")]
        public static extern int SimCard_readSMS(IntPtr shandle, ref IntPtr lpsimtb, ref int recordNum);

        /// <summary>
        /// 返回卡类型
        /// </summary>
        /// <returns>类型值由 SIMTYPE_ 的常量</returns>
        [DllImport(SIMdll, EntryPoint = "SimCard_readType")]
        public static extern int SimCard_readType(IntPtr shandle, ref byte type);

        /// <summary>
        /// 返回imsi号 sim卡的唯一编码
        /// <param name="shandle">SIM卡（COM端口）句柄</param>
        /// <param name="pIMSI">sim imsi编号</param>
        /// </summary>
        [DllImport(SIMdll, EntryPoint = "SimCard_readIMSI")]
        public static extern int SimCard_readIMSI(IntPtr shandle, ref string pIMSI);

        /// <summary>
        /// 读取最后的通话记录
        /// </summary>
        /// <param name="shandle">SIM卡（COM端口）句柄</param>
        /// <param name="pLastDailed">最后通话记录结构体</param>
        /// <param name="recordNum">返回条数</param>
        /// <returns></returns>
        [DllImport(SIMdll, EntryPoint = "SimCard_readlastCalled")]
        public static extern int SimCard_readlastCalled(IntPtr shandle, ref IntPtr pLastDailed, ref int recordNum);

        /// <summary>
        /// 读取本机号码，卡上存储才会有，不存储是没有数据的
        /// </summary>
        /// <param name="shandle">SIM卡（COM端口）句柄</param>
        /// <param name="pTelephonbook"></param>
        /// <param name="recordNum"></param>
        /// <returns></returns>
        [DllImport(SIMdll, EntryPoint = "SimCard_readSimPhoneNo")]
        public static extern int SimCard_readSimPhoneNo(IntPtr shandle, ref IntPtr pTelephonbook, ref int recordNum);

        /// <summary>
        /// 释放SIM句柄
        /// </summary>
        /// <param name="shandle">SIM卡（COM端口）句柄</param>
        /// <returns></returns>
        [DllImport(SIMdll, EntryPoint = "Simcard_unmount")]
        public static extern int Simcard_unmount(ref IntPtr shandle);

        /// <summary>
        /// 释放所有获取到的数据对象;
        /// </summary>
        /// <param name="buffer">数据对象</param>
        /// <returns></returns>
        [DllImport(SIMdll, EntryPoint = "Simcard_releaseBuffer")]
        public static extern int Simcard_releaseBuffer(ref IntPtr buffer);
    }

    /// <summary>
    /// 短信
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct SIMStruct_SMS
    {
        /// <summary>
        /// 短信类型:已发送、未发送
        /// </summary>
        public byte SMSType;

        /// <summary>
        /// 电话号码
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 20)]
        public string PhoneNo;

        /// <summary>
        /// 短信内容
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 500)]
        public string Content;

        /// <summary>
        /// 短信时间
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 50)]
        public string TimeStr;

        /// <summary>
        /// 短信服务中心号码
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 20)]
        public string ServiceNo;
    }

    /// <summary>
    /// 联系人
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct SIMStruct_TelephoneBook
    {
        /// <summary>
        /// 联系人姓名
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 50)]
        public string name;

        /// <summary>
        /// 电话号码
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 50)]
        public string telephone_number;

        /// <summary>
        /// EMAIL
        /// </summary>
        //[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 50)]
        public IntPtr simstruct_Email;


        /// <summary>
        /// EMAIL的个数
        /// </summary>
        public int EMailData_count;

        /// <summary>
        /// 本电话所在的组群
        /// </summary>
        //[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 50)]
        public IntPtr Simstruct_Group;

        /// <summary>
        /// 组群的个数
        /// </summary>
        public int Group_count;

        /// <summary>
        /// 附加数据
        /// </summary>
        public IntPtr AdditionData;

        /// <summary>
        /// 附加数据的个数
        /// </summary>
        public int Addition_count;

        /// <summary>
        /// 第二条目数据
        /// </summary>
        public IntPtr RemarkData;

        /// <summary>
        /// 第二条目的个数
        /// </summary>
        public int Remark_count;
    }

    /// <summary>
    /// 最后通话记录
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct SINStruct_LastPhoneDailed
    {
        /// <summary>
        /// 联系人名称
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 50)]
        public string name;

        /// <summary>
        /// 呼出类型
        /// </summary>
        public byte calltype;

        /// <summary>
        /// 号码
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 50)]
        public string phoneno;
    }

    /// <summary>
    /// COM端口
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct COM
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
        public char[] comstr;
    }


    /// <summary>
    /// 附加数据
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct Simstruct_Additional_tag
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 50)]
        public string Additional_tag_name;		   //附加信息名称

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 50)]
        public char Additional_content;			//附加信息的内容
    }

}
