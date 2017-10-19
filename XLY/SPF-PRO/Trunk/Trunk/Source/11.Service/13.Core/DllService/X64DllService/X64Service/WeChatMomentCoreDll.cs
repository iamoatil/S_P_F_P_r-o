/***************************************************************************
 * 
 * author :  Songbing
 * time: 2017/10/17 9:59:52 
 * explain :  
 *
*****************************************************************************/

using System;
using System.Runtime.InteropServices;

namespace X64Service
{
    /// <summary>
    /// 安卓微信朋友圈数据解析
    /// </summary>
    public static class WeChatMomentCoreDll
    {
        private const string WeiXinMomentDllname = "WeChat_Moment_Extract_for_Android.dll";

        /// <summary>
        /// 解析微信朋友圈内容 
        /// </summary>
        /// <param name="buf">内容在内存中的指针</param>
        /// <param name="size">内容长度</param>
        /// <param name="pHead">返回链表</param>
        /// <returns></returns>
        [DllImport(WeiXinMomentDllname, EntryPoint = "analyzeWeChatMomentInfoforAndroid", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern Int32 analyzeWeChatMomentInfoforAndroid(IntPtr buf, Int32 size, ref IntPtr pHead);

        /// <summary>
        /// 释放微信朋友圈内容
        /// </summary>
        /// <param name="pHead">链表</param>
        [DllImport(WeiXinMomentDllname, EntryPoint = "freeLINK_WXIN_MOMENT_INFO", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern void freeLINK_WXIN_MOMENT_INFO(ref IntPtr pHead);
        /// <summary>
        /// 解析微信朋友圈评论
        /// </summary>
        /// <param name="buf">内容在内存中的指针</param>
        /// <param name="size">内容长度</param>
        /// <param name="pHead">返回链表</param>
        /// <returns></returns>
        [DllImport(WeiXinMomentDllname, EntryPoint = "analyzeWeChatCommentInfoforAndroid", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern Int32 analyzeWeChatCommentInfoforAndroid(IntPtr buf, Int32 size, ref IntPtr pHead);

        /// <summary>
        /// 释放微信朋友圈评论 
        /// </summary>
        /// <param name="pHead">链表</param>
        [DllImport(WeiXinMomentDllname, EntryPoint = "freeLINK_WXIN_COMMENT_INFO", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern void freeLINK_WXIN_COMMENT_INFO(ref IntPtr pHead);

    }

    /// <summary>
    /// 朋友圈主内容
    /// </summary>
    public struct WXIN_MOMENT_ENTRY
    {
        /// <summary>
        /// 朋友圈主内容类型
        /// </summary>
        public WXIN_TYPE_FLAG flag;
        /// <summary>
        /// 长度
        /// </summary>
        public int len;
        /// <summary>
        /// flag为地理位置和文字时，其内容为汉字，编码为utf-8
        /// </summary>
        public IntPtr data_utf;

    }
    /// <summary>
    /// 朋友圈主内容链表
    /// </summary>
    public struct LINK_WXIN_MOMENT_INFO
    {
        public WXIN_MOMENT_ENTRY wme;
        public IntPtr next;
    }

    /// <summary>
    /// 朋友圈评论主内容
    /// </summary>
    public struct WXIN_COMMENT_ENTRY
    {
        /// <summary>
        /// 微信朋友圈评论类型
        /// </summary>
        public WXIN_COMMENT_TYPE_FLAG flag;

        /// <summary>
        /// 评论人的wxid号
        /// </summary>
        public IntPtr uwxid;
        /// <summary>
        /// 评论人的昵称
        /// </summary>
        public IntPtr unick;
        /// <summary>
        /// 评论的内容，如果是点赞类型，该指针的值为1
        /// </summary>
        public IntPtr udata;
    }

    /// <summary>
    /// 朋友圈评论链表
    /// </summary>
    public struct LINK_WXIN_COMMENT_INFO
    {
        public WXIN_COMMENT_ENTRY wce;
        public IntPtr next;

    }

    /// <summary>
    /// 微信朋友圈内容信息类型
    /// </summary>
    public enum WXIN_TYPE_FLAG
    {
        /// <summary>
        /// 未知
        /// </summary>
        WXIN_TYPE_FLAG_UNKNOWN = 0,
        /// <summary>
        /// 文字
        /// </summary>
        WXIN_TYPE_FLAG_WORD = 1,
        /// <summary>
        /// 图片
        /// </summary>
        WXIN_TYPE_FLAG_PIC = 2,
        /// <summary>
        /// 视频
        /// </summary>
        WXIN_TYPE_FLAG_VIDEO = 3,
        /// <summary>
        /// 位置信息
        /// </summary>
        WXIN_TYPE_FLAG_LOCATION = 4,
        /// <summary>
        /// 转发或新闻链接
        /// </summary>
        WXIN_TYPE_FLAG_FORWARD = 5,
    }
    /// <summary>
    /// 微信朋友圈评论类型
    /// </summary>
    public enum WXIN_COMMENT_TYPE_FLAG
    {
        /// <summary>
        /// 未知类型
        /// </summary>
        WXIN_COMMENT_TYPE_FLAG_UNKNOWN = 0,
        /// <summary>
        /// 点赞
        /// </summary>
        WXIN_COMMENT_TYPE_FLAG_LIKE = 1,
        /// <summary>
        /// 评论文字
        /// </summary>
        WXIN_COMMENT_TYPE_FLAG_WORD = 2,
    }

}
