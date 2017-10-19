using PortableDeviceApiLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using XLY.SF.Framework.BaseUtility;
using XLY.SF.Framework.Core.Base.CoreInterface;
using XLY.SF.Project.Domains;

namespace XLY.SF.Project.Devices.DeviceManager.Mtp
{
    public class MtpDeviceManager
    {
        private static readonly Regex VidRegex = new Regex(@"vid_([a-z0-9]{4})");
        private static readonly Regex PidRegex = new Regex(@"pid_([a-z0-9]{4})");

        public static readonly MtpDeviceManager Instance = new MtpDeviceManager();

        private MtpDeviceManager()
        {
        }

        /// <summary>
        /// 获取MTP设备
        /// </summary>
        /// <param name="device"></param>
        /// <returns></returns>
        public MTPDevice GetMTPDevice(Device device)
        {
            var devices = GetDevices();
            if (devices.IsValid())
            {
                //优先使用序列号匹配
                var dd = devices.FirstOrDefault((d) => d.SerialNumber == device.SerialNumber);

                if (null == dd)
                {
                    #region 其他匹配方式，根据USB设备号

                    string left = device.SerialNumber.ToLower();
                    //先比较vid和pid
                    var vid = VidRegex.Match(left).Groups[1].Value;
                    var pid = PidRegex.Match(left).Groups[1].Value;

                    var allFind = devices.FindAll((d) =>
                    {
                        string right = d.DeviceId.ToLower();
                        var resVid = VidRegex.Match(right);
                        var resPid = PidRegex.Match(right);
                        return resVid.Success && resVid.Groups[1].Value == vid &&
                               resPid.Success && resPid.Groups[1].Value == pid;
                    });
                    if (allFind.IsValid())
                    {
                        if (1 == allFind.Count)
                        {//vid和pid相同的设备只有一个，直接返回
                            dd = allFind[0];
                        }
                        else
                        {//vid和pid相同的设备有多个，再额外匹配其他信息
                            var keyword = device.SerialNumber.ToLower().Split('#')[1].Split('&')[1];
                            var allFindOther = allFind.FindAll((lf) =>
                            {
                                try
                                {
                                    return keyword == lf.DeviceId.ToLower().Split('#')[2].Split('&')[1];
                                }
                                catch
                                {
                                    return false;
                                }
                            });
                            if (allFindOther.IsValid() && 1 == allFindOther.Count)
                            {
                                dd = allFindOther[0];
                            }
                        }
                    }

                    #endregion
                }

                return dd;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 从设备复制文件到本地
        /// </summary>
        /// <param name="device"></param>
        /// <param name="file"></param>
        /// <param name="dstPath"></param>
        public bool CopyFileFromDevice(MTPDevice device, MTPFileNode file, string dstPath)
        {
            return TransferContentFromDevice(Path.Combine(dstPath, file.Name), device.DeviceId, file.Id) == string.Empty;
        }

        public void GetDate(MTPDevice device, MTPFileNode fileNode)
        {
            IPortableDeviceContent content = GetDeviceContent(device.DeviceId);

            IPortableDeviceProperties properties;
            content.Properties(out properties);

            IPortableDeviceValues objectValues;
            properties.GetValues(fileNode.Id, null, out objectValues);

            //暂时只获取文件的创建、修改、最后访问时间
            //由于不用手机设备对MTP协议的支持程度不同，导致不同设备可能获取到的多少不一致
            fileNode.DateCreated = GetFileDateCreatedProperty(objectValues);
            fileNode.DateModified = GetFileDateModifiedProperty(objectValues);
            fileNode.DateAuthored = GetFileDateAuthoredProperty(objectValues);
        }

        public MTPFileNode GetRootFileNode(MTPDevice device, IAsyncProgress asyn)
        {
            MTPFileNode root = new MTPFileNode() { Type = MTPFileNodeType.Root, Name = "Root", Childrens = new List<MTPFileNode>(), Level = -1 };

            IPortableDeviceContent content = GetDeviceContent(device.DeviceId);

            IPortableDeviceProperties properties;
            content.Properties(out properties);

            IPortableDeviceValues deviceValues;
            properties.GetValues("DEVICE", null, out deviceValues);

            List<string> storagesId = GetChildrenObjectIds(content, "DEVICE"); //获取存储卡设备

            //asyn.Advance(0, string.Format("获取到{0}个存储设备", storagesId.Count));

            foreach (string storageId in storagesId)
            {
                MTPFileNode deviceNode = new MTPFileNode()
                {
                    Type = MTPFileNodeType.Device,
                    Name = GetNameById(storageId, properties),
                    Id = storageId,
                    Childrens = new List<MTPFileNode>(),
                    Parent = root,
                    //Level = 0
                };
                root.Childrens.Add(deviceNode);
            }

            foreach (var parentNode in root.Childrens)
            {
                //asyn.Advance(0, string.Format(LanguageHelper.Get("LANGKEY_KaiShiHuoQuDeWenJianXiTong_00553"), parentNode.Name));

                List<string> objectsId = GetChildrenObjectIds(content, parentNode.Id);
                if (objectsId != null && objectsId.Count > 0)
                {
                    foreach (string objectId in objectsId)
                    {
                        IPortableDeviceValues objectValues;
                        properties.GetValues(objectId, null, out objectValues);
                        MTPFileNode fileNode = new MTPFileNode()
                        {
                            Type = GetFileTypeProperty(objectValues),
                            Name = GetFullNameProperty(objectValues),
                            //FileSize = GetFileSizeProperty(objectValues),
                            Id = objectId,
                            Childrens = new List<MTPFileNode>(),
                            Parent = parentNode,
                            //Level = parentNode.Level + 1
                        };

                        parentNode.Childrens.Add(fileNode);

                        //asyn.Advance(10.0 / root.Childrens.Count / objectsId.Count, string.Format(LanguageHelper.Get("LANGKEY_HuoQuJieDian_00554"), fileNode.Name));

                        if (fileNode.Type != MTPFileNodeType.File)
                        {
                            CreateTree(fileNode, content, properties, asyn);
                        }
                    }
                }
            }

            return root;
        }

        private void CreateTree(MTPFileNode parentNode, IPortableDeviceContent content, IPortableDeviceProperties properties, IAsyncProgress asyn)
        {
            List<string> objectsId = GetChildrenObjectIds(content, parentNode.Id);
            if (objectsId != null && objectsId.Count > 0)
            {
                foreach (string objectId in objectsId)
                {
                    IPortableDeviceValues objectValues;
                    properties.GetValues(objectId, null, out objectValues);
                    MTPFileNode fileNode = new MTPFileNode()
                    {
                        Type = GetFileTypeProperty(objectValues),
                        Name = GetFullNameProperty(objectValues),
                        //FileSize = GetFileSizeProperty(objectValues),
                        Id = objectId,
                        Childrens = new List<MTPFileNode>(),
                        Parent = parentNode,
                        //Level = parentNode.Level + 1
                    };

                    parentNode.Childrens.Add(fileNode);

                    //asyn.Advance(0, string.Format(LanguageHelper.Get("LANGKEY_HuoQuJieDian_00555"), fileNode.Name));

                    if (fileNode.Type != MTPFileNodeType.File)
                    {
                        CreateTree(fileNode, content, properties, asyn);
                    }
                }
            }
        }

        #region 静态方法

        public static List<MTPDevice> GetDevices()
        {
            var devices = new List<MTPDevice>();

            string[] deviceIds = EnumerateDevices();
            PortableDevice portableDevice;
            IPortableDeviceContent deviceContent;

            MTPDevice tempDevice;
            if (deviceIds.IsInvalid())
            {
                return devices;
            }
            foreach (var deviceId in deviceIds)
            {
                IPortableDeviceValues deviceValues = Connect(deviceId, out portableDevice, out deviceContent);
                MTPDeviceType deviceType = GetDeviceType(deviceValues);
                //if (deviceType == MTPDeviceType.MediaPlayer || deviceType == MTPDeviceType.Phone)
                {
                    tempDevice = new MTPDevice();
                    tempDevice.DeviceId = deviceId;
                    tempDevice.DeviceType = deviceType;
                    tempDevice.SerialNumber = GetSerialNumber(deviceValues);
                    tempDevice.DeviceName = GetDeviceName(deviceValues);
                    tempDevice.FirmwareVersion = GetFirmwareVersion(deviceValues);
                    tempDevice.Manufacturer = GetManufacturer(deviceValues);
                    tempDevice.Model = GetModel(deviceValues);

                    devices.Add(tempDevice);
                }

                Disconnect(portableDevice);
            }

            return devices;
        }

        /// <summary>
        /// 枚举所有便携式设备（MTP模式）
        /// </summary>
        /// <returns>返回设备id数组</returns>
        private static string[] EnumerateDevices()
        {
            string[] devicesIds = null;
            PortableDeviceManagerClass deviceManager = new PortableDeviceManagerClass();
            uint deviceCount = 1;//设备数目初始值必须大于0
            deviceManager.GetDevices(null, ref deviceCount);//获取设备数目必须置第一个参数为null
            if (deviceCount > 0)
            {
                devicesIds = new string[deviceCount];
                deviceManager.GetDevices(devicesIds, ref deviceCount);
            }
            return devicesIds;
        }

        /// <summary>
        /// 连接设备
        /// </summary>
        /// <param name="DeviceId"></param>
        /// <param name="portableDevice"></param>
        /// <param name="deviceContent"></param>
        /// <returns></returns>
        private static IPortableDeviceValues Connect(string DeviceId, out PortableDevice portableDevice, out IPortableDeviceContent deviceContent)
        {
            IPortableDeviceValues clientInfo = (IPortableDeviceValues)new PortableDeviceTypesLib.PortableDeviceValuesClass();
            portableDevice = new PortableDeviceClass();
            portableDevice.Open(DeviceId, clientInfo);
            portableDevice.Content(out deviceContent);

            IPortableDeviceProperties deviceProperties;
            deviceContent.Properties(out deviceProperties);

            IPortableDeviceValues deviceValues;
            deviceProperties.GetValues("DEVICE", null, out deviceValues);
            return deviceValues;
        }

        /// <summary>
        /// 断开设备
        /// </summary>
        /// <param name="portableDevice"></param>
        private static void Disconnect(PortableDevice portableDevice)
        {
            portableDevice.Close();
        }

        /// <summary>
        /// 获取设备类型
        /// </summary>
        /// <param name="DeviceValues"></param>
        /// <returns></returns>
        private static MTPDeviceType GetDeviceType(IPortableDeviceValues DeviceValues)
        {
            _tagpropertykey deviceTypeKey = new _tagpropertykey() { fmtid = new Guid("26d4979a-e643-4626-9e2b-736dc0c92fdc"), pid = 15 };
            uint propertyValue;
            DeviceValues.GetUnsignedIntegerValue(ref deviceTypeKey, out propertyValue);
            MTPDeviceType deviceType = (MTPDeviceType)propertyValue;
            return deviceType;
        }

        /// <summary>
        /// 获取设备名
        /// </summary>
        /// <param name="DeviceValues"></param>
        /// <returns></returns>
        private static string GetDeviceName(IPortableDeviceValues DeviceValues)
        {
            _tagpropertykey property = new _tagpropertykey() { fmtid = new Guid("ef6b490d-5cd8-437a-affc-da8b60ee4a3c"), pid = 4 };
            string name;
            DeviceValues.GetStringValue(ref property, out name);
            return name;
        }

        /// <summary>
        /// 获取序列号
        /// </summary>
        /// <param name="DeviceValues"></param>
        /// <returns></returns>
        private static string GetSerialNumber(IPortableDeviceValues DeviceValues)
        {
            _tagpropertykey property = new _tagpropertykey() { fmtid = new Guid("26d4979a-e643-4626-9e2b-736dc0c92fdc"), pid = 9 };
            string name;
            DeviceValues.GetStringValue(ref property, out name);
            return name;
        }

        /// <summary>
        /// 获取固件版本
        /// </summary>
        /// <param name="DeviceValues"></param>
        /// <returns></returns>
        private static string GetFirmwareVersion(IPortableDeviceValues DeviceValues)
        {
            _tagpropertykey deviceTypeKey = new _tagpropertykey() { fmtid = new Guid("26d4979a-e643-4626-9e2b-736dc0c92fdc"), pid = 3 };
            string firmwareVersion;
            DeviceValues.GetStringValue(ref deviceTypeKey, out firmwareVersion);
            return firmwareVersion;
        }

        /// <summary>
        /// 获取制造商
        /// </summary>
        /// <param name="DeviceValues"></param>
        /// <returns></returns>
        private static string GetManufacturer(IPortableDeviceValues DeviceValues)
        {
            _tagpropertykey property = new _tagpropertykey() { fmtid = new Guid("26d4979a-e643-4626-9e2b-736dc0c92fdc"), pid = 7 };
            string manufacturer;
            DeviceValues.GetStringValue(ref property, out manufacturer);
            return manufacturer;
        }

        /// <summary>
        /// 获取型号
        /// </summary>
        /// <param name="DeviceValues"></param>
        /// <returns></returns>
        private static string GetModel(IPortableDeviceValues DeviceValues)
        {
            _tagpropertykey property = new _tagpropertykey() { fmtid = new Guid("26d4979a-e643-4626-9e2b-736dc0c92fdc"), pid = 8 };
            string model;
            DeviceValues.GetStringValue(ref property, out model);
            return model;
        }

        /// <summary>
        /// 获取总容量和可用容量
        /// </summary>
        /// <param name="deviceContent"></param>
        /// <param name="storageId"></param>
        /// <param name="freeSpace"></param>
        /// <param name="storageCapacity"></param>
        private static void GetStorageCapacityAnFreeSpace(IPortableDeviceContent deviceContent, string storageId, out ulong freeSpace, out ulong storageCapacity)
        {
            try
            {
                IPortableDeviceProperties deviceProperties;
                deviceContent.Properties(out deviceProperties);

                IPortableDeviceKeyCollection keyCollection = (IPortableDeviceKeyCollection)new PortableDeviceTypesLib.PortableDeviceKeyCollectionClass();
                _tagpropertykey freeSpaceKey = new _tagpropertykey();
                freeSpaceKey.fmtid = new Guid("01a3057a-74d6-4e80-bea7-dc4c212ce50a");
                freeSpaceKey.pid = 5;

                _tagpropertykey storageCapacityKey = new _tagpropertykey();
                storageCapacityKey.fmtid = new Guid("01a3057a-74d6-4e80-bea7-dc4c212ce50a");
                storageCapacityKey.pid = 4;

                keyCollection.Add(freeSpaceKey);
                keyCollection.Add(storageCapacityKey);

                IPortableDeviceValues deviceValues;
                deviceProperties.GetValues(storageId, keyCollection, out deviceValues);

                deviceValues.GetUnsignedLargeIntegerValue(ref freeSpaceKey, out freeSpace);
                deviceValues.GetUnsignedLargeIntegerValue(ref storageCapacityKey, out storageCapacity);
            }
            catch
            {
                freeSpace = 0;
                storageCapacity = 0;
            }
        }

        /// <summary>
        /// 获取设备信息
        /// </summary>
        /// <param name="deviceId"></param>
        /// <returns></returns>
        private static IPortableDeviceContent GetDeviceContent(string deviceId)
        {
            IPortableDeviceValues clientInfo = (IPortableDeviceValues)new PortableDeviceTypesLib.PortableDeviceValuesClass();
            PortableDeviceClass portableDevice = new PortableDeviceClass();
            portableDevice.Open(deviceId, clientInfo);

            IPortableDeviceContent content;
            portableDevice.Content(out content);
            return content;
        }

        /// <summary>
        /// 获取设备或设备下文件夹的所有对象（文件、文件夹）的ObjectId
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="parentId"></param>
        /// <returns></returns>
        private static List<string> GetChildrenObjectIds(IPortableDeviceContent content, string parentId)
        {
            IEnumPortableDeviceObjectIDs objectIds;
            content.EnumObjects(0, parentId, null, out objectIds);

            List<string> childItems = new List<string>();
            uint fetched = 0;
            do
            {
                string objectId;
                objectIds.Next(1, out objectId, ref fetched);


                // Check if anything was retrieved.


                if (fetched > 0)
                {
                    childItems.Add(objectId);
                }
            } while (fetched > 0);
            return childItems;
        }

        /// <summary>
        /// 获取名称属性
        /// </summary>
        /// <param name="deviceValues"></param>
        /// <returns></returns>
        private static string GetNameProperty(IPortableDeviceValues deviceValues)
        {
            _tagpropertykey nameKey = new _tagpropertykey();
            nameKey.fmtid = new Guid("EF6B490D-5CD8-437A-AFFC-DA8B60EE4A3C");//guid唯一值
            nameKey.pid = 4;//索引

            string nameProperty = null;
            deviceValues.GetStringValue(ref nameKey, out nameProperty);
            return nameProperty;
        }

        /// <summary>
        /// 获取名称属性
        /// </summary>
        /// <param name="deviceValues"></param>
        /// <returns></returns>
        private static string GetFullNameProperty(IPortableDeviceValues deviceValues)
        {
            try
            {
                _tagpropertykey nameKey = new _tagpropertykey();
                nameKey.fmtid = new Guid(0xEF6B490D, 0x5CD8, 0x437A, 0xAF, 0xFC, 0xDA, 0x8B, 0x60, 0xEE, 0x4A, 0x3C);//guid唯一值
                nameKey.pid = 12;//索引

                string nameProperty = null;
                deviceValues.GetStringValue(ref nameKey, out nameProperty);
                return nameProperty;
            }
            catch (Exception)
            {
                return GetNameProperty(deviceValues);
            }
        }

        /// <summary>
        /// 获取大小属性
        /// </summary>
        /// <param name="deviceValues"></param>
        /// <returns></returns>
        private static ulong GetFileSizeProperty(IPortableDeviceValues deviceValues)
        {
            _tagpropertykey nameKey = new _tagpropertykey();
            nameKey.fmtid = new Guid(0xEF6B490D, 0x5CD8, 0x437A, 0xAF, 0xFC, 0xDA, 0x8B, 0x60, 0xEE, 0x4A, 0x3C);//guid唯一值
            nameKey.pid = 11;//索引

            ulong sizeProperty = 0;
            deviceValues.GetUnsignedLargeIntegerValue(ref nameKey, out sizeProperty);
            return sizeProperty;
        }

        /// <summary>
        /// 获取创建时间属性
        /// </summary>
        /// <param name="deviceValues"></param>
        /// <returns></returns>
        private static string GetFileDateCreatedProperty(IPortableDeviceValues deviceValues)
        {
            _tagpropertykey nameKey = new _tagpropertykey();
            nameKey.fmtid = new Guid(0xEF6B490D, 0x5CD8, 0x437A, 0xAF, 0xFC, 0xDA, 0x8B, 0x60, 0xEE, 0x4A, 0x3C);//guid唯一值
            nameKey.pid = 18;//索引

            string sizeProperty = string.Empty;

            try
            {
                deviceValues.GetStringValue(ref nameKey, out sizeProperty);
            }
            catch
            {
                return "";
            }

            return sizeProperty;
        }

        /// <summary>
        /// 获取修改时间属性
        /// </summary>
        /// <param name="deviceValues"></param>
        /// <returns></returns>
        private static string GetFileDateModifiedProperty(IPortableDeviceValues deviceValues)
        {
            _tagpropertykey nameKey = new _tagpropertykey();
            nameKey.fmtid = new Guid(0xEF6B490D, 0x5CD8, 0x437A, 0xAF, 0xFC, 0xDA, 0x8B, 0x60, 0xEE, 0x4A, 0x3C);//guid唯一值
            nameKey.pid = 19;//索引

            string sizeProperty = string.Empty;

            try
            {
                deviceValues.GetStringValue(ref nameKey, out sizeProperty);
            }
            catch
            {
                return "";
            }

            return sizeProperty;
        }

        /// <summary>
        /// 获取访问时间属性
        /// </summary>
        /// <param name="deviceValues"></param>
        /// <returns></returns>
        private static string GetFileDateAuthoredProperty(IPortableDeviceValues deviceValues)
        {
            _tagpropertykey nameKey = new _tagpropertykey();
            nameKey.fmtid = new Guid(0xEF6B490D, 0x5CD8, 0x437A, 0xAF, 0xFC, 0xDA, 0x8B, 0x60, 0xEE, 0x4A, 0x3C);//guid唯一值
            nameKey.pid = 20;//索引

            string sizeProperty = string.Empty;

            try
            {
                deviceValues.GetStringValue(ref nameKey, out sizeProperty);
            }
            catch
            {
                return "";
            }

            return sizeProperty;
        }

        /// <summary>
        /// 获取文件属性
        /// </summary>
        /// <param name="deviceValues"></param>
        /// <returns></returns>
        private static MTPFileNodeType GetFileTypeProperty(IPortableDeviceValues deviceValues)
        {
            Guid contentType;
            _tagpropertykey property = new _tagpropertykey();
            property.fmtid = new Guid(0xEF6B490D, 0x5CD8, 0x437A, 0xAF, 0xFC, 0xDA, 0x8B, 0x60, 0xEE, 0x4A, 0x3C);
            property.pid = 7;
            deviceValues.GetGuidValue(property, out contentType);

            var folderType = new Guid(0x27E2E392, 0xA111, 0x48E0, 0xAB, 0x0C, 0xE1, 0x77, 0x05, 0xA0, 0x5F, 0x85);
            var functionalType = new Guid(0x99ED0160, 0x17FF, 0x4C44, 0x9D, 0x98, 0x1D, 0x7A, 0x6F, 0x94, 0x19, 0x21);

            if (contentType == folderType || contentType == functionalType)
            {
                return MTPFileNodeType.Directory;
            }
            return MTPFileNodeType.File;
        }

        // <summary>
        /// 创建符合要求的文件
        /// </summary>
        /// <param name="sourcePath"></param>
        /// <param name="parentId"></param>
        /// <returns></returns>
        private static IPortableDeviceValues GetRequiredPropertiesForContentType(string sourcePath, string parentId)
        {
            IPortableDeviceValues values = new PortableDeviceTypesLib.PortableDeviceValues() as IPortableDeviceValues;


            _tagpropertykey WPD_OBJECT_PARENT_ID = new _tagpropertykey();
            WPD_OBJECT_PARENT_ID.fmtid = new Guid(0xEF6B490D, 0x5CD8, 0x437A, 0xAF, 0xFC, 0xDA, 0x8B, 0x60, 0xEE, 0x4A, 0x3C);
            WPD_OBJECT_PARENT_ID.pid = 3;
            values.SetStringValue(ref WPD_OBJECT_PARENT_ID, parentId);


            _tagpropertykey WPD_OBJECT_SIZE = new _tagpropertykey();
            WPD_OBJECT_SIZE.fmtid = new Guid(0xEF6B490D, 0x5CD8, 0x437A, 0xAF, 0xFC, 0xDA, 0x8B, 0x60, 0xEE, 0x4A, 0x3C);
            WPD_OBJECT_SIZE.pid = 11;


            FileInfo fileInfo = new FileInfo(sourcePath);
            values.SetUnsignedLargeIntegerValue(WPD_OBJECT_SIZE, (ulong)fileInfo.Length);


            _tagpropertykey WPD_OBJECT_ORIGINAL_FILE_NAME = new _tagpropertykey();
            WPD_OBJECT_ORIGINAL_FILE_NAME.fmtid = new Guid(0xEF6B490D, 0x5CD8, 0x437A, 0xAF, 0xFC, 0xDA, 0x8B, 0x60, 0xEE, 0x4A, 0x3C);
            WPD_OBJECT_ORIGINAL_FILE_NAME.pid = 12;
            values.SetStringValue(WPD_OBJECT_ORIGINAL_FILE_NAME, Path.GetFileName(sourcePath));


            _tagpropertykey WPD_OBJECT_NAME = new _tagpropertykey();
            WPD_OBJECT_NAME.fmtid = new Guid(0xEF6B490D, 0x5CD8, 0x437A, 0xAF, 0xFC, 0xDA, 0x8B, 0x60, 0xEE, 0x4A, 0x3C);
            WPD_OBJECT_NAME.pid = 4;
            values.SetStringValue(WPD_OBJECT_NAME, Path.GetFileName(sourcePath));


            return values;
        }

        /// <summary>
        /// 创建符合要求的文件夹
        /// </summary>
        /// <param name="parentId"></param>
        /// <param name="folderName"></param>
        /// <returns></returns>
        private static IPortableDeviceValues GetRequiredPropertiesForFolder(string parentId, string folderName)
        {
            IPortableDeviceValues values = new PortableDeviceTypesLib.PortableDeviceValues() as IPortableDeviceValues;


            _tagpropertykey WPD_OBJECT_PARENT_ID = new _tagpropertykey();
            WPD_OBJECT_PARENT_ID.fmtid = new Guid(0xEF6B490D, 0x5CD8, 0x437A, 0xAF, 0xFC, 0xDA, 0x8B, 0x60, 0xEE, 0x4A, 0x3C);
            WPD_OBJECT_PARENT_ID.pid = 3;
            values.SetStringValue(ref WPD_OBJECT_PARENT_ID, parentId);


            _tagpropertykey WPD_OBJECT_NAME = new _tagpropertykey();
            WPD_OBJECT_NAME.fmtid = new Guid(0xEF6B490D, 0x5CD8, 0x437A, 0xAF, 0xFC, 0xDA, 0x8B, 0x60, 0xEE, 0x4A, 0x3C);
            WPD_OBJECT_NAME.pid = 4;
            values.SetStringValue(WPD_OBJECT_NAME, folderName);


            _tagpropertykey WPD_OBJECT_ORIGINAL_FILE_NAME = new _tagpropertykey();
            WPD_OBJECT_ORIGINAL_FILE_NAME.fmtid = new Guid(0xEF6B490D, 0x5CD8, 0x437A, 0xAF, 0xFC, 0xDA, 0x8B, 0x60, 0xEE, 0x4A, 0x3C);
            WPD_OBJECT_ORIGINAL_FILE_NAME.pid = 12;
            values.SetStringValue(WPD_OBJECT_ORIGINAL_FILE_NAME, folderName);


            _tagpropertykey WPD_OBJECT_CONTENT_TYPE = new _tagpropertykey();
            WPD_OBJECT_CONTENT_TYPE.fmtid = new Guid(0xEF6B490D, 0x5CD8, 0x437A, 0xAF, 0xFC, 0xDA, 0x8B, 0x60, 0xEE, 0x4A, 0x3C);
            WPD_OBJECT_CONTENT_TYPE.pid = 12;
            values.SetStringValue(WPD_OBJECT_ORIGINAL_FILE_NAME, folderName);


            return values;
        }

        /// <summary>
        /// 写入数据
        /// </summary>
        /// <param name="sourceFile"></param>
        /// <param name="portableDevice"></param>
        /// <param name="parentId"></param>
        private static void TransferContentToDevice(string sourceFile, PortableDeviceClass portableDevice, string parentId)
        {
            IPortableDeviceContent content;
            portableDevice.Content(out content);


            IPortableDeviceValues values = GetRequiredPropertiesForContentType(sourceFile, parentId);


            IStream tempStream;
            uint optimalTransferSizeBytes = 0;
            content.CreateObjectWithPropertiesAndData(values, out tempStream, ref optimalTransferSizeBytes, null);


            System.Runtime.InteropServices.ComTypes.IStream targetStream = (System.Runtime.InteropServices.ComTypes.IStream)tempStream;


            try
            {
                using (FileStream sourceStream = new FileStream(sourceFile, FileMode.Open, FileAccess.Read))
                {
                    int bytesRead = 0;
                    do
                    {
                        int count = 1024 * 1024;
                        byte[] buffer = new byte[count];
                        bytesRead = sourceStream.Read(buffer, 0, count);


                        IntPtr pcbWritten = IntPtr.Zero;
                        targetStream.Write(buffer, bytesRead, pcbWritten);
                    }
                    while (bytesRead > 0);
                }
                targetStream.Commit(0);
            }
            finally
            {
                Marshal.ReleaseComObject(tempStream);
            }
        }

        private static string TransferContentFromDevice(string saveToPath, IPortableDeviceContent content, string parentObjectID)
        {
            IPortableDeviceResources resources;
            content.Transfer(out resources);
            PortableDeviceApiLib.IStream wpdStream = null;
            uint optimalTransferSize = int.MaxValue;
            PortableDeviceApiLib._tagpropertykey property = new PortableDeviceApiLib._tagpropertykey();
            property.fmtid = new Guid(0xE81E79BE, 0x34F0, 0x41BF, 0xB5, 0x3F, 0xF1, 0xA0, 0x6A, 0xE8, 0x78, 0x42);
            property.pid = 0;

            try
            {
                resources.GetStream(parentObjectID, ref property, 0, ref optimalTransferSize, out wpdStream);
                System.Runtime.InteropServices.ComTypes.IStream sourceStream = (System.Runtime.InteropServices.ComTypes.IStream)wpdStream;

                using (FileStream targetStream = new FileStream(saveToPath, FileMode.Create, FileAccess.Write))
                {
                    int filesize = int.Parse(optimalTransferSize.ToString());
                    var buffer = new byte[filesize];
                    int bytesRead = 0;
                    IntPtr bytesReadIntPtr = new IntPtr(bytesRead);
                    //设备建议读取长度optimalTransferSize长度一般为262144即256k，  
                    //注释掉的sourceStream.Read不能更新bytesRead值，do循环只能执行一次即写入256k数据。  
                    //创建nextBufferSize变量，用于每次Read后计算下一次buffer长度  
                    int nextBufferSize = 0;
                    do
                    {
                        if (bytesReadIntPtr == IntPtr.Zero)
                        {
                            bytesReadIntPtr = Marshal.AllocCoTaskMem(Marshal.SizeOf(typeof(int)));
                        }
                        sourceStream.Read(buffer, filesize, bytesReadIntPtr);
                        nextBufferSize = Marshal.ReadInt32(bytesReadIntPtr);
                        if (filesize > nextBufferSize)
                        {
                            filesize = nextBufferSize;
                        }

                        targetStream.Write(buffer, 0, filesize);
                    } while (nextBufferSize > 0);
                    Marshal.FreeCoTaskMem(bytesReadIntPtr);
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            finally
            {
                if (wpdStream != null)
                    Marshal.ReleaseComObject(wpdStream);
            }
            return string.Empty;
        }

        private static string TransferContentFromDevice(string saveToPath, string deviceid, string parentObjectID)
        {
            return TransferContentFromDevice(saveToPath, GetDeviceContent(deviceid), parentObjectID);
        }

        private static string GetNameById(string objectId, IPortableDeviceProperties properties)
        {
            IPortableDeviceValues objectValues;
            properties.GetValues(objectId, null, out objectValues);
            return GetNameProperty(objectValues);
        }

        private static MTPFileNodeType GetFileTypeById(string objectId, IPortableDeviceProperties properties)
        {
            IPortableDeviceValues objectValues;
            properties.GetValues(objectId, null, out objectValues);
            return GetFileTypeProperty(objectValues);
        }

        #endregion
    }
}
