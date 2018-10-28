using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using Harmony;

namespace CooperateRim
{
    public class tester
    {
        public int internal_value = 100500;

        public void DoSomething(string name_something)
        {
            Console.WriteLine("is it delayed? " + name_something);
        }

    }

    public class ParrotWrapper
    {
        static AssemblyBuilder dynamicAssembly;
        static ModuleBuilder dynamicModule;

        public static void Initialize()
        {
            dynamicAssembly = AppDomain.CurrentDomain.DefineDynamicAssembly(new AssemblyName("ParrotPatchDynamicAssembly"), AssemblyBuilderAccess.RunAndSave);
            dynamicModule = dynamicAssembly.DefineDynamicModule("ParrotPatchDynamicModule");
        }
        
        static HarmonyInstance hi = HarmonyInstance.Create("id1");
        
        public static void SerializeThis<T>(T arg)
        {
            Console.WriteLine("type : " + typeof(T) + " | " + arg);
            SerializationService.SerializeObject<T>(arg);
        }

        public static void SetWrapperIndex(int i)
        {
            Console.WriteLine("wrapper index : " + i);
            SerializationService.SetMethodWrapperIndexAndFinish(i);
        }

        public static void IndexedCall(int i, object[] args)
        {
            wrappers[i].Invoke(null, args);
        }
        
        static List<MethodInfo> wrappers = new List<MethodInfo>();
        
        public static void ParrotPatchExpressionTargetWatchValue<exprT>(Expression<exprT> method, FieldInfo fi)
        {
            MethodInfo mi = ExpressionHelper.GetMethodInfo(method);
            MethodInfo serializerMethod = typeof(ParrotWrapper).GetMethod(nameof(ParrotWrapper.SerializeThis));
            
            TypeBuilder ass_patch = dynamicModule.DefineType("<parrot_patch_type>[" + new Random().Next().ToString() + "]");
            List<FieldInfo> trackerFields = new List<FieldInfo>();

            foreach(var a in method.Parameters)
            {
                trackerFields.Add(ass_patch.DefineField("<tracker>[" + a.Name + "::" + new Random().Next() + "]", a.Type, FieldAttributes.Static | FieldAttributes.Public));
            }

            Type t = ass_patch.CreateType();
            var _prefix = new HarmonyMethod(t.GetMethod("[prefix]"));
            var _postfix = new HarmonyMethod(t.GetMethod("[postfix]"));
            hi.Patch(mi, _prefix, _postfix, null);
        }

        internal static void DebugSaveAssembly()
        {
            dynamicAssembly.Save(@"asm.dll");
        }

        public static void ParrotPatchExpressiontarget<T>(Expression<T> expr)
        {
            MethodInfo mi = ExpressionHelper.GetMethodInfo(expr);
            MethodInfo serializerMethod = typeof(ParrotWrapper).GetMethod(nameof(ParrotWrapper.SerializeThis));
            List<Type> @params = new List<Type>();

            foreach(var a in expr.Parameters)
            {
                @params.Add(a.Type);

                if (!SerializationService.CheckForSurrogate(a.Type))
                {
                    Console.WriteLine("missing surrogate for " + a.Type);
                }
            }
            
            int pos = 0;
            
            TypeBuilder ass_patch = dynamicModule.DefineType("<parrot_patch_type>[" + new Random().Next().ToString() + "]");
            MethodBuilder mbb = ass_patch.DefineMethod("[prefix]", MethodAttributes.Static | MethodAttributes.Public, typeof(bool), @params.ToArray());
            List<Type> originalMethodArgs = new List<Type>();

            if (!mi.IsStatic)
            {
                originalMethodArgs.Add(mi.DeclaringType);
            }

            foreach (var a in mi.GetParameters())
            {
                originalMethodArgs.Add(a.ParameterType);
            }

            MethodBuilder wrapper = ass_patch.DefineMethod("<wrapper_method>[" + wrappers.Count + "]", MethodAttributes.Static | MethodAttributes.Public, typeof(void), originalMethodArgs.ToArray());
            int ppos = 0;

            foreach (var a in mi.GetParameters())
            {
                wrapper.DefineParameter(++ppos, a.Attributes, a.Name);
            }
            
            ppos = 0;
            ILGenerator wrapperGenerator = wrapper.GetILGenerator();
            originalMethodArgs.Reverse();
            
            foreach (var a in originalMethodArgs)
            {
                wrapperGenerator.Emit(OpCodes.Ldarg, ppos++);
            }

            wrapperGenerator.Emit(OpCodes.Call, mi);
            wrapperGenerator.Emit(OpCodes.Ret);

            originalMethodArgs.Reverse();
            foreach (var a in expr.Parameters)
            {
                mbb.DefineParameter(++pos, ParameterAttributes.None, a.Name);
            }

            ILGenerator ilg = mbb.GetILGenerator();
            Label end = ilg.DefineLabel();
            ilg =  mbb.GetILGenerator();
            ilg.Emit(OpCodes.Ldsfld, typeof(SyncTickData).GetField(nameof(SyncTickData.AvoidLoop)));
            ilg.Emit(OpCodes.Brfalse, end);
            ilg.Emit(OpCodes.Ldc_I4_1);
            ilg.Emit(OpCodes.Ret);
            ilg.MarkLabel(end);

            int cc = 0;
            foreach (var a in expr.Parameters)
            {
                MethodInfo __mi = typeof(ParrotWrapper).GetMethod(nameof(ParrotWrapper.SerializeThis)).MakeGenericMethod(a.Type);
                ilg.Emit(OpCodes.Ldarg, cc++);
                ilg.EmitCall(OpCodes.Call, __mi, new Type[] { a.Type });
            }

            ilg.Emit(OpCodes.Ldc_I4, wrappers.Count);
            ilg.EmitCall(OpCodes.Call, typeof(ParrotWrapper).GetMethod(nameof(ParrotWrapper.SetWrapperIndex)), null);

            ilg.Emit(OpCodes.Ldc_I4_0);
            ilg.Emit(OpCodes.Ret);

            Type t = ass_patch.CreateType();
            wrappers.Add(t.GetMethod(wrapper.Name));
            var hm = new HarmonyMethod(t.GetMethod("[prefix]"));
            hi.Patch(mi, hm, null, null);

            //var body = (System.Linq.Expressions.MethodCallExpression)expr.Body;
        }
    }
}
