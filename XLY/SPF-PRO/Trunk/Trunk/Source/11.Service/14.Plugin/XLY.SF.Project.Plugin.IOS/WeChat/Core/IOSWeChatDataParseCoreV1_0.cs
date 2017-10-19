/***************************************************************************
 * 
 * author :  Songbing
 * time: 2017/10/17 10:25:46 
 * explain :  
 *
*****************************************************************************/

using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using XLY.SF.Framework.BaseUtility;
using XLY.SF.Project.BaseUtility.Helper;
using XLY.SF.Project.Domains;
using XLY.SF.Project.Persistable.Primitive;

namespace XLY.SF.Project.Plugin.IOS
{
    /// <summary>
    /// IOS微信数据解析
    /// </summary>
    public class IOSWeChatDataParseCoreV1_0
    {
        /// <summary>
        /// IOS微信数据解析核心类
        /// </summary>
        /// <param name="savedatadbpath">数据保存数据库路径</param>
        /// <param name="name">微信 例如 微信、小米微信分身</param>
        /// <param name="datapath">微信数据文件根目录，例如 I:\本地数据\com.tencent.xin</param>
        public IOSWeChatDataParseCoreV1_0(string savedatadbpath, string name, string datapath)
        {
            DbFilePath = savedatadbpath;
            WeChatName = name;
            DataFileRootPath = datapath;
        }

        #region 其他属性

        private const string _ftsMsgTables = "fts_message_table_0_content,fts_message_table_1_content,fts_message_table_2_content,fts_message_table_3_content,fts_message_table_4_content,fts_message_table_5_content,fts_message_table_6_content,fts_message_table_7_content,fts_message_table_8_content,fts_message_table_9_content";
        private const string _ftsMsgNewTable = "fts_message_content";

        #endregion

        #region 构造属性

        /// <summary>
        /// 数据库路径
        /// </summary>
        private string DbFilePath { get; set; }

        /// <summary>
        /// 微信名称，例如 微信、粉色微信
        /// </summary>
        private string WeChatName { get; set; }

        /// <summary>
        ///微信数据文件根目录，例如 I:\本地数据\com.tencent.xin
        ///该文件下包含了Documents子文件夹，保存了微信相关数据和文件
        /// </summary>
        private string DataFileRootPath { get; set; }

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
        /// 主数据库 MM.sqlite
        /// </summary>
        private SqliteContext MainContext { get; set; }

        /// <summary>
        /// fts_message.db 用于删除数据提取
        /// </summary>
        private SqliteContext FtsContext { get; set; }

        /// <summary>
        /// 好友列表
        /// </summary>
        private List<WeChatFriendShow> LsAllFriends { get; set; }

        /// <summary>
        /// 群组好友
        /// </summary>
        private List<WeChatGroupShow> LsAllGroupFriends { get; set; }

        /// <summary>
        /// 清除临时属性
        /// 一般用于插件执行完毕后执行
        /// </summary>
        private void ClearCache()
        {
            MD5AccountPath = string.Empty;
            WeChatAccount = null;
            WeChatAccountShowName = string.Empty;

            MainContext?.Dispose();
            MainContext = null;

            FtsContext?.Dispose();
            FtsContext = null;

            LsAllFriends?.Clear();
            LsAllFriends = null;

            LsAllGroupFriends?.Clear();
            LsAllGroupFriends = null;

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

                var accountPaths = LoadSourcePath();
                foreach (var path in accountPaths)
                {
                    ClearCache();

                    BuildWeChatTree(rootNode, path);
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
        /// 构建微信帐号树
        /// </summary>
        /// <param name="rootNode"></param>
        /// <param name="sourcePath">微信帐号文件夹 例如 D:\com.tencent.xin\Documents\9cb0637d719e7caa3c5b90a27427c097</param>
        private void BuildWeChatTree(TreeNode rootNode, string sourcePath)
        {
            MD5AccountPath = sourcePath;

            //构建帐号信息
            var accountNode = BuildAccountInfo(rootNode);

            //数据库恢复
            GetSqliteContext();

            //获取通讯录列表
            LoadAllContacts();

            //构建好友和公众号
            BuildContactsNode(accountNode);

            //构建通讯录消息
            BuildContactsMsgNode(accountNode);

            //构建群聊
            BuildGroupNode(accountNode);

            //构建群聊消息
            BuildGroupMsgNode(accountNode);

            //构建群发消息树
            BuildMassendMsgNode(accountNode);

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

            foreach (var friend in LsAllFriends)
            {
                if (friend.FriendType == WeChatFriendTypeEnum.Subscription)
                {
                    gongzhonghaoNode.Items.Add(friend);
                }
                else
                {
                    friendNode.Items.Add(friend);
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
        }


        #endregion

        #region 辅助方法

        /// <summary>
        ///  加载微信用户数据源路径
        /// </summary>
        private List<string> LoadSourcePath()
        {
            var sourcePath = Path.Combine(DataFileRootPath, "Documents");

            var sourcepaths = new List<string>();

            var dirInfo = new DirectoryInfo(sourcePath);
            if (dirInfo.Exists)
            {
                var files = dirInfo.GetFiles("mmsetting.archive", SearchOption.AllDirectories);
                foreach (var file in files)
                {
                    if (!file.Directory.Name.IsMatch("^0+$"))
                    {
                        sourcepaths.Add(file.Directory.ToString());
                    }
                }
            }

            return sourcepaths;
        }

        /// <summary>
        /// 获取微信帐号信息
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        private WeChatLoginShow GetAccountUser()
        {
            var show = new WeChatLoginShow();

            var sourcePath = Path.Combine(MD5AccountPath, "mmsetting.archive");
            var data = PListToJsonHelper.PlistToJson(sourcePath) as JObject;

            show.WeChatId = data["UsrName"].ToSafeString();
            show.WeChatAccout = data["AliasName"].ToSafeString();
            show.Nick = data["NickName"].ToSafeString();
            show.Signature = data["Signature"].ToSafeString() == "$null" ? "" : data["Signature"].ToSafeString();
            show.BindingQQ = data["PushmailFolderUrl"].ToSafeString() == "$null" ? "" : data["PushmailFolderUrl"].ToSafeString();
            if (Regex.IsMatch(show.BindingQQ, @"uin=(\d+)"))
            {
                show.BindingQQ = Regex.Match(show.BindingQQ, @"uin=(\d+)").Groups[1].Value;
                show.BindingQQ = show.BindingQQ == "0" ? "" : show.BindingQQ;
            }
            else
            {
                show.BindingQQ = string.Empty;
            }
            show.BindingEmail = data["Email"].ToSafeString() == "$null" ? "" : data["Email"].ToSafeString();
            show.BindingPhone = data["Mobile"].ToSafeString() == "$null" ? "" : data["Mobile"].ToSafeString();
            show.BindingWeiBo = data["WeiboAddress"].ToSafeString() == "$null" ? "" : data["WeiboAddress"].ToSafeString();
            show.WeiBoNick = data["WeiboNickName"].ToSafeString() == "$null" ? "" : data["WeiboNickName"].ToSafeString();
            show.Address = data["City"].ToSafeString() == "$null" ? "" : data["City"].ToSafeString();

            if (null != data["new_dicsetting"])
            {
                show.HeadPng = data["new_dicsetting"]["headhdimgurl"].ToSafeString() == "$null" ? "" : data["new_dicsetting"]["headhdimgurl"].ToSafeString();
            }

            //show.WeiBoBackImg = data["m_pcWCBGImgID"].ToSafeString() == "$null" ? "" : data["m_pcWCBGImgID"].ToSafeString();
            //data["m_pcWCBGImgID"].ToSafeString() == "$null" ? "" : data["m_pcWCBGImgID"].ToSafeString(); //背景图

            if ("1" == data["Sex"].ToSafeString())
            {
                show.Gender = EnumSex.Male;
            }
            else
            {
                show.Gender = EnumSex.Female;
            }

            return show;
        }

        /// <summary>
        /// 构建动态恢复的表
        /// </summary>
        /// <param name="sourcedb"></param>
        /// <returns></returns>
        private string RecoveryTables(string sourcedb)
        {
            var sqliteObj = new SqliteContext(sourcedb);

            var listTables = sqliteObj.Find(new SQLiteString("select tbl_name from sqlite_master where type = 'table'")).
                Select(table => DynamicConvert.ToSafeString(table.tbl_name)).Cast<string>().Where(t => t.StartsWith("Chat_")).ToList();

            listTables.Add("Friend");
            listTables.Add("Friend_Ext");

            return string.Join(",", listTables);
        }

        /// <summary>
        /// 获取数据库
        /// </summary>
        private void GetSqliteContext()
        {
            string sourceDbPath = Path.Combine(MD5AccountPath, "DB", "MM.sqlite");
            var recoveryDb = SqliteRecoveryHelper.DataRecovery(sourceDbPath, @"chalib\IOS_TencentWeiXin_V5.3\MM.sqlite.charactor", RecoveryTables(sourceDbPath), true);
            MainContext = new SqliteContext(recoveryDb);

            string sourceFtsPath = Path.Combine(MD5AccountPath, "fts", "fts_message.db");
            if (File.Exists(sourceFtsPath))
            {
                try
                {
                    Dictionary<string, string> dicIndex = new Dictionary<string, string>();
                    dicIndex.Add("fts_username_id", "UsrName");

                    FtsContext = new SqliteContext(SqliteRecoveryHelper.DataRecovery(
                                                sourceFtsPath, @"chalib\IOS_TencentWeiXin_V5.3\fts_message.db.charactor", _ftsMsgTables + ",fts_username_id", "", "", true, dicIndex));
                    FtsContext.MergeTables(_ftsMsgTables, _ftsMsgNewTable, "IndexNameId", "c0usernameid");
                }
                catch
                {
                    FtsContext = null;
                }
            }
        }

        /// <summary>
        /// 获取通讯录  包括好友和群聊
        /// </summary>
        private void LoadAllContacts()
        {
            LsAllFriends = new List<WeChatFriendShow>();
            LsAllGroupFriends = new List<WeChatGroupShow>();

            #region 从MM.sqlite的Friend表获取好友信息

            var sql = @"SELECT
                            f.*, 
                            e.*
                        FROM
                            Friend f
                        LEFT JOIN Friend_Ext e ON f.UsrName = e.UsrName";
            MainContext.UsingSafeConnection(sql, r =>
            {
                dynamic data;
                WeChatFriendShow friendInfo;

                while (r.Read())
                {
                    data = r.ToDynamic();

                    if (DynamicConvert.ToSafeString(data.UsrName).Contains("@chatroom"))
                    {
                        LsAllGroupFriends.Add(CreateWeChatGroupShow(data));
                    }
                    else
                    {
                        friendInfo = new WeChatFriendShow();
                        CreateWeChatFriendShow(data, ref friendInfo);

                        LsAllFriends.Add(friendInfo);
                    }
                }
            });

            #endregion

            #region 从WCDB_Contact.sqlite的表Friend获取好友信息

            string WCDBPath = Path.Combine(MD5AccountPath, "DB", "WCDB_Contact.sqlite");
            if (FileHelper.IsValid(WCDBPath))
            {
                List<char> listC = new List<char>();
                for (int i = 0; i < 32; i++)
                {
                    listC.Add((char)i);
                }
                listC.Add((char)127);
                var arrC = listC.ToArray();//分割字符

                using (var WCDB = new SqliteContext(WCDBPath))
                {
                    WCDB.UsingSafeConnection("SELECT * FROM Friend WHERE imgStatus != 1", r =>
                    {//imgStatus为1的记录为系统功能
                        dynamic fd;
                        DynamicEx dy;
                        WeChatFriendShow friendInfo;

                        while (r.Read())
                        {
                            fd = r.ToDynamic();

                            dy = new DynamicEx();
                            dy.Set("UsrName", fd.userName);
                            dy.Set("type", fd.type);
                            dy.Set("certificationFlag", fd.certificationFlag);
                            dy.Set("imgStatus", fd.imgStatus);
                            dy.Set("XLY_DataType", "2");

                            byte[] dbContactRemark = fd.dbContactRemark;
                            if (dbContactRemark.IsValid())
                            {//获取昵称、备注、修改后微信号
                                GetdbContactRemark(dbContactRemark, ref dy);
                            }

                            byte[] dbContactProfile = fd.dbContactProfile;
                            if (dbContactProfile.IsValid())
                            {//获取位置和签名
                                GetdbContactProfile(dbContactProfile, ref dy);
                            }

                            byte[] dbContactChatRoom = fd.dbContactChatRoom;
                            if (dbContactChatRoom.IsValid())
                            {//获取群组成员列表
                                var dArr = System.Text.Encoding.UTF8.GetString(dbContactChatRoom).Split(arrC, StringSplitOptions.RemoveEmptyEntries).ToList();
                                if (dArr.IsValid())
                                {
                                    string ConChatRoomMem = dArr.FirstOrDefault(d => d.Contains(";"));
                                    string ConChatRoomOwner = "";
                                    if (dArr.IndexOf(ConChatRoomMem) + 1 < dArr.Count)
                                    {//群组的创建者在群组成员后面
                                        ConChatRoomOwner = dArr[dArr.IndexOf(ConChatRoomMem) + 1];
                                    }

                                    if (0 == dArr.IndexOf(ConChatRoomMem))
                                    {//如果成员的索引是0，前面会多出一个随机的字符
                                        ConChatRoomMem = ConChatRoomMem.Substring(1);
                                    }

                                    dy.Set("ConChatRoomMem", ConChatRoomMem);
                                    dy.Set("ConChatRoomOwner", ConChatRoomOwner);
                                }
                            }

                            if (DynamicConvert.ToSafeString(fd.userName).Contains("@chatroom"))
                            {
                                LsAllGroupFriends.Add(CreateWeChatGroupShow(dy));
                            }
                            else
                            {
                                friendInfo = new WeChatFriendShow();
                                CreateWeChatFriendShow(dy, ref friendInfo);

                                LsAllFriends.Add(friendInfo);
                            }
                        }
                    });
                }
            }

            #endregion
        }

        /// <summary>
        /// 构建好友信息
        /// </summary>
        /// <param name="data"></param>
        /// <param name="friendInfo"></param>
        private void CreateWeChatFriendShow(dynamic data, ref WeChatFriendShow friendInfo)
        {
            friendInfo.DataState = DynamicConvert.ToEnumByValue<EnumDataState>(data.XLY_DataType, EnumDataState.Normal);
            friendInfo.WeChatId = DynamicConvert.ToSafeString(data.UsrName);
            friendInfo.WeChatAccout = DynamicConvert.ToSafeString(data.Alias);
            friendInfo.Nick = DynamicConvert.ToSafeString(data.NickName);
            friendInfo.Email = DynamicConvert.ToSafeString(data.Email);
            friendInfo.Mobile = DynamicConvert.ToSafeString(data.Mobile);
            friendInfo.Remark = DynamicConvert.ToSafeString(data.ConRemark);
            friendInfo.Signature = DynamicConvert.ToSafeString(data.Signature);

            if (DynamicConvert.ToSafeString(data.Sex) == "1")
            {
                friendInfo.Gender = EnumSex.Male;
            }
            else
            {
                friendInfo.Gender = EnumSex.Female;
            }

            friendInfo.FriendType = GetFriendType(friendInfo.WeChatId,
                DynamicConvert.ToSafeString(data.certificationFlag), DynamicConvert.ToSafeString(data.imgStatus), DynamicConvert.ToSafeString(data.type));

            GetFriendInfo(DynamicConvert.ToSafeString(data.ConStrRes2), ref friendInfo);

            if (friendInfo.Remark.IsInvalid())
            {
                friendInfo.Remark = DynamicConvert.ToSafeString(data.RemarkName);
            }
            if (friendInfo.Address.IsInvalid())
            {
                friendInfo.Address = string.Format("{0}-{1}-{2}", DynamicConvert.ToSafeString(data.Country), DynamicConvert.ToSafeString(data.Province), DynamicConvert.ToSafeString(data.City));
            }

            friendInfo.HeadPng = GetHeadImage(friendInfo.WeChatId);
        }

        /// <summary>
        /// 构建群聊信息
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private WeChatGroupShow CreateWeChatGroupShow(dynamic data)
        {
            var group = new WeChatGroupShow();
            group.DataState = DynamicConvert.ToEnumByValue<EnumDataState>(data.XLY_DataType, EnumDataState.Normal);
            group.WeChatId = data.UsrName;
            group.GroupName = data.NickName;
            group.Member = DynamicConvert.ToSafeString(data.ConChatRoomMem);
            group.MemberNum = group.Member.Split(';').Length;

            string xmlText = DynamicConvert.ToSafeString(data.ConStrRes2);
            if (xmlText.IsValid())
            {
                GetGroupInfo(xmlText, ref group);
            }

            if (group.GroupOwnerUser.IsInvalid())
            {
                group.GroupOwnerUser = DynamicConvert.ToSafeString(data.ConChatRoomOwner);
            }

            group.HeadPng = GetHeadImage(group.WeChatId);

            return group;
        }

        /// <summary>
        /// 获取好友类型
        /// </summary>
        /// <param name="weChatId"></param>
        /// <param name="certificationFlag"></param>
        /// <param name="imgStatus"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        private WeChatFriendTypeEnum GetFriendType(string weChatId, string certificationFlag, string imgStatus, string type)
        {
            if (imgStatus == "1")
            {
                return WeChatFriendTypeEnum.Subscription;
            }

            if (certificationFlag == "24" || weChatId.StartsWith("gh_"))
            {
                return WeChatFriendTypeEnum.Subscription;
            }

            if (type == "4")
            {
                return WeChatFriendTypeEnum.ChatRoom;
            }

            return WeChatFriendTypeEnum.Normal;
        }

        /// <summary>
        /// 获取头像文件地址
        /// </summary>
        /// <param name="wechatID"></param>
        /// <returns></returns>
        private string GetHeadImage(string wechatID)
        {
            var headImageRootPath = Path.Combine(MD5AccountPath, "prcode");

            if (wechatID.IsInvalid() || headImageRootPath.IsInvalid() || !Directory.Exists(headImageRootPath))
            {
                return string.Empty;
            }

            var md5 = CryptographyHelper.MD5FromString(wechatID);
            var path = Path.Combine(headImageRootPath, string.Format("{0}.jpg", md5));

            if (File.Exists(path))
            {
                return path;
            }

            return "";
        }

        private void GetFriendInfo(string xmlText, ref WeChatFriendShow friendInfo)
        {
            if (xmlText.IsInvalid())
            {
                return;
            }
            var xml = new XmlDocument();
            try
            {
                xml.LoadXml("<Root>" + xmlText + "</Root>");
            }
            catch
            {
                return;
            }
            friendInfo.HeadUrl = GetXmlNodeValueByKey(xml, "HeadImgHDUrl");
            //friendInfo.WeiBoAddr = string.Format("{0}({1})", GetXmlNodeValueByKey(xml, "weibonickname"), GetXmlNodeValueByKey(xml, "weiboaddress"));
            friendInfo.Address = string.Format("{0}-{1}-{2}", GetXmlNodeValueByKey(xml, "country"), GetXmlNodeValueByKey(xml, "province"), GetXmlNodeValueByKey(xml, "city"));
            //friendInfo.QQ = GetXmlNodeValueByKey(xml, "qquin");
            //friendInfo.QQNick = GetXmlNodeValueByKey(xml, "qqnickname");
        }

        private void GetGroupInfo(string xmlText, ref WeChatGroupShow groupInfo)
        {
            var xml = new XmlDocument();
            try
            {
                xml.LoadXml("<Root>" + xmlText + "</Root>");
            }
            catch
            {
                return;
            }
            groupInfo.GroupOwnerUser = GetXmlNodeValueByKey(xml, "owner");
        }

        private string GetXmlNodeValueByKey(XmlNode xml, string xpath)
        {
            try
            {
                var node = xml.SelectSingleNode("/Root/" + xpath);
                return node != null ? node.InnerText : "";
            }
            catch
            {
                return string.Empty;
            }
        }

        private void GetdbContactRemark(byte[] data, ref DynamicEx friend)
        {
            try
            {
                int index = 0;

                if (data[index++] != 0x0A)
                {
                    return;
                }

                int length = data[index++];
                if (0 != length)
                {//昵称NickName
                    var res = System.Text.Encoding.UTF8.GetString(data, index, length);
                    friend.Set("NickName", res);
                }
                index += length;

                if (index >= data.Length || data[index++] != 0x12)
                {
                    return;
                }

                length = data[index++];
                if (0 != length)
                {//修改后微信号Alias
                    var res = System.Text.Encoding.UTF8.GetString(data, index, length);
                    friend.Set("Alias", res);
                }
                index += length;

                if (index >= data.Length || data[index++] != 0x1A)
                {
                    return;
                }

                length = data[index++];
                if (0 != length)
                {//备注RemarkName
                    var res = System.Text.Encoding.UTF8.GetString(data, index, length);
                    friend.Set("RemarkName", res);
                }
                index += length;
            }
            catch
            {
            }
        }

        private void GetdbContactProfile(byte[] data, ref DynamicEx friend)
        {
            try
            {
                int index = 0;
                while (index < data.Length && data[index] != 0x12)
                {
                    index++;
                }

                if (index >= data.Length || data[index++] != 0x12)
                {
                    return;
                }

                int length = data[index++];
                if (0 != length)
                {//国家Country
                    var res = System.Text.Encoding.UTF8.GetString(data, index, length);
                    friend.Set("Country", res);
                }
                index += length;

                if (index >= data.Length || data[index++] != 0x1a)
                {
                    return;
                }

                length = data[index++];
                if (0 != length)
                {//省Province
                    var res = System.Text.Encoding.UTF8.GetString(data, index, length);
                    friend.Set("Province", res);
                }
                index += length;

                if (index >= data.Length || data[index++] != 0x22)
                {
                    return;
                }

                length = data[index++];
                if (0 != length)
                {//市City
                    var res = System.Text.Encoding.UTF8.GetString(data, index, length);
                    friend.Set("City", res);
                }
                index += length;

                if (index >= data.Length || data[index++] != 0x2a)
                {
                    return;
                }

                length = data[index++];
                if (0 != length)
                {//个性签名Signature
                    var res = System.Text.Encoding.UTF8.GetString(data, index, length);
                    friend.Set("Signature", res);
                }
                index += length;
            }
            catch
            {
            }
        }



        #endregion

    }
}
