using System.Collections.Generic;
using System.IO;

namespace XLY.SF.Project.Domains
{
    public enum MTPFileNodeType
    {
        Root,
        Device,
        Directory,
        File
    }

    public class MTPFileNode
    {
        public static readonly string[] ExtensionsWithAudio = new[] { ".m4a", ".mpeg-4", ".mp3", ".wma", ".wav", ".ape", ".acc", ".ogg", ".amr", ".3ga" };
        public static readonly string[] ExtensionsWithImage = new[] { ".jpg", ".jpeg", ".bmp", ".png", ".exif", ".dxf", ".pcx", ".fpx", ".ufo", ".tiff", ".svg", ".eps", ".gif", ".psd", ".ai", ".cdr", ".tga", ".pcd", ".hdri", ".map" };
        public static readonly string[] ExtensionsWithVideo = new[] { ".mp4", ".mpeg", ".mpg", ".dat", ".avi", ".m4v", ".mov", ".3gp", ".rm", ".flv", ".wmv", ".asf", ".navi", "mkv", "f4v", "rmvb", ".webm", ".real video" };

        public string Id { get; set; }
        public string Name { get; set; }
        public MTPFileNodeType Type { get; set; }
        public ulong FileSize { get; set; }
        public int Level { get; set; }
        public List<MTPFileNode> Childrens { get; set; }
        public MTPFileNode Parent { get; set; }

        public string DateCreated { get; set; }
        public string DateModified { get; set; }
        public string DateAuthored { get; set; }

        public string GetFullPath()
        {
            string result = string.Empty;

            MTPFileNode node = this;
            while (null != node.Parent && MTPFileNodeType.Root != node.Parent.Type)
            {
                result = Path.Combine(node.Parent.Name, result);

                node = node.Parent;
            }

            return result;
        }

    }

}
