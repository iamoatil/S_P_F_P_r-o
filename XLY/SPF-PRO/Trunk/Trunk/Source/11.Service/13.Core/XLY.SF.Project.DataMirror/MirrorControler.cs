/***************************************************************************
 * 
 * author :  Songbing
 * time: 2017/8/11 13:44:19 
 * explain :  
 *
*****************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XLY.SF.Framework.BaseUtility;
using XLY.SF.Framework.Core.Base.CoreInterface;
using XLY.SF.Project.BaseUtility.Helper;
using XLY.SF.Project.Domains;

namespace XLY.SF.Project.DataMirror
{
    /// <summary>
    /// 镜像控制器
    /// </summary>
    public class MirrorControler
    {
        /// <summary>
        /// 镜像服务
        /// </summary>
        private IMirrorService MirrorService { get; set; }

        /// <summary>
        /// 执行数据镜像
        /// </summary>
        /// <param name="task">任务</param>
        /// <param name="mirror">镜像源信息</param>
        /// <param name="asyn">异步通知</param>
        public void Execute(SPFTask task, Mirror mirror, IAsyncProgress asyn)
        {
            //生成保存目录
            FileHelper.CreateExitsDirectorySafe(mirror.Target);

            //镜像
            MirrorService = SingleWrapperHelper<MirrorServiceFactory>.GetInstance().GetInstance(mirror);
            MirrorService.Execute(mirror, asyn);

            if (asyn.IsSuccess && FileHelper.IsValid(mirror.Local))
            {//镜像成功
                mirror.VerifyCode = FileHelper.MD5FromFileUpper(mirror.Local);

                //生成MD5文件
                var md5File = mirror.Local.TrimEnd(SPFTask.EXT_MIRROR) + SPFTask.EXT_VERIFYCODE_FILE;
                FileHelper.CreateFile(md5File, mirror.VerifyCode, Encoding.UTF8);

                //生成设备信息文件
                var deviceFile = mirror.Local.TrimEnd(SPFTask.EXT_MIRROR) + SPFTask.EXT_DEVICE;
                Serializer.SerializeToBinary(task.Device, deviceFile);

                task.TaskMirrorFilePath = mirror.Local;
                task.VerifyCodes.Add(new FileVerifyCode() { FilePath = mirror.Local, VerifyCode = mirror.VerifyCode });
            }
        }

        /// <summary>
        /// 停止镜像
        /// </summary>
        public void Stop(IAsyncProgress asyn)
        {
            MirrorService?.Stop(asyn);
        }

        /// <summary>
        /// 暂停镜像
        /// </summary>
        public void Pause(IAsyncProgress asyn)
        {
            if (null != MirrorService && MirrorService.EnableSuspend)
            {
                MirrorService.Suspend(asyn);
            }
        }

        /// <summary>
        /// 继续镜像
        /// </summary>
        public void Continue(IAsyncProgress asyn)
        {
            if (null != MirrorService && MirrorService.EnableSuspend)
            {
                MirrorService.Continue(asyn);
            }
        }
    }
}
