using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using XLY.SF.Framework.Core.Base.MessageAggregation;
using XLY.SF.Framework.Core.Base.MessageBase;
using XLY.SF.Framework.Core.Base.ModuleBase;
using XLY.SF.Project.ViewDomain.MefKeys;


/*************************************************
 * 创建人：Bob
 * 创建时间：2017/3/1 11:43:36
 * 类功能说明：
 *
 *************************************************/

namespace XLY.SF.Project.Views
{
    [Export(ExportKeys.OtherLoadModule, typeof(IModule))]
    [PartCreationPolicy(System.ComponentModel.Composition.CreationPolicy.Shared)]
    public class ViewModule : IModule
    {
        public void InitModule()
        {

        }

        public void LoadModule(dynamic parameter)
        {

        }

        private void ChangedLanguageCallback(GeneralArgs args)
        {

        }
    }
}
