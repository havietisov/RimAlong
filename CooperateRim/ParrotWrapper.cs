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
            //Console.WriteLine("is it delayed? " + name_something);
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

        public static void SerializeInstance<T>(T arg)
        {
            //Console.WriteLine("instance of : " + typeof(T) + " | " + arg);
            SerializationService.SerializeInstance<T>(arg);

        }

        public static void SerializeThis<T>(T arg)
        {
            //Console.WriteLine("type : " + typeof(T) + " | " + arg);
            SerializationService.SerializeObject<T>(arg);
        }

        public static void SetWrapperIndex(int i)
        {
            //Console.WriteLine("wrapper index : " + i);
            SerializationService.SetMethodWrapperIndexAndFinish(i);
        }

        public static void IndexedCall(int i, object[] args)
        {
            wrappers[i].Invoke(null, args);
        }
        
        static List<MethodInfo> wrappers = new List<MethodInfo>();
        
        internal static void DebugSaveAssembly()
        {
            dynamicAssembly.Save(@"asm.dll");
        }

        static Random r = new Random();

        public static void ParrotPatchExpressiontarget<T>(Expression<T> expr)
        {
            MethodInfo mi = ExpressionHelper.GetMethodInfo(expr);
            MethodInfo serializerMethod = typeof(ParrotWrapper).GetMethod(nameof(ParrotWrapper.SerializeThis));
            MethodInfo serializeInstance = typeof(ParrotWrapper).GetMethod(nameof(ParrotWrapper.SerializeInstance));
            List<Type> @params = new List<Type>();
            List<Type> partchParams = new List<Type>();
            List<string> @paramNames = new List<string>();
            ParameterExpression instanceParameter = null;

            foreach(var a in expr.Parameters)
            {
                if (a.Name != "__instance")
                {
                    @params.Add(a.Type);
                    @paramNames.Add(a.Name);
                }
                else
                {
                    instanceParameter = a;
                }

                partchParams.Add(a.Type);

                if (!SerializationService.CheckForSurrogate(a.Type))
                {
                    //Console.WriteLine("missing surrogate for " + a.Type);
                }
            }
            
            int pos = 0;
            
            TypeBuilder ass_patch = dynamicModule.DefineType("<parrot_patch_type>[" + r.Next().ToString() + "]");
            MethodBuilder mbb = ass_patch.DefineMethod("[prefix]", MethodAttributes.Static | MethodAttributes.Public, typeof(bool), partchParams.ToArray());
            List<Type> originalMethodArgs = new List<Type>();
            List<Type> wrapperMethodArgs = new List<Type>();

            if (instanceParameter != null)
            {
                wrapperMethodArgs.Add(instanceParameter.Type);
            }

            foreach (var a in mi.GetParameters())
            {
                originalMethodArgs.Add(a.ParameterType);
                wrapperMethodArgs.Add(a.ParameterType);
            }

            MethodBuilder wrapper = ass_patch.DefineMethod("<wrapper_method>[" + wrappers.Count + "]", MethodAttributes.Static | MethodAttributes.Public, typeof(void), wrapperMethodArgs.ToArray());
            int ppos = 0;

            foreach (var a in expr.Parameters)
            {
                wrapper.DefineParameter(++ppos, ParameterAttributes.None, a.Name);
            }
            
            ppos = 0;
            ILGenerator wrapperGenerator = wrapper.GetILGenerator();
            originalMethodArgs.Reverse();
            
            foreach (var a in wrapperMethodArgs)
            {
                wrapperGenerator.Emit(OpCodes.Ldarg, ppos++);
            }
            
            wrapperGenerator.Emit(mi.IsVirtual ? OpCodes.Callvirt : OpCodes.Call, mi);
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
                ilg.EmitCall(OpCodes.Call, __mi, null);
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
