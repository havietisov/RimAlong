using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace CooperateRim
{
    public static class SerializationService
    {
        static MemoryStream ms = new MemoryStream();
        static SurrogateSelector selector;
        static StreamingContext sc;
        
        public static void Initialize()
        {
            selector = new SurrogateSelector();
            sc = new StreamingContext(StreamingContextStates.All);
            selector.AddSurrogate(typeof(tester), sc, new TesterSurrogate());
            curData.Add(new SerializationData());
        }

        public static void AppendSurrogate(Type t, ISerializationSurrogate surrogate)
        {
            selector.AddSurrogate(t, sc, surrogate);
        }

        public static bool CheckForSurrogate(Type T)
        {
            ISurrogateSelector sur;
            BinaryFormatter bf = new BinaryFormatter() { TypeFormat = System.Runtime.Serialization.Formatters.FormatterTypeStyle.TypesWhenNeeded };
            ISerializationSurrogate s = selector.GetSurrogate(T, sc, out sur);
            return s != null || T.IsSerializable;
        }

        public static Func<object, SerializationInfo, StreamingContext, ISurrogateSelector, T> SetObjectDataOf<T>()
        {
            return (object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector) => (T)SerializationService.GetSurrogateFor(typeof(T)).SetObjectData(obj, info, context, selector);
        }

        public static Action<T, SerializationInfo, StreamingContext> GetObjectDataOf<T>()
        {
            return (T obj, SerializationInfo info, StreamingContext context) => SerializationService.GetSurrogateFor(typeof(T)).GetObjectData(obj, info, context);
        }

        public static byte[] Flush()
        {
            BinaryFormatter bf = new BinaryFormatter(selector, sc) { TypeFormat = System.Runtime.Serialization.Formatters.FormatterTypeStyle.TypesWhenNeeded };
            bf.Serialize(ms, curData);
            ms.Flush();
            sc = new StreamingContext();
            curData = new List<SerializationData>();
            byte[] result = ms.GetBuffer();
            List<byte> __result = new List<byte>(result);
            //Utilities.RimLog.Message(result.Length + "::::<>::::" + ms.Length);
            __result.RemoveRange((int)ms.Length, result.Length - (int)ms.Length);
            if (result.Length > 32000)
            {
                var a = 15000;
                a = 2;
            }
            ms = new MemoryStream();
            return __result.ToArray();
        }

        public static List<SerializationData> DeserializeFrom(byte[] bytes)
        {
            BinaryFormatter bf = new BinaryFormatter(selector, sc) { TypeFormat = System.Runtime.Serialization.Formatters.FormatterTypeStyle.TypesWhenNeeded };
            return (List<SerializationData>)bf.Deserialize(new MemoryStream(bytes));
        }

        [Serializable]
        public class SerializationData
        {
            public int methodContext = -1;
            public List<object> dataObjects = new List<object>();
            public object instance;
        }

        static List<SerializationData> curData = new List<SerializationData>();

        public static ISerializationSurrogate GetSurrogateFor(Type t)
        {
            ISurrogateSelector sr;
            ISerializationSurrogate res = selector.GetSurrogate(t, sc, out sr);
            if (res == null)
            {
                Utilities.RimLog.Message("No surrogate for " + t);
            }
            return res;
        }

        public static void SetMethodWrapperIndexAndFinish(int i)
        {
            curData[curData.Count-1].methodContext = i;
            curData.Add(new SerializationData());
        }

        public static void SerializeInstance<T>(T o)
        {
            if (curData.Count == 0)
            {
                curData.Add(new SerializationData());
            }

            curData[curData.Count - 1].instance = (o);
        }

        public static void SerializeObject<T>(T o)
        {
            if (curData.Count == 0)
            {
                curData.Add(new SerializationData());
            }

            curData[curData.Count - 1].dataObjects.Add(o);
        }
    }
}
