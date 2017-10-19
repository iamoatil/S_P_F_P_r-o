using GalaSoft.MvvmLight.CommandWpf;
using ProjectExtend.Context;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows.Input;
using XLY.SF.Framework.Core.Base.CoreInterface;
using XLY.SF.Framework.Core.Base.ViewModel;
using XLY.SF.Framework.Language;
using XLY.SF.Project.CaseManagement;
using XLY.SF.Project.Models;
using XLY.SF.Project.Models.Entities;
using XLY.SF.Project.Models.Logical;
using XLY.SF.Project.ViewDomain.MefKeys;

namespace XLY.SF.Project.ViewModels.Main.CaseManagement
{
    [Export(ExportKeys.CaseListViewModel, typeof(ViewModelBase))]
    public class CaseListViewModel : ViewModelBase
    {
        #region Constructors

        public CaseListViewModel()
        {
            FilterArgs = new CaseFilterArgs();
            OpenCommand = new RelayCommand<RecentCaseEntityModel>(Open);
            DeleteCommand = new RelayCommand(Delete, CanDelete);
            SearchCommand = new RelayCommand(Search);
            SelectAllCommand = new RelayCommand<Boolean>(SelectAll);
        }

        #endregion

        #region Properties
        
        #region Items

        private IEnumerable<CaseItem> _items;
        public IEnumerable<CaseItem> Items
        {
            get => _items;
            private set
            {
                _items = value;
                OnPropertyChanged();
            }
        }

        #endregion

        public CaseFilterArgs FilterArgs { get; }

        public ICommand OpenCommand { get; }

        public ICommand DeleteCommand { get; }

        public ICommand SearchCommand { get; }

        public ICommand SelectAllCommand { get; }

        [Import(typeof(IMessageBox))]
        private IMessageBox MessageBox
        {
            get;
            set;
        }

        [Import(typeof(IDatabaseContext))]
        private IDatabaseContext DbService { get; set; }

        //#region PageNo

        //private Int32 _pageNo = 1;

        //public Int32 PageNo
        //{
        //    get => _pageNo;
        //    private set
        //    {
        //        _pageNo = value;
        //        OnPropertyChanged();
        //    }
        //}

        //#endregion

        //#region PageCount

        //private Int32 _pageCount = 0;

        //public Int32 PageCount
        //{
        //    get => _pageCount;
        //    private set
        //    {
        //        _pageCount = value;
        //        OnPropertyChanged();
        //    }
        //}

        //#endregion

        #endregion

        #region Methods     

        #region Protected

        protected override void LoadCore(object parameters)
        {
            //PagingRequest paging = new PagingRequest(1, 100);
            //var result = DbService.QueryOfPaging<RecentCaseEntity, RecentCaseEntityModel>(paging, (e) => true);
            //PageCount = result.PageCount;
            var result = DbService.RecentCases.ToModels<RecentCase, RecentCaseEntityModel>().ToArray();
            Int32 index = 1;
            Items = result.OrderByDescending(x=>x.Timestamp).Select(x => new CaseItem(x, index++)).ToArray();
        }

        #endregion

        #region Private

        private void Open(RecentCaseEntityModel caseInfo)
        {
            Case currentCase = SystemContext.Instance.CurrentCase;
            if (currentCase != null && currentCase.CaseInfo.Id == caseInfo.CaseID)
            {
                return;
            }
            currentCase = Case.Open(caseInfo.CaseProjectFile);
            if (currentCase != null)
            {
                SystemContext.Instance.CurrentCase = Case.Open(caseInfo.CaseProjectFile);
                NavigationForMainWindow(ExportKeys.DeviceSelectView);
                CloseView();
            }
            else
            {
                MessageBox.ShowNoticeMsg(LanguageHelperSingle.Instance.GetLanguageByKey(Languagekeys.ViewLanguage_View_CaseNotExist));
            }
        }

        private void Delete()
        {
            if (_items == null) return;
            var selected = _items.Where(x => x.IsChecked).ToArray();
            Case @case = null;
            foreach (CaseItem item in selected)
            {
                @case = Case.Open(item.CaseInfo.CaseProjectFile);
                if (@case == null) continue;
                @case.Delete();
            }
            DbService.RemoveRange(selected.Select(x=>x.CaseInfo).ToArray());
            Items = _items.Except(selected).ToArray();
        }

        private Boolean CanDelete()
        {
            if (_items == null) return false;
            return _items.Any(x => x.IsChecked);
        }

        private void Search()
        {
            String keyword = FilterArgs.Keyword;
            DateTime? begin = FilterArgs.Begin;
            DateTime? end = FilterArgs.End;
            //var result = DbService.QueryAll<RecentCaseEntity, RecentCaseEntityModel>((e) => (e.Name.Contains(keyword) || e.Number.Contains(keyword))
            //&& (e.Timestamp >= begin.Value)
            //&& (e.Timestamp <= end.Value));
            //Int32 index = 1;
            //Items = result.OrderByDescending(x => x.Timestamp).Select(x => new CaseItem(x, index++)).ToArray();
        }

        private void SelectAll(Boolean isChecked)
        {
            if (_items == null) return;
            foreach (CaseItem item in _items)
            {
                item.IsChecked = isChecked;
            }
        }

        #endregion

        #endregion
    }
}
