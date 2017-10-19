using System;
using System.Collections.Generic;
using XLY.SF.Framework.BaseUtility;
using XLY.SF.Project.Domains;

namespace XLY.SF.Project.Devices.AdbSocketManagement
{
    /// <summary>
    /// 安装的应用列表数据接收器.
    /// </summary>
    internal class APPReceiver : AbstractOutputReceiver
    {
        public List<AppEntity> APPs;

        /// <summary>
        /// 执行数据解析
        /// </summary>
        public override void DoResolver()
        {
            APPs = new List<AppEntity>();
            AppEntity app = null;
            bool start = false;
            foreach (var line in Lines)
            {
                if (line.IsInvalid()) continue;
                if (!start)
                {
                    if (line == "Packages:")
                    {
                        start = true;
                    }
                    continue;
                }
                try
                {
                    if (line.StartsWith("  Package ["))
                    {
                        app = new AppEntity();
                        app.Name = line.Substring(line.IndexOf('[') + 1, line.LastIndexOf(']') - line.IndexOf('[') - 1);
                        app.AppId = app.Name;
                        APPs.Add(app);
                        continue;
                    }
                    if (app == null) continue;

                    if (line.Contains("codePath"))
                    {
                        app.InstallPath = line.Split('=')[1];
                        continue;
                    }
                    if (line.Contains("versionName"))
                    {
                        app.Version = line.Split('=')[1].ToSafeVersion();
                        continue;
                    }
                    if (line.Contains("dataDir"))
                    {
                        app.DataPath = line.Split('=')[1];
                        continue;
                    }
                    if (line.Contains("firstInstallTime"))
                    {
                        app.InstallDate = line.Split('=')[1].ToSafeDateTime();
                        app = null;
                    }
                }
                catch (Exception e)
                {
                    Framework.Log4NetService.LoggerManagerSingle.Instance.Error(e, "read package line failed:" + line);
                }
            }
        }
    }
}