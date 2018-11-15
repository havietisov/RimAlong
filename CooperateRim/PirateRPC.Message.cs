using System;
using System.Collections.Generic;
using CooperateRim;

namespace PirateRPC
{
    public partial class PirateRPC
    {
        [Serializable]
        public class Message
        {
            public List<SyncTickData> sdl;
        }
    }
}