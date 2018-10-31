using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Reflection;
using System.Runtime.Serialization;
using System.Threading;
using CooperateRim;

namespace PirateRPC
{
    public class PirateRPC
    {
        [Serializable]
        public class Modification : ISerializable
        {
            Action<Stream> action;
            
            internal Modification(Action<Stream> action)
            {
                this.action = action;
            }

            internal Modification(SerializationInfo info, StreamingContext context)
            {
                // Simply retrieve the action if it is serializable
                if (info.GetBoolean("isSerializable"))
                    this.action = (Action<Stream>)info.GetValue("action", typeof(Action<Stream>));
                // Otherwise, recreate the action based on its serialized components
                else
                {
                    // Retrieve the serialized method reference
                    MethodInfo method = (MethodInfo)info.GetValue("method", typeof(MethodInfo));

                    // Create an instance of the anonymous delegate class
                    object target = System.Activator.CreateInstance(method.DeclaringType);// ReflectionUtil.CreateObject(method.DeclaringType, BindingFlags.NonPublic | BindingFlags.Instance);

                    // Initialize the fields of the anonymous instance
                    foreach (FieldInfo field in method.DeclaringType.GetFields())
                        field.SetValue(target, info.GetValue(field.Name, field.FieldType));

                    // Recreate the action delegate
                    action = (Action<Stream>)Delegate.CreateDelegate(typeof(Action<Stream>), target, method.Name);
                }
            }

            void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
            {
                // Serialize the action delegate directly if the target is serializable
                if (action.Target == null || action.Target.GetType().GetCustomAttributes(typeof(SerializableAttribute), false).Length > 0)
                {
                    info.AddValue("isSerializable", true);
                    info.AddValue("action", action);
                }
                // Otherwise, serialize information necessary to recreate the action delegate
                else
                {
                    info.AddValue("isSerializable", false);
                    info.AddValue("method", action.Method);
                    foreach (FieldInfo field in action.Method.DeclaringType.GetFields())
                        info.AddValue(field.Name, field.GetValue(action.Target));
                }
            }

            public void Apply(Stream s)
            {
                action(s);
            }
        }

        [Serializable]
        public class Message
        {
            public List<SyncTickData> sdl;
        }

        public static void SendInvocation(Stream s, Action<Stream> act)
        {
            DateTime dt12 = DateTime.Now;

            MemoryStream initialBuffer = new MemoryStream();
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(initialBuffer, new Modification(act));
            initialBuffer.Flush();
            string ss = Convert.ToBase64String(initialBuffer.GetBuffer());
            MemoryStream ms2 = new MemoryStream();
            BinaryWriter bw = new BinaryWriter(ms2);
            bw.Write(ss);
            ms2.Flush();
            byte[] buff = ms2.GetBuffer();

            if (buff.Length > 48000)
            {
                var ffs = 0;
                ffs = 2;
            }

            //NetDemo.log("Serialization took " + (DateTime.Now - dt12).TotalMilliseconds);

            dt12 = DateTime.Now;
            DateTime dt1 = DateTime.Now;
            s.Write(buff, 0, (int)ms2.Length);
            //NetDemo.log("sync write ended in " + (DateTime.Now - dt1).TotalMilliseconds);
            /*
            s.BeginWrite(buff, 0, , u =>
            {
                NetDemo.log("async callback delay " + (DateTime.Now - dt12).TotalMilliseconds);
                
                (u.AsyncState as Stream).EndWrite(u);
                (u.AsyncState as Stream).Flush();
                NetDemo.log("async write ended in " + (DateTime.Now - dt1).TotalMilliseconds);
                //NetDemo.log("async write ended in "  + (DateTime.Now - dt1).TotalMilliseconds);
            }, s);*/
        }

        static int counter;
        static object of = "";

        public static void ReceiveInvocation(Stream s)
        {
            BinaryReader br = new BinaryReader(s);
            string ss = br.ReadString();
            byte[] b = Convert.FromBase64String(ss);
            NetDemo.log("RPC size : " + b.Length);
            MemoryStream ms = new MemoryStream(b);
            BinaryFormatter bf = new BinaryFormatter();
            
            {
                try
                {
                    (bf.Deserialize(ms) as Modification).Apply(s);
                }
                catch (Exception ee) { NetDemo.log(ee.ToString()); }
                finally
                {
                }
            }
        }
    }
}