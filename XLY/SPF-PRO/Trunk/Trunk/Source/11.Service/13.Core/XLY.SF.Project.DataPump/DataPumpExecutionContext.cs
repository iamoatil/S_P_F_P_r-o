using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XLY.SF.Project.Domains;

namespace XLY.SF.Project.DataPump
{
    /// <summary>
    /// 表示执行数据泵的执行上下文。
    /// </summary>
    public class DataPumpExecutionContext
    {
        #region Fields

        private readonly Hashtable _datas = new Hashtable();

        #endregion

        #region Constructors

        /// <summary>
        /// 初始化类型 XLY.SF.Project.DataPump.DataPumpTaskContext 实例。
        /// </summary>
        /// <param name="pumpDescriptor">对任务进行描述的元数据。</param>
        /// <param name="targetDirectory">数据保存目录。</param>
        /// <param name="source">数据源。如果不需要数据源则设置为null。</param>
        internal DataPumpExecutionContext(Pump pumpDescriptor,String targetDirectory, SourceFileItem source)
        {
            PumpDescriptor = pumpDescriptor ?? throw new ArgumentNullException("metadata");
            if (String.IsNullOrWhiteSpace(targetDirectory)) throw new ArgumentNullException("targetDirectory");
            Source = source;
            TargetDirectory = targetDirectory;
        }

        /// <summary>
        /// 初始化类型 XLY.SF.Project.DataPump.DataPumpTaskContext 实例。
        /// 为了保持兼容提供此方法，强烈建议不要使用此构造器，因为它的Source属性不安全。
        /// </summary>
        /// <param name="pumpDescriptor">对任务进行描述的元数据。</param>
        /// <param name="targetDirectory">数据保存目录。</param>
        [Obsolete("在以后的版本中会移除该方法")]
        internal DataPumpExecutionContext(Pump pumpDescriptor, String targetDirectory)
            : this(pumpDescriptor, targetDirectory, null)
        {
        }

        #endregion

        #region Properties

        /// <summary>
        /// 数据保存目录。
        /// </summary>
        public String TargetDirectory { get; }

        /// <summary>
        /// 对任务进行描述的元数据。
        /// </summary>
        public Pump PumpDescriptor { get; }

        /// <summary>
        /// 数据源。
        /// </summary>
        public SourceFileItem Source { get; private set; }

        /// <summary>
        /// 数据源。不安全地设置数据源，这可能导致运行时不稳定。
        /// </summary>
        public SourceFileItem UnsafeSource
        {
            get => Source;
            set => Source = value;
        }

        /// <summary>
        /// 提取项列表。
        /// </summary>
        public IEnumerable<ExtractItem> ExtractItems { get; set; }

        /// <summary>
        /// 拥有该任务的服务。
        /// </summary>
        internal DataPumpBase Owner { get; set; }

        /// <summary>
        /// 自定义数据。
        /// </summary>
        /// <param name="name">数据名称。</param>
        /// <returns>数据值。</returns>
        internal Object this[String name]
        {
            get => _datas[name];
            set => _datas[name] = value;
        }

        /// <summary>
        /// 是否已经被初始过。
        /// </summary>
        internal Boolean IsInit { get; set; }

        #endregion

        #region Methods

        #region Public

        /// <summary>
        /// 获取对象的Hash值。
        /// </summary>
        /// <returns>对象的Hash值。</returns>
        public override Int32 GetHashCode()
        {
            return PumpDescriptor.GetHashCode();
        }

        #endregion

        #endregion
    }
}
