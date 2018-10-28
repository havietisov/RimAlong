using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using System.Reflection;

namespace CooperateRim
{
    public class MemberTracker<T2>
    {
        static Dictionary<FieldInfo, KeyValuePair<int, Action<T2>>> memberTable = new Dictionary<FieldInfo, KeyValuePair<int, Action<T2>>>();

        public static int TrackPublicField<T>(Expression<T> expr, Action<T2> onChange)
        {
            MemberExpression ex = expr.Body as MemberExpression;
            int index = memberTable.Count;
            memberTable.Add(ex.Member as FieldInfo, new KeyValuePair<int, Action<T2>>(memberTable.Count, onChange));
            return index;
        }

        public static int GetIndexOf<T>(Expression<T> expr)
        {
            MemberExpression ex = expr.Body as MemberExpression;
            return memberTable[ex.Member as FieldInfo].Key;
        }

        public static void ApplyChange(T2 newVal, int index)
        {
            if (memberTable.ElementAt(index).Value.Value == null)
            {
                CooperateRimming.Log("action is null, wtf");
            }
            memberTable.ElementAt(index).Value.Value(newVal);
        }
    }
}
