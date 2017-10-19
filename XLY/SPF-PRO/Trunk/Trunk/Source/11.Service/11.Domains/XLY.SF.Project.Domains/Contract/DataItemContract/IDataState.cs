/* ==============================================================================
* Description：IDataState  
* Author     ：Fhjun
* Create Date：2017/3/17 16:53:05
* ==============================================================================*/

namespace XLY.SF.Project.Domains.Contract.DataItemContract
{
    /// <summary>
    /// 数据状态接口
    /// </summary>
    public interface IDataState
    {
        /// <summary>
        /// 数据状态
        /// </summary>
        EnumDataState DataState { get; set; }
    }
}
