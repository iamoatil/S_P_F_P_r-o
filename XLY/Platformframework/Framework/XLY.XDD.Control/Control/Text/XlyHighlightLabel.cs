using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Documents;
using System.Windows.Controls;

namespace XLY.XDD.Control
{
    /// <summary>
    /// 高亮标签
    /// </summary>
    [TemplatePart(Name = "PART_TextBlock", Type = typeof(TextBlock))]
    public class XlyHighlightLabel : System.Windows.Controls.Control
    {
        static XlyHighlightLabel()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(XlyHighlightLabel), new FrameworkPropertyMetadata(typeof(XlyHighlightLabel)));
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            this.PART_TextBlock = this.Template.FindName("PART_TextBlock", this) as TextBlock;
            this._UpdateHighlight();
        }

        #region PART

        private TextBlock PART_TextBlock;

        #endregion

        #region Text -- 内容

        /// <summary>
        /// 内容
        /// </summary>
        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Text.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(XlyHighlightLabel), new UIPropertyMetadata(null, new PropertyChangedCallback((s, e) =>
            {
                XlyHighlightLabel label = s as XlyHighlightLabel;
                label._UpdateHighlight();
            })));

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
            DependencyProperty.Register("HighlightText", typeof(string), typeof(XlyHighlightLabel), new UIPropertyMetadata(null, new PropertyChangedCallback((s, e) =>
            {
                XlyHighlightLabel label = s as XlyHighlightLabel;
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
            DependencyProperty.Register("HighlightOffset", typeof(int), typeof(XlyHighlightLabel), new UIPropertyMetadata(0, new PropertyChangedCallback((s, e) =>
            {
                XlyHighlightLabel label = s as XlyHighlightLabel;
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
            DependencyProperty.Register("HighlightLength", typeof(int), typeof(XlyHighlightLabel), new UIPropertyMetadata(0, new PropertyChangedCallback((s, e) =>
            {
                XlyHighlightLabel label = s as XlyHighlightLabel;
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
            DependencyProperty.Register("HighlightMode", typeof(XlyHighlightMode), typeof(XlyHighlightLabel), new UIPropertyMetadata(XlyHighlightMode.Text, new PropertyChangedCallback((s, e) =>
            {
                XlyHighlightLabel label = s as XlyHighlightLabel;
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
            DependencyProperty.Register("HighlightTextForeground", typeof(Brush), typeof(XlyHighlightLabel), new UIPropertyMetadata(null, new PropertyChangedCallback((s, e) =>
            {
                XlyHighlightLabel label = s as XlyHighlightLabel;
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
            DependencyProperty.Register("HighlightTextBrush", typeof(Brush), typeof(XlyHighlightLabel), new UIPropertyMetadata(null, new PropertyChangedCallback((s, e) =>
            {
                XlyHighlightLabel label = s as XlyHighlightLabel;
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
            DependencyProperty.Register("IsIgnoringCase", typeof(bool), typeof(XlyHighlightLabel), new UIPropertyMetadata(true, new PropertyChangedCallback((s, e) =>
            {
                XlyHighlightLabel label = s as XlyHighlightLabel;
                label._UpdateHighlight();
            })));

        #endregion

        #region TextTrimming -- 文本修整

        /// <summary>
        /// 文本修整
        /// </summary>
        public TextTrimming TextTrimming
        {
            get { return (TextTrimming)GetValue(TextTrimmingProperty); }
            set { SetValue(TextTrimmingProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TextTrimming.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TextTrimmingProperty =
            DependencyProperty.Register("TextTrimming", typeof(TextTrimming), typeof(XlyHighlightLabel), new UIPropertyMetadata(System.Windows.TextTrimming.CharacterEllipsis));

        #endregion

        #region TextWrapping -- 文本流方式

        /// <summary>
        /// 文本流方式
        /// </summary>
        public TextWrapping TextWrapping
        {
            get { return (TextWrapping)GetValue(TextWrappingProperty); }
            set { SetValue(TextWrappingProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TextWrapping.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TextWrappingProperty =
            DependencyProperty.Register("TextWrapping", typeof(TextWrapping), typeof(XlyHighlightLabel), new UIPropertyMetadata(System.Windows.TextWrapping.NoWrap));

        #endregion

        #region IsAutoUpdateHighlight -- 是否自动更新高亮

        /// <summary>
        /// 是否自动更新高亮
        /// </summary>
        public bool IsAutoUpdateHighlight
        {
            get { return (bool)GetValue(IsAutoUpdateHighlightProperty); }
            set { SetValue(IsAutoUpdateHighlightProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsAutoUpdateHighlight.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsAutoUpdateHighlightProperty =
            DependencyProperty.Register("IsAutoUpdateHighlight", typeof(bool), typeof(XlyHighlightLabel), new UIPropertyMetadata(true));

        #endregion

        /// <summary>
        /// 强制更新高亮
        /// </summary>
        public void UpdateHighlight()
        {
            if (this.PART_TextBlock == null)
                this.ApplyTemplate();
            if (this.PART_TextBlock == null)
                return;
            this.PART_TextBlock.Inlines.Clear();
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
            if (string.IsNullOrWhiteSpace(this.HighlightText) || this.HighlightText.Length > this.Text.Length)
            {
                this.PART_TextBlock.Text = this.Text;
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
            List<int> index_list = new List<int>();
            while ((index = text.IndexOf(text_h, index)) != -1)
            {
                index_list.Add(index);
                index += this.HighlightText.Length;
            }
            if (index_list.Count == 0)
            {
                this.PART_TextBlock.Text = this.Text;
                return;
            }

            index = 0;
            for (int i = 0; i < index_list.Count; ++i)
            {
                Run r = this._GetRun();
                r.Text = this.Text.Substring(index, index_list[i] - index);
                this.PART_TextBlock.Inlines.Add(r);
                Run r_h = this._GetRun();
                r_h.Text = this.Text.Substring(index_list[i], this.HighlightText.Length);
                r_h.Background = this.HighlightTextBrush;
                r_h.Foreground = this.HighlightTextForeground;
                this.PART_TextBlock.Inlines.Add(r_h);
                index = index_list[i] + this.HighlightText.Length;
            }
            Run r_end = this._GetRun();
            r_end.Text = this.Text.Substring(index_list[index_list.Count - 1] + this.HighlightText.Length, this.Text.Length - index_list[index_list.Count - 1] - this.HighlightText.Length);
            this.PART_TextBlock.Inlines.Add(r_end);
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
                this.PART_TextBlock.Text = this.Text;
                return;
            }
            Run r1 = this._GetRun();
            r1.Text = this.Text.Substring(0, this.HighlightOffset);
            this.PART_TextBlock.Inlines.Add(r1);
            Run r2 = this._GetRun();
            r2.Text = this.Text.Substring(this.HighlightOffset, this.HighlightLength);
            r2.Background = this.HighlightTextBrush;
            r2.Foreground = this.HighlightTextForeground;
            this.PART_TextBlock.Inlines.Add(r2);
            Run r3 = this._GetRun();
            r3.Text = this.Text.Substring(this.HighlightOffset + this.HighlightLength, this.Text.Length - this.HighlightOffset - this.HighlightLength);
            this.PART_TextBlock.Inlines.Add(r3);
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
    }
}
