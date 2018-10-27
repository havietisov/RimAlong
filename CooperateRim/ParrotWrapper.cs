using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.Serialization;
using System.Text;
using Harmony;

namespace CooperateRim
{
    public class tester
    {
        int internal_value = 100500;

        public void DoSomething(string name_something)
        {
            CooperateRimming.Log("is it delayed? " + name_something);
        }

    }

    public class ParrotWrapper
    {
        

        class PatchInfo
        {
            public StreamingContext context;
            public ISurrogateSelector selector;
            public Type instanceType;
        }

        static ISurrogateSelector generalSelector;

        static bool GenericPatch<T>(PatchInfo __state, T __instance, MethodInfo __originalMethod)
        {
            if (SyncTickData.AvoidLoop)
            {
                return false;
            }
            else
            {
                ISurrogateSelector selector;
                ISerializationSurrogate sr = generalSelector.GetSurrogate(__state.instanceType, __state.context, out selector);
                return true;
            }
        }

        static HarmonyInstance hi = HarmonyInstance.Create("id1");

        static void PatchTarget<T>(MethodInfo mi)
        {
            MethodInfo methodPrefix = ExpressionHelper.GetMethodInfo<PatchInfo, T>((PatchInfo args, T instance) => GenericPatch<T>(args, instance, mi));
            hi.Patch(mi, new HarmonyMethod(methodPrefix), null, null);
        }
        
        public static void ParrotPatchTarget<T>(Expression<Action<T>> expr)
        {
            MethodInfo mi = ExpressionHelper.GetMethodInfo<T>(expr);
            PatchTarget<T>(mi);
        }

        static void DummyAction()
        {
            Console.WriteLine("dummy action");
        }

        static bool StandartWrapper(bool condition, Action[] @true, Action[] @false)
        {
            if (condition)
            {
                foreach (var act in @true) { act(); }
                return true;
            }
            else
            {
                foreach (var act in @false) { act(); }
                return false;
            }
        }

        public static void SerializeThis<T>(T arg)
        {

        }
        
        public static void ParrotPatchExpressiontarget<T>(Expression<T> expr)
        {
            MethodInfo mi = ExpressionHelper.GetMethodInfo(expr);
            List<Expression> serializations = new List<Expression>();
            MethodInfo serializerMethod = typeof(ParrotWrapper).GetMethod(nameof(ParrotWrapper.SerializeThis));
            List<Type> @params = new List<Type>();
            foreach(var a in expr.Parameters)
            {
                @params.Add(a.Type);
            }
            
            int pos = 0;

            AssemblyBuilder ass = AppDomain.CurrentDomain.DefineDynamicAssembly(new AssemblyName("ass"), AssemblyBuilderAccess.Run);
            ModuleBuilder mb_ass = ass.DefineDynamicModule("super_ass");
            TypeBuilder ass_patch = mb_ass.DefineType("huehuehue");
            MethodBuilder mbb = ass_patch.DefineMethod("puke", MethodAttributes.Static | MethodAttributes.Public, typeof(bool), @params.ToArray());

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
            ilg.Emit(OpCodes.Ldc_I4_0);

            ilg.EmitWriteLine("test_val");
            ilg.Emit(OpCodes.Ret);

            Type t = ass_patch.CreateType();

            

            //ilg.Emit(Op)
            var hm = new HarmonyMethod(t.GetMethod("puke"));
            hi.Patch(mi, hm, null, null);

            //var body = (System.Linq.Expressions.MethodCallExpression)expr.Body;
        }
    }
}
