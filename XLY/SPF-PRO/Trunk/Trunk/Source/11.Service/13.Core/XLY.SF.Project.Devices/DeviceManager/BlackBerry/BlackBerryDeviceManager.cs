/***************************************************************************
 * 
 * author :  Songbing
 * time: 2017/9/12 13:41:08 
 * explain :  
 *
*****************************************************************************/

using DllClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XLY.SF.Framework.BaseUtility;
using XLY.SF.Framework.Core.Base.CoreInterface;
using XLY.SF.Project.Domains;

namespace XLY.SF.Project.Devices
{
    /// <summary>
    /// 黑莓设备管理
    /// </summary>
    public sealed class BlackBerryDeviceManager : IDeviceManager
    {
        [Obsolete("请使用BlackBerryDeviceManager..Instance获取实例！")]
        public BlackBerryDeviceManager()
        {

        }

        /// <summary>
        /// 黑莓设备管理服务实例，全局唯一单例
        /// </summary>
        public static BlackBerryDeviceManager Instance
        {
            get { return SingleWrapperHelper<BlackBerryDeviceManager>.Instance; }
        }

        /// <summary>
        /// 查找已安装应用列表
        /// </summary>
        /// <param name="device">设备对象</param>
        /// <returns></returns>
        public List<AppEntity> FindInstalledApp(Device device)
        {
            var appEntities = new List<AppEntity>();
            var service = DllClient.X86DLLClientSingle.Instance.BlackBerryDeviceAPIChannel;

            int blackberryHadnle = 0;

            try
            {
                blackberryHadnle = service.BlackBerry_Mount(device.ID);
                if (0 == blackberryHadnle)
                {
                    return appEntities;
                }

                var listappinfos = service.BlackBerry_GetAppDataInfo(blackberryHadnle);
                if (listappinfos.IsInvalid())
                {
                    return appEntities;
                }

                foreach (var appInfo in listappinfos)
                {
                    appEntities.Add(new AppEntity
                    {
                        AppId = appInfo.recordCount.ToString(),
                        Name = appInfo.name
                    });
                }
            }
            catch
            {

            }
            finally
            {
                if (0 != blackberryHadnle)
                {
                    service.BlackBerry_Close(blackberryHadnle);
                }
            }

            return appEntities;
        }

        /// <summary>
        /// 从黑莓系统拷贝一个文件到Windows系统中。
        /// </summary>
        /// <param name="device">任务设备</param>
        /// <param name="source">文件</param>
        /// <param name="targetPath">Windows目标路径。</param>
        /// <param name="asyn">异步通知</param>
        /// <returns>返回Windows路径。</returns>
        public string CopyFile(Device device, string source, string targetPath, IAsyncProgress asyn)
        {
            int blackberryHadnle = 0;

            var service = X86DLLClientSingle.Instance.BlackBerryDeviceAPIChannel;
            try
            {
                blackberryHadnle = service.BlackBerry_Mount(device.ID);
                if (0 == blackberryHadnle)
                {
                    return string.Empty;
                }

                X86DLLClientSingle.Instance.ClientCallback._BlackBerryBackupCallBack += ImageDataCallBack;

                int imageResult = service.BlackBerry_ImageAppData(blackberryHadnle, targetPath);
                if (imageResult != 0)
                {
                    return string.Empty;
                }
            }
            catch
            {

            }
            finally
            {
                X86DLLClientSingle.Instance.ClientCallback._BlackBerryBackupCallBack -= ImageDataCallBack;

                if (0 != blackberryHadnle)
                {
                    service.BlackBerry_Close(blackberryHadnle);
                }
            }

            return targetPath;
        }

        private void ImageDataCallBack(long imagedALLSize, string filename, ref int isStop)
        {

        }

        /// <summary>
        /// 查询设备是否可用
        /// </summary>
        /// <param name="device">设备对象</param>
        /// <returns></returns>
        public bool IsValid(Device device)
        {
            return true;
        }

        /// <summary>
        /// 查询设备是否Root
        /// </summary>
        /// <param name="device">设备对象</param>
        /// <returns></returns>
        public bool IsRoot(Device device)
        {
            return false;
        }

        /// <summary>
        /// 查询设备数据分区信息
        /// </summary>
        /// <param name="device">设备对象</param>
        /// <returns></returns>
        public List<Partition> GetPartitons(Device device)
        {
            var p = new Partition();
            var list = new List<Partition>();
            p.Text = "数据区";
            p.Name = "data";
            list.Add(p);
            return list;
        }

        /// <summary>
        /// 查询设备属性
        /// </summary>
        /// <param name="device">设备对象</param>
        /// <returns></returns>
        public Dictionary<string, string> GetProperties(Device device)
        {
            return new Dictionary<string, string>();
        }

        #region 暂不支持的操作

        /// <summary>
        /// 清除屏幕锁
        /// </summary>
        public void ClearScreenLock(Device device)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 恢复屏幕锁
        /// </summary>
        public void RecoveryScreenLock(Device device)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 查找所有已卸载应用列表，暂时未实现。
        /// </summary>
        public List<AppEntity> FindUnInstalledApp(Device device)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 获取SD卡路径
        /// </summary>
        public string GetSDCardPath(Device device)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 读取手机指定文件目录的内容
        /// </summary>
        public string ReadFile(Device device, string file)
        {
            throw new NotImplementedException();
        }

        #endregion

    }

}
