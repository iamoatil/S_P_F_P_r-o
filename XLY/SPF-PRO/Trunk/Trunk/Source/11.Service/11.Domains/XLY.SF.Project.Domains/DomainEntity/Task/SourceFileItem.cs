using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using XLY.SF.Framework.BaseUtility;

namespace XLY.SF.Project.Domains
{
    /// <summary>
    /// 源文件路径配置项
    /// </summary>
    public class SourceFileItem
    {
        /// <summary>
        /// 配置路径
        /// 未经过处理的
        /// </summary>
        public string Config { get; set; }

        /// <summary>
        /// SD卡实际配置路径
        /// </summary>
        public string SDCardConfig
        {
            get
            {
                return ItemType == SourceFileItemType.AndroidSDCardPath ? Config.TrimEnd("SDCard:") : Config;
            }
        }

        /// <summary>
        /// 源文件路径类型
        /// </summary>
        public SourceFileItemType ItemType { get; set; }

        /// <summary>
        /// 本地路径
        /// </summary>
        public string Local { get; set; }

        #region 构造函数

        /// <summary>
        ///  SourceFileItem-构造函数（初始化）
        /// </summary>
        public SourceFileItem(string config, SourceFileItemType type, string local)
        {
            Config = config;
            ItemType = type;
            Local = local;
        }

        public SourceFileItem() { }

        #endregion
    }

    /// <summary>
    /// 源文件路径类型
    /// </summary>
    public enum SourceFileItemType
    {
        /// <summary>
        /// 普通路径,例如 /data/data/com.android.providers.contacts/databases/#F
        /// #F代表该路径是文件夹
        /// </summary>
        NormalPath,
        /// <summary>
        /// 安卓SD卡路径,例如 SDCard:/DJI/dji.pilot
        /// </summary>
        AndroidSDCardPath,
        /// <summary>
        /// 文件类型，例如 $Audio,m4a;mpeg-4;mp3;wma;wav;ape;acc;ogg;amr;3ga;slk
        /// </summary>
        FileExtension
    }

    #region 源文件路径集合

    /// <summary>
    /// 源文件路径集合
    /// </summary>
    public class SourceFileItems : List<SourceFileItem>
    {
        /// <summary>
        /// 添加配置项
        /// </summary>
        public void AddItem(string configPath)
        {
            if (String.IsNullOrEmpty(configPath)) return;
            var res = Find(s => s.Config == configPath);
            if (res == null)
            {
                SourceFileItemType type = SourceFileItemType.NormalPath;

                if (configPath.StartsWith("SDCard:"))
                {// SDCard:/DJI/dji.pilot
                    type = SourceFileItemType.AndroidSDCardPath;
                }
                else if (Regex.IsMatch(configPath, @"^\$\S+,"))
                {// $Audio,m4a;mpeg-4;mp3;wma;wav;ape;acc;ogg;amr;3ga;slk
                    type = SourceFileItemType.FileExtension;
                }

                Add(new SourceFileItem(configPath, type, string.Empty));
            }
        }

        /// <summary>
        /// 添加配置项集合
        /// </summary>
        public void AddItems(List<string> configPaths)
        {
            if (configPaths == null || configPaths.Count <= 0)
            {
                return;
            }

            foreach (var c in configPaths)
            {
                AddItem(c);
            }
        }

        /// <summary>
        /// 获取配置路径所对应的本地路径，若未找到则返回string.empty
        /// </summary>
        public string this[string config]
        {
            get
            {
                var res = Find(s => s.Config == config);
                if (res == null)
                {
                    return string.Empty;
                }
                return res.Local;
            }
        }
    }

    #endregion

}