using System;
using XLY.SF.Framework.Core.Base.ViewModel;
using XLY.SF.Project.Models.Logical;

namespace XLY.SF.Project.ViewModels.Main.CaseManagement
{
    public class CaseItem : NotifyPropertyBase
    {
        #region Constructors

        public CaseItem(RecentCaseEntityModel caseInfo,Int32 index)
        {
            CaseInfo = caseInfo;
            Index = index;
        }

        #endregion

        #region Properties

        #region IsChecked

        private Boolean _isChecked;

        public Boolean IsChecked
        {
            get => _isChecked;
            set
            {
                _isChecked = value;
                OnPropertyChanged();
            }
        }

        #endregion

        public RecentCaseEntityModel CaseInfo { get; }

        public Int32 Index { get; }

        #endregion
    }
}
