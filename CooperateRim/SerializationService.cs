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
            BinaryFormatter bf = new BinaryFormatter();
            ISerializationSurrogate s = selector.GetSurrogate(T, sc, out sur);
            return s != null || T.IsSerializable;
        }

        public static byte[] Flush()
        {
            BinaryFormatter bf = new BinaryFormatter(selector, sc);
            bf.Serialize(ms, curData);
            ms.Flush();
            sc = new StreamingContext();
            curData = new List<SerializationData>();
            byte[] result = ms.GetBuffer();
            ms = new MemoryStream();
            return result;
        }

        public static List<SerializationData> DeserializeFrom(byte[] bytes)
        {
            BinaryFormatter bf = new BinaryFormatter(selector, sc);
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

        public static void SetMethodWrapperIndexAndFinish(int i)
        {
            curData[curData.Count-1].methodContext = i;
            curData.Add(new SerializationData());
        }

        public static void SerializeInstance<T>(T o)
        {
            NetDemo.log("curdata len : " + curData.Count);
            if (curData.Count == 0)
            {
                curData.Add(new SerializationData());
            }

            curData[curData.Count - 1].instance = (o);
        }

        public static void SerializeObject<T>(T o)
        {
            NetDemo.log("curdata len : " + curData.Count);
            if (curData.Count == 0)
            {
                curData.Add(new SerializationData());
            }

            curData[curData.Count - 1].dataObjects.Add(o);
        }
    }
}
