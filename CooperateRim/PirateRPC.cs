using System;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;

namespace PirateRPC
{
    public partial class PirateRPC
    {

        public static void SendInvocation(Stream s, Action<Stream> act)
        {
            DateTime dt12 = DateTime.Now;
            MemoryStream initialBuffer = new MemoryStream();
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(initialBuffer, new Modification(act));
            initialBuffer.Flush();
            string ss = Convert.ToBase64String(initialBuffer.GetBuffer(), 0, (int)initialBuffer.Length);
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