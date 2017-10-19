using System;
using System.Collections.Generic;
using System.Linq;

namespace XLY.SF.Project.Domains
{
    public enum MTPDeviceType
    {
        Generic = 0,
        Camera = 1,
        MediaPlayer = 2,
        Phone = 3,
        Video = 4,
        PersonalInformationManager = 5,
        AudioRecorder = 6
    }

    public class MTPDevice
    {
        /// <summary>
        /// 设备ID
        /// </summary>
        public string DeviceId { get; set; }

        /// <summary>
        /// 设备类型
        /// </summary>

        public MTPDeviceType DeviceType { get; set; }

        /// <summary>
        /// 设备名称
        /// </summary>
        public string DeviceName { get; set; }

        /// <summary>
        /// 固件版本
        /// </summary>
        public string FirmwareVersion { get; set; }

        /// <summary>
        /// 厂商
        /// </summary>
        public string Manufacturer { get; set; }

        /// <summary>
        /// 型号
        /// </summary>
        public string Model { get; set; }

        /// <summary>
        /// 序列号
        /// </summary>
        public string SerialNumber { get; set; }

        public MTPFileNode RootFileNode { get; set; }

        public List<MTPFileNode> GetFiles(IEnumerable<string> extensions)
        {
            List<MTPFileNode> result = new List<MTPFileNode>();
            GetMTPFileNode(this.RootFileNode, extensions, ref result);
            return result;
        }

        private void GetMTPFileNode(MTPFileNode root, IEnumerable<string> extensions, ref List<MTPFileNode> result)
        {
            foreach (var node in root.Childrens)
            {
                switch (node.Type)
                {
                    case MTPFileNodeType.File:
                        if (extensions.Any((ext) => node.Name.EndsWith(ext, StringComparison.OrdinalIgnoreCase)))
                        {
                            result.Add(node);
                        }
                        break;
                    default:
                        GetMTPFileNode(node, extensions, ref result);
                        break;
                }
            }
        }

    }
}
