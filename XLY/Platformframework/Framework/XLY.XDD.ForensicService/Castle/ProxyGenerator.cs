using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Reflection.Emit;

namespace XLY.XDD.ForensicService
{
    /// <summary>
    /// 代理类生成器
    /// 温馨提示：接口与属性参数可以混合、多次使用，如果已存在的属性名后面会跳过。
    /// 建议顺序：先处理接口，后处理参数类的。
    /// 使用注意：一个ProxyGenerator只可以创建一个代理对象。
    /// </summary>
    public class ProxyGenerator
    {
        private TypeBuilder _TypeBuilder;
        private AssemblyBuilder _AssemblyBuilder;
        /// <summary>
        /// 缓存属性名称，用于重复判断
        /// </summary>
        private ICollection<string> _Names;

        public ProxyGenerator(string assemblyName, string typeName, Type parent = null)
        {
            var assembly = new AssemblyName(assemblyName);
            this._AssemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(assembly, AssemblyBuilderAccess.Run);
            var moudleBuilder = this._AssemblyBuilder.DefineDynamicModule(assemblyName);
            if (parent == null) this._TypeBuilder = moudleBuilder.DefineType(typeName, TypeAttributes.Public);
            else this._TypeBuilder = moudleBuilder.DefineType(typeName, TypeAttributes.Public, parent);
            //this
            this._Names = new Collection<string>();
        }

        #region AppendProperty :string propertyName, Type propertyType
        /// <summary>
        /// 附加指定类型propertyType、名称propertyName的属性。
        /// 若属性名已存在则不处理。
        /// 注意，若与接口有重复，请先处理接口。
        /// </summary>
        public void AppendProperty(string propertyName, Type propertyType)
        {
            //if exist
            if (this._Names.Contains(propertyName)) return;
            this._Names.Add(propertyName);
            //field
            FieldBuilder fieldBuilder = this._TypeBuilder.DefineField("_" + propertyName, propertyType, FieldAttributes.Private);
            //get method
            MethodBuilder getMethodBuilder = this._TypeBuilder.DefineMethod("get_" + propertyName, MethodAttributes.Public, propertyType, null);
            ILGenerator getIL = getMethodBuilder.GetILGenerator();
            getIL.Emit(OpCodes.Ldarg_0);
            getIL.Emit(OpCodes.Ldfld, fieldBuilder);
            getIL.Emit(OpCodes.Ret);
            //set method
            MethodBuilder setMethod = this._TypeBuilder.DefineMethod("set_" + propertyName, MethodAttributes.Public, null, new Type[] { propertyType });
            ILGenerator setIL = setMethod.GetILGenerator();
            setIL.Emit(OpCodes.Ldarg_0);
            setIL.Emit(OpCodes.Ldarg_1);
            setIL.Emit(OpCodes.Stfld, fieldBuilder);
            setIL.Emit(OpCodes.Ret);
            //property
            PropertyBuilder propertyBuilder = this._TypeBuilder.DefineProperty(propertyName, PropertyAttributes.None, propertyType, null);
            propertyBuilder.SetSetMethod(setMethod);
            propertyBuilder.SetGetMethod(getMethodBuilder);
        }
        #endregion

        #region InheritInterface：实现指定接口
        /// <summary>
        /// 实现指定接口，T必须为接口类型
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public void InheritInterface<T>() where T : class
        {
            var interitType = typeof(T);
            if (!interitType.IsInterface) throw new ArgumentException("泛型参数T必须是接口类型");
            var pis = interitType.GetProperties();
            if (pis.IsInvalid()) return;
            foreach (var pro in pis)
            {
                this.AppendProperty(pro);
            }
            //set parent
            this._TypeBuilder.AddInterfaceImplementation(interitType);
        }
        #endregion

        #region GenerateType：生成类型
        /// <summary>
        /// 生成类型
        /// </summary>
        public Type GenerateType()
        {
            return _TypeBuilder != null ? this._TypeBuilder.CreateType() : null;
        }
        #endregion

        /****************** private methods ******************/

        #region AppendProperty：根据属性元数据添加属性，用户接口实现
        /// <summary>
        /// 根据属性元数据添加属性，用户接口实现
        /// </summary>
        private void AppendProperty(PropertyInfo propertyInfo)
        {
            //if exist
            if (this._Names.Contains(propertyInfo.Name)) return;
            this._Names.Add(propertyInfo.Name);
            //field
            FieldBuilder fieldBuilder = this._TypeBuilder.DefineField("_" + propertyInfo.Name, propertyInfo.PropertyType, FieldAttributes.Private);
            //property
            PropertyBuilder propertyBuilder = this._TypeBuilder.DefineProperty(propertyInfo.Name, PropertyAttributes.HasDefault, propertyInfo.PropertyType, null);
            MethodAttributes getSetAttr = MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig | MethodAttributes.Virtual;
            //get method
            var getMethod = propertyInfo.GetGetMethod();
            if (null != getMethod)
            {
                MethodBuilder getMethodBuilder = this._TypeBuilder.DefineMethod("get_" + propertyInfo.Name, getSetAttr, propertyInfo.PropertyType, Type.EmptyTypes);
                ILGenerator getIL = getMethodBuilder.GetILGenerator();
                getIL.Emit(OpCodes.Ldarg_0);
                getIL.Emit(OpCodes.Ldfld, fieldBuilder);
                getIL.Emit(OpCodes.Ret);
                this._TypeBuilder.DefineMethodOverride(getMethodBuilder, getMethod);
                propertyBuilder.SetGetMethod(getMethodBuilder);
            }
            //set method
            var setMethod = propertyInfo.GetSetMethod();
            if (null != setMethod)
            {
                MethodBuilder setMethodBuilder = this._TypeBuilder.DefineMethod("set_" + propertyInfo.Name, getSetAttr,
                    null, new Type[] { propertyInfo.PropertyType });
                ILGenerator setIL = setMethodBuilder.GetILGenerator();
                setIL.Emit(OpCodes.Ldarg_0);
                setIL.Emit(OpCodes.Ldarg_1);
                setIL.Emit(OpCodes.Stfld, fieldBuilder);
                setIL.Emit(OpCodes.Ret);
                this._TypeBuilder.DefineMethodOverride(setMethodBuilder, setMethod);
                propertyBuilder.SetSetMethod(setMethodBuilder);
            }
        }
        #endregion
    }
}