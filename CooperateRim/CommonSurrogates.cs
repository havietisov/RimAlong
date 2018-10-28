using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace CooperateRim
{
    public class TesterSurrogate : ISerializationSurrogate
    {
        public void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
        {
            tester t = (tester)obj;
            info.AddValue(nameof(tester.internal_value), t.internal_value);
        }

        public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
        {
            tester t = (tester)obj;
            t.internal_value = info.GetInt32(nameof(tester.internal_value));
            return t;
        }
    }
}
