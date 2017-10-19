/***************************************************************************
 * 
 * author :  Songbing
 * time: 2017/8/29 10:29:29 
 * explain :  
 *
*****************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using X86DllServer.IService;

namespace X86DllServer.Service
{
    public partial class X86DLLService : IAndroidMirrorAPIService
    {
        public Int32 AndroidMirror_OpenDevice(string deviceSerialnumber)
        {
            return AndroidMirrorAPI.OpenDevice(deviceSerialnumber).ToInt32();
        }

        public Int32 AndroidMirror_Initialize(Int32 oIntPtr, Int32 eachReadMaxSize, Int32 htc)
        {
            return AndroidMirrorAPI.Initialize((IntPtr)oIntPtr, eachReadMaxSize, (IntPtr)htc);
        }

        public Int32 AndroidMirror_ImageDataZone(Int32 imgHandle, string pPhysicDataPhonePath, long start, long count)
        {
            return AndroidMirrorAPI.ImageDataZone((IntPtr)imgHandle, pPhysicDataPhonePath, start, count, ImageDataCallBack);
        }

        private int ImageDataCallBack(IntPtr data, int datasize, ref int stop)
        {
            var buff = new byte[datasize];
            Marshal.Copy(data, buff, 0, datasize);

            _clientCallback.ImageDataCallBack(buff, ref stop);

            return 0;
        }

    }
}
