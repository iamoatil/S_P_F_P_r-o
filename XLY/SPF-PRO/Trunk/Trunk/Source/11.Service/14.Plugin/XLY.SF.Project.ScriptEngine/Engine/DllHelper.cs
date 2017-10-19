using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using System.Text;

/* ==============================================================================
* Description：DllHelper  
* Author     ：Fhjun
* Create Date：2017/4/6 17:48:15
* ==============================================================================*/

namespace XLY.SF.Project.ScriptEngine.Engine
{
    /// <summary>
    /// 脚本支持调用C++底层库接口
    /// </summary>
    public class DllHelper : IDisposable
    {
        /// <summary>
        /// 原型是 :HMODULE LoadLibrary(LPCTSTR lpFileName);
        /// </summary>
        /// <param name="lpFileName">DLL 文件名 </param>
        /// <returns> 函数库模块的句柄 </returns>
        [DllImport("kernel32.dll")]
        static extern IntPtr LoadLibrary(string lpFileName);
        /// <summary>
        /// 原型是 : FARPROC GetProcAddress(HMODULE hModule, LPCWSTR lpProcName);
        /// </summary>
        /// <param name="hModule"> 包含需调用函数的函数库模块的句柄 </param>
        /// <param name="lpProcName"> 调用函数的名称 </param>
        /// <returns> 函数指针 </returns>
        [DllImport("kernel32.dll")]
        static extern IntPtr GetProcAddress(IntPtr hModule, string lpProcName);
        /// <summary>
        /// 原型是 : BOOL FreeLibrary(HMODULE hModule);
        /// </summary>
        /// <param name="hModule"> 需释放的函数库模块的句柄 </param>
        /// <returns> 是否已释放指定的 Dll</returns>
        [DllImport("kernel32", EntryPoint = "FreeLibrary", SetLastError = true)]
        static extern bool FreeLibrary(IntPtr hModule);

        private Dictionary<IntPtr, MethodInfo> _methods = new Dictionary<IntPtr, MethodInfo>();
        private Dictionary<IntPtr, List<IntPtr>> _modules = new Dictionary<IntPtr, List<IntPtr>>();
        /// <summary>
        /// 装载 Dll
        /// </summary>
        /// <param name="lpFileName">DLL 文件名 </param>
        public IntPtr LoadDll(string lpFileName)
        {
            IntPtr moduleHandle = LoadLibrary(lpFileName);
            if (moduleHandle == IntPtr.Zero)
                throw (new Exception(" Do not found file " + lpFileName));
            _modules[moduleHandle] = new List<IntPtr>();
            return moduleHandle;
        }

        /// <summary>
        /// 获得函数指针
        /// </summary>
        /// <param name="moduleHandle"> dll句柄 </param>
        /// <param name="lpProcName"> 调用函数的名称 </param>
        /// <param name="paramTypes"> 函数的传入参数类型列表 </param>
        /// <param name="returnType"> 函数的返回值类型 </param>
        public IntPtr LoadFun(IntPtr moduleHandle, string lpProcName, string[] paramTypes, string returnType)
        { 
            // 若函数库模块的句柄为空，则抛出异常
            if (moduleHandle == IntPtr.Zero)
                throw (new Exception("Dll is not mounted!"));
            // 取得函数指针
            IntPtr functionHandle = GetProcAddress(moduleHandle, lpProcName);
            // 若函数指针，则抛出异常
            if (functionHandle == IntPtr.Zero)
                throw (new Exception(" Do not found the function :" + lpProcName));
            _modules[moduleHandle].Add(functionHandle);

            Type[] ts = new Type[paramTypes == null ? 0 : paramTypes.Length];
            ModePass[] paramPassTypes = new ModePass[ts.Length];
            for (int i = 0; i < ts.Length; i++)
            {
                ts[i] = StringToType(paramTypes[i]);
                paramPassTypes[i] = ModePass.ByValue;
            }
            Type tyre = StringToType(returnType);

            // 下面是创建 MyAssemblyName 对象并设置其 Name 属性
            AssemblyName myAssemblyName = new AssemblyName();
            myAssemblyName.Name = "InvokeFun";
            // 生成单模块配件
            AssemblyBuilder myAssemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(myAssemblyName, AssemblyBuilderAccess.Run);
            ModuleBuilder myModuleBuilder = myAssemblyBuilder.DefineDynamicModule("InvokeDll");
            // 定义要调用的方法 , 方法名为“ MyFun ”，返回类型是“ Type_Return ”参数类型是“ TypeArray_ParameterType ”
            MethodBuilder myMethodBuilder = myModuleBuilder.DefineGlobalMethod("MyFun", MethodAttributes.Public | MethodAttributes.Static, tyre, ts);
            // 获取一个 ILGenerator ，用于发送所需的 IL
            ILGenerator il = myMethodBuilder.GetILGenerator();
            for (int i = 0; i < ts.Length; i++)
            {// 用循环将参数依次压入堆栈
                switch (paramPassTypes[i])
                {
                    case ModePass.ByValue:
                        il.Emit(OpCodes.Ldarg, i);
                        break;
                    case ModePass.ByRef:
                        il.Emit(OpCodes.Ldarga, i);
                        break;
                    default:
                        throw (new Exception(" parameter[" + (i + 1) + "]'s format is not valid! ."));
                }
            }
            if (IntPtr.Size == 4)
            {// 判断处理器类型
                il.Emit(OpCodes.Ldc_I4, functionHandle.ToInt32());
            }
            else if (IntPtr.Size == 8)
            {
                il.Emit(OpCodes.Ldc_I8, functionHandle.ToInt64());
            }
            else
            {
                throw new PlatformNotSupportedException();
            }
            il.EmitCalli(OpCodes.Calli, CallingConvention.StdCall, tyre, ts);
            il.Emit(OpCodes.Ret); // 返回值
            myModuleBuilder.CreateGlobalFunctions();
            // 取得方法信息
            _methods[functionHandle] = myModuleBuilder.GetMethod("MyFun");
            return functionHandle;
        }

        /// <summary>
        /// 卸载 Dll
        /// </summary>
        public void UnLoadDll(IntPtr moduleHandle)
        {
            foreach (var m in _modules[moduleHandle])
            {
                _methods.Remove(m);
            }
            _modules.Remove(moduleHandle);
            FreeLibrary(moduleHandle);
        }

        private Type StringToType(string str)
        {
            Type ts = null;
            switch (str.ToLower().Trim())
            {
                case "char *":
                case "char*":
                    ts = typeof(string);
                    break;
                case "int":
                case "int32":
                    ts = typeof(int);
                    break;
                case "uint":
                case "uint32":
                    ts = typeof(uint);
                    break;
                case "long":
                case "int64":
                    ts = typeof(long);
                    break;
                case "ulong":
                case "uint64":
                    ts = typeof(ulong);
                    break;
                case "float":
                    ts = typeof(float);
                    break;
                case "double":
                    ts = typeof(double);
                    break;
                case "datetime":
                    ts = typeof(DateTime);
                    break;
                default:
                    //ts = typeof(object);
                    break;
            }
            return ts;
        }

        /// <summary>
        /// 调用所设定的函数
        /// </summary>
        /// <param name="funcitonHandle">函数句柄 </param>
        /// <param name="paramValues"> 实参 </param>
        /// <returns> 返回所调用函数的 object</returns>
        public object Invoke(IntPtr funcitonHandle, object[] paramValues)
        {
            return _methods[funcitonHandle].Invoke(null, paramValues);// 调用方法，并返回其值
        }

        /// <summary>
        /// 参数传递方式枚举 ,ByValue 表示值传递 ,ByRef 表示址传递
        /// </summary>
        public enum ModePass
        {
            ByValue = 0x0001,
            ByRef = 0x0002
        }

        public void Dispose()
        {
            foreach (var m in _modules.Keys)
            {
                UnLoadDll(m);
            }
        }
    }
}
