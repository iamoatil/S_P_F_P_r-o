/***************************************************************************
 * 
 * author :  Songbing
 * time: 2017/9/11 14:56:13 
 * explain :  
 *
*****************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using X86DllServer.IService;

namespace X86DllServer.Service
{
    public partial class X86DLLService : IBlackBerryDeviceAPIService
    {
        public List<BlackPhoneInfo> BlackBerry_FindDevices()
        {
            List<BlackPhoneInfo> list = new List<BlackPhoneInfo>();

            IntPtr linkPhone = IntPtr.Zero;
            int nums = 0;
            int result = 0;

            try
            {
                KillProcess("Rim.Desktop");
                result = BlackBerryDeviceAPI.BlackBerry_DeviceFind(ref linkPhone, ref nums);

                if (result != 0 || linkPhone == IntPtr.Zero)
                {
                    return list;
                }

                IntPtr pitem = linkPhone;

                for (int pos = 0; pos < nums; pos++)
                {
                    list.Add(Marshal.PtrToStructure<BlackPhoneInfo>(pitem));

                    pitem += Marshal.SizeOf(typeof(BlackPhoneInfo));
                }
            }
            catch
            {

            }
            finally
            {
                if (IntPtr.Zero != linkPhone)
                {
                    BlackBerryDeviceAPI.BlackBerry_ReleaseBuffer(ref linkPhone);
                }
            }

            return list;
        }

        public int BlackBerry_Mount(string pinStr)
        {
            BlackPhoneInfo phone = new BlackPhoneInfo() { pinStr = pinStr };

            return BlackBerryDeviceAPI.BlackBerry_Mount(phone).ToInt32();
        }

        public List<BlackPhoneAppContentInfo> BlackBerry_GetAppDataInfo(int blackberryHadnle)
        {
            List<BlackPhoneAppContentInfo> list = new List<BlackPhoneAppContentInfo>();

            IntPtr linkApps = IntPtr.Zero;
            int nums = 0;
            int result = 0;

            try
            {
                result = BlackBerryDeviceAPI.BlackBerry_GetAppDataInfo((IntPtr)blackberryHadnle, ref linkApps, ref nums);
                if (0 != result || IntPtr.Zero == linkApps || 0 == nums)
                {
                    return list;
                }

                IntPtr pitem = linkApps;

                for (int pos = 0; pos < nums; pos++)
                {
                    list.Add(Marshal.PtrToStructure<BlackPhoneAppContentInfo>(pitem));

                    pitem += Marshal.SizeOf(typeof(BlackPhoneAppContentInfo));
                }
            }
            catch
            {

            }
            finally
            {
                if (linkApps != IntPtr.Zero)
                {
                    BlackBerryDeviceAPI.BlackBerry_ReleaseBuffer(ref linkApps);
                }
            }

            return list;
        }

        public void BlackBerry_Close(int blackberryHandle)
        {
            IntPtr ip = (IntPtr)blackberryHandle;

            BlackBerryDeviceAPI.BlackBerry_Close(ref ip);
        }

        public int BlackBerry_ImageAppData(int blackberryHadnle, string psavedir)
        {
            IntPtr linkApps = IntPtr.Zero;
            int nums = 0;
            try
            {
                int backResult = BlackBerryDeviceAPI.BlackBerry_GetAppDataInfo((IntPtr)blackberryHadnle, ref linkApps, ref nums);

                return BlackBerryDeviceAPI.BlackBerry_ImageAppData((IntPtr)blackberryHadnle, psavedir, backResult, nums, BlackBerryImageDataCallBack);
            }
            finally
            {
                if (linkApps != IntPtr.Zero)
                {
                    BlackBerryDeviceAPI.BlackBerry_ReleaseBuffer(ref linkApps);
                }
            }
        }

        private int BlackBerryImageDataCallBack(long size, string filename, ref int stop)
        {
            _clientCallback.BlackBerryImageDataCallBack(size, filename, ref stop);

            return 0;
        }

        public void BlackBerry_ReleaseBuffer(int dataHandle)
        {
            IntPtr ip = (IntPtr)dataHandle;

            BlackBerryDeviceAPI.BlackBerry_ReleaseBuffer(ref ip);
        }

        #region KillProcess

        /// <summary>
        /// 关闭指定名称的进程
        /// </summary>
        public static void KillProcess(string proName)
        {
            if (proName.IsInvalid())
            {
                return;
            }
            var pro = System.Diagnostics.Process.GetProcessesByName(proName);
            if (pro.IsInvalid())
            {
                return;
            }
            foreach (System.Diagnostics.Process p in pro)
            {
                try
                {
                    p.Kill();
                    p.Close();
                    p.Dispose();
                }
                catch { }

            }
        }

        #endregion

    }
}
