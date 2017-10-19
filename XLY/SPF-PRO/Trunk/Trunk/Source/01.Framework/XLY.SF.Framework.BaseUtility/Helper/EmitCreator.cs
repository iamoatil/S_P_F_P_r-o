using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

/* ==============================================================================
* Description：EmitCreator  
* Author     ：Fhjun
* Create Date：2017/3/20 10:59:52
* ==============================================================================*/

namespace XLY.SF.Framework.BaseUtility
{
    /// <summary>
    /// 使用emit创建类型和实例帮助类
    /// </summary>
    public class EmitCreator
    {
        private TypeBuilder _typeBuilder = null;
        private AssemblyBuilder _assemblyBuilder = null;
        private const MethodAttributes PROPERTY_ATTRIBUTE = MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig | MethodAttributes.Virtual | MethodAttributes.NewSlot | MethodAttributes.Final;

        /// <summary>
        /// 动态创建类型的默认程序集名称
        /// </summary>
        public const string DefaultAssemblyName = "XLY.Script.DynamicType";

        /// <summary>
        /// 返回当前类对象
        /// </summary>
        public Type ThisType { get; set; }

        /// <summary>
        /// 保存emit创建的类型
        /// </summary>
        public static Dictionary<string, Type> DicEmitType = new Dictionary<string, Type>();

        /// <summary>
        /// 创建一个新类型，并指定其基类
        /// </summary>
        /// <param name="assembly">程序集名称</param>
        /// <param name="type">类名称</param>
        /// <param name="parent">继承的基类</param>
        /// <param name="interfaces">继承的接口</param>
        public void CreateType(string type, string assembly = null, Type parent = null, Type[] interfaces = null)
        {
            AssemblyName demoName = new AssemblyName(assembly ?? DefaultAssemblyName);
            _assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(demoName, AssemblyBuilderAccess.RunAndSave);
            ModuleBuilder mb = _assemblyBuilder.DefineDynamicModule(demoName.Name, demoName.Name + ".dll");
            _typeBuilder = mb.DefineType(type, TypeAttributes.Public | TypeAttributes.Class | TypeAttributes.Serializable, parent, interfaces);
        }

        /// <summary>
        /// 在当前类型中创建一个属性，并指定其类型
        /// </summary>
        /// <param name="propertyName">属性名称</param>
        /// <param name="propertyType">属性类型</param>
        public PropertyBuilder CreateProperty(string propertyName, Type propertyType)
        {
            TypeBuilder tb = _typeBuilder;
            var field = tb.DefineField("_" + propertyName, propertyType, FieldAttributes.Private);
            var getMethod = tb.DefineMethod("get_" + propertyName, PROPERTY_ATTRIBUTE, propertyType, null);
            var setMethod = tb.DefineMethod("set_" + propertyName, PROPERTY_ATTRIBUTE, null, new Type[] { propertyType });
            var ilGet = getMethod.GetILGenerator();
            ilGet.Emit(OpCodes.Ldarg_0);
            ilGet.Emit(OpCodes.Ldfld, field);
            ilGet.Emit(OpCodes.Ret);

            var ilSet = setMethod.GetILGenerator();
            ilSet.Emit(OpCodes.Ldarg_0);
            ilSet.Emit(OpCodes.Ldarg_1);
            ilSet.Emit(OpCodes.Stfld, field);
            ilSet.Emit(OpCodes.Ret);

            var property = tb.DefineProperty(propertyName, PropertyAttributes.None, propertyType, null);
            property.SetGetMethod(getMethod);
            property.SetSetMethod(setMethod);

            return property;
        }

        /// <summary>
        /// 将某特性绑定到属性上
        /// </summary>
        /// <param name="property"></param>
        /// <param name="attributeType">特性类型，比如Display</param>
        /// <param name="ctorParams">构造函数的参数类型</param>
        /// <param name="ctorValues">构造函数的参数值</param>
        /// <param name="namedProperties">需要设置的属性类型</param>
        /// <param name="propertyValues">需要设置的属性值</param>
        public void SetPropertyAttribute(PropertyBuilder property, Type attributeType, Type[] ctorParams = null, object[] ctorValues = null, PropertyInfo[] namedProperties = null, object[] propertyValues = null)
        {
            ConstructorInfo classCtorInfo = attributeType.GetConstructor(ctorParams ?? Type.EmptyTypes);
            if (classCtorInfo != null)
            {
                CustomAttributeBuilder attributeBuilder = new CustomAttributeBuilder(classCtorInfo, ctorValues ?? new object[0], namedProperties ?? new PropertyInfo[0], propertyValues ?? new object[0]);
                property.SetCustomAttribute(attributeBuilder);
            }
        }

        /// <summary>
        /// 完成类型创建，并保存
        /// </summary>
        /// <returns></returns>
        public Type Save()
        {
            ThisType = _typeBuilder.CreateType();
            //_assemblyBuilder.Save(_assemblyBuilder.GetName().Name + ".dll");
            DicEmitType[ThisType.Name] = ThisType;
            return ThisType;
        }

        /// <summary>
        /// 从当前类型创建一个实例
        /// </summary>
        /// <returns></returns>
        public object CreateInstance()
        {
            return Activator.CreateInstance(ThisType);
        }

        /// <summary>
        /// 从当前类型创建一个实例
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T CreateInstance<T>() where T: class 
        {
            return Activator.CreateInstance(ThisType) as T;
        }

    }

    public static class EmitHelper
    {
        public static object Getter<T>(this T obj, string propertyName)
        {
            //var t = obj.GetType();// + propertyName;
            //if (!_dicGetter3.ContainsKey(t))
            //{
            //    _dicGetter3.Add(t, EmitGetter<T>(propertyName));
            //}
            //return (_dicGetter3[t] as Func<T, object>)(obj);
            Type t = obj.GetType();
            if (!_dicGetter2.ContainsKey(t))
            {
                _dicGetter2[t] = new Dictionary<string, object>();
            }
            if (!_dicGetter2[t].ContainsKey(propertyName))
            {
                _dicGetter2[t].Add(propertyName, EmitGetter<T>(propertyName));
            }
            return (_dicGetter2[t][propertyName] as Func<T, object>)(obj);
        }

        public static void Setter<T>(this T obj, string propertyName, object value)
        {
            //string t = obj.GetType().FullName;// + propertyName;
            //if (!_dicSetter.ContainsKey(t))
            //{
            //    _dicSetter.Add(t, EmitSetter<T>(propertyName));
            //}
            //(_dicSetter[t] as Action<T, object>)(obj, value);
            Type t = obj.GetType();
            if (!_dicSetter2.ContainsKey(t))
            {
                _dicSetter2[t] = new Dictionary<string, object>();
            }
            if (!_dicSetter2[t].ContainsKey(propertyName))
            {
                _dicSetter2[t].Add(propertyName, EmitSetter<T>(propertyName));
            }
            (_dicSetter2[t][propertyName] as Action<T, object>)(obj, value);
        }

        private static Dictionary<Type, Dictionary<string, object>> _dicGetter2 = new Dictionary<Type, Dictionary<string, object>>();
        private static Dictionary<Type, Dictionary<string, object>> _dicSetter2 = new Dictionary<Type, Dictionary<string, object>>();

        //private static Dictionary<string, object> _dicGetter = new Dictionary<string, object>();
        //private static Dictionary<string, object> _dicSetter = new Dictionary<string, object>();


        private static Func<T, object> EmitGetter<T>(string propertyName)
        {
            var type = typeof(T);

            var dynamicMethod = new DynamicMethod("get_" + propertyName, typeof(object), new[] { type }, type);
            var iLGenerator = dynamicMethod.GetILGenerator();
            iLGenerator.Emit(OpCodes.Ldarg_0);


            var property = type.GetProperty(propertyName);
            iLGenerator.Emit(OpCodes.Callvirt, property.GetMethod);


            if (property.PropertyType.IsValueType)
            {
                // 如果是值类型，装箱
                iLGenerator.Emit(OpCodes.Box, property.PropertyType);
            }
            else
            {
                // 如果是引用类型，转换
                iLGenerator.Emit(OpCodes.Castclass, property.PropertyType);
            }


            iLGenerator.Emit(OpCodes.Ret);


            return dynamicMethod.CreateDelegate(typeof(Func<T, object>)) as Func<T, object>;
        }
        private static Action<T, object> EmitSetter<T>(string propertyName)
        {
            var type = typeof(T);
            var dynamicMethod = new DynamicMethod("EmitCallable", null, new[] { type, typeof(object) }, type.Module);
            var iLGenerator = dynamicMethod.GetILGenerator();


            var callMethod = type.GetMethod("set_" + propertyName, BindingFlags.Instance | BindingFlags.IgnoreCase | BindingFlags.Public);
            var parameterInfo = callMethod.GetParameters()[0];
            var local = iLGenerator.DeclareLocal(parameterInfo.ParameterType, true);


            iLGenerator.Emit(OpCodes.Ldarg_1);
            if (parameterInfo.ParameterType.IsValueType)
            {
                // 如果是值类型，拆箱
                iLGenerator.Emit(OpCodes.Unbox_Any, parameterInfo.ParameterType);
            }
            else
            {
                // 如果是引用类型，转换
                iLGenerator.Emit(OpCodes.Castclass, parameterInfo.ParameterType);
            }


            iLGenerator.Emit(OpCodes.Stloc, local);
            iLGenerator.Emit(OpCodes.Ldarg_0);
            iLGenerator.Emit(OpCodes.Ldloc, local);


            iLGenerator.EmitCall(OpCodes.Callvirt, callMethod, null);
            iLGenerator.Emit(OpCodes.Ret);


            return dynamicMethod.CreateDelegate(typeof(Action<T, object>)) as Action<T, object>;
        }
    }
}
