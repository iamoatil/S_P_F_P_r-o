/***************************************************************************
 * 
 * author :  Songbing
 * time: 2017/9/21 14:09:08 
 * explain :  
 *
*****************************************************************************/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using XLY.SF.Framework.BaseUtility;
using XLY.SF.Project.BaseUtility.Helper;
using XLY.SF.Project.Domains;
using XLY.SF.Project.Persistable.Primitive;
using XLY.SF.Project.Services;

namespace XLY.SF.Project.Plugin.Android
{
    /// <summary>
    /// 安卓QQ数据解析核心类
    /// </summary>
    public class AndroidQQDataParseCoreV1_0
    {
        /// <summary>
        /// 安卓QQ数据解析核心类
        /// </summary>
        /// <param name="savedatadbpath">数据保存数据库路径</param>
        /// <param name="name">QQ名称  例如 QQ、小米QQ分身</param>
        /// <param name="datapath">QQ数据文件根目录，例如 I:\本地数据\com.tencent.mobileqq</param>
        /// <param name="mediapath">QQ多媒体文件夹根目录，例如 I:\本地数据\Tencent\MobileQQ</param>
        public AndroidQQDataParseCoreV1_0(string savedatadbpath, string name, string datapath, string mediapath)
        {
            DbFilePath = savedatadbpath;
            QQName = name;
            DataFileRootPath = datapath;
            MediaFileRootPath = mediapath;
        }

        #region 构造属性

        /// <summary>
        /// 数据库路径
        /// </summary>
        private string DbFilePath { get; set; }

        /// <summary>
        /// QQ名称，例如 QQ、小米QQ分身
        /// </summary>
        private string QQName { get; set; }

        /// <summary>
        /// QQ数据文件根目录，例如 I:\本地数据\com.tencent.mobileqq
        /// 该文件夹至少包含 databases 和 shared_prefs两个文件夹
        /// 其中，databases保存了QQ数据库文件，shared_prefs保存了解密用的key相关的文件
        /// </summary>
        private string DataFileRootPath { get; set; }

        /// <summary>
        /// QQ多媒体文件夹根目录，例如 I:\本地数据\Tencent\MobileQQ
        /// 该文件夹包含了QQ聊天的相关文件，如图片、语音、视频
        /// </summary>
        private string MediaFileRootPath { get; set; }

        #endregion

        #region 临时属性

        /// <summary>
        /// 主数据库
        /// </summary>
        private SqliteContext MainDbContext { get; set; }

        /// <summary>
        /// 解密key
        /// </summary>
        private string DecryptKey { get; set; }

        /// <summary>
        /// 发送的文件列表
        /// </summary>
        private List<dynamic> LsFileManager { get; set; }

        /// <summary>
        /// 当前QQ号
        /// </summary>
        private string CurQQNumber { get; set; }

        /// <summary>
        /// 当前QQ帐号
        /// </summary>
        private QQAccountShow CurQQAccount { get; set; }

        /// <summary>
        /// 好友消息表列表
        /// </summary>
        private List<string> LsFriendMsgTables { get; set; }

        /// <summary>
        /// QQ群消息表列表
        /// </summary>
        private List<string> LsTroopMsgTables { get; set; }

        /// <summary>
        /// 讨论组消息表列表
        /// </summary>
        private List<string> LsDiscussMsgTables { get; set; }

        /// <summary>
        /// 所有好友
        /// </summary>
        private List<QQFriendShow> AllFrineds { get; set; }

        /// <summary>
        /// 所有群组
        /// </summary>
        private List<QQGroupShow> AllGroups { get; set; }

        /// <summary>
        /// 所有讨论组
        /// </summary>
        private List<QQDiscussShow> AllDiscusss { get; set; }

        /// <summary>
        /// 所有图片文件缓存
        /// </summary>
        private Dictionary<string, string> DicMd5Image { get; set; }


        /// <summary>
        /// 清除临时属性
        /// 一般用于插件执行完毕后执行
        /// </summary>
        private void ClearCache()
        {
            MainDbContext?.Dispose();
            MainDbContext = null;

            DecryptKey = string.Empty;
            CurQQNumber = string.Empty;
            CurQQAccount = null;

            LsFileManager?.Clear();
            LsFileManager = null;

            LsFriendMsgTables?.Clear();
            LsFriendMsgTables = null;

            LsTroopMsgTables?.Clear();
            LsTroopMsgTables = null;

            LsDiscussMsgTables?.Clear();
            LsDiscussMsgTables = null;

            AllFrineds?.Clear();
            AllFrineds = null;

            AllGroups?.Clear();
            AllGroups = null;

            AllDiscusss?.Clear();
            AllDiscusss = null;

            DicMd5Image?.Clear();
            DicMd5Image = null;
        }

        #endregion

        /// <summary>
        /// 解析QQ数据
        /// </summary>
        /// <returns></returns>
        public TreeNode BiuldTree()
        {
            TreeNode rootNode = new TreeNode();
            try
            {
                rootNode.Text = QQName;
                rootNode.Type = typeof(QQAccountShow);
                rootNode.Items = new DataItems<QQAccountShow>(DbFilePath);

                var acountFiles = GetAllQQAccountFileInfos();

                DecryptKey = GetDecryptKey(acountFiles);

                foreach (var file in acountFiles)
                {
                    BuildQQTree(rootNode, file);
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
        /// 解析QQ数据库文件
        /// </summary>
        /// <param name="qqFile"></param>
        /// <returns></returns>
        private void BuildQQTree(TreeNode rootNode, FileInfo qqFile)
        {
            //当前QQ帐号
            CurQQNumber = qqFile.Name.TrimEnd(".db");
            CurQQAccount = new QQAccountShow() { QQNumber = CurQQNumber };
            var accountTree = new TreeNode()
            {
                Text = CurQQNumber,
                Type = typeof(QQAccountShow)
            };

            //数据库恢复
            string recoverTables = GetRecoverTables(qqFile.FullName);
            string charatorPath = @"";
            string recoverDbPath = SqliteRecoveryHelper.DataRecovery(qqFile.FullName, charatorPath, recoverTables);
            MainDbContext = new SqliteContext(recoverDbPath);

            //发送的文件列表
            LsFileManager = MainDbContext.Find("SELECT uniseq,strFilePath,strThumbPath,nFileType FROM mr_fileManager").ToList();

            //好友列表
            BuildQQFriendTree(accountTree);

            //好友消息
            BuildQQFriendMsgTree(accountTree);

            //QQ群
            BuildTroopTree(accountTree);

            //QQ群消息
            BuildTroopMsgTree(accountTree);

            //讨论组
            BuildDiscussTree(accountTree);

            //讨论组消息
            BuildDiscussMsgTree(accountTree);

            //最近联系列表
            BuildRecentTree(accountTree);

            rootNode.TreeNodes.Add(accountTree);
            rootNode.Items.Add(CurQQAccount);
        }

        /// <summary>
        /// 构建好友列表树
        /// </summary>
        private void BuildQQFriendTree(TreeNode accountTree)
        {
            //好友分组信息树
            var friendRootSet = new TreeNode()
            {
                Text = "好友列表",
                Type = typeof(QQFriendSetShow),
                Items = new DataItems<QQFriendSetShow>(DbFilePath)
            };
            accountTree.TreeNodes.Add(friendRootSet);

            //获取好友分组信息
            MainDbContext.UsingSafeConnection("SELECT group_id,group_name,group_friend_count,XLY_DataType FROM Groups ORDER BY seqid", (r) =>
                {
                    while (r.Read())
                    {
                        LoadFriendGroup(friendRootSet, r.ToDynamic());
                    }
                });

            //陌生人/黑名单特殊处理
            var OtherFriendTree = new TreeNode()
            {
                Text = "陌生人、黑名单、其他",
                Id = "-1",
                Type = typeof(QQFriendShow),
                DataState = EnumDataState.Normal,
                Items = new DataItems<QQFriendShow>(DbFilePath)
            };
            friendRootSet.TreeNodes.Add(OtherFriendTree);

            //所有好友信息
            var others = LoadFriendGroupMember(friendRootSet, OtherFriendTree);

            //构建陌生人显示子项个数
            var otherFriendSet = new QQFriendSetShow()
            {
                Name = OtherFriendTree.Text,
                DataState = OtherFriendTree.DataState,
                MemberCount = others
            };
            friendRootSet.Items.Add(otherFriendSet);
        }

        /// <summary>
        /// 构建好友消息树
        /// </summary>
        /// <param name="accountTree"></param>
        private void BuildQQFriendMsgTree(TreeNode accountTree)
        {
            //好友消息树
            var friendMsgRootSet = new TreeNode()
            {
                Text = "好友消息",
            };
            accountTree.TreeNodes.Add(friendMsgRootSet);

            foreach (var friend in AllFrineds)
            {
                if (!LsFriendMsgTables.Any(s => s.Contains(CryptographyHelper.MD5FromStringUpper(friend.QQNumber))))
                {//没有该好友的聊天消息表
                    continue;
                }

                var friendMsgTree = new TreeNode()
                {
                    Text = friend.FullName,
                    Type = typeof(MessageCore),
                    DataState = friend.DataState,
                    Items = new DataItems<MessageCore>(DbFilePath)
                };
                friendMsgRootSet.TreeNodes.Add(friendMsgTree);

                LoadFriendMsg(friendMsgTree, friend);
            }
        }

        /// <summary>
        /// 构建QQ群列表树
        /// </summary>
        /// <param name="accountTree"></param>
        private void BuildTroopTree(TreeNode accountTree)
        {
            AllGroups = new List<QQGroupShow>();

            var troopMemberTree = new TreeNode()
            {
                Text = "群成员",
                Type = typeof(QQGroupShow),
                Items = new DataItems<QQGroupShow>(DbFilePath)
            };
            accountTree.TreeNodes.Add(troopMemberTree);

            var sql = @"SELECT
                        	t.troopuin,
                        	t.troopname,
                        	t.troopmemo,
                        	t.fingertroopmemo,
                        	t.troopCreateTime,
                        	t.troopowneruin,
                        	m.friendnick,
                        	m.troopnick,
                        	t.Administrator,
                        	t.wMemberNum,
                        	t.XLY_DataType
                        FROM
                        	TroopInfo t
                        LEFT JOIN TroopMemberInfo m ON t.troopuin = m.troopuin
                        AND t.troopowneruin = m.memberuin";

            MainDbContext.UsingSafeConnection(sql, (r) =>
             {
                 dynamic troopDy;
                 QQGroupShow groupShow;

                 while (r.Read())
                 {
                     troopDy = r.ToDynamic();

                     //QQ群信息
                     groupShow = new QQGroupShow()
                     {
                         QQNumber = Decrypt(DynamicConvert.ToSafeString(troopDy.troopuin)),
                         Name = Decrypt(DynamicConvert.ToSafeString(troopDy.troopname)),
                         Notice = Decrypt(DynamicConvert.ToSafeString(troopDy.troopmemo)),
                         Desc = Decrypt(DynamicConvert.ToSafeString(troopDy.fingertroopmemo)),
                         MemberCount = DynamicConvert.ToSafeInt(troopDy.wMemberNum),
                         CreateTime = DynamicConvert.ToSafeFromUnixTime(troopDy.troopCreateTime, 1),
                         DataState = DynamicConvert.ToEnumByValue(troopDy.XLY_DataType, EnumDataState.Normal)
                     };

                     string troopowneruin = Decrypt(DynamicConvert.ToSafeString(troopDy.troopowneruin));
                     string troopnick = Decrypt(DynamicConvert.ToSafeString(troopDy.troopnick));
                     string friendnick = Decrypt(DynamicConvert.ToSafeString(troopDy.friendnick));
                     if (troopnick.IsValid())
                     {
                         groupShow.Creator = string.Format("{0}({1})", troopnick, troopowneruin);
                     }
                     else if (friendnick.IsValid())
                     {
                         groupShow.Creator = string.Format("{0}({1})", friendnick, troopowneruin);
                     }
                     else
                     {
                         groupShow.Creator = troopowneruin;
                     }

                     AllGroups.Add(groupShow);

                     //QQ群成员
                     var memberTree = new TreeNode()
                     {
                         Text = string.Format("{0}({1})", groupShow.Name, groupShow.QQNumber),
                         Type = typeof(QQFriendShow),
                         Items = new DataItems<QQFriendShow>(DbFilePath),
                         DataState = groupShow.DataState
                     };

                     string troopuin = DynamicConvert.ToSafeString(troopDy.troopuin);
                     MainDbContext.UsingSafeConnection(string.Format("SELECT memberuin,friendnick,troopnick,XLY_DataType FROM TroopMemberInfo WHERE troopuin = '{0}'", troopuin), (rr) =>
                      {
                          dynamic member;
                          QQFriendShow friendShow;

                          while (rr.Read())
                          {
                              member = rr.ToDynamic();

                              friendShow = new QQFriendShow()
                              {
                                  QQNumber = Decrypt(DynamicConvert.ToSafeString(member.memberuin)),
                                  Nick = Decrypt(DynamicConvert.ToSafeString(member.friendnick)),
                                  Alias = Decrypt(DynamicConvert.ToSafeString(member.troopnick)),
                                  DataState = DynamicConvert.ToEnumByValue(member.XLY_DataType, EnumDataState.Normal)
                              };
                              memberTree.Items.Add(friendShow);
                          }
                      });

                     troopMemberTree.TreeNodes.Add(memberTree);
                     troopMemberTree.Items.Add(groupShow);
                 }
             });
        }

        /// <summary>
        /// 构建QQ群消息树
        /// </summary>
        /// <param name="accountTree"></param>
        private void BuildTroopMsgTree(TreeNode accountTree)
        {
            //群消息树
            var troopMsgRootSet = new TreeNode()
            {
                Text = "群消息",
            };
            accountTree.TreeNodes.Add(troopMsgRootSet);

            foreach (var group in AllGroups)
            {
                if (!LsTroopMsgTables.Any(s => s.Contains(CryptographyHelper.MD5FromStringUpper(group.QQNumber))))
                {//没有该群组的聊天消息表
                    continue;
                }

                var troopMsgTree = new TreeNode()
                {
                    Text = group.FullName,
                    Type = typeof(MessageCore),
                    DataState = group.DataState,
                    Items = new DataItems<MessageCore>(DbFilePath)
                };
                troopMsgRootSet.TreeNodes.Add(troopMsgTree);

                LoadTroopMsg(troopMsgTree, group);
            }
        }

        /// <summary>
        /// 构建讨论组树
        /// </summary>
        /// <param name="accountTree"></param>
        private void BuildDiscussTree(TreeNode accountTree)
        {
            AllDiscusss = new List<QQDiscussShow>();

            var discussMemberTree = new TreeNode()
            {
                Text = "讨论组成员",
                Type = typeof(QQDiscussShow),
                Items = new DataItems<QQDiscussShow>(DbFilePath)
            };
            accountTree.TreeNodes.Add(discussMemberTree);

            var sql = @"SELECT
                        	d.uin,
                        	d.ownerUin,
                        	m.memberName,
                        	m.inteRemark,
                        	d.discussionName,
                        	d.createTime
                        FROM
                        	DiscussionInfo d
                        LEFT JOIN DiscussionMemberInfo m ON d.uin = m.discussionUin
                        AND d.ownerUin = m.memberUin";

            MainDbContext.UsingSafeConnection(sql, (r) =>
            {
                dynamic troopDy;
                QQDiscussShow discussShow;

                while (r.Read())
                {
                    troopDy = r.ToDynamic();

                    //讨论组信息
                    discussShow = new QQDiscussShow()
                    {
                        QQNumber = Decrypt(DynamicConvert.ToSafeString(troopDy.uin)),
                        Name = Decrypt(DynamicConvert.ToSafeString(troopDy.discussionName)),
                        CreateTime = DynamicConvert.ToSafeFromUnixTime(troopDy.createTime, 1),
                        DataState = DynamicConvert.ToEnumByValue(troopDy.XLY_DataType, EnumDataState.Normal)
                    };

                    string createid = Decrypt(DynamicConvert.ToSafeString(troopDy.ownerUin));
                    string memberName = Decrypt(DynamicConvert.ToSafeString(troopDy.memberName));
                    string inteRemark = Decrypt(DynamicConvert.ToSafeString(troopDy.inteRemark));
                    if (inteRemark.IsValid())
                    {
                        discussShow.Creator = string.Format("{0}({1})", inteRemark, createid);
                    }
                    else if (memberName.IsValid())
                    {
                        discussShow.Creator = string.Format("{0}({1})", memberName, createid);
                    }
                    else
                    {
                        discussShow.Creator = createid;
                    }

                    AllDiscusss.Add(discussShow);

                    //讨论组成员
                    var memberTree = new TreeNode()
                    {
                        Text = string.Format("{0}({1})", discussShow.Name, discussShow.QQNumber),
                        Type = typeof(QQFriendShow),
                        Items = new DataItems<QQFriendShow>(DbFilePath),
                        DataState = discussShow.DataState
                    };

                    string uin = DynamicConvert.ToSafeString(troopDy.uin);
                    MainDbContext.UsingSafeConnection(string.Format("SELECT memberUin,memberName,inteRemark,XLY_DataType FROM DiscussionMemberInfo WHERE discussionUin = '{0}'", uin), (rr) =>
                    {
                        dynamic member;
                        QQFriendShow friendShow;
                        while (rr.Read())
                        {
                            member = rr.ToDynamic();

                            friendShow = new QQFriendShow()
                            {
                                QQNumber = Decrypt(DynamicConvert.ToSafeString(member.memberUin)),
                                Nick = Decrypt(DynamicConvert.ToSafeString(member.memberName)),
                                Remark = Decrypt(DynamicConvert.ToSafeString(member.inteRemark)),
                                DataState = DynamicConvert.ToEnumByValue(member.XLY_DataType, EnumDataState.Normal)
                            };
                            memberTree.Items.Add(friendShow);
                        }
                    });

                    discussMemberTree.TreeNodes.Add(memberTree);
                    discussMemberTree.Items.Add(discussShow);
                }
            });
        }

        /// <summary>
        /// 构建讨论组消息树
        /// </summary>
        /// <param name="accountTree"></param>
        private void BuildDiscussMsgTree(TreeNode accountTree)
        {
            //讨论组消息树
            var discussMsgRootSet = new TreeNode()
            {
                Text = "讨论组消息",
            };
            accountTree.TreeNodes.Add(discussMsgRootSet);

            foreach (var discuss in AllDiscusss)
            {
                if (!LsDiscussMsgTables.Any(s => s.Contains(CryptographyHelper.MD5FromStringUpper(discuss.QQNumber))))
                {//没有该讨论组的聊天消息表
                    continue;
                }

                var discussMsgTree = new TreeNode()
                {
                    Text = discuss.FullName,
                    Type = typeof(MessageCore),
                    DataState = discuss.DataState,
                    Items = new DataItems<MessageCore>(DbFilePath)
                };
                discussMsgRootSet.TreeNodes.Add(discussMsgTree);

                LoadDiscussMsg(discussMsgTree, discuss);
            }
        }

        /// <summary>
        /// 构建最近联系人树
        /// </summary>
        /// <param name="accountTree"></param>
        private void BuildRecentTree(TreeNode accountTree)
        {
            var recentTree = new TreeNode()
            {
                Text = "最近联系人",
                Type = typeof(QQRecentShow),
                Items = new DataItems<QQRecentShow>(DbFilePath)
            };
            accountTree.TreeNodes.Add(recentTree);

            MainDbContext.UsingSafeConnection("SELECT uin,troopUin,displayName,lastmsgtime FROM recent WHERE XLY_DataType = 2 ORDER BY lastmsgtime DESC", (r) =>
             {
                 dynamic recentDy;
                 QQRecentShow show;

                 while (r.Read())
                 {
                     recentDy = r.ToDynamic();

                     show = new QQRecentShow()
                     {
                         DataState = EnumDataState.Normal,
                         QQNumber = Decrypt(DynamicConvert.ToSafeString(recentDy.uin)),
                         Name = Decrypt(DynamicConvert.ToSafeString(recentDy.displayName)),
                         RecentDatetime = DynamicConvert.ToSafeFromUnixTime(recentDy.lastmsgtime, 1)
                     };
                     if (show.QQNumber.IsInvalid())
                     {
                         show.QQNumber = Decrypt(DynamicConvert.ToSafeString(recentDy.troopUin));
                     }

                     recentTree.Items.Add(show);
                 }
             });
        }

        /// <summary>
        /// 构建好友分组信息
        /// </summary>
        /// <param name="friendRootSet"></param>
        /// <param name="friGroup"></param>
        private void LoadFriendGroup(TreeNode friendRootSet, dynamic friGroup)
        {
            var frindGroupTree = new TreeNode();

            string groupName = Decrypt(DynamicConvert.ToSafeString(friGroup.group_name));
            if (friendRootSet.TreeNodes.Exists(t => t.Text == groupName))
            {
                return;
            }

            frindGroupTree.Text = Decrypt(DynamicConvert.ToSafeString(friGroup.group_name));
            frindGroupTree.Id = DynamicConvert.ToSafeString(friGroup.group_id);
            frindGroupTree.Type = typeof(QQFriendShow);
            frindGroupTree.DataState = DynamicConvert.ToEnumByValue(friGroup.XLY_DataType, EnumDataState.Normal);
            frindGroupTree.Items = new DataItems<QQFriendShow>(DbFilePath);

            var qqFriendSet = new QQFriendSetShow()
            {
                Name = frindGroupTree.Text,
                DataState = frindGroupTree.DataState,
                MemberCount = (int)DynamicConvert.ToSafeLong(friGroup.group_friend_count)
            };
            friendRootSet.Items.Add(qqFriendSet);
            friendRootSet.TreeNodes.Add(frindGroupTree);
        }

        /// <summary>
        /// 构建好友分组好友列表
        /// </summary>
        /// <param name="friendRootSet"></param>
        /// <param name="otherFriendTree"></param>
        private int LoadFriendGroupMember(TreeNode friendRootSet, TreeNode otherFriendTree)
        {
            int others = 0;
            AllFrineds = new List<QQFriendShow>();

            #region 从Card表获取好友信息缓存

            var CardInfo = new Dictionary<string, dynamic>();
            try
            {
                var cards = MainDbContext.Find(new SQLiteString("SELECT * FROM Card WHERE XLY_DataType = 2"));
                if (cards.IsValid())
                {
                    foreach (var cd in cards)
                    {
                        string key = Decrypt(DynamicConvert.ToSafeString(cd.uin));
                        if (!CardInfo.Keys.Contains(key))
                        {
                            CardInfo.Add(key, cd);
                        }
                    }
                }
            }
            catch
            { }

            #endregion

            //1.从Friends表获取好友信息
            var sql = @"SELECT
                      	f.uin,
                      	f.name,
                      	f.remark,
                      	f.groupid,
                      	f.alias,
	                    f.age,
	                    f.gender,
                      	f.signature,
                      	i.feedTime,
                      	i.feedContent,
                      	i.richTime,
                      	i.richBuffer AS irichBuffer
                      FROM
                      	Friends f
                      LEFT JOIN ExtensionInfo i ON f.uin == i.uin";

            MainDbContext.UsingSafeConnection(sql, (r) =>
                {
                    dynamic friendDy;
                    QQFriendShow friendShow;

                    while (r.Read())
                    {
                        friendDy = r.ToDynamic();

                        string friendQQNumber = Decrypt(DynamicConvert.ToSafeString(friendDy.uin));

                        if (friendQQNumber == CurQQNumber)
                        {//帐号信息
                            SetAccountShow(friendDy, CardInfo[CurQQNumber]);
                        }
                        else if (!(friendQQNumber.Length == 4 && friendQQNumber.StartsWith("99")))
                        {//过滤系统消息帐号 例如9975
                         //好友信息
                            friendShow = new QQFriendShow()
                            {
                                QQNumber = friendQQNumber,
                                Nick = Decrypt(DynamicConvert.ToSafeString(friendDy.name)),
                                Remark = Decrypt(DynamicConvert.ToSafeString(friendDy.remark)),
                                Alias = Decrypt(DynamicConvert.ToSafeString(friendDy.alias)),
                                Age = DynamicConvert.ToSafeInt(friendDy.age),
                                Sex = DynamicConvert.ToSafeString(friendDy.gender) == "1" ? EnumSex.Male : (DynamicConvert.ToSafeString(friendDy.gender) == "2" ? EnumSex.Female : EnumSex.None),
                                Feed = Decrypt(DynamicConvert.ToSafeString(friendDy.feedContent)),
                                LatestUpdate = DynamicConvert.ToSafeFromUnixTime(friendDy.feedTime, 1),
                                DataState = DynamicConvert.ToEnumByValue(friendDy.XLY_DataType, EnumDataState.Normal)
                            };

                            if (friendShow.LatestUpdate == null || !friendShow.LatestUpdate.HasValue)
                            {
                                friendShow.LatestUpdate = DynamicConvert.ToSafeFromUnixTime(friendDy.richTime, 1);
                            }

                            switch (friendQQNumber)
                            {
                                case "1344242394":
                                    friendShow.Nick = "QQ红包";
                                    break;
                                case "2010741172":
                                    friendShow.Nick = "QQ邮箱";
                                    break;
                                case "2711679534":
                                    friendShow.Nick = "QQ钱包";
                                    break;
                            }

                            GetFriendSignature(friendShow, friendDy);

                            //所属分组
                            string groupId = DynamicConvert.ToSafeString(friendDy.groupid);
                            TreeNode currentFriendGroup = friendRootSet.TreeNodes.FirstOrDefault(friendSet => friendSet.Id == groupId);
                            if (currentFriendGroup == null)
                            {//陌生人，黑名单
                                currentFriendGroup = otherFriendTree;
                                others++;
                            }

                            currentFriendGroup.Items.Add(friendShow);

                            AllFrineds.Add(friendShow);
                        }
                    }
                });

            //2.其他好友
            foreach (var uinmd5 in LsFriendMsgTables.Select(s => s.TrimStart("mr_friend_").TrimEnd("_New").ToUpper()).Distinct().ToList())
            {
                if (!AllFrineds.Any(f => CryptographyHelper.MD5FromStringUpper(f.QQNumber) == uinmd5))
                {
                    var data = MainDbContext.Find(string.Format("SELECT frienduin FROM mr_friend_{0}_New LIMIT 1", uinmd5));
                    if (data.IsInvalid())
                    {//空表
                        continue;
                    }

                    string friendQQNumber = Decrypt(DynamicConvert.ToSafeString(data.First().frienduin));
                    if (friendQQNumber.Length == 4 && friendQQNumber.StartsWith("99"))
                    {//系统消息表
                        continue;
                    }

                    QQFriendShow qqfriend = new QQFriendShow() { QQNumber = friendQQNumber, DataState = EnumDataState.Deleted };
                    otherFriendTree.Items.Add(qqfriend);
                    others++;
                }
            }

            CardInfo.Clear();

            return others;
        }

        /// <summary>
        /// 获取好友签名
        /// </summary>
        /// <param name="friendShow"></param>
        /// <param name="friendDy"></param>
        private void GetFriendSignature(QQFriendShow friendShow, dynamic friendDy)
        {
            try
            {
                friendShow.Signature = Decrypt(DynamicConvert.ToSafeString(friendDy.signature));
                if (friendShow.Signature.IsValid())
                {
                    return;
                }

                friendShow.Signature = Decrypt(friendDy.richBuffer as byte[]);
                if (friendShow.Signature.IsValid())
                {
                    var buff = Encoding.UTF8.GetBytes(friendShow.Signature);
                    int index = buff.ToList().IndexOf(0x03);
                    if (index != -1 && buff[index + 1] + index + 2 < buff.Length)
                    {
                        friendShow.Signature = Encoding.UTF8.GetString(buff, index + 2, buff[index + 1]);
                    }
                    else
                    {
                        friendShow.Signature = Encoding.UTF8.GetString(buff);
                    }
                    return;
                }

                byte[] richBuffer = friendDy.irichBuffer as byte[];
                if (richBuffer.IsValid())
                {
                    var buff = Encoding.UTF8.GetBytes(Decrypt(richBuffer));
                    if (buff.IsValid())
                    {
                        int index = buff.ToList().IndexOf(0x03);
                        if (index != -1 && buff[index + 1] + index + 2 < buff.Length)
                        {
                            friendShow.Signature = Encoding.UTF8.GetString(buff, index + 2, buff[index + 1]);
                        }
                        else
                        {
                            friendShow.Signature = Encoding.UTF8.GetString(buff);
                        }
                    }
                }
            }
            catch
            { }
        }

        /// <summary>
        /// 完善当前帐号信息
        /// </summary>
        /// <param name="friendDy"></param>
        private void SetAccountShow(dynamic friendDy, dynamic cardDy)
        {
            CurQQAccount.Nick = Decrypt(DynamicConvert.ToSafeString(friendDy.name));
            CurQQAccount.Age = DynamicConvert.ToSafeInt(friendDy.age);
            CurQQAccount.DataState = EnumDataState.Normal;
            CurQQAccount.RecentLoginDate = DynamicConvert.ToSafeDateTime(friendDy.datetime);

            switch (DynamicConvert.ToSafeString(friendDy.gender))
            {
                case "1":
                    CurQQAccount.Sex = EnumSex.Male;
                    break;
                case "2":
                    CurQQAccount.Sex = EnumSex.Female;
                    break;
                default:
                    CurQQAccount.Sex = EnumSex.None;
                    break;
            }

            //获取个性签名
            try
            {
                CurQQAccount.Signature = Decrypt(friendDy.richBuffer as byte[]);
                if (CurQQAccount.Signature.IsValid())
                {
                    var buff = Encoding.UTF8.GetBytes(CurQQAccount.Signature);
                    int index = buff.ToList().IndexOf(0x03);
                    if (index != -1)
                    {
                        CurQQAccount.Signature = Encoding.UTF8.GetString(buff, index + 2, buff[index + 1]);
                    }
                }
            }
            catch
            { }

            //获取绑定的手机号
            try
            {
                string bindinfofile = Path.Combine(DataFileRootPath, "shared_prefs", string.Format("contact_bind_info{0}.xml", CurQQAccount.QQNumber));
                if (File.Exists(bindinfofile))
                {
                    var doc = new XmlDocument();
                    doc.Load(bindinfofile);
                    XmlNodeList userInfoNodeList = doc.SelectNodes("map//string[@name='contact_bind_info_mobile']");
                    if (userInfoNodeList != null && userInfoNodeList.Count > 0)
                    {
                        CurQQAccount.PhoneNumber = userInfoNodeList[0].InnerText;
                    }
                }
            }
            catch
            { }

            //从Card表获取其他信息
            try
            {
                if (null != cardDy)
                {
                    CurQQAccount.Address = Decrypt(DynamicConvert.ToSafeString(cardDy.strLocationDesc));
                    if (CurQQAccount.Address.IsInvalid())
                    {
                        CurQQAccount.Address = string.Format("{0}-{1}", Decrypt(DynamicConvert.ToSafeString(cardDy.strProvince)), Decrypt(DynamicConvert.ToSafeString(cardDy.strCity)));
                    }
                    CurQQAccount.Company = Decrypt(DynamicConvert.ToSafeString(cardDy.strCompany));
                    CurQQAccount.School = Decrypt(DynamicConvert.ToSafeString(cardDy.strSchool));
                    CurQQAccount.Email = Decrypt(DynamicConvert.ToSafeString(cardDy.strEmail));
                    CurQQAccount.PersonNote = Decrypt(DynamicConvert.ToSafeString(cardDy.strPersonalNote));
                    if (CurQQAccount.Signature.IsInvalid())
                    {
                        byte[] vRichSign = cardDy.vRichSign as byte[];
                        if (vRichSign.IsValid())
                        {
                            var buff = Encoding.UTF8.GetBytes(Decrypt(vRichSign));
                            int index = buff.ToList().IndexOf(0x03);
                            if (index != -1)
                            {
                                CurQQAccount.Signature = Encoding.UTF8.GetString(buff, index + 2, buff[index + 1]);
                            }
                            else
                            {
                                CurQQAccount.Signature = Encoding.UTF8.GetString(buff);
                            }
                        }
                    }
                }
            }
            catch
            { }

        }

        /// <summary>
        /// 构建好友聊天记录
        /// </summary>
        /// <param name="friendMsgTree"></param>
        /// <param name="friend"></param>
        private void LoadFriendMsg(TreeNode friendMsgTree, QQFriendShow friend)
        {
            var tableName = LsFriendMsgTables.FirstOrDefault(s => s.Contains(CryptographyHelper.MD5FromStringUpper(friend.QQNumber)));

            var friendName = friend.FullName;
            var myName = CurQQAccount.FullName;

            var sql = string.Format("SELECT istroop,senderuin,frienduin,issend,msgData,time,msgtype,uniseq,XLY_DataType FROM {0} ORDER BY time", tableName);

            MainDbContext.UsingSafeConnection(sql, (r) =>
              {
                  dynamic friendDyMsg;
                  MessageCore friendMsg;

                  while (r.Read())
                  {
                      friendDyMsg = r.ToDynamic();

                      string isTroop = DynamicConvert.ToSafeString(friendDyMsg.istroop);
                      string senderQQ = Decrypt(DynamicConvert.ToSafeString(friendDyMsg.senderuin));
                      if ("1" == isTroop && friend.QQNumber != senderQQ)
                      {
                          // 屏蔽群组消息（底层恢复时，好友聊天消息中可能包含群组消息）
                          return;
                      }

                      string friendQQ = Decrypt(DynamicConvert.ToSafeString(friendDyMsg.frienduin));
                      if (friendQQ != friend.QQNumber)
                      {
                          return;
                      }

                      friendMsg = new MessageCore();
                      if ("0" == DynamicConvert.ToSafeString(friendDyMsg.issend) || friend.QQNumber == senderQQ)
                      {
                          friendMsg.SenderName = friendName;
                          friendMsg.Receiver = myName;
                          friendMsg.SendState = EnumSendState.Receive;
                      }
                      else
                      {
                          friendMsg.SenderName = myName;
                          friendMsg.Receiver = friendName;
                          friendMsg.SendState = EnumSendState.Send;
                      }

                      friendMsg.Content = Decrypt(friendDyMsg.msgData as byte[]);
                      friendMsg.Date = DynamicConvert.ToSafeDateTime(friendDyMsg.time);
                      friendMsg.DataState = DynamicConvert.ToEnumByValue(friendDyMsg.XLY_DataType, EnumDataState.Normal);

                      GetMessageContent(ref friendMsg, friendDyMsg);

                      friendMsgTree.Items.Add(friendMsg);
                  }
              });
        }

        /// <summary>
        /// 构建群组聊天记录
        /// </summary>
        /// <param name="groupMsgTree"></param>
        /// <param name="group"></param>
        private void LoadTroopMsg(TreeNode groupMsgTree, QQGroupShow group)
        {
            var tableName = LsTroopMsgTables.FirstOrDefault(s => s.Contains(CryptographyHelper.MD5FromStringUpper(group.QQNumber)));

            var groupName = group.FullName;
            var myName = CurQQAccount.FullName;

            var sql = string.Format(@"SELECT
                                      	m.senderuin,
                                      	mem.friendnick,
                                      	mem.troopnick,
                                      	m.msgData,
                                      	m.time,
                                      	m.msgtype,
                                      	m.uniseq,
                                      	m.XLY_DataType
                                      FROM
                                      	{0} m
                                      LEFT JOIN TroopMemberInfo mem ON m.frienduin = mem.troopuin
                                      AND m.senderuin = mem.memberuin
                                      ORDER BY m.time", tableName);

            MainDbContext.UsingSafeConnection(sql, (r) =>
             {
                 dynamic dyMsg;
                 MessageCore troopMsg;

                 while (r.Read())
                 {
                     dyMsg = r.ToDynamic();

                     troopMsg = new MessageCore();
                     string senderQQ = Decrypt(DynamicConvert.ToSafeString(dyMsg.senderuin));
                     if (senderQQ == CurQQNumber)
                     {//自己发送的消息
                         troopMsg.SenderName = CurQQAccount.FullName;
                         troopMsg.SendState = EnumSendState.Send;
                         troopMsg.Receiver = groupName;
                     }
                     else
                     {//接收的消息
                         string friendnick = Decrypt(DynamicConvert.ToSafeString(dyMsg.friendnick));
                         string troopnick = Decrypt(DynamicConvert.ToSafeString(dyMsg.troopnick));

                         if (friendnick.IsValid())
                         {
                             troopMsg.SenderName = string.Format("{0}({1})", friendnick, senderQQ);
                         }
                         else if (troopnick.IsValid())
                         {
                             troopMsg.SenderName = string.Format("{0}({1})", troopnick, senderQQ);
                         }
                         else
                         {
                             troopMsg.SenderName = senderQQ;
                         }

                         troopMsg.SendState = EnumSendState.Receive;
                         troopMsg.Receiver = CurQQAccount.FullName;
                     }

                     troopMsg.Content = Decrypt(dyMsg.msgData as byte[]);
                     troopMsg.Date = DynamicConvert.ToSafeDateTime(dyMsg.time);
                     troopMsg.DataState = DynamicConvert.ToEnumByValue(dyMsg.XLY_DataType, EnumDataState.Normal);

                     GetMessageContent(ref troopMsg, dyMsg);

                     groupMsgTree.Items.Add(troopMsg);
                 }
             });
        }

        /// <summary>
        /// 构建讨论组聊天记录
        /// </summary>
        /// <param name="groupMsgTree"></param>
        /// <param name="discuss"></param>
        private void LoadDiscussMsg(TreeNode groupMsgTree, QQDiscussShow discuss)
        {
            var tableName = LsTroopMsgTables.FirstOrDefault(s => s.Contains(CryptographyHelper.MD5FromStringUpper(discuss.QQNumber)));

            var discussName = discuss.FullName;
            var myName = CurQQAccount.FullName;

            var sql = string.Format(@"SELECT
                                      	m.senderuin,
                                      	mem.inteRemark,
                                      	m.msgData,
                                      	m.time,
                                      	m.msgtype,
                                      	m.uniseq,
                                      	m.XLY_DataType
                                      FROM
                                      	{0} m
                                      LEFT JOIN DiscussionMemberInfo mem ON m.frienduin = mem.discussionUin
                                      AND m.senderuin = mem.memberuin
                                      ORDER BY
                                      	m.time", tableName);

            MainDbContext.UsingSafeConnection(sql, (r) =>
            {
                dynamic dyMsg;
                MessageCore discussMsg;

                while (r.Read())
                {
                    dyMsg = r.ToDynamic();

                    discussMsg = new MessageCore();

                    string senderQQ = Decrypt(DynamicConvert.ToSafeString(dyMsg.senderuin));
                    if (senderQQ == CurQQNumber)
                    {//自己发送的消息
                        discussMsg.SenderName = CurQQAccount.FullName;
                        discussMsg.SendState = EnumSendState.Send;
                        discussMsg.Receiver = discussName;
                    }
                    else
                    {//接收的消息
                        string inteRemark = Decrypt(DynamicConvert.ToSafeString(dyMsg.inteRemark));

                        if (inteRemark.IsValid())
                        {
                            discussMsg.SenderName = string.Format("{0}({1})", inteRemark, senderQQ);
                        }
                        else
                        {
                            discussMsg.SenderName = senderQQ;
                        }

                        discussMsg.SendState = EnumSendState.Receive;
                        discussMsg.Receiver = CurQQAccount.FullName;
                    }

                    discussMsg.Content = Decrypt(dyMsg.msgData as byte[]);
                    discussMsg.Date = DynamicConvert.ToSafeDateTime(dyMsg.time);
                    discussMsg.DataState = DynamicConvert.ToEnumByValue(dyMsg.XLY_DataType, EnumDataState.Normal);

                    GetMessageContent(ref discussMsg, dyMsg);

                    groupMsgTree.Items.Add(discussMsg);
                }
            });
        }

        private readonly static Regex RegexMediaFile = new Regex("(?<=[$`Z%VOLd\\*])[\\s\\S]+(?=)", RegexOptions.Compiled);
        private readonly static Regex RegexVideo = new Regex(@"Tencent/MobileQQ/shortvideo/\S+.mp4", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private readonly static Regex RegexImageMd5 = new Regex(@" ([0-9A-Z]{32})", RegexOptions.Compiled);

        /// <summary>
        /// 解析消息类型和消息内容
        /// </summary>
        /// <param name="friendMsg"></param>
        /// <param name="friendDyMsg"></param>
        private void GetMessageContent(ref MessageCore friendMsg, dynamic friendDyMsg)
        {
            friendMsg.Type = GetEnumColumnType(DynamicConvert.ToSafeString(friendDyMsg.msgtype));

            var sourcePath = string.Empty;

            switch (friendMsg.Type)
            {
                case EnumColumnType.Image:
                    friendMsg.MessageType = "图片";

                    #region 图片处理

                    sourcePath = RegexMediaFile.Match(friendMsg.Content).Value;
                    var image = sourcePath.Split('/').Last();
                    sourcePath = Path.Combine(MediaFileRootPath, "photo", image);

                    if (sourcePath.IsInvalid() || !File.Exists(sourcePath))
                    {
                        var md5 = RegexImageMd5.Match(friendMsg.Content).Groups[1].Value;
                        sourcePath = GetImageFromMd5(md5);
                    }

                    if (sourcePath.IsValid() && File.Exists(sourcePath))
                    {
                        friendMsg.Content = sourcePath;
                    }

                    #endregion

                    break;
                case EnumColumnType.Audio:
                    friendMsg.MessageType = "语音";

                    #region 语音处理

                    var resultAudio = Regex.Match(friendMsg.Content, "(?<=ptt/).*?(amr|slk)");
                    if (!resultAudio.Success)
                    {
                        return;
                    }

                    var audioFileName = resultAudio.Value;
                    sourcePath = Path.Combine(MediaFileRootPath, CurQQNumber, "ptt", audioFileName);
                    if (!File.Exists(sourcePath) && !audioFileName.EndsWith(".slk"))
                    {
                        sourcePath = Path.Combine(MediaFileRootPath, CurQQNumber, "ptt",
                                      string.Format("{0}.slk", audioFileName.TrimEnd(FileHelper.GetExtension(audioFileName))));
                    }

                    if (File.Exists(sourcePath))
                    {
                        friendMsg.Content = AudioDecodeHelper.Decode(sourcePath);
                    }

                    #endregion

                    break;
                case EnumColumnType.Video:
                    friendMsg.MessageType = "视频";

                    #region 视频处理

                    var videoFile = RegexVideo.Match(friendMsg.Content).Value.TrimStart("/");
                    if (videoFile.IsValid())
                    {
                        sourcePath = Path.Combine(MediaFileRootPath, videoFile.Replace("Tencent", "tencent").Replace('/', '\\'));
                        if (File.Exists(sourcePath))
                        {
                            friendMsg.Content = sourcePath;
                        }
                    }

                    #endregion

                    break;
                case EnumColumnType.File:
                    friendMsg.MessageType = "文件";

                    #region 文件处理

                    var fileMangerInfo = LsFileManager.FirstOrDefault(c => c.uniseq == friendDyMsg.uniseq);
                    if (fileMangerInfo != null)
                    {
                        sourcePath = Decrypt(fileMangerInfo.strFilePath);
                        if (sourcePath.IsInvalid())
                        {
                            sourcePath = Decrypt(fileMangerInfo.strThumbPath);
                        }
                        if (sourcePath.IsValid())
                        {
                            var res = sourcePath.Split(new string[] { "/0/" }, StringSplitOptions.RemoveEmptyEntries);
                            if (res.IsValid() && res.Length >= 2)
                            {
                                sourcePath = Path.Combine(MediaFileRootPath, res[1].Replace("Tencent", "tencent").Replace('/', '\\'));
                                if (File.Exists(sourcePath))
                                {
                                    friendMsg.Content = sourcePath;
                                }
                            }
                        }

                        switch (DynamicConvert.ToSafeInt(fileMangerInfo.nFileType))
                        {
                            case 0:
                                friendMsg.Type = EnumColumnType.Image;
                                friendMsg.MessageType = "图片";
                                break;
                            case 1:
                                friendMsg.Type = EnumColumnType.Audio;
                                friendMsg.MessageType = "文件";
                                break;
                            case 2:
                                friendMsg.Type = EnumColumnType.Video;
                                friendMsg.MessageType = "视频";
                                break;
                        }
                    }

                    #endregion

                    break;
                case EnumColumnType.String:
                    friendMsg.MessageType = "文本";

                    break;
            }
        }

        #region 辅助方法

        /// <summary>
        /// 获取QQ数据库文件列表
        /// 一个QQ数据库文件保存了1个QQ的数据，文件名为QQ号，后缀名为.db
        /// </summary>
        /// <returns></returns>
        private IEnumerable<FileInfo> GetAllQQAccountFileInfos()
        {
            var dbPath = Path.Combine(DataFileRootPath, "databases");

            if (Directory.Exists(dbPath))
            {
                var di = new DirectoryInfo(dbPath);

                return di.GetFiles("*.db").Where(f => Regex.IsMatch(f.Name, "^[0-9]+.db"));
            }
            else
            {
                return new List<FileInfo>();
            }
        }

        /// <summary>
        /// 获取解密key
        /// </summary>
        /// <returns></returns>
        private string GetDecryptKey(IEnumerable<FileInfo> qqFiles)
        {
            var dbPath = Path.Combine(DataFileRootPath, "databases");

            var list = GetAllDecryptKeys();
            if (list.IsInvalid())
            {
                return string.Empty;
            }

            if (1 == list.Count || qqFiles.IsInvalid())
            {
                return list[0];
            }

            try
            {
                dynamic data = null;
                foreach (var qqfile in qqFiles)
                {
                    var content = new SqliteContext(qqfile.FullName);
                    var datas = content.Find("SELECT * FROM Friends limit 1");

                    content.Dispose();
                    if (datas.IsValid())
                    {
                        data = datas.First();
                        break;
                    }
                }

                if (null == data)
                {
                    return list[0];
                }

                foreach (var key in list)
                {
                    string qqNumber = DecryptHelper.DecryptAndroidQQMsg(DynamicConvert.ToSafeString(data.uin), key).Trim();
                    qqNumber = qqNumber.TrimEnd('\0');
                    if (Regex.IsMatch(qqNumber, @"^\d+$"))
                    {
                        return key;
                    }
                }
            }
            catch { }

            return list[0];
        }

        /// <summary>
        /// 获取所有配置文件中的解密key
        /// </summary>
        /// <returns></returns>
        private List<string> GetAllDecryptKeys()
        {
            var configPath = Path.Combine(DataFileRootPath, "shared_prefs");

            List<string> list = new List<string>();

            if (!Directory.Exists(configPath))
            {
                return list;
            }

            //从配置文件(mobileQQ.xml)中获取Key
            try
            {
                string mobileQQFile = Path.Combine(configPath, "mobileQQ.xml");
                if (File.Exists(mobileQQFile))
                {
                    var doc = new XmlDocument();
                    doc.Load(mobileQQFile);
                    XmlNodeList userInfoNodeList = doc.SelectNodes("map//string[@name='security_key']");
                    if (userInfoNodeList != null && userInfoNodeList.Count > 0)
                    {
                        string key = userInfoNodeList[0].InnerText;
                        if (key.IsValid())
                        {
                            list.Add(key);
                        }
                    }
                }
            }
            catch { }

            //从配置文件（appcenter_mobileinfo.xml）获取Key
            try
            {
                string appcenterMobileQQFile = Path.Combine(configPath, "appcenter_mobileinfo.xml");
                if (File.Exists(appcenterMobileQQFile))
                {
                    var doc = new XmlDocument();
                    doc.Load(appcenterMobileQQFile);
                    XmlNodeList userInfoNodeList = doc.SelectNodes("map//string[@name='imei']");
                    if (userInfoNodeList != null && userInfoNodeList.Count > 0)
                    {
                        string key = userInfoNodeList[0].InnerText;
                        if (key.IsValid())
                        {
                            list.Add(key);
                        }
                    }

                    XmlNodeList userInfoNodeList1 = doc.SelectNodes("map//string[@name='imsi']");
                    if (userInfoNodeList1 != null && userInfoNodeList1.Count > 0)
                    {
                        string key = userInfoNodeList[0].InnerText;
                        if (key.IsValid())
                        {
                            list.Add(key);
                        }
                    }

                    XmlNodeList userInfoNodeList2 = doc.SelectNodes("map//string[@name='wifi_mac_address']");
                    if (userInfoNodeList2 != null && userInfoNodeList2.Count > 0)
                    {
                        string key = userInfoNodeList2[0].InnerText;
                        if (key.IsValid())
                        {
                            list.Add(key);
                        }
                    }
                }
            }
            catch { }

            // 从配置文件（DENGTA_META.xml）获取Key
            try
            {
                string dengtaMobileQQFile = Path.Combine(configPath, "DENGTA_META.xml");
                if (File.Exists(dengtaMobileQQFile))
                {
                    var doc = new XmlDocument();
                    doc.Load(dengtaMobileQQFile);
                    XmlNodeList userInfoNodeList = doc.SelectNodes("map//string[@name='IMEI_DENGTA']");
                    if (userInfoNodeList != null && userInfoNodeList.Count > 0)
                    {
                        string key = userInfoNodeList[0].InnerText;
                        if (key.IsValid())
                        {
                            list.Add(key);
                        }
                    }

                    XmlNodeList imeiNodeList = doc.SelectNodes("map//string[@name='f_non_empty_qimei_v2']");
                    if (imeiNodeList != null && imeiNodeList.Count > 0)
                    {
                        string key = imeiNodeList[0].InnerText;
                        var arr = key.Split(new string[] { "|||" }, StringSplitOptions.RemoveEmptyEntries);
                        if (arr.IsValid())
                        {
                            list.Add(arr[0].ToUpper());
                        }
                    }
                }
            }
            catch { }

            // 从配置文件（contact_bind_infoQQ号码.xml）获取Key
            try
            {
                var contact_bind_infoPaths = new DirectoryInfo(configPath).GetFiles().Where(f => Regex.IsMatch(f.Name, @"contact_bind_info\d+\.xml"));
                if (contact_bind_infoPaths.IsValid())
                {
                    var filePath = contact_bind_infoPaths.First();
                    var doc = new XmlDocument();
                    doc.Load(filePath.FullName);
                    XmlNodeList userInfoNodeList = doc.SelectNodes("map//string[@name='contact_bind_info_unique']");
                    if (userInfoNodeList != null && userInfoNodeList.Count > 0)
                    {
                        string key = userInfoNodeList[0].InnerText.Split('|')[0];
                        if (key.IsValid())
                        {
                            list.Add(key);
                        }
                    }
                }
            }
            catch { }

            return list.Distinct().ToList();
        }

        /// <summary>
        /// 获取要恢复的数据库表 表名以,分隔
        /// </summary>
        /// <param name="oldDbPath"></param>
        /// <returns></returns>
        private string GetRecoverTables(string oldDbPath)
        {
            LsFriendMsgTables = new List<string>();
            LsTroopMsgTables = new List<string>();
            LsDiscussMsgTables = new List<string>();

            var tablesBuilder = new StringBuilder();
            string[] baseTables = { "Card", "Friends", "ExtensionInfo", "Groups", "TroopInfo", "TroopMemberInfo", "DiscussionInfo", "DiscussionMemberInfo", "mr_fileManager", "recent" };

            List<string> dbAllTables = SqliteRecoveryHelper.ButtomGetAllTables(oldDbPath);
            foreach (var baseTable in baseTables)
            {
                if (dbAllTables.Contains(baseTable))
                {
                    tablesBuilder.Append(baseTable + ",");
                }
            }

            foreach (var tableName in dbAllTables)
            {
                if (tableName.StartsWith("mr_friend_"))
                {
                    LsFriendMsgTables.Add(tableName);
                    tablesBuilder.Append(tableName + ",");
                }
                else if (tableName.StartsWith("mr_troop_"))
                {
                    LsTroopMsgTables.Add(tableName);
                    tablesBuilder.Append(tableName + ",");
                }
                else if (tableName.StartsWith("mr_discusssion_"))
                {
                    LsDiscussMsgTables.Add(tableName);
                    tablesBuilder.Append(tableName + ",");
                }
            }

            return tablesBuilder.ToString().TrimEnd(",");
        }

        /// <summary>
        /// 获取用于会话模式的数据类型
        /// </summary>
        /// <param name="type">消息类型</param>
        /// <returns>返回消息数据类型</returns>
        private EnumColumnType GetEnumColumnType(string type)
        {
            switch (type)
            {
                case "-1000":
                    return EnumColumnType.String;
                case "-2000":
                    return EnumColumnType.Image;
                case "-2022":
                    return EnumColumnType.Video;
                case "-2002":
                    return EnumColumnType.Audio;
                case "-2005":
                case "-3008":
                    return EnumColumnType.File;
                default:
                    return EnumColumnType.String;
            }
        }

        /// <summary>
        /// 解密字符串
        /// </summary>
        /// <param name="enText">源字符串</param>
        /// <returns>解密后字符串</returns>
        private string Decrypt(string enText)
        {
            return DecryptHelper.DecryptAndroidQQMsg(enText, DecryptKey, Encoding.Unicode).TrimEnd("\0");
        }

        /// <summary>
        /// 解密byte数组
        /// </summary>
        /// <param name="enBuffer">源数据</param>
        /// <returns>解密后字符串</returns>
        private string Decrypt(byte[] enBuffer)
        {
            return DecryptHelper.DecryptAndroidQQMsg(enBuffer, DecryptKey).TrimEnd("\0");
        }

        /// <summary>
        /// 获取聊天记录缓存图片
        /// </summary>
        /// <param name="md5"></param>
        /// <returns></returns>
        private string GetImageFromMd5(string md5)
        {
            if (null == DicMd5Image)
            {
                DicMd5Image = new Dictionary<string, string>();

                var local = Path.Combine(MediaFileRootPath, "diskcache");
                if (Directory.Exists(local))
                {
                    foreach (var file in Directory.GetFiles(local))
                    {
                        DicMd5Image.Add(CryptographyHelper.MD5FromFileUpper(file), file);
                    }
                }
            }

            if (DicMd5Image.IsValid() && DicMd5Image.Keys.Contains(md5))
            {
                return DicMd5Image[md5];
            }

            return string.Empty;
        }

        #endregion

        #endregion

    }
}
