using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace CooperateRim
{
    public class ReverseBillTable
    {
        static int idx;
        static List<KeyValuePair<KeyValuePair<int, WeakReference<Bill>>, WeakReference<BillStack>>> billstackReferenceHeap = new List<KeyValuePair<KeyValuePair<int, WeakReference<Bill>>, WeakReference<BillStack>>>();

        public static void Associate(Bill b, BillStack bs)
        {
            KeyValuePair<int, WeakReference<Bill>> entry = new KeyValuePair<int, WeakReference<Bill>>(idx++, new WeakReference<Bill>(b));
            billstackReferenceHeap.Add(new KeyValuePair<KeyValuePair<int, WeakReference<Bill>>, WeakReference<BillStack>>(entry, new WeakReference<BillStack>(bs)));
        }

        public static Bill BillFromBillstack(BillStack bs)
        {
            List<KeyValuePair<KeyValuePair<int, WeakReference<Bill>>, WeakReference<BillStack>>> removalList = new List<KeyValuePair<KeyValuePair<int, WeakReference<Bill>>, WeakReference<BillStack>>>();
            Bill res = null;

            foreach (var val in billstackReferenceHeap)
            {
                if (val.Value.IsAlive)
                {
                    if (val.Value.Equals(bs))
                    {
                        res = val.Key.Value.Target;
                        break;
                    }
                }
                else
                {
                    removalList.Add(val);
                }
            }

            foreach (var val in removalList)
            {
                billstackReferenceHeap.Remove(val);
            }
            return res;
        }
        
        public static int? GetIndexFor(Bill b)
        {
            List<KeyValuePair<KeyValuePair<int, WeakReference<Bill>>, WeakReference<BillStack>>> removalList = new List<KeyValuePair<KeyValuePair<int, WeakReference<Bill>>, WeakReference<BillStack>>>();
            int? res = null;

            foreach (var val in billstackReferenceHeap)
            {
                if (val.Key.Value.IsAlive)
                {
                    if (val.Key.Value.Equals(b))
                    {
                        res = val.Key.Key;
                        break;
                    }
                }
                else
                {
                    removalList.Add(val);
                }
            }

            foreach (var val in removalList)
            {
                billstackReferenceHeap.Remove(val);
            }
            return res;
        }

        public static int? GetIndexFor(BillStack bs)
        {
            List<KeyValuePair<KeyValuePair<int, WeakReference<Bill>>, WeakReference<BillStack>>> removalList = new List<KeyValuePair<KeyValuePair<int, WeakReference<Bill>>, WeakReference<BillStack>>>();
            int? res = null;

            foreach (var val in billstackReferenceHeap)
            {
                if (val.Value.IsAlive)
                {
                    if (val.Value.Equals(bs))
                    {
                        res = val.Key.Key;
                        break;
                    }
                }
                else
                {
                    removalList.Add(val);
                }
            }

            foreach (var val in removalList)
            {
                billstackReferenceHeap.Remove(val);
            }
            return res;
        }
    }
}
