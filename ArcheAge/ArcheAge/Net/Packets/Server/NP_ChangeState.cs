﻿using LocalCommons.Native.Network;

namespace ArcheAge.ArcheAge.Net
{
    public sealed class NP_ChangeState : NetPacket
    {
        public NP_ChangeState(int state) : base(02, 0x00)
        {
            //            ID     state
            //08 00 DD 02 00 00 {00 00 00 00} //SCChangeState
            //08 00 00 02 01 00 {00 00 00 00} //CSFinishState
            state += 1;
            ns.Write(state);
        }
    }
}
