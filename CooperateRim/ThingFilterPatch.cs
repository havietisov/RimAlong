using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CooperateRim
{
    public class ThingFilterPatch
    {
        public static bool avoidThingFilterUsage;
        public static Stack<object> thingFilterCallerStack = new Stack<object>();
    }
}
