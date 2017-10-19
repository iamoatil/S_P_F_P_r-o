using System;
using System.IO;
using zlibNET;

namespace XLY.SF.Project.Devices.AdbSocketManagement
{
    /// <summary>
    /// 设备备份接收器，结果：解析后的压缩文件
    /// </summary>
    public class BackupAndResolveReceiver : AbstractOutputReceiver
    {
        private FileStream fos;
        private ZOutputStream _ZOutStream;
        private bool _IsFirst;

        public BackupAndResolveReceiver(string localfile)
        {
            fos = new FileStream(localfile, FileMode.Create, FileAccess.Write);
            _ZOutStream = new ZOutputStream(fos);
            _IsFirst = true;
        }

        public override void Add(byte[] data, int offset = 0, int length = -1)
        {
            length = length < 0 ? data.Length : length;
            try
            {
                if (_IsFirst)
                {
                    _IsFirst = false;
                    _ZOutStream.Write(data, 24, length - 24);
                }
                else
                    _ZOutStream.Write(data, 0, length);
            }
            catch (IOException e)
            {
                throw new ApplicationException("Backup to file: Writing local file failed!", e);
            }
        }

        public override void Flush()
        {
            try
            {
                _ZOutStream.Flush();
                //解压 此处不解压。
                //using (SevenZip.SevenZipExtractor sz = new SevenZipExtractor(_Stream))
                //{
                //    for (int i = 0; i < sz.ArchiveFileData.Count; i++)
                //    {
                //        sz.ExtractFiles(this._BackupPath, sz.ArchiveFileData[i].Index);
                //    }
                //}
            }
            catch (IOException e)
            {
                throw new ApplicationException("Cat to file: Writing local file failed!", e);
            }
            finally
            {
                Dispose();
            }
        }

        public void Dispose()
        {
            if (_ZOutStream != null) _ZOutStream.Dispose();
            if (fos != null) fos.Dispose();
        }
    }
}
