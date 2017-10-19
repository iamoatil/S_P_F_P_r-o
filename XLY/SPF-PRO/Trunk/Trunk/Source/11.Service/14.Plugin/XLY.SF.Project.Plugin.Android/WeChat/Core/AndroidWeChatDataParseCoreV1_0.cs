/***************************************************************************
 * 
 * author :  Songbing
 * time: 2017/10/11 10:33:00 
 * explain :  
 *
*****************************************************************************/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using X64Service;
using XLY.SF.Framework.BaseUtility;
using XLY.SF.Project.BaseUtility.Helper;
using XLY.SF.Project.Domains;
using XLY.SF.Project.Persistable.Primitive;
using XLY.SF.Project.Services;

namespace XLY.SF.Project.Plugin.Android
{
    /// <summary>
    /// 安卓微信数据解析核心类
    /// </summary>
    public class AndroidWeChatDataParseCoreV1_0
    {
        /// <summary>
        /// 安卓微信数据解析核心类
        /// </summary>
        /// <param name="savedatadbpath">数据保存数据库路径</param>
        /// <param name="name">微信 例如 微信、小米微信分身</param>
        /// <param name="datapath">微信数据文件根目录，例如 I:\本地数据\com.tencent.mm</param>
        /// <param name="mediapath">微信多媒体文件夹根目录，例如 I:\本地数据\Tencent\MicroMsg</param>
        public AndroidWeChatDataParseCoreV1_0(string savedatadbpath, string name, string datapath, string mediapath)
        {
            DbFilePath = savedatadbpath;
            WeChatName = name;
            DataFileRootPath = datapath;
            MediaFileRootPath = mediapath;
        }

        #region 构造属性

        /// <summary>
        /// 数据库路径
        /// </summary>
        private string DbFilePath { get; set; }

        /// <summary>
        /// 微信名称，例如 微信、小米微信分身
        /// </summary>
        private string WeChatName { get; set; }

        /// <summary>
        ///微信数据文件根目录，例如 I:\本地数据\com.tencent.mm
        /// 该文件夹至少包含 MicroMsg 文件夹
        /// </summary>
        private string DataFileRootPath { get; set; }

        /// <summary>
        /// 微信多媒体文件夹根目录，例如 I:\本地数据\Tencent\MicroMsg
        /// 该文件夹包含了微信聊天的相关文件，如图片、语音、视频
        /// </summary>
        private string MediaFileRootPath { get; set; }

        #endregion

        #region 临时属性

        /// <summary>
        /// 当前微信帐号文件夹
        /// </summary>
        private string MD5AccountPath { get; set; }

        /// <summary>
        /// 当前微信帐号
        /// </summary>
        private WeChatLoginShow WeChatAccount { get; set; }

        /// <summary>
        /// 当前微信帐号显示名称
        /// </summary>
        private string WeChatAccountShowName { get; set; }

        /// <summary>
        /// 主数据库 EnMicroMsg.db
        /// </summary>
        private SqliteContext MainDbContext { get; set; }

        /// <summary>
        /// bak数据库 EnMicroMsg.bak.db
        /// </summary>
        private SqliteContext WxbakMsgdataContext { get; set; }

        /// <summary>
        /// IndexMicroMsg.db
        /// </summary>
        private SqliteContext IndexMsgContext { get; set; }

        /// <summary>
        /// FTS5IndexMicroMsg.db
        /// </summary>
        private SqliteContext FTS5IndexMsgContext { get; set; }

        /// <summary>
        /// bak数据库解析帐号列表
        /// </summary>
        private List<string> ListWxbakMsgWxid { get; set; }

        /// <summary>
        /// 好友列表
        /// </summary>
        private List<WeChatFriendShow> LsAllFriends { get; set; }

        /// <summary>
        /// 群聊列表
        /// </summary>
        private List<WeChatGroupShow> LsAllGroups { get; set; }

        /// <summary>
        /// 从IndexMicroMsg.db和FTS5IndexMicroMsg.db获取的删除消息
        /// </summary>
        private Dictionary<string, List<dynamic>> DeleteMsgs { get; set; }

        /// <summary>
        /// 我的收藏文件列表
        /// </summary>
        private List<FileInfo> ListFavoriteFiles { get; set; }

        /// <summary>
        /// 清除临时属性
        /// 一般用于插件执行完毕后执行
        /// </summary>
        private void ClearCache()
        {
            MD5AccountPath = string.Empty;
            WeChatAccount = null;
            WeChatAccountShowName = string.Empty;

            MainDbContext?.Dispose();
            MainDbContext = null;

            WxbakMsgdataContext?.Dispose();
            WxbakMsgdataContext = null;

            IndexMsgContext?.Dispose();
            IndexMsgContext = null;

            FTS5IndexMsgContext?.Dispose();
            FTS5IndexMsgContext = null;

            ListWxbakMsgWxid?.Clear();
            ListWxbakMsgWxid = null;

            LsAllFriends?.Clear();
            LsAllFriends = null;

            LsAllGroups?.Clear();
            LsAllGroups = null;

            DeleteMsgs?.Clear();
            DeleteMsgs = null;

            ListFavoriteFiles?.Clear();
            ListFavoriteFiles = null;

        }

        #endregion

        /// <summary>
        /// 解析微信数据
        /// </summary>
        /// <returns></returns>
        public TreeNode BiuldTree()
        {
            TreeNode rootNode = new TreeNode();
            try
            {
                rootNode.Text = WeChatName;
                rootNode.Type = typeof(WeChatLoginShow);
                rootNode.Items = new DataItems<WeChatLoginShow>(DbFilePath);

                foreach (var account in GetAllWeChatAccountFileInfos())
                {
                    ClearCache();

                    MD5AccountPath = account.Directory.Name;
                    BuildWeChatTree(rootNode, account);
                }

                return rootNode;
            }
            finally
            {
                rootNode.BuildParent();

                ClearCache();
            }
        }

        #region 构建数据

        /// <summary>
        /// EnMicroMsg数据库需要恢复的表
        /// </summary>
        private const string _rEnMicroMsgTables = "userinfo,message,rcontact,friend_ext,chatroom,img_flag,userinfo,WalletBankcard,SafeDeviceInfo,ImgInfo2,EmojiInfo,massendinfo";

        /// <summary>
        /// 好友查询Sql
        /// </summary>
        private const string _sContactsSql = "SELECT * FROM rcontact WHERE type != '33' AND username not like '%@chatroom' AND username not like 'gh_%@app' AND username != 'filehelper' ORDER BY type,username";

        /// <summary>
        /// 构建单个微信帐号数据
        /// </summary>
        /// <param name="rootNode">根节点</param>
        /// <param name="weChatFile">EnMicroMsg.db文件</param>
        private void BuildWeChatTree(TreeNode rootNode, FileInfo weChatFile)
        {
            //数据库处理
            GetSqliteContext(weChatFile);

            //构建微信帐号信息
            var accountNode = BuildAccountInfo(rootNode);

            //从IndexMicroMsg.db和FTS5IndexMicroMsg.db获取删除消息
            GetDeleteMsgFromIndexDb();

            //构建通讯录
            BuildContactsNode(accountNode);

            //构建通讯录消息
            BuildContactsMsgNode(accountNode);

            //构建群聊
            BuildGroupNode(accountNode);

            //构建群聊消息
            BuildGroupMsgNode(accountNode);

            //构建群发消息树
            BuildMassendMsgNode(accountNode);

            //构建IndexMsg数据库其他删除消息
            BuildIndexMsgContextDeleteMsgNode(accountNode);

            //构建朋友圈
            BuildSnsNode(accountNode);

            //构建我的收藏
            BuildFavoriteNode(accountNode);

            //构建小程序
            BuildAppBrandNode(accountNode);

            //构建银行卡列表
            BuildMyWalletNode(accountNode);

            //构建设备列表
            BuildChatSafeDevicesNode(accountNode);

        }

        /// <summary>
        /// 构建微信帐号信息
        /// </summary>
        /// <param name="rootNode">微信根节点</param>
        /// <returns>微信帐号根节点</returns>
        private TreeNode BuildAccountInfo(TreeNode rootNode)
        {
            //获取帐号信息
            WeChatAccount = GetAccountUser();
            WeChatAccountShowName = WeChatAccount.ShowName;

            //当前账户树节点
            var accountNode = new TreeNode
            {
                DataState = EnumDataState.Normal,
                Text = string.Format("{0}({1})", WeChatAccount.Nick, WeChatAccount.WeChatId),
                Id = WeChatAccount.WeChatId,
                Type = typeof(WeChatLoginShow),
                Items = new DataItems<WeChatLoginShow>(DbFilePath)
            };
            accountNode.Items.Add(WeChatAccount);
            rootNode.Items.Add(WeChatAccount);
            rootNode.TreeNodes.Add(accountNode);

            return accountNode;
        }

        /// <summary>
        /// 构建通讯录
        /// </summary>
        /// <param name="rootNode"></param>
        private void BuildContactsNode(TreeNode rootNode)
        {
            var friendNode = new TreeNode
            {
                DataState = EnumDataState.Normal,
                Text = "通讯录",
                Type = typeof(WeChatFriendShow),
                Items = new DataItems<WeChatFriendShow>(DbFilePath),
                Id = WeChatAccount.WeChatId
            };

            var gongzhonghaoNode = new TreeNode
            {
                DataState = EnumDataState.Normal,
                Text = "公众号",
                Type = typeof(WeChatFriendShow),
                Items = new DataItems<WeChatFriendShow>(DbFilePath),
                Id = WeChatAccount.WeChatId
            };

            rootNode.TreeNodes.Add(friendNode);
            rootNode.TreeNodes.Add(gongzhonghaoNode);

            LsAllFriends = new List<WeChatFriendShow>();

            MainDbContext.UsingSafeConnection(_sContactsSql, (r) =>
             {
                 WeChatFriendShow f = null;
                 dynamic df;
                 while (r.Read())
                 {
                     df = r.ToDynamic();

                     f = CreateFriendShow(df);
                     if (null != f)
                     {
                         LsAllFriends.Add(f);
                     }
                 }
             });

            MainDbContext.UsingSafeConnection("SELECT DISTINCT talker FROM message WHERE talker not LIKE '%@chatroom'", (r) =>
             {
                 dynamic dy;
                 while (r.Read())
                 {
                     dy = r.ToDynamic();

                     string wechatid = DynamicConvert.ToSafeString(dy.talker);
                     if (wechatid.IsValid() && !LsAllFriends.Any(f => f.WeChatId == wechatid))
                     {
                         LsAllFriends.Add(new WeChatFriendShow() { WeChatId = wechatid, Nick = "", DataState = EnumDataState.Deleted });
                     }
                 }
             });

            foreach (var ff in LsAllFriends)
            {
                if (ff.FriendType == WeChatFriendTypeEnum.Subscription)
                {
                    gongzhonghaoNode.Items.Add(ff);
                }
                else
                {
                    friendNode.Items.Add(ff);
                }
            }
        }

        /// <summary>
        /// 构建通讯录消息
        /// </summary>
        /// <param name="rootNode"></param>
        private void BuildContactsMsgNode(TreeNode rootNode)
        {
            var msgNode = new TreeNode
            {
                DataState = EnumDataState.Normal,
                Text = "好友消息",
                Id = WeChatAccount.WeChatId
            };

            var ghMsgNode = new TreeNode
            {
                DataState = EnumDataState.Normal,
                Text = "公众号消息",
                Id = WeChatAccount.WeChatId
            };

            rootNode.TreeNodes.Add(msgNode);
            rootNode.TreeNodes.Add(ghMsgNode);

            foreach (var friend in LsAllFriends)
            {
                if (friend.FriendType == WeChatFriendTypeEnum.Subscription)
                {
                    BuildMsgNode(friend, ghMsgNode);
                }
                else
                {
                    BuildMsgNode(friend, msgNode);
                }
            }

            if (null != WxbakMsgdataContext)
            {
                var lsWxid = WxbakMsgdataContext.Find(new SQLiteString("SELECT DISTINCT wxid FROM WxbakMsgdata WHERE wxid not like '%@chatroom'")).
                    Select(s => DynamicConvert.ToSafeString(s.wxid)).Cast<string>();

                var accountName = WeChatAccount.ShowName;

                foreach (var wxid in lsWxid)
                {
                    if (ListWxbakMsgWxid.Contains(wxid))
                    {
                        continue;
                    }

                    var dmsgNode = new TreeNode()
                    {
                        DataState = EnumDataState.Deleted,
                        Text = wxid,
                        Type = typeof(MessageCore),
                        Items = new DataItems<MessageCore>(DbFilePath),
                    };

                    if(wxid.StartsWith("gh_"))
                    {
                        ghMsgNode.TreeNodes.Add(dmsgNode);
                    }
                    else
                    {
                        msgNode.TreeNodes.Add(dmsgNode);
                    }

                    WxbakMsgdataContext.UsingSafeConnection(string.Format("SELECT * FROM WxbakMsgdata WHERE wxid = '{0}'", wxid), r =>
                     {
                         MessageCore message;

                         while (r.Read())
                         {
                             message = new MessageCore();
                             GetWxbakMsgdataContextMsg(r.ToDynamic(), ref message);

                             if (message.Type == EnumColumnType.Audio || message.Type == EnumColumnType.Video || message.Type == EnumColumnType.Image)
                             {
                                 continue;
                             }

                             if (message.Type == EnumColumnType.System)
                             {
                                 if (message.Content.StartsWith("<img src=\"SystemMessages_HongbaoIcon.png\"/>"))
                                 {//领取红包
                                     message.Content = string.Format("微信红包-{0}", Regex.Replace(message.Content, @"<[^<>]*>", "").Trim());
                                 }
                                 message.SendState = EnumSendState.Receive;
                                 message.SenderName = "系统消息";
                                 message.Receiver = accountName;
                             }
                             else if (message.SendState == EnumSendState.Send)
                             {
                                 message.SenderName = accountName;
                                 message.SenderImage = WeChatAccount.HeadPng;
                                 message.Receiver = wxid;
                             }
                             else
                             {
                                 message.SenderName = wxid;
                                 message.Receiver = accountName;
                             }

                             GetMessageContent(message.Content, "", "", message.Type, false);

                             dmsgNode.Items.Add(message);
                         }
                     });
                }
            }
        }

        /// <summary>
        /// 创建好友(公众号)消息树
        /// </summary>
        /// <param name="friend"></param>
        private void BuildMsgNode(WeChatFriendShow friend, TreeNode rootNode)
        {
            var msgNode = new TreeNode
            {
                DataState = friend.DataState,
                Text = friend.ShowName,
                Type = typeof(MessageCore),
                Items = new DataItems<MessageCore>(DbFilePath),
            };
            rootNode.TreeNodes.Add(msgNode);

            var accountName = WeChatAccountShowName;
            var friendname = friend.ShowName;

            //从message表获取聊天记录
            MainDbContext.UsingSafeConnection(string.Format("select * from message where talker  = '{0}'", friend.WeChatId), r =>
             {
                 MessageCore message = null;
                 dynamic mess;
                 while (r.Read())
                 {
                     mess = r.ToDynamic();
                     message = new MessageCore();

                     message.DataState = DynamicConvert.ToEnumByValue<EnumDataState>(mess.XLY_DataType, EnumDataState.Normal);
                     message.Date = DynamicConvert.ToSafeDateTime(mess.createTime);

                     //获取消息类型和内容
                     GetMessage(mess, ref message);

                     if (message.Type == EnumColumnType.System)
                     {
                         if (message.Content.StartsWith("<img src=\"SystemMessages_HongbaoIcon.png\"/>"))
                         {//领取红包
                             message.Content = string.Format("微信红包-{0}", Regex.Replace(message.Content, @"<[^<>]*>", "").Trim());
                         }
                         message.SendState = EnumSendState.Receive;
                         message.SenderName = "系统消息";
                         message.Receiver = accountName;
                     }
                     else
                     {
                         if (DynamicConvert.ToSafeString(mess.isSend) == "1")
                         {//发送
                             message.SendState = EnumSendState.Send;
                             message.SenderName = accountName;
                             message.SenderImage = WeChatAccount.HeadPng;
                             message.Receiver = friendname;
                         }
                         else
                         {//接收
                             message.SendState = EnumSendState.Receive;
                             message.SenderName = friendname;
                             message.SenderImage = friend.HeadPng;
                             message.Receiver = accountName;
                         }
                     }

                     msgNode.Items.Add(message);
                 }
             });

            //从bak数据库获取聊天记录
            if (null != WxbakMsgdataContext)
            {
                ListWxbakMsgWxid.Add(friend.WeChatId);

                WxbakMsgdataContext.UsingSafeConnection(string.Format("SELECT * FROM WxbakMsgdata WHERE wxid = '{0}'", friend.WeChatId), (r) =>
                 {
                     MessageCore message;
                     while (r.Read())
                     {
                         message = new MessageCore();

                         GetWxbakMsgdataContextMsg(r.ToDynamic(), ref message);

                         if (message.Type == EnumColumnType.Audio || message.Type == EnumColumnType.Video || message.Type == EnumColumnType.Image)
                         {
                             continue;
                         }

                         if (message.Type == EnumColumnType.System)
                         {
                             if (message.Content.StartsWith("<img src=\"SystemMessages_HongbaoIcon.png\"/>"))
                             {//领取红包
                                 message.Content = string.Format("微信红包-{0}", Regex.Replace(message.Content, @"<[^<>]*>", "").Trim());
                             }
                             message.SendState = EnumSendState.Receive;
                             message.SenderName = "系统消息";
                             message.Receiver = accountName;
                         }
                         else if (message.SendState == EnumSendState.Send)
                         {
                             message.SenderName = accountName;
                             message.SenderImage = WeChatAccount.HeadPng;
                             message.Receiver = friendname;
                         }
                         else
                         {
                             message.SenderName = friendname;
                             message.SenderImage = friend.HeadPng;
                             message.Receiver = accountName;
                         }

                         GetMessageContent(message.Content, "", "", message.Type, false);

                         msgNode.Items.Add(message);
                     }
                 });
            }

            //从IndexMicroMsg.db和FTS5IndexMicroMsg.db获取删除消息
            if (DeleteMsgs.Keys.Contains(friend.WeChatId))
            {
                var deletes = DeleteMsgs.FirstOrDefault(g => g.Key == friend.WeChatId).Value;
                var talkerid = string.Empty;

                foreach (var mess in deletes)
                {
                    var message = new MessageCore
                    {
                        DataState = EnumDataState.Deleted
                    };

                    message.Date = DynamicConvert.ToSafeDateTime(mess.timestamp);
                    message.Content = DynamicConvert.ToSafeString(mess.c0content);

                    talkerid = DynamicConvert.ToSafeString(mess.talker);
                    if (talkerid.IsValid())
                    {
                        if (talkerid == WeChatAccount.WeChatId)
                        {//发送
                            message.SendState = EnumSendState.Send;
                            message.SenderName = accountName;
                            message.SenderImage = WeChatAccount.HeadPng;
                            message.Receiver = friendname;
                        }
                        else
                        {
                            message.SendState = EnumSendState.Receive;
                            message.SenderName = friendname;
                            message.SenderImage = friend.HeadPng;
                            message.Receiver = accountName;
                        }
                    }

                    msgNode.Items.Add(message);
                }
            }
        }

        /// <summary>
        /// 构建群聊树
        /// </summary>
        /// <param name="rootNode"></param>
        private void BuildGroupNode(TreeNode rootNode)
        {
            var groupNode = new TreeNode
            {
                DataState = EnumDataState.Normal,
                Text = "群聊",
                Type = typeof(WeChatGroupShow),
                Items = new DataItems<WeChatGroupShow>(DbFilePath),
                Id = WeChatAccount.WeChatId
            };

            rootNode.TreeNodes.Add(groupNode);

            LsAllGroups = new List<WeChatGroupShow>();

            var sql = @"SELECT
                        	r.username,
                        	r.nickname,
                        	c.displayname,
                        	c.memberlist,
                        	c.chatroomnick,
                        	c.chatroomnotice,
                        	c.modifytime,
                        	c.roomowner,
                        	r.XLY_DataType
                        FROM
                        	rcontact r,
                        	chatroom c
                        WHERE
                        	r.username LIKE '%@CHATROOM'
                        AND r.username = c.chatroomname";

            MainDbContext.UsingSafeConnection(sql, r =>
             {
                 dynamic data;
                 WeChatGroupShow chatroom = null;

                 while (r.Read())
                 {
                     data = r.ToDynamic();

                     chatroom = CreateWeChatGroupShow(data);
                     if (null != chatroom)
                     {
                         groupNode.Items.Add(chatroom);
                         LsAllGroups.Add(chatroom);
                     }
                 }
             });

            MainDbContext.UsingSafeConnection("SELECT DISTINCT talker FROM message WHERE talker LIKE '%@CHATROOM'", r =>
            {
                dynamic dy;
                string talker;

                while (r.Read())
                {
                    dy = r.ToDynamic();
                    talker = DynamicConvert.ToSafeString(dy.talker);

                    if (!LsAllGroups.Any(g => g.WeChatId == talker))
                    {
                        var chatroom = new WeChatGroupShow() { WeChatId = talker, DataState = EnumDataState.Deleted };

                        groupNode.Items.Add(chatroom);
                        LsAllGroups.Add(chatroom);
                    }
                }
            });
        }

        /// <summary>
        /// 构建群聊消息树
        /// </summary>
        /// <param name="rootNode"></param>
        private void BuildGroupMsgNode(TreeNode rootNode)
        {
            var groupMsgNode = new TreeNode
            {
                DataState = EnumDataState.Normal,
                Text = "群聊消息",
                Id = WeChatAccount.WeChatId
            };

            rootNode.TreeNodes.Add(groupMsgNode);

            foreach (var group in LsAllGroups)
            {
                BuildMsgNode(group, groupMsgNode);
            }

            if (null != WxbakMsgdataContext)
            {
                var lsWxid = WxbakMsgdataContext.Find(new SQLiteString("SELECT DISTINCT wxid FROM WxbakMsgdata WHERE wxid like '%@chatroom'")).
                    Select(s => DynamicConvert.ToSafeString(s.wxid)).Cast<string>();

                var accountName = WeChatAccount.ShowName;

                foreach (var wxid in lsWxid)
                {
                    if (ListWxbakMsgWxid.Contains(wxid))
                    {
                        continue;
                    }

                    var dmsgNode = new TreeNode()
                    {
                        DataState = EnumDataState.Deleted,
                        Text = wxid,
                        Type = typeof(MessageCore),
                        Items = new DataItems<MessageCore>(DbFilePath),
                    };
                    groupMsgNode.TreeNodes.Add(dmsgNode);

                    WxbakMsgdataContext.UsingSafeConnection(string.Format("SELECT * FROM WxbakMsgdata WHERE wxid = '{0}'", wxid), r =>
                    {
                        MessageCore message;

                        while (r.Read())
                        {
                            message = new MessageCore();
                            GetWxbakMsgdataContextMsg(r.ToDynamic(), ref message);

                            if (message.Type == EnumColumnType.Audio || message.Type == EnumColumnType.Video || message.Type == EnumColumnType.Image)
                            {
                                continue;
                            }

                            if (message.Type == EnumColumnType.System)
                            {
                                if (message.Content.StartsWith("<img src=\"SystemMessages_HongbaoIcon.png\"/>"))
                                {//领取红包
                                    message.Content = string.Format("微信红包-{0}", Regex.Replace(message.Content, @"<[^<>]*>", "").Trim());
                                }
                                message.SendState = EnumSendState.Receive;
                                message.SenderName = "系统消息";
                                message.Receiver = accountName;
                            }
                            else if (message.SendState == EnumSendState.Send)
                            {
                                message.SenderName = accountName;
                                message.SenderImage = WeChatAccount.HeadPng;
                                message.Receiver = wxid;
                            }
                            else
                            {
                                message.SenderName = wxid;
                                message.Receiver = accountName;
                            }

                            GetMessageContent(message.Content, "", "", message.Type, true);

                            dmsgNode.Items.Add(message);
                        }
                    });
                }
            }

        }

        /// <summary>
        /// 构建群聊消息
        /// </summary>
        /// <param name="group"></param>
        /// <param name="rootNode"></param>
        private void BuildMsgNode(WeChatGroupShow group, TreeNode rootNode)
        {
            var msgNode = new TreeNode
            {
                DataState = group.DataState,
                Text = group.ShowName,
                Type = typeof(MessageCore),
                Items = new DataItems<MessageCore>(DbFilePath),
            };
            rootNode.TreeNodes.Add(msgNode);

            var accountName = WeChatAccountShowName;
            var groupname = group.ShowName;

            //从message表获取聊天记录
            MainDbContext.UsingSafeConnection(string.Format("select * from message where talker  = '{0}'", group.WeChatId), r =>
            {
                MessageCore message = null;
                dynamic mess;
                while (r.Read())
                {
                    mess = r.ToDynamic();
                    message = new MessageCore();

                    message.DataState = DynamicConvert.ToEnumByValue<EnumDataState>(mess.XLY_DataType, EnumDataState.Normal);
                    message.Date = DynamicConvert.ToSafeDateTime(mess.createTime);

                    //获取消息类型和内容
                    GetMessage(mess, ref message, true);

                    if (message.Type == EnumColumnType.System)
                    {
                        if (message.Content.StartsWith("<img src=\"SystemMessages_HongbaoIcon.png\"/>"))
                        {//领取红包
                            message.Content = string.Format("微信红包-{0}", Regex.Replace(message.Content, @"<[^<>]*>", "").Trim());
                        }
                        message.SendState = EnumSendState.Receive;
                        message.SenderName = "系统消息";
                        message.Receiver = accountName;
                    }
                    else
                    {
                        var isSend = DynamicConvert.ToSafeString(mess.isSend) == "1";
                        if (isSend)
                        {
                            message.SendState = EnumSendState.Send;
                            message.SenderName = accountName;
                            message.SenderImage = WeChatAccount.HeadPng;
                            message.Receiver = groupname;
                        }
                        else
                        {
                            message.SendState = EnumSendState.Receive;
                            message.Receiver = accountName;

                            string content = DynamicConvert.ToSafeString(mess.content);
                            string talkerId = string.Empty;
                            if (content.Contains(":\n"))
                            {//群聊消息   xxxx:\nssssss     xxxx为发言人微信号  ssssss为聊天内容 如果是自己发送的  则直接就是聊天内容
                                talkerId = content.Substring(0, content.IndexOf(":\n"));
                            }
                            else if (content.Contains(":"))
                            {
                                talkerId = content.Substring(0, content.IndexOf(":"));
                            }

                            if (talkerId.IsInvalid())
                            {
                                message.SenderName = groupname;
                                //message.SenderImage = group.HeadPng;
                            }
                            else
                            {
                                var talker = LsAllFriends.FirstOrDefault(f => f.WeChatId == talkerId);
                                if (null != talker)
                                {
                                    message.SenderName = talker.ShowName;
                                    message.SenderImage = talker.HeadPng;
                                }
                                else
                                {
                                    message.SenderName = talkerId;
                                }
                            }
                        }
                    }

                    msgNode.Items.Add(message);
                }
            });

            //从bak数据库获取聊天记录
            if (null != WxbakMsgdataContext)
            {
                ListWxbakMsgWxid.Add(group.WeChatId);

                WxbakMsgdataContext.UsingSafeConnection(string.Format("SELECT * FROM WxbakMsgdata WHERE wxid = '{0}'", group.WeChatId), (r) =>
                {
                    MessageCore message;
                    while (r.Read())
                    {
                        message = new MessageCore();

                        GetWxbakMsgdataContextMsg(r.ToDynamic(), ref message);

                        if (message.Type == EnumColumnType.Audio || message.Type == EnumColumnType.Video || message.Type == EnumColumnType.Image)
                        {
                            continue;
                        }

                        if (message.Type == EnumColumnType.System)
                        {
                            if (message.Content.StartsWith("<img src=\"SystemMessages_HongbaoIcon.png\"/>"))
                            {//领取红包
                                message.Content = string.Format("微信红包-{0}", Regex.Replace(message.Content, @"<[^<>]*>", "").Trim());
                            }
                            message.SendState = EnumSendState.Receive;
                            message.SenderName = "系统消息";
                            message.Receiver = accountName;
                        }
                        else if (message.SendState == EnumSendState.Send)
                        {
                            message.SenderName = accountName;
                            message.SenderImage = WeChatAccount.HeadPng;
                            message.Receiver = groupname;
                        }
                        else
                        {
                            message.SendState = EnumSendState.Receive;
                            message.Receiver = accountName;

                            string content = message.Content;
                            string talkerId = string.Empty;
                            if (content.Contains(":\n"))
                            {//群聊消息   xxxx:\nssssss     xxxx为发言人微信号  ssssss为聊天内容 如果是自己发送的  则直接就是聊天内容
                                talkerId = content.Substring(0, content.IndexOf(":\n"));
                            }
                            else if (content.Contains(":"))
                            {
                                talkerId = content.Substring(0, content.IndexOf(":"));
                            }

                            if (talkerId.IsInvalid())
                            {
                                message.SenderName = groupname;
                                //message.SenderImage = group.HeadPng;
                            }
                            else
                            {
                                var talker = LsAllFriends.FirstOrDefault(f => f.WeChatId == talkerId);
                                if (null != talker)
                                {
                                    message.SenderName = talker.ShowName;
                                    message.SenderImage = talker.HeadPng;
                                }
                                else
                                {
                                    message.SenderName = talkerId;
                                }
                            }
                        }

                        GetMessageContent(message.Content, "", "", message.Type, true);

                        msgNode.Items.Add(message);
                    }
                });
            }

            //从IndexMicroMsg.db和FTS5IndexMicroMsg.db获取删除消息
            if (DeleteMsgs.Keys.Contains(group.WeChatId))
            {
                var deletes = DeleteMsgs.FirstOrDefault(g => g.Key == group.WeChatId).Value;
                var talkerid = string.Empty;

                foreach (var mess in deletes)
                {
                    var message = new MessageCore
                    {
                        DataState = EnumDataState.Deleted
                    };

                    message.Date = DynamicConvert.ToSafeDateTime(mess.timestamp);
                    message.Content = DynamicConvert.ToSafeString(mess.c0content);

                    talkerid = DynamicConvert.ToSafeString(mess.talker);
                    if (talkerid.IsValid())
                    {
                        if (talkerid == WeChatAccount.WeChatId)
                        {//发送
                            message.SendState = EnumSendState.Send;
                            message.SenderName = accountName;
                            message.SenderImage = WeChatAccount.HeadPng;
                            message.Receiver = groupname;
                        }
                        else
                        {
                            message.SendState = EnumSendState.Receive;
                            message.Receiver = accountName;

                            var talker = LsAllFriends.FirstOrDefault(f => f.WeChatId == talkerid);
                            if (null != talker)
                            {
                                message.SenderName = talker.ShowName;
                                message.SenderImage = talker.HeadPng;
                            }
                            else
                            {
                                message.SenderName = talkerid;
                            }
                        }
                    }

                    msgNode.Items.Add(message);
                }
            }
        }

        /// <summary>
        /// 构建群发消息树
        /// </summary>
        /// <param name="rootNode"></param>
        private void BuildMassendMsgNode(TreeNode rootNode)
        {
            TreeNode massendNode = new TreeNode()
            {
                DataState = EnumDataState.Normal,
                Text = "群发消息",
                Items = new DataItems<MessageCore>(DbFilePath),
                Type = typeof(MessageCore)
            };

            rootNode.TreeNodes.Add(massendNode);

            MainDbContext.UsingSafeConnection("SELECT createtime,filename,thumbfilename,tolist,msgtype FROM massendinfo", r =>
             {
                 MessageCore message = null;
                 string msgtype = string.Empty;
                 string tolist = string.Empty;
                 dynamic massendInfo;

                 while (r.Read())
                 {
                     massendInfo = r.ToDynamic();

                     message = new MessageCore();
                     message.SenderName = WeChatAccountShowName;
                     message.SenderImage = WeChatAccount.HeadPng;
                     message.SendState = EnumSendState.Send;
                     message.Date = DynamicConvert.ToSafeDateTime(massendInfo.createtime);
                     message.Content = DynamicConvert.ToSafeString(massendInfo.filename);

                     tolist = DynamicConvert.ToSafeString(massendInfo.tolist);
                     StringBuilder sb = new StringBuilder();
                     foreach (var toUin in tolist.Split(';'))
                     {
                         var toFriend = LsAllFriends.FirstOrDefault(f => f.WeChatId == toUin);
                         if (null != toFriend)
                         {
                             sb.AppendLine(toFriend.ShowName);
                         }
                         else
                         {
                             sb.AppendLine(toUin);
                         }
                     }
                     message.Receiver = sb.ToString().Trim();

                     msgtype = DynamicConvert.ToSafeString(massendInfo.msgtype);
                     switch (msgtype)
                     {
                         case "3":
                             message.Type = EnumColumnType.Image;
                             message.MessageType = "图片";

                             if (FileHelper.IsValidDictory(MediaFileRootPath))
                             {
                                 var img = Path.Combine(MediaFileRootPath, MD5AccountPath, "image", message.Content);
                                 if (FileHelper.IsValid(img))
                                 {
                                     message.Content = img;
                                 }
                             }
                             break;
                         default:
                             message.Type = EnumColumnType.String;
                             message.MessageType = "文本";
                             break;
                     }

                     massendNode.Items.Add(message);
                 }
             });
        }

        /// <summary>
        /// 构建IndexMsg数据库其他删除消息
        /// </summary>
        /// <param name="rootNode"></param>
        private void BuildIndexMsgContextDeleteMsgNode(TreeNode rootNode)
        {
            if (null == IndexMsgContext && null == FTS5IndexMsgContext)
            {
                return;
            }

            var ftsIndexMessageContentNode = new TreeNode
            {
                DataState = EnumDataState.Normal,
                Text = "其他删除消息",
                Items = new DataItems<DelWeChatMessageCore>(DbFilePath),
                Type = typeof(DelWeChatMessageCore)
            };

            var wordMsg = new TreeNode
            {
                DataState = EnumDataState.Normal,
                Text = "文字消息",
                Items = new DataItems<DelWeChatMessageCore>(DbFilePath),
                Type = typeof(DelWeChatMessageCore)
            };
            var numMsg = new TreeNode
            {
                DataState = EnumDataState.Normal,
                Text = "数字消息",
                Items = new DataItems<DelWeChatMessageCore>(DbFilePath),
                Type = typeof(DelWeChatMessageCore)
            };

            ftsIndexMessageContentNode.TreeNodes.Add(wordMsg);
            ftsIndexMessageContentNode.TreeNodes.Add(numMsg);
            rootNode.TreeNodes.Add(ftsIndexMessageContentNode);

            if (null != IndexMsgContext)
            {
                if (IndexMsgContext.ExistTable("FtsIndexMessage_content"))
                {
                    IndexMsgContext.UsingSafeConnection(new SQLiteString("SELECT CAST(c0content as varchar) as c0content FROM FtsIndexMessage_content WHERE XLY_DataType = 1"), (r) =>
                    {
                        DelWeChatMessageCore mc;
                        dynamic data;

                        while (r.Read())
                        {
                            data = r.ToDynamic();
                            string str = DynamicConvert.ToSafeString(data.c0content);
                            if (Regex.IsMatch(str, @"[\u4E00-\u9FA5]") && !str.Any(c => (c >= 0 && c < 32) || c == 127))
                            {
                                mc = new DelWeChatMessageCore();
                                mc.DataState = EnumDataState.Deleted;
                                mc.Content = str;
                                wordMsg.Items.Add(mc);
                            }
                            else if (Regex.IsMatch(str.Trim(), @"^[\d.-]+$"))
                            {
                                mc = new DelWeChatMessageCore();
                                mc.DataState = EnumDataState.Deleted;
                                mc.Content = str;
                                numMsg.Items.Add(mc);
                            }
                        }
                    });
                }
            }

            if (null != FTS5IndexMsgContext)
            {
                if (FTS5IndexMsgContext.ExistTable("FTS5IndexMessage_content"))
                {
                    FTS5IndexMsgContext.UsingSafeConnection(new SQLiteString("SELECT CAST(c0 as varchar) as c0content FROM FTS5IndexMessage_content WHERE XLY_DataType = 1"), (r) =>
                    {
                        DelWeChatMessageCore mc;
                        dynamic data;

                        while (r.Read())
                        {
                            data = r.ToDynamic();
                            string str = DynamicConvert.ToSafeString(data.c0content);
                            if (Regex.IsMatch(str, @"[\u4E00-\u9FA5]") && !str.Any(c => (c >= 0 && c < 32) || c == 127))
                            {
                                mc = new DelWeChatMessageCore();
                                mc.DataState = EnumDataState.Deleted;
                                mc.Content = str;
                                wordMsg.Items.Add(mc);
                            }
                            else if (Regex.IsMatch(str.Trim(), @"^[\d.-]+$"))
                            {
                                mc = new DelWeChatMessageCore();
                                mc.DataState = EnumDataState.Deleted;
                                mc.Content = str;
                                numMsg.Items.Add(mc);
                            }
                        }
                    });
                }
            }
        }

        /// <summary>
        /// 构建 朋友圈
        /// </summary>
        /// <param name="rootNode"></param>
        private void BuildSnsNode(TreeNode rootNode)
        {
            var tree = new TreeNode
            {
                DataState = EnumDataState.Normal,
                Text = "朋友圈",
                Items = new DataItems<WeChatSns>(DbFilePath),
                Type = typeof(WeChatSns)
            };

            rootNode.TreeNodes.Add(tree);

            //朋友圈数据库  
            var favpath = Path.Combine(FileHelper.GetFilePath(MainDbContext.DbFilePath), "SnsMicroMsg.db");
            var favContext = new SqliteContext(favpath);

            favContext.UsingSafeConnection("SELECT snsId,userName,createTime,content,attrBuf FROM SnsInfo ORDER BY createTime DESC", r =>
             {
                 dynamic item;
                 string weChatId, snsId;

                 while (r.Read())
                 {
                     item = r.ToDynamic();

                     weChatId = DynamicConvert.ToSafeString(item.userName);
                     snsId = DynamicConvert.ToSafeString(item.snsId);

                     var curSender = LsAllFriends.FirstOrDefault(f => f.WeChatId == weChatId);
                     if (null == curSender)
                     {
                         curSender = new WeChatFriendShow() { WeChatId = weChatId };
                     }

                     GetSmsInfo(item, curSender, tree);
                     GetSnsComments(item, tree);
                 }
             });
        }

        /// <summary>
        /// 构建我的收藏
        /// </summary>
        /// <param name="rootNode"></param>
        private void BuildFavoriteNode(TreeNode rootNode)
        {
            var favNode = new TreeNode
            {
                DataState = EnumDataState.Normal,
                Text = "我的收藏",
                Items = new DataItems<WeChatFavorite>(DbFilePath),
                Type = typeof(WeChatFavorite)
            };

            rootNode.TreeNodes.Add(favNode);

            //我的收藏数据库  需要解密
            var _favrtable = "FavItemInfo,FavSearchInfo";
            var favpath = Path.Combine(FileHelper.GetFilePath(MainDbContext.DbFilePath), "enFavorite.db");
            var dFavpath = WeChatDataParseHelper.DecryptAndroidWeChatSqlite(favpath);
            var favdbpath = SqliteRecoveryHelper.DataRecovery(dFavpath, "", _favrtable);
            var favContext = new SqliteContext(favdbpath);

            var filePath = Path.Combine(MediaFileRootPath, MD5AccountPath, "favorite");
            if (FileHelper.IsValidDictory(filePath))
            {
                ListFavoriteFiles = new DirectoryInfo(filePath).GetFiles("*.*", SearchOption.AllDirectories).ToList();
            }
            else
            {
                ListFavoriteFiles = null;
            }

            favContext.UsingSafeConnection("SELECT A.localId,A.type,B.xml,A.time,B.updateTime FROM FavSearchInfo A,FavItemInfo B WHERE A.localId = B.localId AND A.XLY_DataType = 2 ORDER BY A.time DESC", r =>
             {
                 dynamic dydata;
                 WeChatFavorite item;

                 while (r.Read())
                 {
                     dydata = r.Read();

                     item = GetFavoriteItem(dydata);
                     if (null != item)
                     {
                         favNode.Items.Add(item);
                     }
                 }

             });

            favContext.Dispose();
            favContext = null;

            ListFavoriteFiles?.Clear();
            ListFavoriteFiles = null;
        }

        /// <summary>
        /// 构建小程序
        /// </summary>
        /// <param name="rootNode"></param>
        private void BuildAppBrandNode(TreeNode rootNode)
        {
            var appNode = new TreeNode
            {
                DataState = EnumDataState.Normal,
                Text = "微信小程序",
                Items = new DataItems<WeChatAppBrand>(DbFilePath),
                Type = typeof(WeChatAppBrand)
            };

            rootNode.TreeNodes.Add(appNode);

            //数据库  需要解密
            var favpath = Path.Combine(FileHelper.GetFilePath(MainDbContext.DbFilePath), "AppBrandComm.db");
            var dFavpath = WeChatDataParseHelper.DecryptAndroidWeChatSqlite(favpath);
            var appBrandContext = new SqliteContext(dFavpath);

            appBrandContext.UsingSafeConnection("SELECT r.appId,i.brandId,i.appName,i.signature,r.pkgPath,r.downloadURL,i.appIcon FROM AppBrandWxaPkgManifestRecord r,AppBrandWxaAppInfo i WHERE r.appId = i.appId", r =>
             {
                 dynamic appBrand;
                 WeChatAppBrand item;

                 while (r.Read())
                 {
                     appBrand = r.ToDynamic();

                     item = new WeChatAppBrand();
                     item.AppIcon = DynamicConvert.ToSafeString(appBrand.appIcon);
                     item.AppId = DynamicConvert.ToSafeString(appBrand.appId);
                     item.BrandId = DynamicConvert.ToSafeString(appBrand.brandId);
                     item.AppName = DynamicConvert.ToSafeString(appBrand.appName);
                     item.Signature = DynamicConvert.ToSafeString(appBrand.signature);
                     item.PkgPath = DynamicConvert.ToSafeString(appBrand.pkgPath);
                     item.DownloadURL = DynamicConvert.ToSafeString(appBrand.downloadURL);

                     appNode.Items.Add(item);

                     var dataList = appBrandContext.Find(string.Format("SELECT key,data,dataType,size FROM AppBrandKVData WHERE key like '{0}_%' AND dataType NOTNULL", item.AppId));
                     var dataNode = new TreeNode
                     {
                         DataState = EnumDataState.Normal,
                         Text = item.AppName,
                         Type = typeof(WeChatAppBrandKVData),
                         Items = new DataItems<WeChatAppBrand>(DbFilePath)
                     };

                     foreach (var data in dataList)
                     {
                         WeChatAppBrandKVData kv = new WeChatAppBrandKVData();
                         kv.Key = DynamicConvert.ToSafeString(data.key);
                         kv.Key = kv.Key.Substring(kv.Key.IndexOf('_') + 2);
                         kv.DataType = DynamicConvert.ToSafeString(data.dataType);
                         kv.DataSize = DynamicConvert.ToSafeString(data.size);
                         kv.Data = DynamicConvert.ToSafeString(data.data);

                         dataNode.Items.Add(kv);
                     }

                     if (dataList.IsValid())
                     {
                         appNode.TreeNodes.Add(dataNode);
                     }
                 }
             });
        }

        /// <summary>
        /// 构建银行卡列表
        /// </summary>
        /// <param name="rootNode"></param>
        private void BuildMyWalletNode(TreeNode rootNode)
        {
            var tree = new TreeNode
            {
                DataState = EnumDataState.Normal,
                Text = "微信支付",
                Items = new DataItems<WeChatBackCard>(DbFilePath),
                Type = typeof(WeChatBackCard)
            };

            rootNode.TreeNodes.Add(tree);

            var cardItems = MainDbContext.FindByName("WalletBankcard");
            foreach (var card in cardItems)
            {
                var cardInfo = new WeChatBackCard();
                cardInfo.BankName = card.bankName;
                cardInfo.BankType = card.bankcardTypeName;
                cardInfo.Phone = card.mobile;
                cardInfo.Number = card.bankcardTail;
                tree.Items.Add(cardInfo);
            }
        }

        /// <summary>
        /// 构建设备列表
        /// </summary>
        /// <param name="rootNode"></param>
        private void BuildChatSafeDevicesNode(TreeNode rootNode)
        {
            var tree = new TreeNode
            {
                DataState = EnumDataState.Normal,
                Text = "常用设备",
                Items = new DataItems<WeChatBackCard>(DbFilePath),
                Type = typeof(WeChatBackCard)
            };

            rootNode.TreeNodes.Add(tree);

            var allDevice = MainDbContext.FindByName("SafeDeviceInfo");
            foreach (var dev in allDevice)
            {
                try
                {
                    var device = new WeChatSafeDevice();
                    device.DataState = DynamicConvert.ToEnumByValue<EnumDataState>(dev.XLY_DataType, EnumDataState.Normal);
                    device.DeviceName = dev.name;
                    device.Guid = dev.uid;
                    device.SystemType = dev.devicetype;
                    device.LoginTime = DynamicConvert.ToSafeDateTime(dev.createtime);

                    rootNode.Items.Add(device);
                }
                catch 
                {
                }
            }

        }

        #endregion

        #region 辅助方法

        /// <summary>
        /// 获取微信数据库文件列表
        /// 一个微信数据库文件保存了1个微信帐号的数据，文件名为EnMicroMsg.db，所在文件夹名称为微信帐号的MD5 小写
        /// </summary>
        /// <returns></returns>
        private IEnumerable<FileInfo> GetAllWeChatAccountFileInfos()
        {
            var dbPath = Path.Combine(DataFileRootPath, "MicroMsg");

            if (Directory.Exists(dbPath))
            {
                var di = new DirectoryInfo(dbPath);

                return di.GetFiles("EnMicroMsg.db", SearchOption.AllDirectories);
            }
            else
            {
                return new List<FileInfo>();
            }
        }

        /// <summary>
        /// 数据库文件处理
        /// </summary>
        /// <param name="weChatFile">EnMicroMsg.db文件</param>
        private void GetSqliteContext(FileInfo weChatFile)
        {
            //EnMicroMsg.db数据库解密
            var bakDbFile = string.Empty;
            var dbPath = WeChatDataParseHelper.DecryptAndroidWeChatSqliteGetDelete(weChatFile.FullName, ref bakDbFile);

            //EnMicroMsg.db数据库恢复
            Dictionary<string, string> dicIndex = new Dictionary<string, string>
            {
                { "message", "talker" },
                { "ImgInfo2", "msgSvrId" }
            };
            var rdbpath = SqliteRecoveryHelper.DataRecovery(dbPath, @"chalib\Android_WeChat\wechat.charactor", _rEnMicroMsgTables,
                            @"chalib\Android_WeChat\weixinmessage.db", @"chalib\Android_WeChat\weixinmessage.charactor", false, dicIndex);

            MainDbContext = new SqliteContext(rdbpath);
            if (FileHelper.IsValid(bakDbFile))
            {
                WxbakMsgdataContext = new SqliteContext(bakDbFile);
            }
            else
            {
                WxbakMsgdataContext = null;
            }
            ListWxbakMsgWxid = new List<string>();

            var indexMicroMsgDbPath = weChatFile.FullName.TrimEnd("EnMicroMsg.db") + "IndexMicroMsg.db";
            if (FileHelper.IsValid(indexMicroMsgDbPath))
            {//备注：该数据库必须直接恢复原始位置的原始数据库，如果位置发生变动，底层接口将不会解密FtsIndexMessage_content表的c0content加密字段
                var indexMsgDB = SqliteRecoveryHelper.DataRecovery(indexMicroMsgDbPath,
                    @"chalib\Android_WeChat\IndexMicroMsg.db.charactor", "FtsMetaMessage,FtsIndexMessage_content", "", "");
                IndexMsgContext = new SqliteContext(indexMsgDB);
            }
            else
            {
                IndexMsgContext = null;
            }

            var fts5IndexMicroMsgDbPath = weChatFile.FullName.TrimEnd("EnMicroMsg.db") + "FTS5IndexMicroMsg.db";
            if (FileHelper.IsValid(fts5IndexMicroMsgDbPath))
            {//备注：该数据库必须直接恢复原始位置的原始数据库，如果位置发生变动，底层接口将不会解密FtsIndexMessage_content表的c0content加密字段
                var indexMsgDB = SqliteRecoveryHelper.DataRecovery(fts5IndexMicroMsgDbPath,
                    @"chalib\Android_WeChat\FTS5IndexMicroMsg.db.charactor", "FTS5MetaMessage,FTS5IndexMessage_content", "", "");
                FTS5IndexMsgContext = new SqliteContext(indexMsgDB);
            }
            else
            {
                FTS5IndexMsgContext = null;
            }
        }

        /// <summary>
        /// 从IndexMicroMsg.db和FTS5IndexMicroMsg.db获取删除消息
        /// </summary>
        private void GetDeleteMsgFromIndexDb()
        {
            DeleteMsgs = new Dictionary<string, List<dynamic>>();

            if (null != IndexMsgContext)
            {
                var sql = string.Format(@"ATTACH DATABASE '{0}' AS MsgDB;
                                           SELECT
                                           	msg.aux_index,
                                           	msg.timestamp,
                                           	CAST (data.c0content AS varchar) AS c0content
                                           FROM
                                           	FtsMetaMessage msg,
                                           	FtsIndexMessage_content data
                                           WHERE
                                           	msg.XLY_DataType = 2
                                           AND msg.entity_id NOT IN (SELECT msgid FROM MsgDB.message WHERE msgid NOTNULL)
                                           AND msg.docid = data.docid", MainDbContext.DbFilePath);

                IndexMsgContext.UsingSafeConnection(sql, r =>
                 {
                     dynamic msg;
                     string id;
                     while (r.Read())
                     {
                         msg = r.ToDynamic();

                         id = DynamicConvert.ToSafeString(msg.aux_index);
                         if (DeleteMsgs.Keys.Contains(id))
                         {
                             DeleteMsgs[id].Add(msg);
                         }
                         else
                         {
                             List<dynamic> list = new List<dynamic>();
                             list.Add(msg);
                             DeleteMsgs.Add(id, list);
                         }
                     }
                 });
            }

            if (null != FTS5IndexMsgContext)
            {
                var sql = string.Format(@"ATTACH DATABASE '{0}' AS MsgDB;
                                          SELECT
                                          	msg.aux_index,
                                          	msg.timestamp,
                                          	msg.talker,
                                          	CAST (data.c0 AS varchar) AS c0content
                                          FROM
                                          	FTS5MetaMessage msg,
                                          	FTS5IndexMessage_content data
                                          WHERE
                                          	msg.XLY_DataType = 2
                                          AND msg.entity_id NOT IN (SELECT msgid FROM MsgDB.message WHERE msgid NOTNULL)
                                          AND msg.docid = data.id", MainDbContext.DbFilePath);

                FTS5IndexMsgContext.UsingSafeConnection(sql, r =>
                {
                    dynamic msg;
                    string id;
                    while (r.Read())
                    {
                        msg = r.ToDynamic();

                        id = DynamicConvert.ToSafeString(msg.aux_index);
                        if (DeleteMsgs.Keys.Contains(id))
                        {
                            DeleteMsgs[id].Add(msg);
                        }
                        else
                        {
                            List<dynamic> list = new List<dynamic>();
                            list.Add(msg);
                            DeleteMsgs.Add(id, list);
                        }
                    }
                });
            }

        }

        /// <summary>
        /// 获取微信帐号信息
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        private WeChatLoginShow GetAccountUser()
        {
            var user = MainDbContext.FindByName("userinfo");

            var userInfo = new WeChatLoginShow();
            //用户更改过的微信号保存在42，系统自动生成的微信号保存在2
            userInfo.WeChatId = GetLoginUserInfo(2, user);
            userInfo.WeChatAccout = GetLoginUserInfo(42, user);
            userInfo.Nick = GetLoginUserInfo(4, user);
            userInfo.BindingEmail = GetLoginUserInfo(5, user);
            userInfo.BindingPhone = GetLoginUserInfo(6, user);
            userInfo.BindingQQ = GetLoginUserInfo(9, user);

            try
            {//修复10位QQ越界的问题
                if (userInfo.BindingQQ.StartsWith("-"))
                {
                    var res = int.Parse(userInfo.BindingQQ);
                    userInfo.BindingQQ = ((UInt32)res).ToString();
                }
            }
            catch { }

            userInfo.BindingWeiBo = GetLoginUserInfo(12307, user);
            userInfo.WeiBoNick = GetLoginUserInfo(43, user);
            userInfo.Signature = GetLoginUserInfo(12291, user);
            userInfo.Address = string.Format("{0}-{1}", GetLoginUserInfo(12293, user), GetLoginUserInfo(12292, user));
            userInfo.HeadPng = GetHeadImgPath(userInfo.WeChatId);
            userInfo.DataState = EnumDataState.Normal;

            #region  从rcontact表获取其他信息

            try
            {
                var dy = MainDbContext.Find(string.Format("SELECT lvbuff FROM rcontact WHERE username = '{0}'", userInfo.WeChatId)).FirstOrDefault();
                if (null != dy)
                {
                    byte[] lvbuff = dy.lvbuff as byte[];
                    if (lvbuff[0] == 123 && lvbuff[lvbuff.Length - 1] == 125)
                    {
                        var sex = lvbuff[8];
                        if (sex == 1)
                        {//男
                            userInfo.Gender = EnumSex.Male;
                        }
                        else if (sex == 2)
                        {//女
                            userInfo.Gender = EnumSex.Female;
                        }

                        if (userInfo.Signature.IsInvalid())
                        { //个性签名
                            var length = lvbuff[48];
                            userInfo.Signature = Encoding.UTF8.GetString(lvbuff, 49, length);
                        }
                    }
                }
            }
            catch { }

            #endregion

            return userInfo;
        }

        private string GetLoginUserInfo(dynamic id, IEnumerable<dynamic> userinfo)
        {
            try
            {
                var d = userinfo.FirstOrDefault(o => o.id == id);

                return d == null ? string.Empty : d.value;
            }
            catch
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// 获取微信帐号头像
        /// </summary>
        /// <param name="wechatid"></param>
        /// <returns></returns>
        private string GetHeadImgPath(string wechatid)
        {
            if (wechatid.IsInvalid() || MediaFileRootPath.IsInvalid() || !Directory.Exists(MediaFileRootPath))
            {
                return string.Empty;
            }

            var md5 = CryptographyHelper.MD5FromString(wechatid);
            var path = Path.Combine(MediaFileRootPath, MD5AccountPath, "avatar", md5.Substring(0, 2), md5.Substring(2, 2), string.Format("user_{0}.png", md5));

            if (File.Exists(path))
            {
                return path;
            }

            return string.Empty;
        }

        /// <summary>
        /// 创建好友对象信息
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private WeChatFriendShow CreateFriendShow(dynamic data)
        {
            if (data == null)
            {
                return null;
            }

            var friend = new WeChatFriendShow();

            friend.DataState = DynamicConvert.ToEnumByValue<EnumDataState>(data.XLY_DataType, EnumDataState.Normal);
            friend.WeChatId = DynamicConvert.ToSafeString(data.alias);
            if (friend.WeChatId.IsValid())
            {
                friend.WeChatAccout = DynamicConvert.ToSafeString(data.username);
            }
            else
            {
                friend.WeChatId = DynamicConvert.ToSafeString(data.username);
            }
            friend.Nick = DynamicConvert.ToSafeString(data.nickname);
            friend.Remark = DynamicConvert.ToSafeString(data.conRemark);
            friend.FriendType = GetFriendType(DynamicConvert.ToSafeString(data.type), friend.WeChatId);
            friend.HeadPng = GetHeadImgPath(friend.WeChatId);

            DealLvbuff(data, friend);

            return friend;
        }

        /// <summary>
        /// 获取好友类型
        /// </summary>
        /// <param name="typeCode"></param>
        /// <param name="weChatId"></param>
        /// <returns></returns>
        private WeChatFriendTypeEnum GetFriendType(string typeCode, string weChatId)
        {
            if (weChatId.StartsWith("gh_") || weChatId == "weixin")
            {
                return WeChatFriendTypeEnum.Subscription;
            }

            switch (typeCode)
            {
                case "0":
                case "4":
                    return WeChatFriendTypeEnum.ChatRoom;
                default:
                    return WeChatFriendTypeEnum.Normal;
            }
        }

        /// <summary>
        /// 创建群聊对象信息
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private WeChatGroupShow CreateWeChatGroupShow(dynamic data)
        {
            var chatroom = new WeChatGroupShow();
            chatroom.DataState = DynamicConvert.ToEnumByValue<EnumDataState>(data.XLY_DataType, EnumDataState.Normal);
            chatroom.WeChatId = DynamicConvert.ToSafeString(data.username);
            chatroom.GroupName = DynamicConvert.ToSafeString(data.nickname);
            chatroom.RemarkName = DynamicConvert.ToSafeString(data.chatroomnick);
            chatroom.LastMessageTime = DynamicConvert.ToSafeDateTime(data.modifytime);
            chatroom.Notice = DynamicConvert.ToSafeDateTime(data.chatroomnotice);

            string roomowner = DynamicConvert.ToSafeDateTime(data.roomowner);
            var names = DynamicConvert.ToSafeString(data.displayname).Split('、');
            var ids = DynamicConvert.ToSafeString(data.memberlist).Split(';');
            var members = new List<KeyValueItem>();
            if (names.Length == ids.Length)
            {
                for (var i = 0; i < names.Length; i++)
                {
                    var res = LsAllFriends.FirstOrDefault(f => f.WeChatId == ids[i]);
                    if (null != res)
                    {
                        var item = new KeyValueItem { Key = ids[i], Value = res.Nick };
                        members.Add(item);
                    }
                    else
                    {
                        var item = new KeyValueItem { Key = ids[i], Value = names[i] };
                        members.Add(item);
                    }
                }
            }
            else
            {
                for (var i = 0; i < ids.Length; i++)
                {
                    var res = LsAllFriends.FirstOrDefault(f => f.WeChatId == ids[i]);
                    if (null != res)
                    {
                        var item = new KeyValueItem { Key = ids[i], Value = res.Nick };
                        members.Add(item);
                    }
                    else
                    {
                        var item = new KeyValueItem { Key = ids[i], Value = "" };
                        members.Add(item);
                    }
                }
            }

            chatroom.MemberNum = members.Count;
            chatroom.Member = members.Aggregate(string.Empty, (current, m) => current + string.Format("{0}({1}){2}", m.Value, m.Key, Environment.NewLine));

            var owner = members.FirstOrDefault(k => k.Key == roomowner);
            if (null != owner && owner.Key != owner.Value)
            {
                chatroom.GroupOwnerUser = string.Format("{0}({1})", owner.Value, owner.Key);
            }
            else
            {
                var ff = LsAllFriends.FirstOrDefault(f => f.WeChatId == roomowner);
                if (null != ff)
                {
                    chatroom.GroupOwnerUser = ff.ShowName;
                }
                else
                {
                    chatroom.GroupOwnerUser = roomowner;
                }
            }

            return chatroom;
        }

        /// <summary>
        /// 处理好友扩展信息，包括所在城市、性别、个性签名和描述
        /// </summary>
        /// <param name="data"></param>
        /// <param name="friend"></param>
        private void DealLvbuff(dynamic data, WeChatFriendShow friend)
        {
            //TODO：获取描述信息
            try
            {
                byte[] lvbuff = data.lvbuff as byte[];

                if (lvbuff[0] != 123 || lvbuff[lvbuff.Length - 1] != 125)
                {//第一位固定为123 最后一位固定为125
                    return;
                }

                var sex = lvbuff[8];
                if (sex == 1)
                {//男
                    friend.Gender = EnumSex.Male;
                }
                else if (sex == 2)
                {//女
                    friend.Gender = EnumSex.Female;
                }

                //个性签名
                var length = lvbuff[48];
                friend.Signature = Encoding.UTF8.GetString(lvbuff, 49, length);

                //省
                int index = 48 + length;
                index++;
                if (lvbuff[index] != 0)
                {
                    return;
                }
                index++;
                length = lvbuff[index];
                index++;
                friend.Address = Encoding.UTF8.GetString(lvbuff, index, length);
                index += length;

                //市
                if (lvbuff[index] != 0)
                {
                    return;
                }
                index++;
                length = lvbuff[index];
                index++;
                friend.Address = string.Format("{0}-{1}", friend.Address, Encoding.UTF8.GetString(lvbuff, index, length));
            }
            catch { }
        }

        /// <summary>
        /// 解析消息
        /// </summary>
        /// <param name="mesData"></param>
        /// <param name="msg"></param>
        private void GetMessage(dynamic mesData, ref MessageCore msg, bool isGroup = false)
        {
            //获取消息类型
            GetMessageType(DynamicConvert.ToSafeString(mesData.type), ref msg);

            //获取消息内容
            string file = DynamicConvert.ToSafeString(mesData.imgPath);
            string content = DynamicConvert.ToSafeString(mesData.content);
            string msgSvrId = DynamicConvert.ToSafeString(mesData.msgSvrId);

            msg.Content = GetMessageContent(content, file, msgSvrId, msg.Type, isGroup);
        }

        /// <summary>
        /// 解析消息类型
        /// </summary>
        /// <param name="type"></param>
        /// <param name="msg"></param>
        private void GetMessageType(string type, ref MessageCore msg)
        {
            switch (type)
            {
                case "1":
                    msg.Type = EnumColumnType.String;
                    msg.MessageType = "文本";
                    break;
                case "3":
                    msg.Type = EnumColumnType.Image;
                    msg.MessageType = "图片";
                    break;
                case "34":
                    msg.Type = EnumColumnType.Audio;
                    msg.MessageType = "语音";
                    break;
                case "48":
                    msg.Type = EnumColumnType.Location;
                    msg.MessageType = "位置信息";
                    break;
                case "62":
                case "43":
                    msg.Type = EnumColumnType.Video;
                    msg.MessageType = "视频";
                    break;
                case "42":
                    msg.Type = EnumColumnType.Card;
                    msg.MessageType = "名片";
                    break;
                case "50":
                    msg.Type = EnumColumnType.VideoChat;
                    msg.MessageType = "视频聊天";
                    break;
                case "10000":
                    msg.Type = EnumColumnType.System;
                    msg.MessageType = "系统消息";
                    break;
                case "49":
                    msg.Type = EnumColumnType.HTML;
                    msg.MessageType = "文本";
                    break;
                case "47":
                    msg.Type = EnumColumnType.Emoji;
                    msg.MessageType = "动画表情";
                    break;
                case "285212721":
                    msg.Type = EnumColumnType.GongZhongHao;
                    msg.MessageType = "公众号消息";
                    break;
                case "318767153":
                    msg.Type = EnumColumnType.WeChatZhifu;
                    msg.MessageType = "微信支付";
                    break;
                case "436207665":
                    msg.Type = EnumColumnType.WeChatRedPack;
                    msg.MessageType = "微信红包";
                    break;
                case "419430449":
                    msg.Type = EnumColumnType.WeChatTransfer;
                    msg.MessageType = "微信转账";
                    break;
                default:
                    msg.Type = EnumColumnType.String;
                    msg.MessageType = "文本";
                    break;
            }
        }

        /// <summary>
        /// 解析消息内容
        /// </summary>
        private string GetMessageContent(string content, string file, string msgSvrId, EnumColumnType type, bool isGroup = false)
        {
            if (isGroup)
            {
                if (content.Contains(":\n"))
                {//群聊消息   xxxx:\nssssss     xxxx为发言人微信号  ssssss为聊天内容 如果是自己发送的  则直接就是聊天内容
                    content = content.Substring(content.IndexOf(":\n") + 2);
                }
                else if (content.Contains(":"))
                {
                    content = content.Substring(content.IndexOf(":") + 1);
                }
            }

            var mediaFile = string.Empty;

            switch (type)
            {
                case EnumColumnType.String:
                    return content;
                case EnumColumnType.Audio:
                    #region 语音

                    var md5Str = CryptographyHelper.MD5FromString(file);
                    var findNameAmr = string.Format("msg_{0}.amr", file);
                    mediaFile = Path.Combine(MediaFileRootPath, MD5AccountPath, "voice2", md5Str.Substring(0, 2), md5Str.Substring(2, 2), findNameAmr);
                    if (FileHelper.IsValid(mediaFile))
                    {
                        return AudioDecodeHelper.Decode(mediaFile);
                    }
                    else
                    {
                        return findNameAmr;
                    }

                #endregion
                case EnumColumnType.Image:
                    #region 图片

                    if (file.StartsWith("THUMBNAIL_DIRPATH://th_"))
                    {
                        var result = MainDbContext.Find(string.Format("select bigImgPath from ImgInfo2 where msgSvrId = '{0}'", msgSvrId));
                        if (result.IsValid())
                        {
                            string bigImgPath = DynamicConvert.ToSafeString(result.FirstOrDefault().bigImgPath);
                            if (!bigImgPath.StartsWith("SERVERID://"))
                            {
                                file = bigImgPath;
                            }
                        }
                    }
                    var imgmd5 = file.TrimStart("THUMBNAIL_DIRPATH://").TrimStart("th_");
                    var filePathJpg = Path.Combine(MediaFileRootPath, MD5AccountPath, "image2", imgmd5.Substring(0, 2), imgmd5.Substring(2, 2), imgmd5);
                    if (File.Exists(filePathJpg))
                    {
                        return filePathJpg;
                    }

                    filePathJpg = Path.Combine(MediaFileRootPath, MD5AccountPath, "image2", imgmd5.Substring(0, 2), imgmd5.Substring(2, 2), file.TrimStart("THUMBNAIL_DIRPATH://"));
                    if (File.Exists(filePathJpg))
                    {
                        return filePathJpg;
                    }

                    filePathJpg = Path.Combine(MediaFileRootPath, MD5AccountPath, "image2", imgmd5.Substring(0, 2), imgmd5.Substring(2, 2), file);
                    if (File.Exists(filePathJpg))
                    {
                        return filePathJpg;
                    }

                    return file;

                #endregion
                case EnumColumnType.Video:
                    #region 视频

                    mediaFile = Path.Combine(MediaFileRootPath, MD5AccountPath, "video", file);
                    if (FileHelper.IsValid(mediaFile))
                    {
                        return mediaFile;
                    }
                    else
                    {
                        return file;
                    }

                #endregion
                case EnumColumnType.Emoji:
                    #region Emoji

                    mediaFile = Path.Combine(MediaFileRootPath, MD5AccountPath, "emoji", file);
                    if (FileHelper.IsValid(mediaFile))
                    {
                        return mediaFile;
                    }

                    var emojiInfo = MainDbContext.Find(new SQLiteString(string.Format("SELECT groupId FROM EmojiInfo WHERE md5 = '{0}' AND XLY_DataType = 2", file))).FirstOrDefault();
                    if (null != emojiInfo)
                    {
                        string group = DynamicConvert.ToSafeString(emojiInfo.groupId);
                        if (group.IsValid())
                        {
                            mediaFile = Path.Combine(MediaFileRootPath, MD5AccountPath, "emoji", group, file);
                            if (FileHelper.IsValid(mediaFile))
                            {
                                return mediaFile;
                            }
                        }
                    }
                    return file;

                #endregion
                case EnumColumnType.Location:
                    return MessageToLocation(content);
                case EnumColumnType.HTML:
                    return MessageToHTML(content);
                case EnumColumnType.WeChatRedPack:
                    return MessageToRedPack(content);
                case EnumColumnType.WeChatTransfer:
                    return MessageToTransfer(content);
                case EnumColumnType.WeChatZhifu:
                    return MessageToWeChatZhifu(content);
                case EnumColumnType.GongZhongHao:
                    return MessageToWeChatGongZhongHao(content);
                case EnumColumnType.Card:
                    return MessageToWeChatCard(content);
                default:
                    return content;
            }
        }

        /// <summary>
        /// 解析定位信息
        /// </summary>
        /// <param name="strMessage"></param>
        /// <returns></returns>
        private string MessageToLocation(string strMessage)
        {
            string strX = "";
            string strY = "";
            string strLabel = "";
            string strPoiname = "";
            try
            {
                // 解析 XML 数据
                if (strMessage.Contains("<msg>"))
                    strMessage = strMessage.Substring(strMessage.IndexOf("<msg>"));
                MemoryStream msXmlLoc = new MemoryStream(Encoding.UTF8.GetBytes(strMessage));
                XmlTextReader xmlLoc = new XmlTextReader(msXmlLoc);
                while (xmlLoc.Read())
                {
                    if (xmlLoc.NodeType == XmlNodeType.Element)
                    {
                        if (xmlLoc.Name == "location")
                        {
                            strX = xmlLoc.GetAttribute("x");
                            strY = xmlLoc.GetAttribute("y");
                            strLabel = xmlLoc.GetAttribute("label");
                            strPoiname = xmlLoc.GetAttribute("poiname");
                            if (strLabel.IsInvalid())
                                strLabel = " ";
                            if (strPoiname.IsInvalid())
                                strPoiname = " ";
                        }
                    }
                }
                xmlLoc.Close();
                msXmlLoc.Close();

                return string.Format("位置信息--标题:{0},地址:{1},坐标:{2},{3}", strPoiname, strLabel, strX, strY);
            }
            catch { }

            return strMessage;
        }

        /// <summary>
        /// 解析HTML消息
        /// </summary>
        /// <param name="strMessage"></param>
        /// <returns></returns>
        private string MessageToHTML(string strMessage)
        {
            string strType = "";
            string strTitle = "";
            string strDesc = "";
            string strUrl = "";
            try
            {
                // 解析 XML 数据
                if (strMessage.Contains("<msg>"))
                    strMessage = strMessage.Substring(strMessage.IndexOf("<msg>"));
                MemoryStream msXmlLink = new MemoryStream(Encoding.UTF8.GetBytes(strMessage));
                XmlReader xmlLink = XmlReader.Create(msXmlLink);
                while (xmlLink.Read())
                {
                    if (xmlLink.NodeType == XmlNodeType.Element)
                    {
                        if (xmlLink.Name == "type")
                            strType = xmlLink.ReadString();
                        else if (xmlLink.Name == "title")
                            strTitle = xmlLink.ReadString();
                        else if (xmlLink.Name == "des")
                            strDesc = xmlLink.ReadString();
                        else if (xmlLink.Name == "url")
                            strUrl = xmlLink.ReadString();
                    }
                }
                xmlLink.Close();
                msXmlLink.Close();

                if (strType == "17")
                {
                    //strMessage = "<a style=\"color:#888\"> 实时位置信息... </a>";
                }
                else
                {
                    if (strUrl.Trim().Length == 0)
                        strMessage = string.Format("分享链接--标题:{0}, 描述:{1}", strTitle, strDesc);
                    else
                        strMessage = string.Format("分享链接--标题:{0}, 描述:{1}, 链接:{2}", strTitle, strDesc, strUrl);
                }
            }
            catch { }

            return strMessage;
        }

        /// <summary>
        /// 微信转账
        /// </summary>
        /// <returns></returns>
        private string MessageToTransfer(string strMessage)
        {
            string strType = "";
            string strTitle = "";
            string strDesc = "";
            string strUrl = "";
            try
            {
                // 解析 XML 数据
                if (strMessage.Contains("<msg>"))
                    strMessage = strMessage.Substring(strMessage.IndexOf("<msg>"));
                MemoryStream msXmlLink = new MemoryStream(Encoding.UTF8.GetBytes(strMessage));
                XmlReader xmlLink = XmlReader.Create(msXmlLink);
                while (xmlLink.Read())
                {
                    if (xmlLink.NodeType == XmlNodeType.Element)
                    {
                        if (xmlLink.Name == "type")
                            strType = xmlLink.ReadString();
                        else if (xmlLink.Name == "title")
                            strTitle = xmlLink.ReadString();
                        else if (xmlLink.Name == "des")
                            strDesc = xmlLink.ReadString();
                        else if (xmlLink.Name == "url")
                            strUrl = xmlLink.ReadString();
                    }
                }
                xmlLink.Close();
                msXmlLink.Close();
            }
            catch
            {
                if (strTitle.IsInvalid() && strDesc.IsInvalid() && strUrl.IsInvalid())
                {
                    return strMessage;
                }
            }

            if (strType == "2000")
            {//转账记录
                if (strMessage.Contains("<paysubtype>1</paysubtype>"))
                {//转账
                    strMessage = string.Format("{0}-{1}", strTitle, strDesc);
                }
                else if (strMessage.Contains("<paysubtype>3</paysubtype>"))
                {//接收转账
                    strMessage = string.Format("{0}-{1}", strTitle, "我已经确认转账！");
                }
            }

            return strMessage;
        }

        /// <summary>
        /// 微信红包
        /// </summary>
        /// <returns></returns>
        private string MessageToRedPack(string strMessage)
        {
            //string strType = "";
            string strTitle = "";
            string strDesc = "";
            string strUrl = "";
            string strSendertitle = "";
            try
            {
                // 解析 XML 数据
                if (strMessage.Contains("<msg>"))
                    strMessage = strMessage.Substring(strMessage.IndexOf("<msg>"));
                MemoryStream msXmlLink = new MemoryStream(Encoding.UTF8.GetBytes(strMessage));
                XmlReader xmlLink = XmlReader.Create(msXmlLink);
                while (xmlLink.Read())
                {
                    if (xmlLink.NodeType == XmlNodeType.Element)
                    {
                        //if (xmlLink.Name == "type")
                        //    strType = xmlLink.ReadString();
                        if (xmlLink.Name == "title")
                            strTitle = xmlLink.ReadString();
                        else if (xmlLink.Name == "des")
                            strDesc = xmlLink.ReadString();
                        //else if (xmlLink.Name == "url")
                        //    strUrl = xmlLink.ReadString();
                        else if (xmlLink.Name == "sendertitle")
                            strSendertitle = xmlLink.ReadString();
                    }
                }
                xmlLink.Close();
                msXmlLink.Close();
            }
            catch
            {
                if (strTitle.IsInvalid() && strDesc.IsInvalid() && strUrl.IsInvalid())
                {
                    return strMessage;
                }
            }

            return string.Format("{0} {1}", strDesc, strSendertitle);
        }

        /// <summary>
        /// 微信支付
        /// </summary>
        /// <returns></returns>
        private string MessageToWeChatZhifu(string strMessage)
        {
            try
            {
                // 解析 XML 数据
                var doc = new XmlDocument();
                doc.LoadXml(strMessage.Trim());
                var dataNode = doc.SelectSingleNode("/msg/appmsg/des");

                if (null != dataNode)
                {
                    return dataNode.InnerText.Trim();
                }
            }
            catch { }

            return strMessage;
        }

        /// <summary>
        /// 公众号
        /// </summary>
        private string MessageToWeChatGongZhongHao(string content)
        {
            try
            {//TODO:完善
                var mcs = Regex.Matches(content, @"[\u2E80-\u9FFF][\u2E80-\u9FFF\d]+");

                if (mcs.Count > 0)
                {
                    StringBuilder sb = new StringBuilder();
                    foreach (Match mc in mcs)
                    {
                        sb.Append(mc.Value);
                    }

                    return sb.ToString();
                }
            }
            catch { }

            return content;
        }

        /// <summary>
        /// 名片
        /// </summary>
        private string MessageToWeChatCard(string strMessage)
        {
            try
            {
                // 解析 XML 数据
                if (strMessage.Contains("<msg"))
                    strMessage = strMessage.Substring(strMessage.IndexOf("<msg"));

                var doc = new XmlDocument();
                doc.LoadXml(strMessage.Trim());
                var dataNode = doc.SelectSingleNode("/msg");
                if (null != dataNode)
                {
                    var username = dataNode.Attributes["username"].Value;
                    var nickname = dataNode.Attributes["nickname"].Value;
                    var alias = dataNode.Attributes["alias"].Value;
                    var province = dataNode.Attributes["province"].Value;
                    var city = dataNode.Attributes["city"].Value;
                    var sign = dataNode.Attributes["sign"].Value;
                    var sex = dataNode.Attributes["sex"].Value;

                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine(string.Format("微信账号:{0}  别名:{1}", username, alias));
                    sb.AppendLine(string.Format("昵称:{0} 性别:{1} 城市:{2}-{3}", nickname, sex == "2" ? "女" : "男", province, city));
                    sb.AppendLine(string.Format("个性签名:{0}", sign));

                    return sb.ToString();
                }
            }
            catch { }

            return strMessage;
        }

        /// <summary>
        /// 解析bak消息记录
        /// </summary>
        /// <param name="msgData"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        private MessageCore GetWxbakMsgdataContextMsg(dynamic msgData, ref MessageCore msg)
        {
            int type, isSend;
            byte[] msgBArr;

            #region 获取消息

            //获取消息
            msg.DataState = EnumDataState.Deleted;
            msg.Date = DynamicConvert.ToSafeDateTime(msgData.utctime);

            msgBArr = msgData.pmsg as byte[];
            if (msgBArr.IsValid())
            {
                msg.Content = Encoding.UTF8.GetString(msgBArr);
            }
            else
            {
                msg.Content = string.Empty;
            }

            type = DynamicConvert.ToSafeInt(msgData.type);
            isSend = DynamicConvert.ToSafeInt(msgData.isSend);
            switch (type)
            {
                case 1:
                    msg.Type = EnumColumnType.String;
                    msg.MessageType = "文本";
                    break;
                case 3:
                    msg.Type = EnumColumnType.Image;
                    msg.MessageType = "图片";
                    break;
                case 34:
                    msg.Type = EnumColumnType.Audio;
                    msg.MessageType = "语音";
                    break;
                case 48:
                    msg.Type = EnumColumnType.Location;
                    msg.MessageType = "定位信息";
                    break;
                case 62:
                case 43:
                    msg.Type = EnumColumnType.Video;
                    msg.MessageType = "视频";
                    break;
                case 42:
                    msg.Type = EnumColumnType.Card;
                    msg.MessageType = "名片";
                    break;
                case 50:
                    msg.Type = EnumColumnType.VideoChat;
                    msg.MessageType = "视频聊天";
                    break;
                case 10000:
                    msg.Type = EnumColumnType.System;
                    msg.MessageType = "系统消息";
                    break;
                case 49:
                    msg.Type = EnumColumnType.HTML;
                    msg.MessageType = "文本";
                    break;
                case 47:
                    msg.Type = EnumColumnType.Emoji;
                    msg.MessageType = "动画表情";
                    break;
                case 285212721:
                    msg.Type = EnumColumnType.GongZhongHao;
                    msg.MessageType = "公众号消息";
                    break;
                case 318767153:
                    msg.Type = EnumColumnType.WeChatZhifu;
                    msg.MessageType = "微信支付";
                    break;
                case 436207665:
                    msg.Type = EnumColumnType.WeChatRedPack;
                    msg.MessageType = "微信红包";
                    break;
                case 419430449:
                    msg.Type = EnumColumnType.WeChatTransfer;
                    msg.MessageType = "微信转账";
                    break;
                default:
                    msg.Type = EnumColumnType.String;
                    msg.MessageType = "文本";
                    break;
            }

            if (isSend == 3 || isSend == 6)
            {//接收
                msg.SendState = EnumSendState.Receive;
                msg.SenderName = DynamicConvert.ToSafeString(msgData.wxid);
            }
            else if (isSend == 2)
            {//发送
                msg.SendState = EnumSendState.Send;
                msg.Receiver = DynamicConvert.ToSafeString(msgData.wxid);
            }
            else
            {//系统消息
                msg.SendState = EnumSendState.Receive;
                msg.SenderName = DynamicConvert.ToSafeString(msgData.wxid);
                msg.Type = EnumColumnType.System;
                msg.MessageType = "系统消息";
            }

            #endregion

            return msg;
        }

        /// <summary>
        /// 解析我的收藏数据
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private WeChatFavorite GetFavoriteItem(dynamic data)
        {
            try
            {
                WeChatFavorite wcf = new WeChatFavorite();

                GetFavoriteItemBaseInfo(data, ref wcf);

                int type = DynamicConvert.ToSafeInt(data.type);
                switch (type)
                {
                    case 1://文字信息
                        GetFavoriteItemText(data, ref wcf);
                        break;
                    case 2://图片
                        GetFavoriteItemImage(data, ref wcf);
                        break;
                    case 3://音频
                        GetFavoriteItemAudio(data, ref wcf);
                        break;
                    case 4://视频
                        GetFavoriteItemVideo(data, ref wcf);
                        break;
                    case 5://网页链接
                        GetFavoriteItemLink(data, ref wcf);
                        break;
                    case 6://位置信息
                        GetFavoriteItemLocation(data, ref wcf);
                        break;
                    case 8://文件
                        GetFavoriteItemFile(data, ref wcf);
                        break;
                    case 14://聊天记录
                        GetFavoriteItemMoreMsg(data, ref wcf);
                        break;
                    case 16://视频
                        GetFavoriteItemVideo(data, ref wcf);
                        break;
                    case 18://自定义
                        GetFavoriteItemUser(data, ref wcf);
                        break;
                    default:
                        wcf.DataType = type.ToString();
                        break;
                }
                if (wcf.Text.IsInvalid())
                {
                    wcf.Text = DynamicConvert.ToSafeString(data.xml);
                }

                wcf.Time = DynamicConvert.ToSafeFromUnixTime(data.time, 1000);
                //wcf.SourceCreateTime = DynamicConvert.ToSafeFromUnixTime(data.sourceCreateTime, 1000);
                wcf.UpdateTime = DynamicConvert.ToSafeFromUnixTime(data.updateTime, 1000);

                return wcf;
            }
            catch 
            {
                return null;
            }
        }

        #region 我的收藏内容解析

        /// <summary>
        /// 解析 我的收藏
        /// </summary>
        private void GetFavoriteItemBaseInfo(dynamic data, ref WeChatFavorite wcf)
        {
            try
            {
                string text = DynamicConvert.ToSafeString(data.xml);

                var doc = new XmlDocument();
                doc.LoadXml(text);
                var taglistNodes = doc.SelectNodes("//taglist/tag");
                var recommendtaglistNodes = doc.SelectNodes("//recommendtaglist/tag");
                var sourceNode = doc.SelectSingleNode("//source");
                var fromusrNode = sourceNode.SelectSingleNode("//source/fromusr");
                //如果是收藏的群组消息，则fromusr是群组ID，realchatname才是发送者ID
                var realchatnameNode = sourceNode.SelectSingleNode("//source/realchatname");
                var tousrNode = sourceNode.SelectSingleNode("//source/tousr");
                //var msgidNode = sourceNode.SelectSingleNode("//source/msgid");

                string source = null == sourceNode ? "" : sourceNode.OuterXml;
                string sourceinfo = null == sourceNode ? "" : sourceNode.OuterXml;
                string fromusr = null == fromusrNode ? "" : fromusrNode.InnerText;
                string realchatname = null == realchatnameNode ? fromusr : realchatnameNode.InnerText;
                string tousr = null == tousrNode ? "" : tousrNode.InnerText;
                //string msgid = null == msgidNode ? "" : msgidNode.InnerText;

                if (taglistNodes.IsValid())
                {
                    foreach (XmlNode tag in taglistNodes)
                    {
                        wcf.Tag += string.Format("{0}\r\n", tag.InnerText);
                    }
                }
                if (recommendtaglistNodes.IsValid())
                {
                    foreach (XmlNode tag in recommendtaglistNodes)
                    {
                        wcf.Tag += string.Format("{0}\r\n", tag.InnerText);
                    }
                }
                wcf.Tag = wcf.Tag.TrimEnd("\r\n");

                if (fromusr.IsValid())
                {
                    var sourceer = LsAllFriends.FirstOrDefault(f => f.WeChatId == fromusr);
                    wcf.Source = sourceer == null ? fromusr : sourceer.ToString();
                }

                if (realchatname.IsValid())
                {
                    var sender = LsAllFriends.FirstOrDefault(f => f.WeChatId == realchatname);
                    wcf.Sender = sender == null ? realchatname : sender.ToString();
                }

                if (tousr.IsValid())
                {
                    var receiver = LsAllFriends.FirstOrDefault(f => f.WeChatId == tousr);
                    wcf.Receiver = receiver == null ? tousr : receiver.ToString();
                }

                //wcf.SourceInfo = sourceinfo;
            }
            catch 
            {
            }
        }

        /// <summary>
        /// 解析 我的收藏 文字信息
        /// </summary>
        private void GetFavoriteItemText(dynamic data, ref WeChatFavorite wcf)
        {
            wcf.DataType = "文字信息";

            var text = DynamicConvert.ToSafeString(data.xml);
            try
            {
                var doc = new XmlDocument();
                doc.LoadXml(text);
                var descNode = doc.SelectSingleNode("//desc");

                wcf.Text = null == descNode ? "" : descNode.InnerText;
            }
            catch
            {
            }
        }

        /// <summary>
        /// 解析 我的收藏  图片
        /// </summary>
        private void GetFavoriteItemImage(dynamic data, ref WeChatFavorite wcf)
        {
            wcf.DataType = "图片";

            string text = DynamicConvert.ToSafeString(data.xml);
            try
            {
                var doc = new XmlDocument();
                doc.LoadXml(text);

                var dataitemNode = doc.SelectSingleNode("//datalist/dataitem");
                var datafmtNode = doc.SelectSingleNode("//datalist/dataitem/datafmt");

                string fileName = dataitemNode.Attributes["dataid"].Value;
                if (null != datafmtNode)
                {
                    fileName = string.Format("{0}.{1}", dataitemNode.Attributes["dataid"].Value, datafmtNode.InnerText);
                }

                if (null != ListFavoriteFiles)
                {
                    var res = ListFavoriteFiles.FirstOrDefault(f => f.Name == fileName);

                    wcf.Text = res != null ? res.FullName : fileName;
                }
                else
                {
                    wcf.Text = fileName;
                }
            }
            catch 
            {
            }
        }

        /// <summary>
        /// 解析 我的收藏 音频
        /// </summary>
        private void GetFavoriteItemAudio(dynamic data, ref WeChatFavorite wcf)
        {
            wcf.DataType = "语音";

            string text = DynamicConvert.ToSafeString(data.xml);
            try
            {
                var doc = new XmlDocument();
                doc.LoadXml(text);

                var dataitemNode = doc.SelectSingleNode("//datalist/dataitem");
                var datafmtNode = doc.SelectSingleNode("//datalist/dataitem/datafmt");

                string fileName = dataitemNode.Attributes["dataid"].Value;
                if (null != datafmtNode)
                {
                    fileName = string.Format("{0}.{1}", dataitemNode.Attributes["dataid"].Value, datafmtNode.InnerText);
                }

                if (null != ListFavoriteFiles)
                {
                    var res = ListFavoriteFiles.FirstOrDefault(f => f.Name == fileName);

                    wcf.Text = res != null ? res.FullName : fileName;
                }
                else
                {
                    wcf.Text = fileName;
                }
            }
            catch 
            {
            }
        }

        /// <summary>
        /// 解析 我的收藏 文件
        /// </summary>
        private void GetFavoriteItemFile(dynamic data, ref WeChatFavorite wcf)
        {
            wcf.DataType = "文件";

            string text = DynamicConvert.ToSafeString(data.xml);
            try
            {
                var doc = new XmlDocument();
                doc.LoadXml(text);

                var dataitemNode = doc.SelectSingleNode("//datalist/dataitem");
                var datafmtNode = doc.SelectSingleNode("//datalist/dataitem/datafmt");

                string fileName = dataitemNode.Attributes["dataid"].Value;
                if (null != datafmtNode)
                {
                    fileName = string.Format("{0}.{1}", dataitemNode.Attributes["dataid"].Value, datafmtNode.InnerText);
                }

                if (null != ListFavoriteFiles)
                {
                    var res = ListFavoriteFiles.FirstOrDefault(f => f.Name == fileName);

                    wcf.Text = res != null ? res.FullName : fileName;
                }
                else
                {
                    wcf.Text = fileName;
                }
            }
            catch 
            {
            }
        }

        /// <summary>
        /// 解析 我的收藏 网页链接
        /// </summary>
        private void GetFavoriteItemLink(dynamic data, ref WeChatFavorite wcf)
        {
            wcf.DataType = "网页链接";

            string text = DynamicConvert.ToSafeString(data.xml);
            try
            {
                var doc = new XmlDocument();
                doc.LoadXml(text);
                var linkNode = doc.SelectSingleNode("//source/link");
                var pagetitleNode = doc.SelectSingleNode("//weburlitem/pagetitle");
                if (null == pagetitleNode)
                {
                    pagetitleNode = doc.SelectSingleNode("//datalist/dataitem/datatitle");
                }

                string link = null == linkNode ? "" : linkNode.InnerText;
                string title = null == pagetitleNode ? "" : pagetitleNode.InnerText;

                wcf.Text = string.Format("{0}\r\n{1}", title, link).TrimStart("\r\n");
            }
            catch 
            {
            }
        }

        /// <summary>
        /// 解析 我的收藏 视频
        /// </summary>
        private void GetFavoriteItemVideo(dynamic data, ref WeChatFavorite wcf)
        {
            wcf.DataType = "视频";

            string text = DynamicConvert.ToSafeString(data.xml);
            try
            {
                var doc = new XmlDocument();
                doc.LoadXml(text);

                var dataitemNode = doc.SelectSingleNode("//datalist/dataitem");
                var datafmtNode = doc.SelectSingleNode("//datalist/dataitem/datafmt");

                string fileName = dataitemNode.Attributes["dataid"].Value;
                if (null != datafmtNode)
                {
                    fileName = string.Format("{0}.{1}", dataitemNode.Attributes["dataid"].Value, datafmtNode.InnerText);
                }

                if (null != ListFavoriteFiles)
                {
                    var res = ListFavoriteFiles.FirstOrDefault(f => f.Name == fileName);

                    wcf.Text = res != null ? res.FullName : fileName;
                }
                else
                {
                    wcf.Text = fileName;
                }
            }
            catch 
            {
            }
        }

        /// <summary>
        /// 解析 我的收藏 位置信息
        /// </summary>
        private void GetFavoriteItemLocation(dynamic data, ref WeChatFavorite wcf)
        {
            wcf.DataType = "位置信息";

            string text = DynamicConvert.ToSafeString(data.xml);
            try
            {
                var doc = new XmlDocument();
                doc.LoadXml(text);
                var labelNode = doc.SelectSingleNode("//locitem/label");
                var poinameNode = doc.SelectSingleNode("//locitem/poiname");
                var latNode = doc.SelectSingleNode("//locitem/lat");
                var lngNode = doc.SelectSingleNode("//locitem/lng");

                string label = null == labelNode ? "" : labelNode.InnerText;
                string poiname = null == poinameNode ? "" : poinameNode.InnerText;
                string lat = null == latNode ? "" : latNode.InnerText;
                string lng = null == lngNode ? "" : lngNode.InnerText;

                wcf.Text = string.Format("{0}-{1}\r\n经度:{2} 纬度:{3}", poiname, label, lat, lng);
            }
            catch
            {
            }
        }

        /// <summary>
        /// 解析 我的收藏 聊天记录
        /// </summary>
        private void GetFavoriteItemMoreMsg(dynamic data, ref WeChatFavorite wcf)
        {
            wcf.DataType = "聊天记录";

            string text = DynamicConvert.ToSafeString(data.xml);
            try
            {
                var doc = new XmlDocument();
                doc.LoadXml(text);

                var titleNode = doc.SelectSingleNode("//title");
                var dataitemNodes = doc.SelectNodes("//datalist/dataitem");

                StringBuilder sb = new StringBuilder();
                if (null != titleNode)
                {
                    sb.AppendLine(string.Format("{0} 共{1}条", titleNode.InnerText, dataitemNodes.Count));
                }
                foreach (XmlNode node in dataitemNodes)
                {
                    var res = GetFavoriteDataItemString(node);
                    if (res.IsValid())
                    {
                        sb.AppendLine(res);
                    }
                }

                wcf.Text = sb.ToString();
            }
            catch 
            {
            }
        }

        /// <summary>
        /// 解析 我的收藏 自定义
        /// </summary>
        private void GetFavoriteItemUser(dynamic data, ref WeChatFavorite wcf)
        {
            wcf.DataType = "自定义";

            string text = DynamicConvert.ToSafeString(data.xml);
            try
            {
                var doc = new XmlDocument();
                doc.LoadXml(text);

                var dataitemNodes = doc.SelectNodes("//datalist/dataitem");

                StringBuilder sb = new StringBuilder();
                sb.AppendLine(string.Format("共{0}条", dataitemNodes.Count));
                foreach (XmlNode node in dataitemNodes)
                {
                    var res = GetFavoriteDataItemString(node);
                    if (res.IsValid())
                    {
                        sb.AppendLine(res);
                    }
                }

                wcf.Text = sb.ToString();
            }
            catch
            {
            }
        }

        /// <summary>
        /// 解析 我的收藏 dataitem
        /// </summary>
        /// <param name="dataitem"></param>
        /// <returns></returns>
        private string GetFavoriteDataItemString(XmlNode dataitemNode)
        {
            try
            {
                string datatype = dataitemNode.Attributes["datatype"].Value;
                var datafmtNode = dataitemNode.SelectSingleNode("datafmt");

                StringBuilder sb = new StringBuilder();
                string fileName = "";
                switch (datatype)
                {
                    case "1"://文本
                        #region 文本
                        sb.Append("消息类型:文本 ");

                        var datadesc = dataitemNode.SelectSingleNode("datadesc");
                        sb.AppendFormat("内容:{0} ", datadesc.InnerText);
                        #endregion
                        break;
                    case "2"://图片
                        #region 图片
                        sb.Append("消息类型:图片 ");

                        fileName = dataitemNode.Attributes["dataid"].Value;
                        if (null != datafmtNode)
                        {
                            fileName = string.Format("{0}.{1}", dataitemNode.Attributes["dataid"].Value, datafmtNode.InnerText);
                        }
                        if (null != ListFavoriteFiles)
                        {
                            var res = ListFavoriteFiles.FirstOrDefault(f => f.Name == fileName);

                            sb.Append("文件路径:" + (res != null ? res.FullName.TrimStart(MediaFileRootPath) : fileName) + " ");
                        }
                        else
                        {
                            sb.Append("文件路径:" + fileName + " ");
                        }
                        #endregion
                        break;
                    case "3"://语音
                        #region 语音
                        sb.Append("消息类型:语音 ");

                        fileName = dataitemNode.Attributes["dataid"].Value;
                        if (null != datafmtNode)
                        {
                            fileName = string.Format("{0}.{1}", dataitemNode.Attributes["dataid"].Value, datafmtNode.InnerText);
                        }
                        if (null != ListFavoriteFiles)
                        {
                            var res = ListFavoriteFiles.FirstOrDefault(f => f.Name == fileName);

                            sb.Append("文件路径:" + (res != null ? res.FullName.TrimStart(MediaFileRootPath) : fileName) + " ");
                        }
                        else
                        {
                            sb.Append("文件路径:" + fileName + " ");
                        }
                        #endregion
                        break;
                    case "4"://视频
                        #region 视频
                        sb.Append("消息类型:视频 ");

                        fileName = dataitemNode.Attributes["dataid"].Value;
                        if (null != datafmtNode)
                        {
                            fileName = string.Format("{0}.{1}", dataitemNode.Attributes["dataid"].Value, datafmtNode.InnerText);
                        }
                        if (null != ListFavoriteFiles)
                        {
                            var res = ListFavoriteFiles.FirstOrDefault(f => f.Name == fileName);

                            sb.Append("文件路径:" + (res != null ? res.FullName.TrimStart(MediaFileRootPath) : fileName) + " ");
                        }
                        else
                        {
                            sb.Append("文件路径:" + fileName + " ");
                        }
                        #endregion
                        break;
                    case "6"://定位
                        #region 定位信息
                        sb.Append("消息类型:定位信息 ");

                        var poinameNode = dataitemNode.SelectSingleNode("locitem/poiname");
                        var labelNode = dataitemNode.SelectSingleNode("locitem/label");
                        var latNode = dataitemNode.SelectSingleNode("locitem/lat");
                        var lngNode = dataitemNode.SelectSingleNode("locitem/lng");

                        sb.AppendFormat("{0}-{1} 经度:{2} 纬度:{3} ", poinameNode.InnerText, labelNode.InnerText, latNode.InnerText, lngNode.InnerText);
                        #endregion
                        break;
                    case "8"://文件
                        #region 文件
                        sb.Append("消息类型:文件 ");

                        fileName = dataitemNode.Attributes["dataid"].Value;
                        if (null != datafmtNode)
                        {
                            fileName = string.Format("{0}.{1}", dataitemNode.Attributes["dataid"].Value, datafmtNode.InnerText);
                        }
                        if (null != ListFavoriteFiles)
                        {
                            var res = ListFavoriteFiles.FirstOrDefault(f => f.Name == fileName);

                            sb.Append("文件路径:" + (res != null ? res.FullName.TrimStart(MediaFileRootPath) : fileName) + " ");
                        }
                        else
                        {
                            sb.Append("文件路径:" + fileName + " ");
                        }
                        #endregion
                        break;
                    case "15"://视频 OR 文件
                        #region 视频
                        sb.Append("消息类型:视频 ");

                        fileName = dataitemNode.Attributes["dataid"].Value;
                        if (null != datafmtNode)
                        {
                            fileName = string.Format("{0}.{1}", dataitemNode.Attributes["dataid"].Value, datafmtNode.InnerText);
                        }
                        if (null != ListFavoriteFiles)
                        {
                            var res = ListFavoriteFiles.FirstOrDefault(f => f.Name == fileName);

                            sb.Append("文件路径:" + (res != null ? res.FullName.TrimStart(MediaFileRootPath) : fileName) + " ");
                        }
                        else
                        {
                            sb.Append("文件路径:" + fileName + " ");
                        }
                        #endregion
                        break;
                    default:
                        sb.AppendFormat("消息类型:{0} ", datatype);
                        break;
                }

                var fromusrNode = dataitemNode.SelectSingleNode("dataitemsource/fromusr");
                var tousrNode = dataitemNode.SelectSingleNode("dataitemsource/tousr");
                var realchatnameNode = dataitemNode.SelectSingleNode("dataitemsource/realchatname");
                var datasrctimeNode = dataitemNode.SelectSingleNode("datasrctime");
                var datasrcnameNode = dataitemNode.SelectSingleNode("datasrcname");

                if (null != fromusrNode)
                {//来源
                    var fromusr = fromusrNode.InnerText;
                    var usr = LsAllFriends.FirstOrDefault(f => f.WeChatId == fromusr);
                    if (null != usr)
                    {
                        sb.AppendFormat("来源:{0} ", usr.ShowName);
                    }
                    else
                    {
                        sb.AppendFormat("来源:{0} ", fromusr);
                    }
                }
                else if (null != datasrcnameNode)
                {
                    sb.AppendFormat("来源:{0} ", datasrcnameNode.InnerText);
                }

                if (null != realchatnameNode)
                {//发送人
                    var realchatname = realchatnameNode.InnerText;
                    var usr = LsAllFriends.FirstOrDefault(f => f.WeChatId == realchatname);
                    if (null != usr)
                    {
                        sb.AppendFormat("发送者:{0} ", usr.ShowName);
                    }
                    else
                    {
                        sb.AppendFormat("发送者:{0} ", realchatname);
                    }
                }

                if (null != tousrNode)
                {//接受人
                    var tousr = tousrNode.InnerText;
                    var usr = LsAllFriends.FirstOrDefault(f => f.WeChatId == tousr);
                    if (null != usr)
                    {
                        sb.AppendFormat("接收者:{0} ", usr.ShowName);
                    }
                    else
                    {
                        sb.AppendFormat("接收者:{0} ", tousr);
                    }
                }

                if (null != datasrctimeNode)
                {//发送时间
                    sb.AppendFormat("发送时间:{0} ", datasrctimeNode.InnerText);
                }

                return sb.ToString().TrimEnd();
            }
            catch
            {
                return null;
            }
        }

        #endregion

        #region 朋友圈数据解析

        private void GetSmsInfo(dynamic item, WeChatFriendShow curSender, TreeNode tree)
        {
            var snsInfo = new WeChatSns();
            snsInfo.WeChatId = curSender.WeChatId;
            snsInfo.NickName = curSender.Nick;
            snsInfo.UserName = curSender.WeChatId;
            snsInfo.CreateTime = DynamicConvert.ToSafeDateTime(item.createTime);
            snsInfo.TypeDesc = "发表朋友圈";

            GetSnsContent(snsInfo, item.content as byte[]);

            tree.Items.Add(snsInfo);
        }

        private void GetSnsContent(WeChatSns snsInfo, byte[] array)
        {
            var memont = new LINK_WXIN_MOMENT_INFO();
            IntPtr intPtr = IntPtr.Zero;
            GCHandle hObject = GCHandle.Alloc(array, GCHandleType.Pinned);
            IntPtr pObject = hObject.AddrOfPinnedObject();
            StringBuilder sb = new StringBuilder();

            var result = WeChatMomentCoreDll.analyzeWeChatMomentInfoforAndroid(pObject, array.Length, ref intPtr);
            if (result == 0)
            {
                memont = intPtr.ToStruct<LINK_WXIN_MOMENT_INFO>();
                byte[] dataByte = memont.wme.data_utf.Ptr2Bytes();
                if (memont.wme.flag == WXIN_TYPE_FLAG.WXIN_TYPE_FLAG_LOCATION)
                {
                    snsInfo.Location = Encoding.UTF8.GetString(dataByte);
                }
                else if (memont.wme.flag == WXIN_TYPE_FLAG.WXIN_TYPE_FLAG_PIC || memont.wme.flag == WXIN_TYPE_FLAG.WXIN_TYPE_FLAG_VIDEO
                    || memont.wme.flag == WXIN_TYPE_FLAG.WXIN_TYPE_FLAG_FORWARD)
                {
                    snsInfo.MediaList.Add(Encoding.UTF8.GetString(dataByte));
                }
                else
                {
                    sb.AppendLine(Encoding.UTF8.GetString(dataByte));
                }
                while (memont.next != IntPtr.Zero)
                {
                    memont = memont.next.ToStruct<LINK_WXIN_MOMENT_INFO>();
                    dataByte = memont.wme.data_utf.Ptr2Bytes();
                    if (memont.wme.flag == WXIN_TYPE_FLAG.WXIN_TYPE_FLAG_LOCATION)
                    {
                        snsInfo.Location = Encoding.UTF8.GetString(dataByte);
                    }
                    else if (memont.wme.flag == WXIN_TYPE_FLAG.WXIN_TYPE_FLAG_PIC || memont.wme.flag == WXIN_TYPE_FLAG.WXIN_TYPE_FLAG_VIDEO
                            || memont.wme.flag == WXIN_TYPE_FLAG.WXIN_TYPE_FLAG_FORWARD)
                    {
                        snsInfo.MediaList.Add(Encoding.UTF8.GetString(dataByte));
                    }
                    else
                    {
                        sb.AppendLine(Encoding.UTF8.GetString(dataByte));
                    }
                }

                snsInfo.Content = sb.ToString().TrimEnd();
            }
            WeChatMomentCoreDll.freeLINK_WXIN_MOMENT_INFO(ref intPtr);
            hObject.Free();
        }

        /// <summary>
        /// 获取朋友圈评论
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        private void GetSnsComments(dynamic item, TreeNode tree)
        {
            byte[] array = item.attrBuf;
            var comment = new LINK_WXIN_COMMENT_INFO();
            IntPtr intPtr = IntPtr.Zero;
            GCHandle hObject = GCHandle.Alloc(array, GCHandleType.Pinned);
            IntPtr pObject = hObject.AddrOfPinnedObject();
            WeChatSns snsInfo;

            var result = WeChatMomentCoreDll.analyzeWeChatCommentInfoforAndroid(pObject, array.Length, ref intPtr);
            if (result == 0 && intPtr != IntPtr.Zero)
            {
                snsInfo = new WeChatSns();
                snsInfo.CreateTime = DynamicConvert.ToSafeDateTime(item.createTime);
                comment = intPtr.ToStruct<LINK_WXIN_COMMENT_INFO>();

                snsInfo.WeChatId = Encoding.UTF8.GetString(comment.wce.uwxid.Ptr2Bytes());
                if (!string.IsNullOrEmpty(snsInfo.WeChatId))
                {
                    snsInfo.NickName = Encoding.UTF8.GetString(comment.wce.unick.Ptr2Bytes());
                    snsInfo.UserName = snsInfo.WeChatId;
                    byte[] dataByte = comment.wce.udata.Ptr2Bytes();
                    if (comment.wce.flag == WXIN_COMMENT_TYPE_FLAG.WXIN_COMMENT_TYPE_FLAG_LIKE)
                    {
                        snsInfo.TypeDesc = "点赞";
                        snsInfo.Content = "点赞";
                    }
                    else
                    {
                        snsInfo.TypeDesc = "评论";
                        snsInfo.Content = Encoding.UTF8.GetString(dataByte);
                    }
                    tree.Items.Add(snsInfo);

                    while (comment.next != IntPtr.Zero)
                    {
                        snsInfo = new WeChatSns();
                        snsInfo.CreateTime = DynamicConvert.ToSafeDateTime(item.createTime);
                        comment = comment.next.ToStruct<LINK_WXIN_COMMENT_INFO>();

                        snsInfo.WeChatId = Encoding.UTF8.GetString(comment.wce.uwxid.Ptr2Bytes());
                        snsInfo.NickName = Encoding.UTF8.GetString(comment.wce.unick.Ptr2Bytes());
                        snsInfo.UserName = snsInfo.WeChatId;
                        dataByte = comment.wce.udata.Ptr2Bytes();
                        if (comment.wce.flag == WXIN_COMMENT_TYPE_FLAG.WXIN_COMMENT_TYPE_FLAG_LIKE)
                        {
                            snsInfo.TypeDesc = "点赞";
                            snsInfo.Content = "点赞";
                        }
                        else
                        {
                            snsInfo.TypeDesc = "评论";
                            snsInfo.Content = Encoding.UTF8.GetString(dataByte);
                        }
                        tree.Items.Add(snsInfo);
                    }
                }
            }

            WeChatMomentCoreDll.freeLINK_WXIN_COMMENT_INFO(ref intPtr);
            hObject.Free();
        }

        #endregion

        #endregion
    }
}
