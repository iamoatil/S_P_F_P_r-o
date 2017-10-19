using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;

namespace System.Utility.Helper
{
    /// <summary>
    /// 图像信息处理
    /// </summary>
    public class ExifManager : IDisposable
    {
        #region ExifManager这个类的使用方法
        ///// <summary>
        ///// 使用方法(按照这个方法使用ExifManager类就可以)
        ///// </summary>
        ///// <param name="imgPath">图片路径</param>
        //public void Test(string imgPath)
        //{
        //    ExifManager exif = new ExifManager(imgPath);
        //    string text = "软件：" + exif.Software + "\r\n" +
        //                  "标题：" + exif.Title + "\r\n" +
        //                  "相机存档时间：" + exif.DateTimeDigitized + "\r\n" +
        //                  "最后修改时间：" + exif.DateTimeLastModified + "\r\n" +
        //                  "拍摄日期：" + exif.DateTimeOriginal + "\r\n" +
        //                  "光圈：" + exif.Aperture + "\r\n" +
        //                  "作者：" + exif.Artist + "\r\n" +
        //                  "版权：" + exif.Copyright + "\r\n" +
        //                  "标题：" + exif.Description + "\r\n" +
        //                  "照相机制造商：" + exif.EquipmentMaker + "\r\n" +
        //                  "照相机型号：" + exif.EquipmentModel + "\r\n" +
        //                  "测光模式：" + exif.ExposureMeteringMode + "\r\n" +
        //                  "曝光程序：" + exif.ExposureProgram + "\r\n" +
        //                  "曝光时间：" + exif.ExposureTime + "\r\n" +
        //                  "快门|曝光时间：" + exif.ExposureTimeAbs + "\r\n" +
        //                  "闪光灯模式：" + exif.FlashMode + "\r\n" +
        //                  "焦距：" + exif.FocalLength + "\r\n" +
        //                  "设备型号：" + exif.UserComment + "\r\n" +
        //                  "焦距：" + exif.FocalLength + "\r\n" +
        //                  "ISO速度：" + exif.ISO + "\r\n" +
        //                  "光源：" + exif.LightSource + "\r\n" +
        //                  "方向；目标：" + exif.Orientation + "\r\n" +
        //                  "分辨率X：" + exif.ResolutionX + "\r\n" +
        //                  "分辨率Y：" + exif.ResolutionY + "\r\n" +
        //                  "摄影目标距离：" + exif.SubjectDistance + "\r\n" +
        //                  "GPS坐标：" + exif.GetGPS(imgPath) + "\r\n" +
        //                  "Image 宽：" + exif.Width + "\r\n" +
        //                  "Image 高：" + exif.Height + "\r\n";
        //}
        #endregion

        #region 图像信息处理类的代码

        private System.Drawing.Bitmap _Image;
        private System.Text.Encoding _Encoding = System.Text.Encoding.UTF8;

        /// <summary>
        /// 这一类的新实例初始化。 
        /// 位图来读取EXIF信息
        /// </summary>
        /// <param name="Bitmap"></param>
        public ExifManager(System.Drawing.Bitmap Bitmap)
        {
            if (Bitmap == null)
                throw new ArgumentNullException("Bitmap");
            this._Image = Bitmap;
        }

        /// <summary>
        /// 这一类的新实例初始化。
        /// 要加载的文件的名称 
        /// </summary>
        /// <param name="FileName"></param>
        public ExifManager(string FileName)
        {
            this._Image = (System.Drawing.Bitmap)System.Drawing.Bitmap.FromFile(FileName);
        }

        /// <summary>
        /// 代表理性的是一些EXIF属性类型 
        /// </summary>
        public struct Rational
        {
            public Int32 Numerator;
            public Int32 Denominator;

            /// <summary>
            /// 转换到字符串表示
            /// 可选，默认为“/”，字符串作为分隔符的组件。
            /// 字符串表示。 
            /// </summary>
            /// <returns></returns>
            public override string ToString()
            {
                return ToString("/");
            }

            public string ToString(string Delimiter)
            {
                return Numerator + "/" + Denominator;
            }

            /// <summary>
            /// 将数转换为双精度实数
            /// 双精度实数。
            /// </summary>
            /// <returns></returns>
            public double ToDouble()
            {
                return (double)Numerator / Denominator;
            }
        }

        /// <summary>
        /// 用于字符串元数据的获取或设置编码
        /// 用于字符串元数据的编码 
        /// 默认的编码是UTF-8 
        /// </summary>
        private System.Text.Encoding Encoding
        {
            get
            {
                return this._Encoding;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException();
                this._Encoding = value;
            }
        }

        /// <summary>
        /// 返回此实例的位图副本
        /// </summary>
        /// <returns></returns>
        private System.Drawing.Bitmap GetBitmap()
        {
            return (System.Drawing.Bitmap)this._Image.Clone();
        }

        /// <summary>
        /// 返回格式化后的字符串形式的所有可用数据 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            System.Text.StringBuilder SB = new StringBuilder();

            SB.Append("Image:");
            SB.Append("\n\tDimensions: " + this.Width + " x " + this.Height + " px");
            SB.Append("\n\tResolution: " + this.ResolutionX + " x " + this.ResolutionY + " dpi");
            SB.Append("\n\tOrientation: " + Enum.GetName(typeof(Orientations), this.Orientation));
            SB.Append("\n\tTitle: " + this.Title);
            SB.Append("\n\tDescription: " + this.Description);
            SB.Append("\n\tCopyright: " + this.Copyright);
            SB.Append("\nEquipment:");
            SB.Append("\n\tMaker: " + this.EquipmentMaker);
            SB.Append("\n\tModel: " + this.EquipmentModel);
            SB.Append("\n\tSoftware: " + this.Software);
            SB.Append("\nDate and time:");
            SB.Append("\n\tGeneral: " + this.DateTimeLastModified.ToString());
            SB.Append("\n\tOriginal: " + this.DateTimeOriginal.ToString());
            SB.Append("\n\tDigitized: " + this.DateTimeDigitized.ToString());
            SB.Append("\nShooting conditions:");
            SB.Append("\n\tExposure time: " + this.ExposureTime.ToString("N4") + " s");
            SB.Append("\n\tExposure program: " + Enum.GetName(typeof(ExposurePrograms), this.ExposureProgram));
            SB.Append("\n\tExposure mode: " + Enum.GetName(typeof(ExposureMeteringModes), this.ExposureMeteringMode));
            SB.Append("\n\tAperture: F" + this.Aperture.ToString("N2"));
            SB.Append("\n\tISO sensitivity: " + this.ISO);
            SB.Append("\n\tSubject distance: " + this.SubjectDistance.ToString("N2") + " m");
            SB.Append("\n\tFocal length: " + this.FocalLength);
            SB.Append("\n\tFlash: " + Enum.GetName(typeof(FlashModes), this.FlashMode));
            SB.Append("\n\tLight source (WB): " + Enum.GetName(typeof(LightSources), this.LightSource));
            //SB.Replace("\n", vbCrLf);
            //SB.Replace("\t", vbTab);
            return SB.ToString();
        }

        /// <summary>
        /// 照相机制造商
        /// </summary>
        public string EquipmentMaker
        {
            get
            {
                return this.GetPropertyString((int)TagNames.EquipMake);
            }
        }

        /// <summary>
        /// 照相机型号
        /// </summary>
        public string EquipmentModel
        {
            get
            {
                return this.GetPropertyString((int)TagNames.EquipModel);
            }
        }

        /// <summary>
        /// 软件
        /// </summary>
        public string Software
        {
            get
            {
                return this.GetPropertyString((int)TagNames.SoftwareUsed);
            }
        }

        /// <summary>
        /// n. 方向；目标；定向；适应；情况介绍；向东方
        /// </summary>
        public Orientations Orientation
        {
            get
            {
                Int32 X = this.GetPropertyInt16((int)TagNames.Orientation);

                if (!Enum.IsDefined(typeof(Orientations), X))
                    return Orientations.TopLeft;
                else
                    return (Orientations)Enum.Parse(typeof(Orientations), Enum.GetName(typeof(Orientations), X));
            }
        }

        /// <summary>
        /// 最后修改时间
        /// </summary>
        public DateTime DateTimeLastModified
        {
            get
            {
                try
                {
                    return DateTime.ParseExact(this.GetPropertyString((int)TagNames.DateTime), @"yyyy\:MM\:dd HH\:mm\:ss", null);
                }
                catch
                {
                    return DateTime.MinValue;
                }
            }
            set
            {
                try
                {
                    this.SetPropertyString((int)TagNames.DateTime, value.ToString(@"yyyy\:MM\:dd HH\:mm\:ss"));
                }
                catch
                { }
            }
        }

        /// <summary>
        /// 拍摄日期
        /// </summary>
        public DateTime DateTimeOriginal
        {
            get
            {
                try
                {
                    return DateTime.ParseExact(this.GetPropertyString((int)TagNames.ExifDTOrig), @"yyyy\:MM\:dd HH\:mm\:ss", null);
                }
                catch
                {
                    return DateTime.MinValue;
                }
            }
            set
            {
                try
                {
                    this.SetPropertyString((int)TagNames.ExifDTOrig, value.ToString(@"yyyy\:MM\:dd HH\:mm\:ss"));
                }
                catch
                { }
            }
        }

        /// <summary>
        /// 相机存档时间
        /// </summary>
        public DateTime DateTimeDigitized
        {
            get
            {
                try
                {
                    return DateTime.ParseExact(this.GetPropertyString((int)TagNames.ExifDTDigitized), @"yyyy\:MM\:dd HH\:mm\:ss", null);
                }
                catch
                {
                    return DateTime.MinValue;
                }
            }
            set
            {
                try
                {
                    this.SetPropertyString((int)TagNames.ExifDTDigitized, value.ToString(@"yyyy\:MM\:dd HH\:mm\:ss"));
                }
                catch
                { }
            }
        }

        /// <summary>
        /// Image 宽
        /// </summary>
        public Int32 Width
        {
            get { return this._Image.Width; }
        }

        /// <summary>
        /// Image 高
        /// </summary>
        public Int32 Height
        {
            get { return this._Image.Height; }
        }

        /// <summary>
        /// 分辨率 X
        /// </summary>
        public double ResolutionX
        {
            get
            {
                double R = this.GetPropertyRational((int)TagNames.XResolution).ToDouble();

                if (this.GetPropertyInt16((int)TagNames.ResolutionUnit) == 3)
                {
                    return R * 2.54;
                }
                else
                {
                    return R;
                }
            }
        }

        /// <summary>
        /// 分辨率 Y
        /// </summary>
        public double ResolutionY
        {
            get
            {
                double R = this.GetPropertyRational((int)TagNames.YResolution).ToDouble();

                if (this.GetPropertyInt16((int)TagNames.ResolutionUnit) == 3)
                {
                    return R * 2.54;
                }
                else
                {
                    return R;
                }
            }
        }

        /// <summary>
        /// Image 标题(不确定)
        /// </summary>
        public string Title
        {
            get
            {
                return this.GetPropertyString((int)TagNames.ImageTitle);
            }
            set
            {
                try
                {
                    this.SetPropertyString((int)TagNames.ImageTitle, value);
                }
                catch { }
            }
        }

        /// <summary>
        /// 设备型号（不确定）
        /// </summary>
        public string UserComment
        {
            get
            {
                return this.GetPropertyString((int)TagNames.ExifUserComment);
            }
            set
            {
                try
                {
                    this.SetPropertyString((int)TagNames.ExifUserComment, value);
                }
                catch { }
            }
        }

        /// <summary>
        /// 作者
        /// </summary>
        public string Artist
        {
            get
            {
                return this.GetPropertyString((int)TagNames.Artist);
            }
            set
            {
                try
                {
                    this.SetPropertyString((int)TagNames.Artist, value);
                }
                catch { }
            }
        }

        /// <summary>
        ///Image 标题
        /// </summary>
        public string Description
        {
            get
            {
                return this.GetPropertyString((int)TagNames.ImageDescription);
            }
            set
            {
                try
                {
                    this.SetPropertyString((int)TagNames.ImageDescription, value);
                }
                catch { }
            }
        }

        /// <summary>
        /// 版权
        /// </summary>
        public string Copyright
        {
            get
            {
                return this.GetPropertyString((int)TagNames.Copyright);
            }
            set
            {
                try
                {
                    this.SetPropertyString((int)TagNames.Copyright, value);
                }
                catch { }
            }
        }

        /// <summary>
        /// 快门|曝光时间
        /// </summary>
        public double ExposureTimeAbs
        {
            get
            {
                if (this.IsPropertyDefined((int)TagNames.ExifExposureTime))
                    return this.GetPropertyRational((int)TagNames.ExifExposureTime).ToDouble();
                else
                    if (this.IsPropertyDefined((int)TagNames.ExifShutterSpeed))
                        return (1 / Math.Pow(2, this.GetPropertyRational((int)TagNames.ExifShutterSpeed).ToDouble()));
                    else
                        return 0;
            }
        }
        /// <summary>
        /// 曝光时间
        /// </summary>
        public Rational ExposureTime
        {
            get
            {
                if (this.IsPropertyDefined((int)TagNames.ExifExposureTime))
                    return this.GetPropertyRational((int)TagNames.ExifExposureTime);
                else
                    return new Rational();
            }
        }

        /// <summary>
        ///  光圈
        /// </summary>
        public double Aperture
        {
            get
            {
                if (this.IsPropertyDefined((int)TagNames.ExifFNumber))
                    return this.GetPropertyRational((int)TagNames.ExifFNumber).ToDouble();
                else
                    if (this.IsPropertyDefined((int)TagNames.ExifAperture))
                        return Math.Pow(System.Math.Sqrt(2), this.GetPropertyRational((int)TagNames.ExifAperture).ToDouble());
                    else
                        return 0;
            }
        }

        /// <summary>
        /// 曝光程序
        /// </summary>
        public ExposurePrograms ExposureProgram
        {
            get
            {
                Int32 X = this.GetPropertyInt16((int)TagNames.ExifExposureProg);

                if (Enum.IsDefined(typeof(ExposurePrograms), X))
                    return (ExposurePrograms)Enum.Parse(typeof(ExposurePrograms), Enum.GetName(typeof(ExposurePrograms), X));
                else
                    return ExposurePrograms.Normal;
            }
        }

        /// <summary>
        /// ISO速度
        /// </summary>
        public Int16 ISO
        {
            get { return this.GetPropertyInt16((int)TagNames.ExifISOSpeed); }
        }

        /// <summary>
        /// 摄影目标距离 
        /// </summary>
        public double SubjectDistance
        {
            get { return this.GetPropertyRational((int)TagNames.ExifSubjectDist).ToDouble(); }
        }

        /// <summary>
        /// 测光模式
        /// </summary>
        public ExposureMeteringModes ExposureMeteringMode
        {
            get
            {
                Int32 X = this.GetPropertyInt16((int)TagNames.ExifMeteringMode);

                if (Enum.IsDefined(typeof(ExposureMeteringModes), X))
                    return (ExposureMeteringModes)Enum.Parse(typeof(ExposureMeteringModes), Enum.GetName(typeof(ExposureMeteringModes), X));
                else
                    return ExposureMeteringModes.Unknown;
            }
        }

        /// <summary>
        /// 焦距
        /// </summary>
        public double FocalLength
        {
            get { return this.GetPropertyRational((int)TagNames.ExifFocalLength).ToDouble(); }
        }

        /// <summary>
        /// 闪光灯模式
        /// </summary>
        public FlashModes FlashMode
        {
            get
            {
                Int32 X = this.GetPropertyInt16((int)TagNames.ExifFlash);

                if (Enum.IsDefined(typeof(FlashModes), X))
                    return (FlashModes)Enum.Parse(typeof(FlashModes), Enum.GetName(typeof(FlashModes), X));
                else
                    return FlashModes.NotFired;
            }
        }

        /// <summary>
        /// 光源
        /// </summary>
        public LightSources LightSource
        {
            get
            {
                Int32 X = this.GetPropertyInt16((int)TagNames.ExifLightSource);

                if (Enum.IsDefined(typeof(LightSources), X))
                    return (LightSources)Enum.Parse(typeof(LightSources), Enum.GetName(typeof(LightSources), X));
                else
                    return LightSources.Unknown;
            }
        }

        /// <summary>
        /// 是属性定义
        /// </summary>
        /// <param name="PID"></param>
        /// <returns></returns>
        private bool IsPropertyDefined(Int32 PID)
        {
            return (Array.IndexOf(this._Image.PropertyIdList, PID) > -1);
        }

        /// <summary>
        /// 获取值
        /// </summary>
        /// <param name="PID"></param>
        /// <returns></returns>
        private Int32 GetPropertyInt32(Int32 PID)
        {
            return GetPropertyInt32(PID, 0);
        }

        private Int32 GetPropertyInt32(Int32 PID, Int32 DefaultValue)
        {
            if (IsPropertyDefined(PID))
                return GetInt32(this._Image.GetPropertyItem(PID).Value);
            else
                return DefaultValue;
        }

        /// <summary>
        /// 获取指定项目属性 
        /// 属性ID 
        /// 可选，默认为0。如果属性不存在，则返回默认值。 
        /// 属性值或默认值如果不存在的。 
        /// </summary>
        /// <param name="PID"></param>
        /// <returns></returns>
        private Int16 GetPropertyInt16(Int32 PID)
        {
            return GetPropertyInt16(PID, 0);
        }

        private Int16 GetPropertyInt16(Int32 PID, Int16 DefaultValue)
        {
            if (IsPropertyDefined(PID))
                return GetInt16(this._Image.GetPropertyItem(PID).Value);
            else
                return DefaultValue;
        }

        /// <summary>
        /// 所有权字符串
        /// </summary>
        /// <param name="PID"></param>
        /// <returns></returns>
        private string GetPropertyString(Int32 PID)
        {
            return GetPropertyString(PID, "");
        }

        private string GetPropertyString(Int32 PID, string DefaultValue)
        {
            if (IsPropertyDefined(PID))
                return GetString(this._Image.GetPropertyItem(PID).Value);
            else
                return DefaultValue;
        }

        /// <summary>
        /// 获取指定项目属性 
        /// 属性ID 
        /// 可选，默认为0。如果属性不存在，则返回默认值。 
        /// 属性值或默认值如果不存在的。 
        /// </summary>
        /// <param name="PID"></param>
        /// <returns></returns>
        private byte[] GetProperty(Int32 PID, byte[] DefaultValue)
        {
            if (IsPropertyDefined(PID))
                return this._Image.GetPropertyItem(PID).Value;
            else
                return DefaultValue;
        }

        private byte[] GetProperty(Int32 PID)
        {
            return GetProperty(PID, null);
        }

        /// <summary>
        /// 获取指定项目属性 
        /// 属性ID 
        /// 可选，默认为0。如果属性不存在，则返回默认值。 
        /// 属性值或默认值如果不存在的。 
        /// </summary>
        /// <param name="PID"></param>
        /// <returns></returns>
        private Rational GetPropertyRational(Int32 PID)
        {
            if (IsPropertyDefined(PID))
                return GetRational(this._Image.GetPropertyItem(PID).Value);
            else
            {
                Rational R;
                R.Numerator = 0;
                R.Denominator = 1;
                return R;
            }
        }

        /// <summary>
        /// 设置指定的字符串属性
        /// </summary>
        /// <param name="PID"></param>
        /// <param name="Value"></param>
        private void SetPropertyString(Int32 PID, string Value)
        {
            byte[] Data = this._Encoding.GetBytes(Value + '\0');
            SetProperty(PID, Data, ExifDataTypes.AsciiString);
        }

        /// <summary>
        /// 指定项目属性设置 
        /// </summary>
        /// <param name="PID"></param>
        /// <param name="Value"></param>
        private void SetPropertyInt16(Int32 PID, Int16 Value)
        {
            byte[] Data = new byte[2];
            Data[0] = (byte)(Value & 0xFF);
            Data[1] = (byte)((Value & 0xFF00) >> 8);
            SetProperty(PID, Data, ExifDataTypes.SignedShort);
        }

        /// <summary>
        /// 指定Int32属性设置 
        /// </summary>
        /// <param name="PID"></param>
        /// <param name="Value"></param>
        private void SetPropertyInt32(Int32 PID, Int32 Value)
        {
            byte[] Data = new byte[4];
            for (int I = 0; I < 4; I++)
            {
                Data[I] = (byte)(Value & 0xFF);
                Value >>= 8;
            }
            SetProperty(PID, Data, ExifDataTypes.SignedLong);
        }

        /// <summary>
        /// 以原始形式设置指定的属性 
        /// </summary>
        /// <param name="PID"></param>
        /// <param name="Data"></param>
        /// <param name="Type"></param>
        private void SetProperty(Int32 PID, byte[] Data, ExifDataTypes Type)
        {
            System.Drawing.Imaging.PropertyItem P = this._Image.PropertyItems[0];
            P.Id = PID;
            P.Value = Data;
            P.Type = (Int16)Type;
            P.Len = Data.Length;
            this._Image.SetPropertyItem(P);
        }

        /// <summary>
        /// 读取exif ByteArray Int32。 
        /// </summary>
        /// <param name="B"></param>
        /// <returns></returns>
        private Int32 GetInt32(byte[] B)
        {
            if (B.Length < 4)
            {
                throw new ArgumentException("Data too short (4 bytes expected)", "B");
            }

            return B[3] << 24 | B[2] << 16 | B[1] << 8 | B[0];
        }

        /// <summary>
        /// 读取exif ByteArray int16。 
        /// </summary>
        /// <param name="B"></param>
        /// <returns></returns>
        private Int16 GetInt16(byte[] B)
        {
            if (B.Length < 2)
            {
                throw new ArgumentException("Data too short (2 bytes expected)", "B");
            }

            return (short)(B[1] << 8 | B[0]);
        }

        /// <summary>
        /// 读取exif ByteArray int16。
        /// </summary>
        /// <param name="B"></param>
        /// <returns></returns>
        private string GetString(byte[] B)
        {
            string R = this._Encoding.GetString(B);
            //if (R.EndsWith("\0"))
            //{
            //    R = R.Substring(0, R.Length - 1);
            //}

            return R.Replace("\0", "");
        }

        /// <summary>
        /// 读取exif ByteArray int16。
        /// </summary>
        /// <param name="B"></param>
        /// <returns></returns>
        private Rational GetRational(byte[] B)
        {
            Rational R = new Rational();
            byte[] N = new byte[4];
            byte[] D = new byte[4];
            Array.Copy(B, 0, N, 0, 4);
            Array.Copy(B, 4, D, 0, 4);
            R.Denominator = this.GetInt32(D);
            R.Numerator = this.GetInt32(N);

            return R;
        }

        /// <summary>
        /// 处理这类非托管资源 
        /// </summary>
        public void Dispose()
        {
            this._Image.Dispose();
        }

        public string GPSInfo { get; set; }

        /// <summary>
        /// 获取图片中的GPS坐标点
        /// </summary>
        /// <param name="imgPath">图片路径</param>
        /// <returns>返回坐标【纬度+经度】用"+"分割 取数组中第0和1个位置的值</returns>
        public string GetGPS(string imgPath)
        {
            System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(imgPath);
            string GPS = "";
            //取得所有的属性(以PropertyId做排序)   
            var propertyItems = bmp.PropertyItems.OrderBy(x => x.Id);
            //暂定纬度N(北纬)   
            char chrGPSLatitudeRef = 'N';
            //暂定经度为E(东经)   
            char chrGPSLongitudeRef = 'E';
            foreach (PropertyItem objItem in propertyItems)
            {
                //只取Id范围为0x0000到0x001e
                if (objItem.Id >= 0x0000 && objItem.Id <= 0x001e)
                {
                    objItem.Id = 0x0002;
                    switch (objItem.Id)
                    {
                        case 0x0000:
                            var query = from tmpb in objItem.Value select tmpb.ToString();
                            string sreVersion = string.Join(".", query.ToArray());
                            break;
                        case 0x0001:
                            chrGPSLatitudeRef = BitConverter.ToChar(objItem.Value, 0);
                            break;
                        case 0x0002:
                            if (objItem.Value.Length == 24)
                            {
                                //degrees(将byte[0]~byte[3]转成uint, 除以byte[4]~byte[7]转成的uint)   
                                double d = BitConverter.ToUInt32(objItem.Value, 0) * 1.0d / BitConverter.ToUInt32(objItem.Value, 4);
                                //minutes(將byte[8]~byte[11]转成uint, 除以byte[12]~byte[15]转成的uint)   
                                double m = BitConverter.ToUInt32(objItem.Value, 8) * 1.0d / BitConverter.ToUInt32(objItem.Value, 12);
                                //seconds(將byte[16]~byte[19]转成uint, 除以byte[20]~byte[23]转成的uint)   
                                double s = BitConverter.ToUInt32(objItem.Value, 16) * 1.0d / BitConverter.ToUInt32(objItem.Value, 20);
                                //计算经纬度数值, 如果是南纬, 要乘上(-1)   
                                double dblGPSLatitude = (((s / 60 + m) / 60) + d) * (chrGPSLatitudeRef.Equals('N') ? 1 : -1);
                                string strLatitude = string.Format("{0:#} deg {1:#}' {2:#.00}\" {3}", d
                                                                    , m, s, chrGPSLatitudeRef);
                                //纬度+经度
                                GPS = string.Format("{0}{1},", GPS, dblGPSLatitude);
                            }
                            break;
                        case 0x0003:
                            //透过BitConverter, 将Value转成Char('E' / 'W')   
                            //此值在后续的Longitude计算上会用到   
                            chrGPSLongitudeRef = BitConverter.ToChar(objItem.Value, 0);
                            break;
                        case 0x0004:
                            if (objItem.Value.Length == 24)
                            {
                                //degrees(将byte[0]~byte[3]转成uint, 除以byte[4]~byte[7]转成的uint)   
                                double d = BitConverter.ToUInt32(objItem.Value, 0) * 1.0d / BitConverter.ToUInt32(objItem.Value, 4);
                                //minutes(将byte[8]~byte[11]转成uint, 除以byte[12]~byte[15]转成的uint)   
                                double m = BitConverter.ToUInt32(objItem.Value, 8) * 1.0d / BitConverter.ToUInt32(objItem.Value, 12);
                                //seconds(将byte[16]~byte[19]转成uint, 除以byte[20]~byte[23]转成的uint)   
                                double s = BitConverter.ToUInt32(objItem.Value, 16) * 1.0d / BitConverter.ToUInt32(objItem.Value, 20);
                                //计算精度的数值, 如果是西经, 要乘上(-1)   
                                double dblGPSLongitude = (((s / 60 + m) / 60) + d) * (chrGPSLongitudeRef.Equals('E') ? 1 : -1);
                                string strLatitude = string.Format("{0:#} deg {1:#}' {2:#.00}\" {3}", d
                                                                    , m, s, chrGPSLongitudeRef);
                                //纬度+经度
                                GPS = string.Format("{0}{1},", GPS, dblGPSLongitude);
                            }
                            break;
                        case 0x0005:
                            string strAltitude = BitConverter.ToBoolean(objItem.Value, 0) ? "0" : "1";
                            break;
                        case 0x0006:
                            if (objItem.Value.Length == 8)
                            {
                                //将byte[0]~byte[3]转成uint, 除以byte[4]~byte[7]转成的uint   
                                double dblAltitude = BitConverter.ToUInt32(objItem.Value, 0) * 1.0d / BitConverter.ToUInt32(objItem.Value, 4);
                            }
                            break;
                    }
                }
            }
            return GPS;
        }
        
        #endregion
    }
}
