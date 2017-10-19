using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Media;
using System.Windows.Documents;
using System.Windows.Controls;

namespace XLY.XDD.Control
{
    /// <summary>
    /// 高亮只读文本框
    /// </summary>
    [TemplatePart(Name = "PART_RichTextBox", Type = typeof(RichTextBox))]
    public class XlyHighlightTextBox : System.Windows.Controls.Control
    {
        static XlyHighlightTextBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(XlyHighlightTextBox), new FrameworkPropertyMetadata(typeof(XlyHighlightTextBox)));
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            this.PART_RichTextBox = this.Template.FindName("PART_RichTextBox", this) as RichTextBox;
            this.PART_RichTextBox.IsReadOnly = true;
            //if (this.PART_RichTextBox.ContextMenu == null)
            //{
            //    ContextMenu contentMenu = new ContextMenu();
            //    MenuItem item = new MenuItem();
            //    item.Header = "复制";
            //    item.Click += new RoutedEventHandler(item_Click);
            //    contentMenu.Items.Add(item);
            //    this.PART_RichTextBox.ContextMenu = contentMenu;
            //}
            this._UpdateHighlight();
        }

        /// <summary>
        /// 复制
        /// </summary>
        private void item_Click(object sender, RoutedEventArgs e)
        {
            this.PART_RichTextBox.Copy();
        }

        #region Public Interface

        /// <summary>
        /// 拷贝选择内容
        /// </summary>
        public void Copy()
        {
            if (this.PART_RichTextBox != null)
                this.PART_RichTextBox.Copy();
        }

        /// <summary>
        /// 获取选中的文本内容
        /// </summary>
        /// <returns>当前选中的内容文本</returns>
        public string GetSelectionText()
        {
            if (this.PART_RichTextBox != null && this.PART_RichTextBox.Selection != null)
            {
                return this.PART_RichTextBox.Selection.Text;
            }
            else
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// 全选
        /// </summary>
        public void SelectAll()
        {
            if (this.PART_RichTextBox != null && this.PART_RichTextBox.Selection != null)
            {
                this.PART_RichTextBox.SelectAll();
            }
        }

        #endregion

        #region PART

        internal RichTextBox PART_RichTextBox;

        #endregion

        #region HighlightText -- 高亮字符串

        /// <summary>
        /// 高亮字符串
        /// </summary>
        public string HighlightText
        {
            get { return (string)GetValue(HighlightTextProperty); }
            set { SetValue(HighlightTextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for HighlightText.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HighlightTextProperty =
            DependencyProperty.Register("HighlightText", typeof(string), typeof(XlyHighlightTextBox), new UIPropertyMetadata(null, new PropertyChangedCallback((s, e) =>
            {
                XlyHighlightTextBox label = s as XlyHighlightTextBox;
                label._UpdateHighlight();
            })));

        #endregion

        #region HighlightOffset -- 高亮偏移量

        /// <summary>
        /// 高亮偏移量
        /// </summary>
        public int HighlightOffset
        {
            get { return (int)GetValue(HighlightOffsetProperty); }
            set { SetValue(HighlightOffsetProperty, value); }
        }

        // Using a DependencyProperty as the backing store for HighlightOffset.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HighlightOffsetProperty =
            DependencyProperty.Register("HighlightOffset", typeof(int), typeof(XlyHighlightTextBox), new UIPropertyMetadata(0, new PropertyChangedCallback((s, e) =>
            {
                XlyHighlightTextBox label = s as XlyHighlightTextBox;
                label._UpdateHighlight();
            })));

        #endregion

        #region HighlightLength -- 高亮部分的长度

        /// <summary>
        /// 高亮部分的长度
        /// </summary>
        public int HighlightLength
        {
            get { return (int)GetValue(HighlightLengthProperty); }
            set { SetValue(HighlightLengthProperty, value); }
        }

        // Using a DependencyProperty as the backing store for HighlightLength.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HighlightLengthProperty =
            DependencyProperty.Register("HighlightLength", typeof(int), typeof(XlyHighlightTextBox), new UIPropertyMetadata(0, new PropertyChangedCallback((s, e) =>
            {
                XlyHighlightTextBox label = s as XlyHighlightTextBox;
                label._UpdateHighlight();
            })));

        #endregion

        #region HighlightMode -- 高亮模式

        /// <summary>
        /// 高亮模式
        /// </summary>
        public XlyHighlightMode HighlightMode
        {
            get { return (XlyHighlightMode)GetValue(HighlightModeProperty); }
            set { SetValue(HighlightModeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for HighlightMode.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HighlightModeProperty =
            DependencyProperty.Register("HighlightMode", typeof(XlyHighlightMode), typeof(XlyHighlightTextBox), new UIPropertyMetadata(XlyHighlightMode.Text, new PropertyChangedCallback((s, e) =>
            {
                XlyHighlightTextBox label = s as XlyHighlightTextBox;
                label._UpdateHighlight();
            })));

        #endregion

        #region HighlightTextForeground -- 高亮字体

        /// <summary>
        /// 高亮字体
        /// </summary>
        public Brush HighlightTextForeground
        {
            get { return (Brush)GetValue(HighlightTextForegroundProperty); }
            set { SetValue(HighlightTextForegroundProperty, value); }
        }

        // Using a DependencyProperty as the backing store for HighlightTextForeground.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HighlightTextForegroundProperty =
            DependencyProperty.Register("HighlightTextForeground", typeof(Brush), typeof(XlyHighlightTextBox), new UIPropertyMetadata(null, new PropertyChangedCallback((s, e) =>
            {
                XlyHighlightTextBox label = s as XlyHighlightTextBox;
                label._UpdateHighlight();
            })));

        #endregion

        #region HighlightTextBrush -- 高亮画刷

        /// <summary>
        /// 高亮画刷
        /// </summary>
        public Brush HighlightTextBrush
        {
            get { return (Brush)GetValue(HighlightTextBrushProperty); }
            set { SetValue(HighlightTextBrushProperty, value); }
        }

        // Using a DependencyProperty as the backing store for HighlightTextBrush.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HighlightTextBrushProperty =
            DependencyProperty.Register("HighlightTextBrush", typeof(Brush), typeof(XlyHighlightTextBox), new UIPropertyMetadata(null, new PropertyChangedCallback((s, e) =>
            {
                XlyHighlightTextBox label = s as XlyHighlightTextBox;
                label._UpdateHighlight();
            })));

        #endregion

        #region LineHeight -- 行高

        /// <summary>
        /// 行高
        /// </summary>
        public double LineHeight
        {
            get { return (double)GetValue(LineHeightProperty); }
            set { SetValue(LineHeightProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LineHeight.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LineHeightProperty =
            DependencyProperty.Register("LineHeight", typeof(double), typeof(XlyHighlightTextBox), new UIPropertyMetadata(18d));

        #endregion

        #region Text -- 文本内容

        /// <summary>
        /// 文本内容
        /// </summary>
        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Text.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(XlyHighlightTextBox), new UIPropertyMetadata(null, new PropertyChangedCallback((s, e) =>
            {
                XlyHighlightTextBox label = s as XlyHighlightTextBox;
                label._UpdateHighlight();
            })));

        #endregion

        #region IsIgnoringCase -- 是否忽略大小写

        /// <summary>
        /// 是否忽略大小写
        /// </summary>
        public bool IsIgnoringCase
        {
            get { return (bool)GetValue(IsIgnoringCaseProperty); }
            set { SetValue(IsIgnoringCaseProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsIgnoringCase.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsIgnoringCaseProperty =
            DependencyProperty.Register("IsIgnoringCase", typeof(bool), typeof(XlyHighlightTextBox), new UIPropertyMetadata(true));

        #endregion

        #region VerticalScrollBarVisibility -- 垂直滚动条可见性

        /// <summary>
        /// 垂直滚动条可见性
        /// </summary>
        public ScrollBarVisibility VerticalScrollBarVisibility
        {
            get { return (ScrollBarVisibility)GetValue(VerticalScrollBarVisibilityProperty); }
            set { SetValue(VerticalScrollBarVisibilityProperty, value); }
        }

        // Using a DependencyProperty as the backing store for VerticalScrollBarVisibility.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty VerticalScrollBarVisibilityProperty =
            DependencyProperty.Register("VerticalScrollBarVisibility", typeof(ScrollBarVisibility), typeof(XlyHighlightTextBox), new UIPropertyMetadata(ScrollBarVisibility.Hidden));

        #endregion

        #region HorizontalScrollBarVisibility -- 水平滚动条可见性

        /// <summary>
        /// 水平滚动条可见性
        /// </summary>
        public ScrollBarVisibility HorizontalScrollBarVisibility
        {
            get { return (ScrollBarVisibility)GetValue(HorizontalScrollBarVisibilityProperty); }
            set { SetValue(HorizontalScrollBarVisibilityProperty, value); }
        }

        // Using a DependencyProperty as the backing store for HorizontalScrollBarVisibility.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HorizontalScrollBarVisibilityProperty =
            DependencyProperty.Register("HorizontalScrollBarVisibility", typeof(ScrollBarVisibility), typeof(XlyHighlightTextBox), new UIPropertyMetadata(ScrollBarVisibility.Hidden));

        #endregion

        #region IsAutoUpdateHighlight -- 是否自动更新高亮部分

        /// <summary>
        /// 是否自动更新高亮部分
        /// </summary>
        public bool IsAutoUpdateHighlight
        {
            get { return (bool)GetValue(IsAutoUpdateHighlightProperty); }
            set { SetValue(IsAutoUpdateHighlightProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsAutoUpdateHighlight.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsAutoUpdateHighlightProperty =
            DependencyProperty.Register("IsAutoUpdateHighlight", typeof(bool), typeof(XlyHighlightTextBox), new UIPropertyMetadata(true));

        #endregion

        ///<summary>
        /// 强制更新高亮
        /// </summary>
        public void UpdateHighlight()
        {
            if (this.PART_RichTextBox == null)
                this.ApplyTemplate();
            if (this.PART_RichTextBox == null)
                return;
            this.PART_RichTextBox.Document.Blocks.Clear();
            if (string.IsNullOrWhiteSpace(this.Text))
                return;

            if (this.HighlightMode == XlyHighlightMode.Text)
                this._UpdateHighlightWidthText();
            else
                this._UpdateHighlightWidthOffset();
        }

        /// <summary>
        /// 更新高亮
        /// </summary>
        private void _UpdateHighlight()
        {
            if (!this.IsAutoUpdateHighlight)
                return;
            this.UpdateHighlight();
        }

        /// <summary>
        /// 根据文本内容进行高亮
        /// </summary>
        private void _UpdateHighlightWidthText()
        {
            if (string.IsNullOrWhiteSpace(this.HighlightText) || 
                (!this.HighlightText.Contains("\\s*[\\.．▪●★☆]?\\s*") && 
                this.HighlightText.Length > this.Text.Length))
            {
                Paragraph temp_p = new Paragraph();
                temp_p.LineHeight = this.LineHeight;
                Run temp_r = this._GetRun();
                temp_r.Text = this.Text;
                temp_p.Inlines.Add(temp_r);
                this.PART_RichTextBox.Document.Blocks.Add(temp_p);
                return;
            }
            string text = null;
            string text_h = null;
            if (this.IsIgnoringCase)
            {
                text = this.Text.ToLower();
                text_h = this.HighlightText.ToLower();
            }
            else
            {
                text = this.Text;
                text_h = this.HighlightText;
            }
            int index = 0;
            try
            {
                if (text_h.Contains("\\s*[\\.．▪●★☆]?\\s*"))
                {
                    List<Tuple<int, string>> index_list = new List<Tuple<int, string>>();
                    var matchs = Regex.Matches(text, text_h);
                    if (matchs.IsValid())
                    {
                        index_list.AddRange(from Match match in matchs
                            select new Tuple<int, string>(match.Index, match.Value));
                    }
                    if (index_list.Count == 0)
                    {
                        this.Text = this.Text;
                        Paragraph temp_p = new Paragraph();
                        temp_p.LineHeight = this.LineHeight;
                        Run temp_r = this._GetRun();
                        temp_r.Text = this.Text;
                        temp_p.Inlines.Add(temp_r);
                        this.PART_RichTextBox.Document.Blocks.Add(temp_p);
                        return;
                    }
                    index = 0;
                    Paragraph p = new Paragraph();
                    p.LineHeight = this.LineHeight;
                    bool isFirst = true;
                    for (int i = 0; i < index_list.Count; ++i)
                    {
                        Run r = this._GetRun();
                        r.Text = this.Text.Substring(index, index_list[i].Item1 - index);
                        p.Inlines.Add(r);
                        Run r_h = this._GetRun();
                        r_h.Text = this.Text.Substring(index_list[i].Item1, index_list[i].Item2.Length);
                        r_h.Background = this.HighlightTextBrush;
                        r_h.Foreground = this.HighlightTextForeground;
                        if (isFirst)
                        {
                            r_h.Loaded += new RoutedEventHandler(r_h_Loaded);
                            isFirst = false;
                        }
                        p.Inlines.Add(r_h);
                        index = index_list[i].Item1 + index_list[i].Item2.Length;
                    }
                    Run r_end = this._GetRun();
                    r_end.Text =
                        this.Text.Substring(
                            index_list[index_list.Count - 1].Item1 + index_list[index_list.Count - 1].Item2.Length,
                            this.Text.Length - index_list[index_list.Count - 1].Item1 -
                            index_list[index_list.Count - 1].Item2.Length);
                    p.Inlines.Add(r_end);
                    this.PART_RichTextBox.Document.Blocks.Add(p);
                }
                else
                {
                    List<int> index_list = new List<int>();
                    while ((index = text.IndexOf(text_h, index)) != -1)
                    {
                        index_list.Add(index++);
                    }
                    if (index_list.Count == 0)
                    {
                        this.Text = this.Text;
                        Paragraph temp_p = new Paragraph();
                        temp_p.LineHeight = this.LineHeight;
                        Run temp_r = this._GetRun();
                        temp_r.Text = this.Text;
                        temp_p.Inlines.Add(temp_r);
                        this.PART_RichTextBox.Document.Blocks.Add(temp_p);
                        return;
                    }
                    index = 0;
                    Paragraph p = new Paragraph();
                    p.LineHeight = this.LineHeight;
                    bool isFirst = true;
                    for (int i = 0; i < index_list.Count; ++i)
                    {
                        Run r = this._GetRun();
                        r.Text = this.Text.Substring(index, index_list[i] - index);
                        p.Inlines.Add(r);
                        Run r_h = this._GetRun();
                        r_h.Text = this.Text.Substring(index_list[i], this.HighlightText.Length);
                        r_h.Background = this.HighlightTextBrush;
                        r_h.Foreground = this.HighlightTextForeground;
                        if (isFirst)
                        {
                            r_h.Loaded += new RoutedEventHandler(r_h_Loaded);
                            isFirst = false;
                        }
                        p.Inlines.Add(r_h);
                        index = index_list[i] + this.HighlightText.Length;
                    }
                    Run r_end = this._GetRun();
                    r_end.Text = this.Text.Substring(index_list[index_list.Count - 1] + this.HighlightText.Length,
                        this.Text.Length - index_list[index_list.Count - 1] - this.HighlightText.Length);
                    p.Inlines.Add(r_end);
                    this.PART_RichTextBox.Document.Blocks.Add(p);
                }
            }
            catch (Exception ex)
            {
                this.Text = this.Text;
                Paragraph temp_p = new Paragraph();
                temp_p.LineHeight = this.LineHeight;
                Run temp_r = this._GetRun();
                temp_r.Text = this.Text;
                temp_p.Inlines.Add(temp_r);
                this.PART_RichTextBox.Document.Blocks.Add(temp_p);
            }
        }

        /// <summary>
        /// 根据偏移量进行高亮
        /// </summary>
        private void _UpdateHighlightWidthOffset()
        {
            if (this.Text == null
                || this.HighlightOffset > this.Text.Length
                || this.HighlightOffset < 0
                || this.HighlightOffset + this.HighlightLength > this.Text.Length)
            {
                Paragraph temp_p = new Paragraph();
                temp_p.LineHeight = this.LineHeight;
                Run temp_r = this._GetRun();
                temp_r.Text = this.Text;
                temp_p.Inlines.Add(temp_r);
                this.PART_RichTextBox.Document.Blocks.Add(temp_p);
                return;
            }
            Paragraph p = new Paragraph();
            p.LineHeight = this.LineHeight;
            Run r1 = this._GetRun();
            r1.Text = this.Text.Substring(0, this.HighlightOffset);
            p.Inlines.Add(r1);
            Run r2 = this._GetRun();
            r2.Text = this.Text.Substring(this.HighlightOffset, this.HighlightLength);
            r2.Background = this.HighlightTextBrush;
            r2.Foreground = this.HighlightTextForeground;
            r2.Loaded += new RoutedEventHandler(r_h_Loaded);
            p.Inlines.Add(r2);
            Run r3 = this._GetRun();
            r3.Text = this.Text.Substring(this.HighlightOffset + this.HighlightLength, this.Text.Length - this.HighlightOffset - this.HighlightLength);
            p.Inlines.Add(r3);
            this.PART_RichTextBox.Document.Blocks.Add(p);
        }

        /// <summary>
        /// 获取Run标签
        /// </summary>
        private Run _GetRun()
        {
            Run run = new Run();
            run.Foreground = this.Foreground;
            run.FontSize = this.FontSize;
            run.FontFamily = this.FontFamily;
            run.FontWeight = this.FontWeight;
            run.FontStyle = this.FontStyle;
            run.FontStretch = this.FontStretch;
            return run;
        }

        private void r_h_Loaded(object sender, RoutedEventArgs e)
        {
            Run run = ((Run)sender);
            run.BringIntoView();
            run.Loaded -= new RoutedEventHandler(r_h_Loaded);
        }
    }
}
