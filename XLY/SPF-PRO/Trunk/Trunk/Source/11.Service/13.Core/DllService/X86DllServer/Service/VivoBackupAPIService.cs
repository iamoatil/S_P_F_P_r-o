/***************************************************************************
 * 
 * author :  Songbing
 * time: 2017/8/29 16:59:55 
 * explain :  
 *
*****************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using X86DllServer.DllEntry;
using X86DllServer.IService;

namespace X86DllServer.Service
{
    public partial class X86DLLService : IVivoBackupAPIService
    {
        public int VivoBackup_OpenDevice(string deviceSerialnumber)
        {
            return VivoBackupAPI.Android_imageOpenByType(deviceSerialnumber, 1).ToInt32();
        }

        public int VivoBackup_Initialize(int imgHandle)
        {
            return VivoBackupAPI.Android_imageIniEnvironment((IntPtr)imgHandle, 61440, IntPtr.Zero);
        }

        public int VivoBackup_GetAppIDList(int imgHandle, ref List<string> listAppId)
        {
            IntPtr pUserdata = new IntPtr();
            int nums = 0;

            var res = VivoBackupAPI.Android_get_backup_applist((IntPtr)imgHandle, ref pUserdata, ref nums);

            if (0 == res && pUserdata != IntPtr.Zero)
            {
                UserDataAPPInfo[] lsApp = pUserdata.IntPtrToStructs<UserDataAPPInfo>(nums);
                foreach (var app in lsApp)
                {
                    string id = Encoding.UTF8.GetString(app.appid).TrimEnd('\0');
                    if (!listAppId.Contains(id))
                    {
                        listAppId.Add(id);
                    }
                }
            }

            return res;
        }

        public int VivoBackup_BackupFiles(int imgHandle, string psavePath, string[] pbackupappid, int nums)
        {
            return VivoBackupAPI.Android_imgData_backup((IntPtr)imgHandle, psavePath, pbackupappid, nums, VivoBackupCallBack);
        }

        private int VivoBackupCallBack(long size, string filename, ref int stop)
        {
            _clientCallback.VivoBackupCallBack(size, filename, ref stop);

            return 0;
        }

        public int VivoBackup_Close(ref int imgHandle)
        {
            IntPtr pi = (IntPtr)imgHandle;

            var res = VivoBackupAPI.Android_imgData_Close(ref pi);

            imgHandle = pi.ToInt32();

            return res;
        }
    }
}
