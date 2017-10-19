using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;
using System.Text;
using X64Service;
using XLY.SF.Framework.BaseUtility;
using XLY.SF.Framework.Core.Base.CoreInterface;
using XLY.SF.Framework.Language;
using XLY.SF.Framework.Log4NetService;
using XLY.SF.Project.BaseUtility.Helper;
using XLY.SF.Project.Domains;

namespace XLY.SF.Project.Services
{
    /// <summary>
    /// 文件系统扩展服务
    /// </summary>
    public abstract class FileServiceAbstractX
    {
        public bool _isStop = false;
        //private static readonly int MaxSize = 1024 * 64;
        //private static readonly string _UnKnowFile = "unknow_{0}_{1}";
        //private static readonly string _DelFile = "del_{0}";
        public ICollection<string> _normalFolderIdSet = new Collection<string>();
        public ICollection<string> _delFolderIdSet = new Collection<string>();
        public ICollection<string> _problemFolderIdSet = new List<string>();

        /// <summary>
        /// RAW模式下提取的文件格式
        /// </summary>
        //private const string RAW_TYPES = "xml;rtf;doc;xls;png;jpeg;jpg;gif;tif;tiff;bmp;mov;3gp;3g2;m4v;mp4;avs;avi;wav;wma;wmv;rm;rmvb;mpg;mpeg;flv;swf;mp3;mkv;amr;ppt;docx;xlsx;pptx;pdf;bplist;plist;";
        public readonly int[] RAW_TYPES = new int[] { 1, 3, 4, 6, 9 };//1对应的是文档;2是压缩文件;3是多媒体视频 ;4是数据库文件;5是可执行文件;9 是多媒体音频;6是图片文件

        /// <summary>
        /// Raw模式下文件类型与文件头对应列表
        /// </summary>
        public static List<LIST_FILE_RAW_TYPE_INFO> _listRawTypeInfos = null;

        /// <summary>
        /// 消息通知
        /// </summary>
        public IAsyncProgress Asyn { get; private set; }

        /// <summary>
        /// 设备数据
        /// </summary>
        public IFileSystemDevice Device { get; private set; }

        /// <summary>
        /// 当前执行的分区
        /// </summary>
        public FileSystemPartition RunPartition { get; set; }

        /// <summary>
        /// 获取用户数据分区文件列表集合
        /// </summary>
        public List<FNodeX> GetUserPartitionFiles { get; protected set; }

        /// <summary>
        /// 所有文件对象
        /// </summary>
        public List<FNodeX> AllFileNodeX { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="device">当前处理的设备</param>
        /// <param name="iAsyn">异步消息通知</param>
        protected FileServiceAbstractX(IFileSystemDevice device, IAsyncProgress iAsyn)
        {
            Asyn = iAsyn;
            Device = device;

            if (AllFileNodeX == null)
            {
                AllFileNodeX = new List<FNodeX>();
            }
            if (GetUserPartitionFiles == null)
            {
                GetUserPartitionFiles = new NodeCollection();
            }
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Clear()
        {
            if (AllFileNodeX != null)
            {
                AllFileNodeX.Clear();
            }
            if (GetUserPartitionFiles != null)
            {
                GetUserPartitionFiles.Clear();
            }

            Asyn = null;
        }

        /// <summary>
        /// 打开设备
        /// </summary>
        /// <returns>当前设备句柄</returns>
        public abstract IntPtr OpenDevice();

        /// <summary>
        /// 加载磁盘分区
        /// </summary>
        /// <returns></returns>
        public abstract void LoadDevicePartitions();

        /// <summary>
        /// 装载设备
        /// </summary>
        /// <returns></returns>
        public virtual IntPtr MountDevice()
        {
            if (RunPartition == null)
            {
                return IntPtr.Zero;
            }
            if (RunPartition.Mount != IntPtr.Zero)
            {
                return RunPartition.Mount;
            }
            byte fs =
                (byte)
                    (Device.ScanModel == 0xC0  //this.Device.ScanModel == 0x87 || 
                        ? 0xFF
                        : RunPartition.FileSystem);   //高级模式或者RAW模式下设置为0xFF
            IntPtr mount = IntPtr.Zero;
            try
            {
                mount = FileServiceCoreDll.MountPartition(RunPartition.SnapShotHandle, Device.Handle,
                                                          RunPartition.SectorOffset, RunPartition.TotalSectors, fs,
                                                          RunPartition.PartType, RunPartition.DevType);
                if (mount == IntPtr.Zero)
                {
                    LoggerManagerSingle.Instance.Error(string.Format("装载系统分区(名称:{0},描述:{1},大小:{2})失败!", RunPartition.Name, RunPartition.Discription, RunPartition.Size));
                }
            }
            catch (Exception)
            {
                LoggerManagerSingle.Instance.Error(string.Format("FileServiceCoreDll.MountPartition({0},{1},{2},{3},{4},{5},{6}) error!",
                    RunPartition.SnapShotHandle, Device.Handle,
                    RunPartition.SectorOffset, RunPartition.TotalSectors, fs,
                    RunPartition.PartType, RunPartition.DevType));
            }

            RunPartition.Mount = mount;
            return mount;
        }

        /// <summary>
        /// 获取文件系统
        /// </summary>
        /// <returns></returns>
        public FNodeX GetFileSystem()
        {
            _isStop = false;
            if (Device == null)
            {
                return null;
            }
            // 打开设备
            OpenDevice();
            // 加载设备分区
            if (Device.Parts.Count == 0)
            {
                LoadDevicePartitions();
            }
            var fileTree = new FNodeX { IsRoot = true };
            int count = Device.Parts.Count;
            // 构建分区文件系统
            foreach (var part in Device.Parts)
            {
                RunPartition = (FileSystemPartition)part;
                Asyn?.Advance(1 / count, string.Format(LanguageHelperSingle.Instance.GetLanguageByKey(Languagekeys.FileServiceLanguage_File_ZuZhuangFenQuDeWenJianXiTongSh), RunPartition.Name));
                MountDevice();
                var tree = ScanFileSystem(Device, RunPartition);
                if (tree == null)
                {
                    continue;
                }
                // 扫描设备
                fileTree.Collection.Add(tree);
                if (_isStop)
                {
                    break;
                }
            }
            LoggerManagerSingle.Instance.Info(string.Format("扫描文件系统结束, 得到文件/文件夹数:{0}", AllFileNodeX.Count));
            Asyn?.Advance(1, LanguageHelperSingle.Instance.GetLanguageByKey(Languagekeys.FileServiceLanguage_File_WenJianXiTongZuZhuangWanBi));
            return fileTree;
        }

        /// <summary>
        /// 创建磁盘分区
        /// </summary>
        /// <param name="link"></param>
        /// <returns></returns>
        protected List<FileSystemPartition> CreatePartition(DSK_PART_TABLE link)
        {
            var parts = new List<FileSystemPartition>();
            var index = 0;
            while (link.next != IntPtr.Zero)
            {
                link = link.next.ToStruct<DSK_PART_TABLE>();
                if (link.disk_part_info.file_system != 0x00)
                {
                    var partition = new FileSystemPartition();
                    partition.Name = LanguageHelperSingle.Instance.GetLanguageByKey(Languagekeys.FileServiceLanguage_File_FenQu) + ++index;
                    partition.VolName = link.disk_part_info.vol_name == "userdata" ? "data" : link.disk_part_info.vol_name;
                    partition.FileSystem = link.disk_part_info.file_system;
                    partition.SerialNumber = String.Format("{0:X}", link.disk_part_info.serial_num);
                    partition.Size = link.disk_part_info.sectors * 512;
                    partition.SectorOffset = link.disk_part_info.start_lba;
                    partition.TotalSectors = link.disk_part_info.sectors;
                    //partition.PartType = link.disk_part_info.vol_type;
                    parts.Add(partition);
                }
            }
            return parts;
        }

        /// <summary>
        /// 扫描文件系统
        /// </summary>
        /// <param name="partition"></param>
        /// <returns></returns>
        [HandleProcessCorruptedStateExceptions]
        private FNodeX ScanFileSystem(IFileSystemDevice device, FileSystemPartition partition)
        {
            ScanCallBack callBack = (ref ScanCallBackData pdi) => { };
            LINK_DIR_FILE_NODE_INFO link = new LINK_DIR_FILE_NODE_INFO();
            LIST_FILE_RAW_TYPE_INFO raw = CreatRawLink();
            try
            {
                var result = FileServiceCoreDll.ScanFiles(partition.Mount, callBack, ref link, device.ScanModel, ref raw, 0);
                if (result != 0)
                {
                    LoggerManagerSingle.Instance.Error(string.Format("获取文件系统(名称:{ 0},描述: { 1},大小: { 2})失败,错误码: { 3}", partition.Name, partition.Discription, partition.Size, result));

                    return null;
                }
                partition.NodeLinkList = link;
            }
            catch (Exception ex)
            {
                LoggerManagerSingle.Instance.Error(string.Format("获取文件系统(名称:{0},描述:{1},大小:{2},Mount:{3})时底层dll出错", partition.Name, partition.Discription, partition.Size, partition.Mount), ex);
                return null;
            }

            // 构建当前分区节点
            var treeNode = new FNodeX
            {
                Source = link.NodeDataInfo,
                FileName = partition.VolName,
                IsRoot = true,
                IsPartition = true,
                Key = link.NodeDataInfo.FileId,
                ParentKey = link.NodeDataInfo.ParentFileId
            };
            // 加载树结构;
            var source = BuildDataDictionary(link);
            BuildTree(treeNode, source);
            return treeNode;
        }

        private LIST_FILE_RAW_TYPE_INFO CreatRawLink()
        {
            if (_listRawTypeInfos == null)
            {
                _listRawTypeInfos = new List<LIST_FILE_RAW_TYPE_INFO>();

                try
                {
                    StringBuilder sb = new StringBuilder();
                    sb.AppendFormat("select * from FileClass where 1 <> 1 ");
                    RAW_TYPES.ForEach(s => sb.AppendFormat(" or FileType = {0} ", s));
                    SqliteContext sql = new SqliteContext(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "FileRawTypeLib.db"));
                    DataTable dt = sql.FindDataTable(sb.ToString());

                    IntPtr lastPtr = IntPtr.Zero;
                    foreach (DataRow dr in dt.Rows)
                    {
                        FILE_RAW_TYPE_INFO raw = new FILE_RAW_TYPE_INFO();

                        raw.FileType = dr["FileExName"].ToSafeString().Replace(";", "_"); //dr["FileTypeName"].ToSafeString();
                        raw.Size = dr["FileDefaultSize"].ToSafeString().ToSafeInt();
                        raw.ExtensionName = dr["FileExName"].ToSafeString();
                        raw.HeadMagic = dr["FileHeadFlag"].ToSafeString();
                        raw.HeadOffsetBytes = (ushort)dr["FileHeadBeginPos"].ToSafeString().ToSafeInt();
                        raw.FootMagic = dr["FileEndFlag"].ToSafeString();
                        raw.FootOffsetBytes = dr["FileEndPOS"].ToSafeString().ToSafeInt();
                        raw.HeadLen = dr["FileHeadLength"].ToSafeString().ToSafeInt();
                        raw.FootLen = 0;

                        LIST_FILE_RAW_TYPE_INFO rawlist = new LIST_FILE_RAW_TYPE_INFO();
                        rawlist.Check = 1;          //0 - 不检查, 1 - 检查
                        rawlist.Num = 0;
                        rawlist.rt = raw;
                        rawlist.next = lastPtr;

                        int size = Marshal.SizeOf(rawlist);
                        IntPtr rawptr = Marshal.AllocHGlobal(size);
                        Marshal.StructureToPtr(rawlist, rawptr, true);

                        _listRawTypeInfos.Add(rawlist);
                        lastPtr = rawptr;
                    }
                }
                catch (Exception ex)
                {
                    LoggerManagerSingle.Instance.Error("获取Raw文件类型失败！", ex);
                }
            }

            return _listRawTypeInfos.Count > 0 ? _listRawTypeInfos[_listRawTypeInfos.Count - 1] : new LIST_FILE_RAW_TYPE_INFO();
        }

        /// <summary>
        /// 创建数据字典
        /// </summary>
        /// <param name="linkInfo"></param>
        /// <returns></returns>
        public Dictionary<ulong, ICollection<FNodeX>> BuildDataDictionary(LINK_DIR_FILE_NODE_INFO linkInfo)
        {
            var data = new Dictionary<ulong, ICollection<FNodeX>>();
            while (linkInfo.next != IntPtr.Zero)
            {
                linkInfo = linkInfo.next.ToStruct<LINK_DIR_FILE_NODE_INFO>();
                if (linkInfo.NodeDataInfo.FileId == linkInfo.NodeDataInfo.ParentFileId)
                {
                    continue;
                }
                ulong parentId = linkInfo.NodeDataInfo.ParentFileId;
                FNodeX file = BuildFileXNode(linkInfo.NodeDataInfo);
                if (data.ContainsKey(parentId))
                {
                    if (!file.IsDelete)
                        data[parentId].Add(file);
                }
                else
                {
                    data.Add(parentId, new Collection<FNodeX> { file });
                }
                if (RunPartition.VolName != null && RunPartition.VolName.Equals("userdata", StringComparison.OrdinalIgnoreCase))
                {
                    GetUserPartitionFiles.Add(file);
                }
                AllFileNodeX.Add(file);
            }

            foreach (var nfid in _delFolderIdSet)
            {
                if (_normalFolderIdSet.Contains(nfid))
                {
                    _problemFolderIdSet.Add(nfid);
                }
            }
            return data;
        }
        /// <summary>
        /// 建立文件树节点
        /// </summary>
        /// <param name="linkInfo"></param>
        /// <returns></returns>
        public FNodeX BuildFileXNode(DIR_FILE_NODE_INFO linkInfo)
        {
            var file = new FNodeX();
            file.FileName = linkInfo.FileName;
            file.Key = linkInfo.FileId;
            file.ParentKey = linkInfo.ParentFileId;
            file.Mount = RunPartition.Mount;
            file.Source = linkInfo;
            if (!file.IsDelete)
            {
                _normalFolderIdSet.Add(string.Format("{0} - {1}", file.ParentKey, file.Key));
            }
            else
            {
                _delFolderIdSet.Add(string.Format("{0} - {1}", file.ParentKey, file.Key));
            }
            return file;
        }

        public void BuildTree(FNodeX node, Dictionary<ulong, ICollection<FNodeX>> data)
        {
            // 构造正常文件树;
            BuildNormalChildren(node, data);

            // 装载丢失文件树;
        }

        public void BuildNormalChildren(FNodeX node, Dictionary<ulong, ICollection<FNodeX>> dictionary)
        {
            if (dictionary.ContainsKey(node.Key))
            {
                if (_problemFolderIdSet.Contains(string.Format("{0} - {1}", node.ParentKey, node.Key)) && node.IsDelete)
                {
                    return;
                }

                var files = dictionary[node.Key];
                dictionary.Remove(node.Key);

                foreach (var fileX in files)
                {
                    fileX.Directory = node.FullPath;
                    node.Collection.Add(fileX);
                    if (fileX.IsFolder)
                    {
                        BuildNormalChildren(fileX, dictionary);
                    }
                }
            }
        }

        /// <summary>
        /// 把当前文件结构列表归类(按照后缀名)
        /// </summary>
        /// <param name="fNodeXs"></param>
        /// <returns></returns>
        public Dictionary<string, List<FNodeX>> GetDictionarySuffix(IEnumerable<FNodeX> fNodeXs = null)
        {
            var tempDictionary = new Dictionary<string, List<FNodeX>>();
            var source = fNodeXs ?? AllFileNodeX;
            if (source == null || !source.Any())
            {
                return tempDictionary;
            }

            foreach (var fnodex in source)
            {
                if (fnodex.IsFolder)
                {
                    continue;
                }
                if (string.IsNullOrWhiteSpace(fnodex.FileName))
                {
                    continue;
                }
                try
                {
                    var suffix = FileHelper.GetExtension(fnodex.FileName);
                    if (tempDictionary.ContainsKey(suffix))
                    {
                        tempDictionary[suffix].Add(fnodex);
                    }
                    else
                    {
                        tempDictionary.Add(suffix, new List<FNodeX> { fnodex });
                    }
                }
                catch (Exception ex)
                {
                    LoggerManagerSingle.Instance.Error("GetDictionarySuffix Error!", ex);
                }
            }
            return tempDictionary;
        }

        /// <summary>
        /// 读取文件
        /// </summary>
        /// <param name="source">FNodeX数据对象</param>
        /// <param name="offset">偏移位置</param>
        /// <param name="len">读取数据大小</param>
        /// <returns></returns>
        [HandleProcessCorruptedStateExceptions]
        private byte[] ReadFileBytes(FNodeX source, int offset = 0, int len = 0)
        {
            var node = source.Source;
            FILE_RECOVERY_INFO file = new FILE_RECOVERY_INFO();
            file.Attr = node.FileAttribute;
            file.FileId = node.FileId;
            file.OffSet = node.OffsetBytes;
            file.OffsetSec = node.OffsetSec;
            file.ParentFileId = node.ParentFileId;
            file.Size = node.Size;
            byte[] buff = new byte[len];
            try
            {
                FileServiceCoreDll.ReadFileByBottom(source.Mount, ref file, (ulong)offset, len, buff);
            }
            catch (Exception ex)
            {
                LoggerManagerSingle.Instance.Error("提取文件时底层库FileServiceCoreDll.ReadFileByBottom出现异常！", ex);
            }
            return buff;
        }

        /// <summary>
        /// 底层接口恢复文件
        /// </summary>
        /// <param name="source"></param>
        /// <param name="savePath"></param>
        [HandleProcessCorruptedStateExceptions]
        private bool RecoveryFile(FNodeX source, string savePath)
        {
            var node = source.Source;
            FILE_RECOVERY_INFO file = new FILE_RECOVERY_INFO();
            file.Attr = node.FileAttribute;
            file.FileId = node.FileId;
            file.OffSet = node.OffsetBytes;
            file.OffsetSec = node.OffsetSec;
            file.ParentFileId = node.ParentFileId;
            file.Size = node.Size;

            CHECK_SUM_INFO check = new CHECK_SUM_INFO();

            int res = 0;

            IntPtr fileHandle = IntPtr.Zero;
            try
            {
                fileHandle = FileServiceCoreDll.CreateNewFile(savePath);

                res = FileServiceCoreDll.RecoveryFile(source.Mount, fileHandle, LpfnRecoveryFileProgressEx, ref file, ref check);

                if (0 != res)
                {
                    LoggerManagerSingle.Instance.Error(string.Format("提取文件时底层库FileServiceCoreDll.RecoveryFile失败！错误码:{0}", res));
                }
            }
            catch (Exception ex)
            {
                LoggerManagerSingle.Instance.Error("提取文件时底层库FileServiceCoreDll.RecoveryFile出现异常！", ex);
            }
            finally
            {
                if (IntPtr.Zero != fileHandle)
                {
                    FileServiceCoreDll.CloseDevice(fileHandle);
                }
            }

            return res == 0;
        }

        public void LpfnRecoveryFileProgressEx(ref CALL_BACK_RECOVERY_CURRENT_FILE pcf)
        {

        }

        /// <summary>
        /// 恢复文件
        /// </summary>
        /// <param name="file">文件节点对象</param>
        /// <param name="savePath">保存文件路径</param>
        /// <param name="isCover">是否覆盖已存在的文件</param>
        /// <returns></returns>
        public void ExportFileX(FNodeX file, string savePath, bool isMedia = false, bool isCover = false)
        {
            try
            {
                if (file.IsFolder)
                {
                    ExportFolder(file, savePath);
                }
                else
                {
                    ExportFile(file, savePath, isMedia, isCover);
                }
            }
            catch (Exception ex)
            {
                LoggerManagerSingle.Instance.Error(string.Format("export file error[{0}]", file.FullPath), ex);
            }
        }

        /// <summary>
        /// 导出文件夹及递归其下所有文件、文件夹
        /// </summary>
        private void ExportFolder(FNodeX file, string savePath)
        {
            var path = FileHelper.ConnectPath(savePath, file.Directory, file.IsDelete ? "Del_" + file.FileName : file.FileName);
            //1.1创建文件夹
            FileHelper.CreateDirectory(path);
            var creatime = BaseTypeExtension.ToSafeDateTime(file.Source.CreateTime);
            if (creatime != null)
            {
                Directory.SetCreationTime(path, (DateTime)creatime);
            }
            var accessTime = BaseTypeExtension.ToSafeDateTime(file.Source.LastAccessTime);
            if (accessTime != null)
            {
                Directory.SetLastAccessTime(path, (DateTime)accessTime);
            }

            var modifyTime = BaseTypeExtension.ToSafeDateTime(file.Source.ModifyTime);
            if (modifyTime != null)
            {
                Directory.SetLastWriteTime(path, (DateTime)modifyTime);
            }
        }

        /// <summary>
        /// 导出单个文件到指定目录
        /// isCover是否覆盖已存在的文件，默认不覆盖
        /// </summary>
        private void ExportFile(FNodeX file, string savePath, bool isMedia = false, bool isCover = false)
        {
            var path = FileHelper.ConnectPath(savePath, file.Directory.ToSafeString(),
                (isMedia && file.IsDelete) ? "Del_" + file.FileName : file.FileName);
            if (!file.IsFolder)
            {
                path = path.TrimEnd('\\');
            }
            if (!isCover && File.Exists(path))
            {
                return;
            }
            const int t = 500 * 1024 * 1024;
            if (file.Size > t)
            {
                return;
            }
            Directory.CreateDirectory(FileHelper.GetFilePath(path));

            RecoveryFile(file, path);

            Asyn?.Advance(0, string.Format(LanguageHelperSingle.Instance.GetLanguageByKey(Languagekeys.FileServiceLanguage_File_WenJianHuiFuChengGong), file.FullPath.TrimEnd(@"\")));

            var creatime = BaseTypeExtension.ToSafeDateTime(file.Source.CreateTime);
            if (creatime != null)
            {
                File.SetCreationTime(path, (DateTime)creatime);
            }
            var accessTime = BaseTypeExtension.ToSafeDateTime(file.Source.LastAccessTime);
            if (accessTime != null)
            {
                File.SetLastAccessTime(path, (DateTime)accessTime);
            }

            var modifyTime = BaseTypeExtension.ToSafeDateTime(file.Source.ModifyTime);
            if (modifyTime != null)
            {
                File.SetLastWriteTime(path, (DateTime)modifyTime);
            }
        }

        public string FilterLinuxFileName(string filename)
        {
            var index = filename.IndexOfAny(Path.GetInvalidFileNameChars());
            if (index >= 0)
            {
                return FilterLinuxFileName(filename.Replace(filename[index], '_'));
            }
            return filename;
        }

        /// <summary>
        /// 停止操作
        /// </summary>
        public virtual void Stop()
        {
            _isStop = true;
        }

        /// <summary>
        /// 暂停操作
        /// </summary>
        public virtual void Pause()
        {

        }

        /// <summary>
        /// 关闭
        /// </summary>
        public abstract void Close();


    }
}
