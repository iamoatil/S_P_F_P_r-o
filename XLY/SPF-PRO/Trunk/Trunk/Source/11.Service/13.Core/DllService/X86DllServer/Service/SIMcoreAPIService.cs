/***************************************************************************
 * 
 * author :  Songbing
 * time: 2017/9/14 15:08:19 
 * explain :  
 *
*****************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using X86DllServer.IService;

namespace X86DllServer.Service
{
    public partial class X86DLLService : ISIMcoreAPIService
    {
        public int SimCard_scanCom(ref List<string> listComs)
        {
            IntPtr comPoninter = IntPtr.Zero;
            int comnumber = 0, result = -1;

            try
            {
                result = SIMCoreAPI.SimCard_scanCom(ref comPoninter, ref comnumber);
                if (0 == result)
                {
                    for (int j = 0; j < comnumber; j++)
                    {
                        var commodel = comPoninter.ToStruct<COM>();//将指针转化为相应的结构

                        listComs.Add(new string(commodel.comstr).Trim('\0'));

                        comPoninter = comPoninter.Increment<COM>();
                    }
                }
            }
            catch
            {

            }

            return result;
        }

        public int SimCard_readSimPhoneNoAndIMSI(string comstr, ref List<string> listPhoneNo, ref string imsi)
        {
            IntPtr handle = IntPtr.Zero;
            int result = -1;

            try
            {
                result = SIMCoreAPI.SimCard_Mount(ref handle, comstr, ReadProcessBack);
                if (0 != result)
                {
                    return result;
                }

                byte simType = 0;
                result = SIMCoreAPI.SimCard_readType(handle, ref simType);
                if (0 != result)
                {
                    return result;
                }

                result = SIMCoreAPI.SimCard_readIMSI(handle, ref imsi);
                if (0 != result)
                {
                    return result;
                }

                IntPtr pPhoneNo = IntPtr.Zero;
                int phoneNoCount = 0;

                result = SIMCoreAPI.SimCard_readSimPhoneNo(handle, ref pPhoneNo, ref phoneNoCount);
                if (0 == result)
                {

                }
            }
            catch
            {

            }
            finally
            {
                if (IntPtr.Zero != handle)
                {
                    SIMCoreAPI.Simcard_unmount(ref handle);
                }
            }

            return result;
        }

        public string SimCard_readAddressbook(string comstr)
        {
            IntPtr handle = InitSIMCard(comstr);
            if (IntPtr.Zero == handle)
            {
                return string.Empty;
            }

            int phonenumber = 0;
            IntPtr lastphone = IntPtr.Zero;
            var r = SIMCoreAPI.SimCard_readAddressbook(handle, ref lastphone, ref phonenumber);

            List<SIMStruct_TelephoneBook> list = new List<SIMStruct_TelephoneBook>();

            if (r == 0 && IntPtr.Zero != lastphone && phonenumber > 0)
            {
                var temp = lastphone;
                for (int i = 0; i < phonenumber; i++)
                {
                    try
                    {
                        list.Add(temp.ToStruct<SIMStruct_TelephoneBook>());

                        temp = temp.Increment<SIMStruct_TelephoneBook>();
                    }
                    catch
                    {
                    }
                }
            }

            SIMCoreAPI.Simcard_releaseBuffer(ref lastphone);
            SIMCoreAPI.Simcard_unmount(ref handle);

            return string.Empty;
        }

        public string SimCard_readlastCalled(string comstr)
        {
            IntPtr handle = InitSIMCard(comstr);
            if (IntPtr.Zero == handle)
            {
                return string.Empty;
            }

            int callnumber = 0;
            IntPtr call = IntPtr.Zero;
            var r = SIMCoreAPI.SimCard_readlastCalled(handle, ref call, ref callnumber);

            List<SINStruct_LastPhoneDailed> list = new List<SINStruct_LastPhoneDailed>();

            if (r == 0 && IntPtr.Zero != call && callnumber > 0)
            {
                var temp = call;
                for (int i = 0; i < callnumber; i++)
                {
                    try
                    {
                        list.Add(temp.ToStruct<SINStruct_LastPhoneDailed>());

                        temp = temp.Increment<SINStruct_LastPhoneDailed>();
                    }
                    catch
                    {
                    }
                }
            }

            SIMCoreAPI.Simcard_releaseBuffer(ref call);
            SIMCoreAPI.Simcard_unmount(ref handle);

            return string.Empty;
        }

        public string SimCard_readSMS(string comstr)
        {
            IntPtr handle = InitSIMCard(comstr);
            if (IntPtr.Zero == handle)
            {
                return string.Empty;
            }

            int smsnumber = 0;
            IntPtr sms = IntPtr.Zero;
            var r = SIMCoreAPI.SimCard_readSMS(handle, ref sms, ref smsnumber);

            List<SIMStruct_SMS> list = new List<SIMStruct_SMS>();

            if (r == 0 && IntPtr.Zero != sms && smsnumber > 0)
            {
                var temp = sms;
                for (int i = 0; i < smsnumber; i++)
                {
                    try
                    {
                        list.Add(temp.ToStruct<SIMStruct_SMS>());

                        temp = temp.Increment<SIMStruct_SMS>();
                    }
                    catch
                    {
                    }
                }
            }

            SIMCoreAPI.Simcard_releaseBuffer(ref sms);
            SIMCoreAPI.Simcard_unmount(ref handle);

            return string.Empty;
        }

        private IntPtr InitSIMCard(string comStr)
        {
            IntPtr handle = IntPtr.Zero;
            int i = SIMCoreAPI.SimCard_Mount(ref handle, comStr, ReadProcessBack);
            if (i != 0)
            {
                return handle;
            }

            byte simType = 0;
            i = SIMCoreAPI.SimCard_readType(handle, ref simType);   //读取卡类型
            if (i != 0)
            {
                return handle;
            }

            string imsi = "";
            i = SIMCoreAPI.SimCard_readIMSI(handle, ref imsi);      //需要先读取imsi号，才能读取到大于250条数据
            if (i != 0)
            {
                return handle;
            }

            IntPtr pPhoneNo = IntPtr.Zero;
            int phoneNoCount = 0;
            i = SIMCoreAPI.SimCard_readSimPhoneNo(handle, ref pPhoneNo, ref phoneNoCount);      //读取本机号码
            if (i != 0)
            {
                return handle;
            }

            return handle;
        }

        private int ReadProcessBack(int curP, int allProgress, ref int stop)
        {
            return 0;
        }

    }
}
