using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.ComponentModel;
using System.Windows.Forms;
using XLY.XDD.Control;

namespace Be.Windows.Forms
{
    /// <summary>
    /// Defines a build-in ContextMenuStrip manager for HexBox control to show Copy, Cut, Paste menu in contextmenu of the control.
    /// </summary>
    [TypeConverterAttribute(typeof(ExpandableObjectConverter))]
    public sealed class BuiltInContextMenu : Component
    {
        /// <summary>
        /// 16进制视图
        /// </summary>
        HexViewer _hexViewer;
        /// <summary>
        /// Contains the HexBox control.
        /// </summary>
        HexBox _hexBox;
        /// <summary>
        /// Contains the ContextMenuStrip control.
        /// </summary>
        ContextMenuStrip _contextMenuStrip;
        /// <summary>
        /// Contains the "Cut"-ToolStripMenuItem object.
        /// </summary>
        ToolStripMenuItem _cutToolStripMenuItem;
        /// <summary>
        /// Contains the "Copy"-ToolStripMenuItem object.
        /// </summary>
        ToolStripMenuItem _copyToolStripMenuItem;
        /// <summary>
        /// Contains the "Paste"-ToolStripMenuItem object.
        /// </summary>
        ToolStripMenuItem _pasteToolStripMenuItem;

        // 添加右键

        /// <summary>
        /// ASCII编码
        /// </summary>
        ToolStripMenuItem _ASCII;

        /// <summary>
        /// UTF-8编码
        /// </summary>
        ToolStripMenuItem _UTF8;

        /// <summary>
        /// Unicode编码
        /// </summary>
        ToolStripMenuItem _Unicode;

        /// <summary>
        /// Big5编码
        /// </summary>
        ToolStripMenuItem _Big5;

        /// <summary>
        /// Gb2312编码
        /// </summary>
        ToolStripMenuItem _Gb2312;

        /// <summary>
        /// Base64编码
        /// </summary>
        ToolStripMenuItem _Base64;


        /// <summary>
        /// Contains the "Select All"-ToolStripMenuItem object.
        /// </summary>
        ToolStripMenuItem _selectAllToolStripMenuItem;
        /// <summary>
        /// Initializes a new instance of BuildInContextMenu class.
        /// </summary>
        /// <param name="hexBox">the HexBox control</param>
        internal BuiltInContextMenu(HexBox hexBox, HexViewer viewer)
        {
            _hexBox = hexBox;
            _hexViewer = viewer;
            _hexBox.ByteProviderChanged += new EventHandler(HexBox_ByteProviderChanged);
        }
        /// <summary>
        /// If ByteProvider
        /// </summary>
        /// <param name="sender">the sender object</param>
        /// <param name="e">the event data</param>
        void HexBox_ByteProviderChanged(object sender, EventArgs e)
        {
            CheckBuiltInContextMenu();
        }

        /// <summary>
        /// Assigns the ContextMenuStrip control to the HexBox control.
        /// </summary>
        void CheckBuiltInContextMenu()
        {
            if (this.DesignMode)
                return;

            if (this._contextMenuStrip == null)//创建菜单
            {
                // 添加右键
                ContextMenuStrip cms = new ContextMenuStrip();
                cms.Opening += new CancelEventHandler(cms_Opening);

                System.Windows.Controls.ContextMenu cm_wpf = new System.Windows.Controls.ContextMenu();

                if (!this._hexViewer.IsUserContextMenuOverride)
                {
                    // ASCII
                    _ASCII = new ToolStripMenuItem(ASCIIMenuItemTextInternal, CutMenuItemImage, new EventHandler(ASCII_Click));
                    cms.Items.Add(_ASCII);

                    System.Windows.Controls.MenuItem ascii_mi_wpf = new System.Windows.Controls.MenuItem();
                    ascii_mi_wpf.Header = ASCIIMenuItemTextInternal;
                    ascii_mi_wpf.Click += new System.Windows.RoutedEventHandler(ASCII_Click);
                    cm_wpf.Items.Add(ascii_mi_wpf);

                    // UTF8
                    _UTF8 = new ToolStripMenuItem(UTF8MenuItemTextInternal, CutMenuItemImage, new EventHandler(UTF8_Click));
                    cms.Items.Add(_UTF8);

                    System.Windows.Controls.MenuItem utf8_mi_wpf = new System.Windows.Controls.MenuItem();
                    utf8_mi_wpf.Header = UTF8MenuItemTextInternal;
                    utf8_mi_wpf.Click += new System.Windows.RoutedEventHandler(UTF8_Click);
                    cm_wpf.Items.Add(utf8_mi_wpf);

                    // Unicode
                    _Unicode = new ToolStripMenuItem(UnicodeMenuItemTextInternal, CutMenuItemImage, new EventHandler(Unicode_Click));
                    cms.Items.Add(_Unicode);

                    System.Windows.Controls.MenuItem unicode_mi_wpf = new System.Windows.Controls.MenuItem();
                    unicode_mi_wpf.Header = UnicodeMenuItemTextInternal;
                    unicode_mi_wpf.Click += new System.Windows.RoutedEventHandler(Unicode_Click);
                    cm_wpf.Items.Add(unicode_mi_wpf);

                    // Big5
                    _Big5 = new ToolStripMenuItem(Big5MenuItemTextInternal, CutMenuItemImage, new EventHandler(Big5_Click));
                    cms.Items.Add(_Big5);

                    System.Windows.Controls.MenuItem big5_mi_wpf = new System.Windows.Controls.MenuItem();
                    big5_mi_wpf.Header = Big5MenuItemTextInternal;
                    big5_mi_wpf.Click += new System.Windows.RoutedEventHandler(Big5_Click);
                    cm_wpf.Items.Add(big5_mi_wpf);

                    // Gb2312
                    _Gb2312 = new ToolStripMenuItem(Gb2312MenuItemTextInternal, CutMenuItemImage, new EventHandler(Gb2312_Click));
                    cms.Items.Add(_Gb2312);

                    System.Windows.Controls.MenuItem gb2312_mi_wpf = new System.Windows.Controls.MenuItem();
                    gb2312_mi_wpf.Header = Gb2312MenuItemTextInternal;
                    gb2312_mi_wpf.Click += new System.Windows.RoutedEventHandler(Gb2312_Click);
                    cm_wpf.Items.Add(gb2312_mi_wpf);

                    /*// Base64
                    _Base64 = new ToolStripMenuItem(Base64MenuItemTextInternal, CutMenuItemImage, new EventHandler(Base64_Click));
                    cms.Items.Add(_Base64);
                    // ☆★☆★☆★☆★Base64应该是一个加密方式，不是编码方式。不应该出现在这里的。暂时注释掉这个菜单按钮。
                    System.Windows.Controls.MenuItem base64_mi_wpf = new System.Windows.Controls.MenuItem();
                    base64_mi_wpf.Header = Base64MenuItemTextInternal;
                    base64_mi_wpf.Click += new System.Windows.RoutedEventHandler(Base64_Click);
                    cm_wpf.Items.Add(base64_mi_wpf);*/

                    // Copy
                    _copyToolStripMenuItem = new ToolStripMenuItem(CopyMenuItemTextInternal, CopyMenuItemImage, new EventHandler(CopyMenuItem_Click));
                    cms.Items.Add(_copyToolStripMenuItem);

                    System.Windows.Controls.MenuItem copy_mi_wpf = new System.Windows.Controls.MenuItem();
                    copy_mi_wpf.Header = CopyMenuItemTextInternal;
                    copy_mi_wpf.Click += new System.Windows.RoutedEventHandler(CopyMenuItem_Click);
                    cm_wpf.Items.Add(copy_mi_wpf);

                    //  _pasteToolStripMenuItem = new ToolStripMenuItem(PasteMenuItemTextInternal, PasteMenuItemImage, new EventHandler(PasteMenuItem_Click));
                    //    cms.Items.Add(_pasteToolStripMenuItem);

                    // SelectAll
                    _selectAllToolStripMenuItem = new ToolStripMenuItem(SelectAllMenuItemTextInternal, SelectAllMenuItemImage, new EventHandler(SelectAllMenuItem_Click));
                    cms.Items.Add(_selectAllToolStripMenuItem);

                    System.Windows.Controls.MenuItem selectall_mi_wpf = new System.Windows.Controls.MenuItem();
                    selectall_mi_wpf.Header = SelectAllMenuItemTextInternal;
                    selectall_mi_wpf.Click += new System.Windows.RoutedEventHandler(SelectAllMenuItem_Click);
                    cm_wpf.Items.Add(selectall_mi_wpf);

                    cms.Items.Add(new ToolStripSeparator());
                }


                if (this._hexBox != null && this._hexViewer.ContextMenuActions != null)
                {
                    for (int i = 0; i < this._hexViewer.ContextMenuActions.Count; ++i)
                    {
                        MenuActionInfo info = this._hexViewer.ContextMenuActions[i];

                        System.Windows.Controls.MenuItem item = new System.Windows.Controls.MenuItem();
                        item.Header = info.Header;
                        item.Click += new System.Windows.RoutedEventHandler((s, e) => { info.DoOnClick(this._hexViewer, e); });
                        cm_wpf.Items.Add(item);
                    }

                    cms.Opening += new CancelEventHandler(BuildInContextMenuStrip_Opening);

                    this._hexViewer.ContextMenu = cm_wpf;
                }

                _contextMenuStrip = cms;
            }

            if (this._hexBox.ByteProvider == null && this._hexBox.ContextMenuStrip != null)
                this._hexBox.ContextMenuStrip = null;
            else if (this._hexBox.ByteProvider != null && this._hexBox.ContextMenuStrip == null)
                this._hexBox.ContextMenuStrip = _contextMenuStrip;
        }

        /// <summary>
        /// 截获右键菜单
        /// </summary>
        private void cms_Opening(object sender, CancelEventArgs e)
        {
            if (this._hexViewer != null && this._hexViewer.ContextMenuActions != null && this._hexViewer.ContextMenu != null)
            {
                this._hexViewer.ContextMenu.IsOpen = true;
                e.Cancel = true;
            }
        }

        /// <summary>
        /// Before opening the ContextMenuStrip, we manage the availability of the items.
        /// </summary>
        /// <param name="sender">the sender object</param>
        /// <param name="e">the event data</param>
        void BuildInContextMenuStrip_Opening(object sender, CancelEventArgs e)
        {
            // _cutToolStripMenuItem.Enabled = this._hexBox.CanCut();
            _copyToolStripMenuItem.Enabled = this._hexBox.CanCopy();
            // _pasteToolStripMenuItem.Enabled = this._hexBox.CanPaste();
            _selectAllToolStripMenuItem.Enabled = this._hexBox.CanSelect();
        }

        //添加右键

        /// <summary>
        /// 选择ASCII字符后的事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void ASCII_Click(object sender, EventArgs e) { this._hexBox.ChangeASCII(); }
        /// <summary>
        /// 选择UTF8字符后的事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void UTF8_Click(object sender, EventArgs e) { this._hexBox.ChangeUTF8(); }
        /// <summary>
        /// 选择Unicode字符后的事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Unicode_Click(object sender, EventArgs e) { this._hexBox.ChangeUnicode(); }

        /// <summary>
        /// 选择big5字符后的事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Big5_Click(object sender, EventArgs e) { this._hexBox.ChangeBig5(); }

        /// <summary>
        /// 选择gb2312字符后的事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Gb2312_Click(object sender, EventArgs e) { this._hexBox.ChangeBGb2312(); }

        /// <summary>
        /// 选择base64字符后的事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Base64_Click(object sender, EventArgs e) { this._hexBox.ChangeBase64(); }

        /// <summary>
        /// The handler for the "Cut"-Click event
        /// </summary>
        /// <param name="sender">the sender object</param>
        /// <param name="e">the event data</param>
        void CutMenuItem_Click(object sender, EventArgs e) { this._hexBox.Copy(); }
        /// <summary>
        /// The handler for the "Copy"-Click event
        /// </summary>
        /// <param name="sender">the sender object</param>
        /// <param name="e">the event data</param>
        void CopyMenuItem_Click(object sender, EventArgs e) { this._hexBox.Copy(); }
        /// <summary>
        /// The handler for the "Paste"-Click event
        /// </summary>
        /// <param name="sender">the sender object</param>
        /// <param name="e">the event data</param>
        void PasteMenuItem_Click(object sender, EventArgs e) { this._hexBox.Copy(); }
        /// <summary>
        /// The handler for the "Select All"-Click event
        /// </summary>
        /// <param name="sender">the sender object</param>
        /// <param name="e">the event data</param>
        void SelectAllMenuItem_Click(object sender, EventArgs e) { this._hexBox.SelectAll(); }
        /// <summary>
        /// Gets or sets the custom text of the "Copy" ContextMenuStrip item.
        /// </summary>
        [Category("BuiltIn-ContextMenu"), DefaultValue(null), Localizable(true)]
        public string CopyMenuItemText
        {
            get { return _copyMenuItemText; }
            set { _copyMenuItemText = value; }
        } string _copyMenuItemText;

        /// <summary>
        /// Gets or sets the custom text of the "Cut" ContextMenuStrip item.
        /// </summary>
        [Category("BuiltIn-ContextMenu"), DefaultValue(null), Localizable(true)]
        public string CutMenuItemText
        {
            get { return _cutMenuItemText; }
            set { _cutMenuItemText = value; }
        } string _cutMenuItemText;

        /// <summary>
        /// Gets or sets the custom text of the "Paste" ContextMenuStrip item.
        /// </summary>
        [Category("BuiltIn-ContextMenu"), DefaultValue(null), Localizable(true)]
        public string PasteMenuItemText
        {
            get { return _pasteMenuItemText; }
            set { _pasteMenuItemText = value; }
        } string _pasteMenuItemText;

        /// <summary>
        /// Gets or sets the custom text of the "Select All" ContextMenuStrip item.
        /// </summary>
        [Category("BuiltIn-ContextMenu"), DefaultValue(null), Localizable(true)]
        public string SelectAllMenuItemText
        {
            get { return _selectAllMenuItemText; }
            set { _selectAllMenuItemText = value; }
        } string _selectAllMenuItemText = null;


        // 添加右键
        internal string ASCIIMenuItemTextInternal { get { return !string.IsNullOrEmpty(CutMenuItemText) ? CutMenuItemText : "ASCII"; } }
        internal string UTF8MenuItemTextInternal { get { return !string.IsNullOrEmpty(CutMenuItemText) ? CutMenuItemText : "UTF-8"; } }
        internal string UnicodeMenuItemTextInternal { get { return !string.IsNullOrEmpty(CutMenuItemText) ? CutMenuItemText : "Unicode"; } }
        internal string Big5MenuItemTextInternal { get { return !string.IsNullOrEmpty(CutMenuItemText) ? CutMenuItemText : "Big5"; } }
        internal string Gb2312MenuItemTextInternal { get { return !string.IsNullOrEmpty(CutMenuItemText) ? CutMenuItemText : "Gb2312"; } }
        internal string Base64MenuItemTextInternal { get { return !string.IsNullOrEmpty(CutMenuItemText) ? CutMenuItemText : "Base64"; } }

        /// <summary>
        /// Gets the text of the "Cut" ContextMenuStrip item.
        /// </summary>
        internal string CutMenuItemTextInternal { get { return !string.IsNullOrEmpty(CutMenuItemText) ? CutMenuItemText : "剪切"; } }
        /// <summary>
        /// Gets the text of the "Copy" ContextMenuStrip item.
        /// </summary>
        internal string CopyMenuItemTextInternal { get { return !string.IsNullOrEmpty(CopyMenuItemText) ? CopyMenuItemText : "复制"; } }
        /// <summary>
        /// Gets the text of the "Paste" ContextMenuStrip item.
        /// </summary>
        internal string PasteMenuItemTextInternal { get { return !string.IsNullOrEmpty(PasteMenuItemText) ? PasteMenuItemText : "粘贴"; } }
        /// <summary>
        /// Gets the text of the "Select All" ContextMenuStrip item.
        /// </summary>
        internal string SelectAllMenuItemTextInternal { get { return !string.IsNullOrEmpty(SelectAllMenuItemText) ? SelectAllMenuItemText : "全选"; } }

        /// <summary>
        /// Gets or sets the image of the "Cut" ContextMenuStrip item.
        /// </summary>
        [Category("BuiltIn-ContextMenu"), DefaultValue(null)]
        public Image CutMenuItemImage
        {
            get { return _cutMenuItemImage; }
            set { _cutMenuItemImage = value; }
        } Image _cutMenuItemImage = null;
        /// <summary>
        /// Gets or sets the image of the "Copy" ContextMenuStrip item.
        /// </summary>
        [Category("BuiltIn-ContextMenu"), DefaultValue(null)]
        public Image CopyMenuItemImage
        {
            get { return _copyMenuItemImage; }
            set { _copyMenuItemImage = value; }
        } Image _copyMenuItemImage = null;
        /// <summary>
        /// Gets or sets the image of the "Paste" ContextMenuStrip item.
        /// </summary>
        [Category("BuiltIn-ContextMenu"), DefaultValue(null)]
        public Image PasteMenuItemImage
        {
            get { return _pasteMenuItemImage; }
            set { _pasteMenuItemImage = value; }
        } Image _pasteMenuItemImage = null;
        /// <summary>
        /// Gets or sets the image of the "Select All" ContextMenuStrip item.
        /// </summary>
        [Category("BuiltIn-ContextMenu"), DefaultValue(null)]
        public Image SelectAllMenuItemImage
        {
            get { return _selectAllMenuItemImage; }
            set { _selectAllMenuItemImage = value; }
        } Image _selectAllMenuItemImage = null;
    }
}
