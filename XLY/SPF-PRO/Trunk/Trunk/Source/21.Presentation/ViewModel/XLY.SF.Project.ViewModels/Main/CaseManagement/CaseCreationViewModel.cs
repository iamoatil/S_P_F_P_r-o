using GalaSoft.MvvmLight.CommandWpf;
using ProjectExtend.Context;
using System;
using System.ComponentModel.Composition;
using System.Windows.Input;
using XLY.SF.Framework.Core.Base.CoreInterface;
using XLY.SF.Framework.Core.Base.ViewModel;
using XLY.SF.Framework.Language;
using XLY.SF.Project.CaseManagement;
using XLY.SF.Project.Models;
using XLY.SF.Project.Models.Logical;
using XLY.SF.Project.ViewDomain.MefKeys;

namespace XLY.SF.Project.ViewModels.Main.CaseManagement
{
    [Export(ExportKeys.CaseCreationViewModel, typeof(ViewModelBase))]
    public class CaseCreationViewModel : ViewModelBase
    {
        #region Fields

        #endregion

        #region Constructors

        public CaseCreationViewModel()
        {
            ConfirmConmmand = new RelayCommand(Confirm, CanConfirm);
            UpdateCaseTypeCommand = new RelayCommand(UpdateCasetType);
        }

        #endregion

        #region Properties

        #region CaseInfo

        private CaseInfo _caseInfo;

        public CaseInfo CaseInfo
        {
            get { return _caseInfo; }
            set
            {
                _caseInfo = value;
                OnPropertyChanged();
            }
        }

        #endregion

        public ICommand ConfirmConmmand { get; }

        public ICommand UpdateCaseTypeCommand { get; }

        [Import(typeof(IMessageBox))]
        private IMessageBox MessageBox
        {
            get;
            set;
        }

        [Import(typeof(IDatabaseContext))]
        private IDatabaseContext DbService
        {
            get;
            set;
        }

        #endregion

        #region Methods

        #region Public

        public override void ViewClosed()
        {
            CaseInfo = null;
        }

        #endregion

        #region Protected

        protected override void LoadCore(object parameters)
        {
            NewCaseInfo();
        }

        #endregion

        #region Private

        private void Confirm()
        {
            Case newCase = Case.New(CaseInfo);
            if (newCase == null) return;
            SystemContext.Instance.CurrentCase = newCase;
            RecentCaseEntityModel model = new RecentCaseEntityModel
            {
                CaseID = CaseInfo.Id,
                Author = CaseInfo.Author,
                Name = CaseInfo.Name,
                Number = CaseInfo.Number,
                Type = CaseInfo.Type,
                Timestamp = CaseInfo.Timestamp,
                CaseProjectFile = newCase.Configuration.FullPath,
            };
            if (!DbService.Add(model))
            {
                MessageBox.ShowDialogErrorMsg(LanguageHelperSingle.Instance.GetLanguageByKey(Languagekeys.ViewLanguage_View_UpdateRecentError));
            }
            else
            {
                NewCaseInfo();
                NavigationForMainWindow(ExportKeys.DeviceSelectView);
            }
        }

        private Boolean CanConfirm()
        {
            return !(String.IsNullOrWhiteSpace(CaseInfo.Name)
                || String.IsNullOrWhiteSpace(CaseInfo.Number)
                || String.IsNullOrWhiteSpace(CaseInfo.Author)
                || String.IsNullOrWhiteSpace(CaseInfo.Type));
        }

        private void UpdateCasetType()
        {
        }

        private void NewCaseInfo()
        {
            CaseInfo = new CaseInfo()
            {
                Name = "默认案例",
                Number = DateTime.Now.ToString("yyyyMMddhhmmss"),
                Author = SystemContext.Instance.CurUserInfo.UserName,
                Path = SystemContext.Instance.CaseSaveFullPath,
                Type = "临时的",
            };
        }

        #endregion

        #endregion
    }
}
