using GalaSoft.MvvmLight.Command;
using System.ComponentModel.Composition;
using System.IO;
using System.Windows.Input;
using XLY.SF.Framework.Core.Base.ViewModel;
using XLY.SF.Project.Views.PreviewFile;

namespace XLY.SF.Shell
{
    /// <summary>
    /// TestPage.xaml 的交互逻辑
    /// </summary>
    [Export("TestPage", typeof(UcViewBase))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class TestPage : UcViewBase
    {
        public TestPage()
        {
            InitializeComponent();            
        }

        [Import("TestPageViewPage", typeof(ViewModelBase))]
        public override ViewModelBase DataSource
        {
            get
            {
                return this.DataContext as ViewModelBase;
            }
            set
            {
                value.SetViewContainer(this);
                this.DataContext = value;
            }
        }
    }

    [Export("TestPageViewPage", typeof(ViewModelBase))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class TestPageViewModel : ViewModelBase
    {
        [ImportingConstructor]
        public TestPageViewModel()
        {
            PreViewCtl = new PreViewControl();
            IntializeDir();
        }


        PathCollection pathCollection = new PathCollection();

        private void IntializeDir()
        {
            string[] files=Directory.GetFiles(@".\", "*.*");            
            pathCollection.AddPaths(files);

            PreViewCtl.ReplaceContent(pathCollection.GetPathByIndex(0));
            Next = new RelayCommand(NextFile);
            Last = new RelayCommand(LastFile);
        }

        private void NextFile()
        {
            PreViewCtl.ReplaceContent(pathCollection.GetNextPath());
        }
        private void LastFile()
        {
            PreViewCtl.ReplaceContent(pathCollection.GetPreviousPath());
        }

        public PreViewControl PreViewCtl { get; private set; }        
        public ICommand Next { get; private set; }
        public ICommand Last { get; private set; }

        
    }
}
