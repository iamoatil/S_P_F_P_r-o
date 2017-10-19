using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


/*************************************************
 * 创建人：Bob
 * 创建时间：2017/3/16 18:12:18
 * 类功能说明：
 *
 *************************************************/

namespace XLY.SF.Project.Domains
{
    /// <summary>
    /// 文件系统类型
    /// </summary>
    public enum FileSystemType : byte
    {
        UnKnown = 0x00, //未知文件系统
        FAT12 = 0x01, //FAT12文件系统
        FAT16 = 0x02, //FAT16文件系统
        FAT32 = 0x03, //FAT32文件系统
        exFAT = 0x04, //exFAT文件系统
        NTFS = 0x05, //NTFS文件系统
        EXT2 = 0x06, //Ext2文件系统
        EXT3 = 0x07, //Ext3文件系统
        EXT4 = 0x08, //Ext4文件系统
        HFS = 0x09,     //HFS文件系统
        HFSPLUS = 0x0A, //HFS+文件系统
        HFSX = 0x0B, //HFSX文件系统
        XFS = 0x0C,     //XFS文件系统
        UFS = 0x0D,	         //UFS, Unix File System
        JFFS = 0x0E,	     //JFFS, Journal
        YAFFS = 0x0F,	      //YAFFS, Yet Another Flash file system
        YAFFS2 = 0x10,	     //YAFFS2,Yet Another Flash file system 	
        REISER = 0x11,	      //
        GFS = 0x12,	          //GFS, google file system	
        KFS = 0x13,     //KFS, Kosmos File System
        HDFS = 0x14,	     //HDFS, Hadoop Distributed File System
        JFS = 0x15,     //IBM Journal file system(JFS) 
        SWAP = 0x16,	     //linux的交换分区
        MAC_FREE_SPACE = 0x17,	     //苹果磁盘空闲分区
        BITLOCKER = 0x3F,             //BitLocker加密分区
        //监控视频分区
        DHFS = 0x40,	 //大华视频文件系统	
        JUANFS = 0x41,	 //九安(JUAN)监控视频文件系统
        DSBAOFS = 0x42,	 //帝视宝(DSBAO)监控视频文件系统
        WFS = 0x43,	     //上海威乾监控视频文件系统
        BSRFS = 0x44,	//蓝色星际监控视频文件系统-bsrfs
        HIKFS = 0x45,	//海康威视监控视频文件系统-hikfs
        ZHLINGFS = 0x46,	    //杭州智领监控视频文件系统-ZhLingFS
        HBFS = 0x47,	//汉邦监控视频文件系统-hbFS
        LOOSAFE = 0x48,	//龙视安监控视频文件系统-LoosafeFS
        BOLIFS = 0x49,	//深圳波粒NVR监控视频文件系统-BoLiFS
        REMISXUNFS = 0x4A,//锐明视讯车载监控视频文件系统-ReMisXunFS	
        RAW_PARTITION = 0xFF     //raw分区,则按raw的方式来查找文件
    }
}
