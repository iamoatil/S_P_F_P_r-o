using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;


/*************************************************
 * 创建人：Bob
 * 创建时间：2017/2/27 13:35:13
 * 类功能说明：
 *
 *************************************************/

namespace XLY.SF.Framework.Language
{
    public class LanguageElement : INotifyPropertyChanged
    {
        #region OnPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName]string propertyName = null)
        {
            if (PropertyChanged == null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        private string _testName;

        public string TestName
        {
            get { return _testName; }
            set
            {
                _testName = value;
                OnPropertyChanged();
            }
        }

    }
}
