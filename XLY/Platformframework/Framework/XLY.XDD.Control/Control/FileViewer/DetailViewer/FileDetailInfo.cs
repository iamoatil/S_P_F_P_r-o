using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Shell32;

namespace XLY.XDD.Control
{
    /// <summary>
    /// 文件详细信息
    /// </summary>
    public class FileDetailInfo
    {
        #region 属性

        /// <summary>
        /// 文件名称
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// 文件全称
        /// </summary>
        public string FullFileName { get; set; }

        /// <summary>
        /// 文件扩展名
        /// </summary>
        public string FileExtension { get; private set; }

        /// <summary>
        /// 文件目录
        /// </summary>
        public string FileDirectory { get; set; }

        /// <summary>
        /// 文件注释
        /// </summary>
        public string FileComment { get; set; }

        /// <summary>
        /// 文件作者
        /// </summary>
        public string FileAuthor { get; set; }

        /// <summary>
        /// 文件标题
        /// </summary>
        public string FileTitle { get; set; }


        /// <summary>
        /// 文件主题
        /// </summary>
        public string Subject { get; set; }

        /// <summary>
        /// 文件分类
        /// </summary>
        public string FileCategory { get; set; }

        /// <summary>
        /// 文件类型
        /// </summary>
        public string FileType { get; set; }

        /// <summary>
        /// 文件长度
        /// </summary>
        public long FileLength { get; set; }

        /// <summary>
        /// 文件版本
        /// </summary>
        public long FileVersion { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? FileCreationDate { get; set; }

        /// <summary>
        /// 修改时间
        /// </summary>
        public DateTime? FileModification { get; set; }

        #endregion

        /// <summary>
        /// 创建一个文件详细信息类
        /// </summary>
        /// <param name="filePath"></param>
        public FileDetailInfo(string filePath)
        {
            if (filePath.IsNullOrEmptyOrWhiteSpace())
                return;

            try
            {

                // 判断是文件还是文件夹
                //fx.Type = (fx.Attributes & EnumFileAttributeFlags.FILE_ATTRIBUTE_DIRECTORY) > 0 ? FileSystemType.Folder : FileSystemType.File;

                FileInfo fInfo = new FileInfo(filePath);

                if (fInfo.Attributes.HasFlag(FileAttributes.Directory))
                {//文件夹
                    FileExtension = string.Empty;
                    FileLength = 0;
                }
                else
                {//文件
                    FileExtension = fInfo.Extension;
                    FileLength = fInfo.Length;
                }

                FileName = fInfo.Name;
                FullFileName = fInfo.FullName;

                if (filePath.Contains(":"))
                {
                    FileDirectory = fInfo.Directory == null ? string.Empty : fInfo.Directory.ToString();
                    FileCreationDate = fInfo.CreationTime;
                    FileModification = fInfo.LastWriteTime;
                }
                ShellClass sh = new ShellClass();
                // Creating a Folder Object from Folder that inculdes the File
                Folder dir = sh.NameSpace(Path.GetDirectoryName(filePath));
                // Creating a new FolderItem from Folder that includes the File
                if (dir == null)
                {
                    FileCategory = string.Empty;
                    return;
                }


                FolderItem item = dir.ParseName(Path.GetFileName(filePath));
                // loop throw the Folder Items
                // read the current detail Info from the FolderItem Object
                //(Retrieves details about an item in a folder. For example, its size, type, or the time of its last modification.)
                // some examples:
                // 0 Retrieves the name of the item.
                // 1 Retrieves the size of the item.
                // 2 Retrieves the type of the item.
                // 3 Retrieves the date and time that the item was last modified.
                // 4 Retrieves the attributes of the item.
                // -1 Retrieves the info tip information for the item.
                FileType = item.Type ?? dir.GetDetailsOf(item, 2);
                if (FileType != "文件夹")
                {
                    try
                    {
                        FileLength = fInfo.Length;
                    }
                    catch
                    {

                    }
                }
                var docType9 = dir.GetDetailsOf(item, 9);
                var docType11 = dir.GetDetailsOf(item, 11);
                FileCategory = string.IsNullOrWhiteSpace(docType11) ? docType11 : docType9;
            }
            catch
            {
            }
        }
    }
}
