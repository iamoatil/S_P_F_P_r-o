using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows;
using System.IO;

namespace XLY.XDD.Control
{
    /// <summary>
    /// 默认单元格模版生成器
    /// </summary>
    public class GridViewCellTemplateProvider : IGridViewCellTemplateProvider
    {
        /// <summary>
        /// 构建一个列表单元格解析器
        /// </summary>
        /// <param name="ui"></param>
        public GridViewCellTemplateProvider(System.Windows.Controls.Control control)
        {
            this.Control = control;
            this.DefaultForeground = All_DefaultForeground;
            this.DefaultHyperlinkForeground = All_DefaultHyperlinkForeground;
            this.DefaultFontSize = All_DefaultFontSize;
            this.DefaultFontFamily = All_DefaultFontFamily;
            this.DefaultCellMaxHeight = All_DefaultCellMaxHeight;
            this.DefaultDateTimeFormat = All_DefaultDateTimeFormat;
            this.DefaultImageWidth = All_DefaultImageWidth;
            this.DefaultImageHeight = All_DefaultImageHeight;
            this.DefaultThumbnailImageWidth = All_DefaultThumbnailImageWidth;
            this.DefaultThumbnailImageHeight = All_DefaultThumbnailImageHeight;
        }

        static GridViewCellTemplateProvider()
        {
            All_DefaultForeground = new SolidColorBrush(Colors.Black);
            All_DefaultHyperlinkForeground = new SolidColorBrush(Colors.Blue);
            All_DefaultFontSize = 12;
            All_DefaultFontFamily = new FontFamily("Microsoft YaHei");
            All_DefaultCellMaxHeight = 40;
            All_DefaultDateTimeFormat = "yyyy-MM-dd HH:mm:ss";
            All_DefaultImageWidth = 20d;
            All_DefaultImageHeight = double.NaN;
            All_DefaultThumbnailImageWidth = 20d;
            All_DefaultThumbnailImageHeight = double.NaN;
        }

        /// <summary>
        /// 所属控件
        /// </summary>
        public System.Windows.Controls.Control Control { get; set; }

        public virtual DataTemplate CreateCellTemplate(IGridViewColumn column)
        {
            if (column is GridViewColumn)
            {
                GridViewColumn c = column as GridViewColumn;
                string template = null;
                if (c.Type == GridViewColumnType.String)
                {
                    // 字符串列
                    template = Resource.StringTemplate;
                }
                else if (c.Type == GridViewColumnType.DateTime)
                {
                    // 日期时间型列
                    template = Resource.DateTimeTemplate.Replace("#Format#", c.Format ?? this.DefaultDateTimeFormat);
                }
                else if (c.Type == GridViewColumnType.Bool)
                {
                    template = Resource.BoolTemplate;
                }
                else if (c.Type == GridViewColumnType.Enum)
                {
                    // 枚举列
                    template = Resource.EnumTemplate;
                }
                else if (c.Type == GridViewColumnType.Int)
                {
                    template = Resource.IntTemplate;
                }
                else if (c.Type == GridViewColumnType.Custom)
                {
                    // 自定义列
                    if (c.Template != null)
                        return c.Template;
                    else if (c.TemplateKey != null)
                        return this.Control.TryFindResource(c.TemplateKey) as DataTemplate;
                }
                else if (c.Type == GridViewColumnType.Image)
                {
                    // 图片列
                    template = Resource.ImageTemplate;
                    double width = c.ContentWidth.Equals(double.NaN) ? this.DefaultImageWidth : c.ContentWidth;
                    double height = c.ContentHeight.Equals(double.NaN) ? this.DefaultImageHeight : c.ContentHeight;
                    template = template.Replace("#ContentWidth#", width.Equals(double.NaN) ? "auto" : width.ToString());
                    template = template.Replace("#ContentHeight#", height.Equals(double.NaN) ? "auto" : height.ToString());
                }
                else if (c.Type == GridViewColumnType.Hyperlink)
                {
                    // 超链接列
                    template = Resource.HyperlinkTemplate;
                    SolidColorBrush hyperlink_foreground = (c.Foreground ?? this.DefaultHyperlinkForeground) as SolidColorBrush;
                    template = template.Replace("#Foreground#", hyperlink_foreground.Color.ToString());
                }
                else if (c.Type == GridViewColumnType.ThumbnailImage)
                {
                    // 缩略图列
                    template = Resource.ThumbnailImageTemplate;
                    double width = c.ContentWidth.Equals(double.NaN) ? this.DefaultImageWidth : c.ContentWidth;
                    double height = c.ContentHeight.Equals(double.NaN) ? this.DefaultImageHeight : c.ContentHeight;
                    template = template.Replace("#ContentWidth#", width.Equals(double.NaN) ? "auto" : width.ToString());
                    template = template.Replace("#ContentHeight#", height.Equals(double.NaN) ? "auto" : height.ToString());
                }

                template = this.ReplaceDefaultArgs(c, template);

                return this.CreateDataTemplate(template);
            }
            return null;
        }

        /// <summary>
        /// 根据字符串创建数据模版
        /// </summary>
        /// <param name="template">字符串数据模版</param>
        /// <returns></returns>
        protected DataTemplate CreateDataTemplate(string template)
        {
            using (Stream sem = new MemoryStream(Encoding.UTF8.GetBytes(template)))
            {
                DataTemplate dt = XamlReader.Load(sem, this.GetConvertParsercontext()) as DataTemplate;
                return dt;
            }
        }

        /// <summary>
        /// 替换默认的字符串
        /// </summary>
        /// <param name="template">模版</param>
        /// <returns></returns>
        protected string ReplaceDefaultArgs(GridViewColumnBase c, string template)
        {
            if (c.IsFileNameColor)
            {
                string bingstr;
                if (c.IsFilex)
                {
                    bingstr = "{Binding Path=RowData.Row.FileNameColor}";
                }
                else
                {
                    bingstr = "{Binding Path=RowData.Row.ProxyFileX.FileNameColor}";
                }

                template = template.Replace("#Foreground#", bingstr);
            }
            else
            {
                SolidColorBrush foreground = (c.Foreground ?? this.DefaultForeground) as SolidColorBrush;
                if (foreground == null)
                {
                    throw new Exception("Foreground is only SolidColorBrush");
                }

                template = template.Replace("#Foreground#", foreground.Color.ToString());
            }

            template = template.Replace("#FieldName#", this._GetBindingString(c.IsDynamic, c.FieldName));

            template = template.Replace("#FontSize#", (c.FontSize ?? this.DefaultFontSize).ToString());
            template = template.Replace("#FontFamily#", (c.FontFamily ?? this.DefaultFontFamily).Source);
            template = template.Replace("#CellMaxHeight#", (c.CellMaxHeight ?? this.DefaultCellMaxHeight).ToString());
            template = template.Replace("#TextAlignment#", c.TextAlignment.ToString());
            template = template.Replace("#ContentWidth#", c.ContentWidth.Equals(double.NaN) ? "auto" : c.ContentWidth.ToString());
            template = template.Replace("#ContentHeight#", c.ContentHeight.Equals(double.NaN) ? "auto" : c.ContentHeight.ToString());
            if (c.IsSupportDetail)
            {
                if (this.Control is GridView)
                {
                    template = template.Replace("#Detail#", Resource.DetailActionButtonTemplate_For_GridView);
                }
                else if (this.Control is TreeView)
                {
                    template = template.Replace("#Detail#", Resource.DetailActionButtonTemplate_For_TreeView);
                }
            }
            else
            {
                template = template.Replace("#Detail#", "");
            }

            return template;
        }

        /// <summary>
        /// 默认字体颜色
        /// </summary>
        public Brush DefaultForeground { get; set; }

        /// <summary>
        /// 用于所有的解析器默认字体
        /// </summary>
        public static Brush All_DefaultForeground { get; set; }

        /// <summary>
        /// 默认超链接颜色
        /// </summary>
        public Brush DefaultHyperlinkForeground { get; set; }

        /// <summary>
        /// 用于所有的解析器的默认超链接颜色
        /// </summary>
        public static Brush All_DefaultHyperlinkForeground { get; set; }

        /// <summary>
        /// 默认字体大小
        /// </summary>
        public double DefaultFontSize { get; set; }

        /// <summary>
        /// 用于所有的解析器的默认字体大小
        /// </summary>
        public static double All_DefaultFontSize { get; set; }

        /// <summary>
        /// 默认字体
        /// </summary>
        public FontFamily DefaultFontFamily { get; set; }

        /// <summary>
        /// 用于所有的解析器的默认字体
        /// </summary>
        public static FontFamily All_DefaultFontFamily { get; set; }

        /// <summary>
        /// 默认最大单元格高度
        /// </summary>
        public double DefaultCellMaxHeight { get; set; }

        /// <summary>
        /// 用于所有的解析器的默认最大单元格高度
        /// </summary>
        public static double All_DefaultCellMaxHeight { get; set; }

        /// <summary>
        /// 默认日期时间格式化字符串
        /// </summary>
        public string DefaultDateTimeFormat { get; set; }

        /// <summary>
        /// 用于所有的解析器的默认日期时间格式化字符串
        /// </summary>
        public static string All_DefaultDateTimeFormat { get; set; }

        /// <summary>
        /// 默认的图片宽度
        /// </summary>
        public double DefaultImageWidth { get; set; }

        /// <summary>
        /// 用于所有的解析器的默认的图片宽度
        /// </summary>
        public static double All_DefaultImageWidth { get; set; }

        /// <summary>
        /// 默认图片高度
        /// </summary>
        public double DefaultImageHeight { get; set; }

        /// <summary>
        /// 用于所有的解析器的默认图片高度
        /// </summary>
        public static double All_DefaultImageHeight { get; set; }

        /// <summary>
        /// 默认缩略图宽度
        /// </summary>
        public double DefaultThumbnailImageWidth { get; set; }

        /// <summary>
        /// 用于所有的解析器的默认缩略图宽度
        /// </summary>
        public static double All_DefaultThumbnailImageWidth { get; set; }

        /// <summary>
        /// 默认缩略图高度
        /// </summary>
        public double DefaultThumbnailImageHeight { get; set; }

        /// <summary>
        /// 用于所有的解析器的默认缩略图高度
        /// </summary>
        public static double All_DefaultThumbnailImageHeight { get; set; }

        /// <summary>
        /// 转换器xmal解析上下文
        /// </summary>
        protected ParserContext _ConvertParsercontext;

        /// <summary>
        /// 获取转换器xmal解析上下文
        /// </summary>
        public ParserContext GetConvertParsercontext()
        {
            if (_ConvertParsercontext == null)
            {
                this._ConvertParsercontext = new ParserContext();
                this._ConvertParsercontext.XamlTypeMapper = new XamlTypeMapper(new string[] { });
                this.OnCreateConvertParsercontext(this._ConvertParsercontext);
            }
            return _ConvertParsercontext;
        }

        /// <summary>
        /// 构建Xaml解析器所需上下文
        /// </summary>
        /// <param name="parserContext"></param>
        public virtual void OnCreateConvertParsercontext(ParserContext parserContext)
        {
            parserContext.XamlTypeMapper.AddMappingProcessingInstruction("clr-namespace:XLY.XDD.Control", "XLY.XDD.Control", "XLY.XDD.Control");
            parserContext.XmlnsDictionary.Add("local", "clr-namespace:XLY.XDD.Control;assembly=XLY.XDD.Control");
        }

        // 获取绑定字符串
        protected string _GetBindingString(bool isDynamic, string fieldName)
        {
            if (isDynamic)
            {
                return string.Format("RowData.Row.{0}", fieldName);
            }
            else
            {
                return string.Format("Data.{0}", fieldName);
            }
        }
    }
}
