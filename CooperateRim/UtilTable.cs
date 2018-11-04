using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CooperateRim
{
    class UtilTable<T1, T2>
    {
        static Dictionary<T1, WeakReference> directTable = new Dictionary<T1, WeakReference>();
        public static T2 TryFindFor(T1 obj)
        {
            WeakReference wr = directTable.ContainsKey(obj) ? directTable[obj] : null;
            return wr != null && wr.IsAlive ? (T2)wr.Target : default(T2);
        }

        public static void Register(T1 obj, T2 keeper)
        {
            directTable.Add(obj, new WeakReference(keeper));
        }
    }
}
