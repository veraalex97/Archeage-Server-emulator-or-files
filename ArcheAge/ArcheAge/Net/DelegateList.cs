﻿using ArcheAge.ArcheAge.Net.Connections;
using ArcheAge.ArcheAge.Structuring;
using LocalCommons.Native.Logging;
using LocalCommons.Native.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ArcheAge.ArcheAge.Net
{
    /// <summary>
    /// Delegate List That Contains Information About Received Packets.
    /// 包含接收包信息的委托列表。
    /// </summary>
    public class DelegateList
    {
        private static int m_Maintained;
        private static PacketHandler<LoginConnection>[] m_LHandlers;
        private static PacketHandler<ClientConnection>[] m_CHandlers;
        private static Dictionary<int, PacketHandler<ClientConnection>[]> levels;
        private static LoginConnection m_CurrentLoginServer;

        public static LoginConnection CurrentLoginServer
        {
            get { return m_CurrentLoginServer; }
        }

        public static Dictionary<int, PacketHandler<ClientConnection>[]> ClientHandlers
        {
            get { return levels; }
        }

        public static PacketHandler<LoginConnection>[] LHandlers
        {
            get { return m_LHandlers; }
        }

        public static void Initialize()
        {
            m_LHandlers = new PacketHandler<LoginConnection>[0x20];
            //m_LHandlers = new PacketHandler<ClientConnection>[0x30];
            levels = new Dictionary<int, PacketHandler<ClientConnection>[]>();

            RegisterDelegates();
        }
        //注册服务
        private static void RegisterDelegates()
        {
            //-------------- Login - Game Communication Packets ------------
            Register(0x00, new OnPacketReceive<LoginConnection>(Handle_GameRegisterResult)); //Taken Fully
            Register(0x01, new OnPacketReceive<LoginConnection>(Handle_AccountInfoReceived)); //Taken Fully

            //-------------- Client Communication Packets ------------------
            //客户端通讯处
            //-------------- Using - Packet Level - Packet Opcode(short) - Receive Delegate -----


            Register(0x01, 0x00, new OnPacketReceive<ClientConnection>(OnPacketReceive_ClientAuthorized));
            Register(0x01, 0x1b7f, new OnPacketReceive<ClientConnection>(OnPacketReceive_Client01));
            //Register(0x02, 0x01, new OnPacketReceive<ClientConnection>(Onpacket0201));
            //Register(0x02, 0x01, new OnPacketReceive<ClientConnection>(Onpacket0201));
            //Register(0x02, 0x12, new OnPacketReceive<ClientConnection>(Onpacket0212));
            //Register(0x02, 0x0012, new OnPacketReceive<ClientConnection>(OnPacketReceive_Ping)); //+

            // NL0bP https://github.com/NL0bP/Archeage-Server-emulator
            //Register(0x01, 0x0000, new OnPacketReceive<ClientConnection>(OnPacketReceive_X2EnterWorld)); //+
            Register(0x02, 0x0012, new OnPacketReceive<ClientConnection>(OnPacketReceive_Ping)); //+
            Register(0x02, 0x0001, new OnPacketReceive<ClientConnection>(OnPacketReceive_FinishState0201)); //todo 等会研究研究发送的包
            Register(0x01, 0xE4FB, new OnPacketReceive<ClientConnection>(OnPacketReceive_ClientE4FB));
            Register(0x01, 0x0D7C, new OnPacketReceive<ClientConnection>(OnPacketReceive_Client0D7C));
            Register(0x01, 0xE17B, new OnPacketReceive<ClientConnection>(OnPacketReceive_ClientE17B));
            Register(0x05, 0x0438, new OnPacketReceive<ClientConnection>(OnPacketReceive_Client0438));
            Register(0x05, 0x0088, new OnPacketReceive<ClientConnection>(OnPacketReceive_ReloginRequest_0x0088));
        }



        #region Client Callbacks Implementation

        //验证用户登录权限  不知如何使用，废弃中
        public static void OnPacketReceive_ClientAuthorized(ClientConnection net, PacketReader reader)
        {
            //B3 04 00 00 B3 04 00 00 8C 28 22 00 E7 F0 0C C6 FF FF FF FF 00 
            //reader.Offset += 2;
            short type = reader.ReadLEInt16(); //type
            //long protocol = reader.ReadLEInt64(); //Protocols?

            //NL0bP
            int pFrom = reader.ReadLEInt32(); //p_from
            int pTo = reader.ReadLEInt32(); //p_to

            long accountId = reader.ReadLEInt64(); //Account Id
            //reader.Offset += 4;
            int sessionId = reader.ReadLEInt32(); //User Session Id

            short tb = reader.ReadByte(); //tb
            int revision = reader.ReadLEInt32(); //revision 资源版本  同客户端标题中的括号中的内容 如（r.321543）
            int index = reader.ReadLEInt32(); //index


            Account m_Authorized = ClientConnection.CurrentAccounts.FirstOrDefault(kv => kv.Value.Session == sessionId && kv.Value.AccountId == accountId).Value;
            if (m_Authorized == null)
            {
                net.Dispose();
                Logger.Trace("账户 {0} 未登录：无法继续。", net);
            }
            else
            {
                net.CurrentAccount = m_Authorized;
                net.SendAsync(new NP_X2EnterWorldResponsePacket());
                net.SendAsync(new NP_ChangeState(-1));
                //net.SendAsync(new NP_ClientConnected2());
                //net.SendAsync(new NP_Client02());
                net.SendAsync(new NP_ClientConnected());
            }
        }

        
        /**
         * 
         * 连接游戏服务器第一包
         * */
        public static void OnPacketReceive_Client01(ClientConnection net,PacketReader reader)
        {
            net.SendAsyncHex(new NP_Hex("0700dd05f2bdb150102a00dd056f6fcc01d3a2724213e3b3e05321512c00dd0205d012452606e6b6865727f7c797704010e0b081512c00dd021300157f26060000000060bee1d96c0100000000058ef05d96663707d219375020f0b62d01007dd3e50ffe00dd058ef95d96663707d7a7775020f0c090613101d1a1724212e2b2835323f3c494643404d5a5754515e6b6865626f7c7976737fed0a0704011e1b1815122f2c292623303d3a3734414e4b4845525f5c596663606d6a7774717e7c0906030fed1a1714111e2b2825222f3c393633304d4a4744415e5b5855526f6c696663707d7a7774010e0b0815121f1c192623202d2a3734313e3b4845424f4c595653505d6a6764616e7b7875727fed0a0704011e1b1815122f2c292623303d3a3744414e4b4855525f5c596663606d6a7774717e7b0805020f0c191613101d2a2724212e3b3835323f4c494643405d5a5754516e6b680b08151860800dd0520b181510f00dd0552379ac797704010e0b08151860800dd0520a188501140"));


        }
        public static void Onpacket0201(ClientConnection net,PacketReader reader)
        {
            byte b3 = reader.ReadByte();
            if ( b3== 0x0)
            {


                //net.SendAsync(new NP_Client0200());//同样返回报错
                net.SendAsyncHex(new NP_Hex("1400dd05fee767865627f6cf97087265899fe9242175"));
                net.SendAsyncHex(new NP_Hex("" +
                    "1e00dd020f000f00735f7069726174655f69736c616e64000000000000000001" +
                    "4d00dd05e1606b03c3a31536778cd1e4324092a4fb031865b9ca6f4768d0bf8f29288d0aa62032df76266a421005dc04e238f2c494643405d5a5754516e6b7875626f6c797704010e0b0815152f322" +
                    "a900dd05e1936731ffe0a119356592c1b87d0d80a6e0105a6bba8a10367483c1ab3c4882a3f1145673bec8196e6492d9ee3f4690acf7111c72a0ca052a138bc0f02457ceeaba044766bec3172173c9d3f536418afec91e347486c3e0254b9dacbc16416abccd13257981c7ab25579cb3b905596bbbc2052472c8c0f5224398a0e2041e62a0c7162b66909cf3265197acf5175128b6d710217f92c5ab314b98b0b911236489dfef51e71747" +
                    "2a00dd050e65cc01d2a2724212e3b3835323f4c4946434053561754516e6b6865727f7c797704010e0b08151" +
                    "7700dd057997103300eea3724212e2b4845524f4c595643505d7a6764616e7b7875767bed0a0704011e1b1815122f2c292623303d3a3744414e4b4855525f5c596663606d6a7774717e7b0805020f0c191613101d2a2724212e3b3835323f4c49465340ac6a5754566a4b3865727f7c781348ddcac8f8b99f2" +
                    "0900dd05a9b6e551104170" +
                    "1c00dd05d0635d04d5af754516e6b9895827f7c897704010e0b0815ec4f4" +
                    "0e00dd057a2fb0c797704010e0b08151" +
                    "0900dd059db9ac53114270" +
                    "0e00dd05282df0c797704010e0b08151" +
                    "2300dd0523e85c835223f4c494643405d5a5754516e6b6865727f7c797704010e0b0815142" + //2300DD0500E82E825223F4C494643405D5A5754516E6B6865727F7C797704010E0B0815142
                    "0a00dd058b7cf511e0b08151"    //0A00DD05817C5812E0B08151
                    ));

            }else if (b3 == 0x01)
            {
                net.SendAsync(new NP_Client02002());
            }
            //net.SendAsync(new NP_Clientdd05bae9());
        }
        public static void OnPacketReceive_FinishState0201(ClientConnection net, PacketReader reader)
        {
            int state = reader.ReadLEInt32(); //считываем state
            //--------------------------------------------------------------------------
            //NP_ChangeState
            net.SendAsync(new NP_ChangeState(state));
            //--------------------------------------------------------------------------
            if (state == 0)
            {
                //выводим один раз
                //пакет №8 DD05 S>C
                //这里有改变95-》55  根据版本而变
                net.SendAsync(new NP_Packet_0x0094()); //1400DD05C2E7E3865627F6CF97087265899FE9242175
                //--------------------------------------------------------------------------
                //SetGameType
                net.SendAsync(new NP_SetGameType());
                //--------------------------------------------------------------------------
                //пакеты для входа в Лобби
                //пакет №10 DD05 S>C
                net.SendAsync(new NP_Packet_0x0034()); //5000DD057F20F4C282625271B0CB11257381D3E43840DBA6F90B2C06A99043486EEFCD4B6745F31F35E70901D0441D85AB78EE825322F4C494643405D5A5754517E7B7875627F7C797704010E0B0115081B0
                //пакет №11 DD05 S>C
                net.SendAsync(new NP_Packet_0x02C3()); //A900DD0574936530FFE0A119356592C1B87D0D80A6E0105A6BBA8A10367483C1AB3C4882A3F1145673BEC8196E6492D9EE3F4690ACF7111C72A0CA052A138BC0F02457CEEABA044766BEC3172173C9D3F536418AFEC91E347486C3E0254B9DACBC16416ABCCD13257981C7AB25579CB3B905596BBBC2052472C8C0F5224398A0E2041E62A0C7162B66909CF3265197ACF5175128B6D710217F92C5AB314B98B0B911236489DFEF51E71747
                //пакет №12 DD05 S>C
                net.SendAsync(new NP_Packet_0x00EC()); //2A00DD05B9656B03D2A2724212E3B3835323F4C494643405B5F1754516E6B6865727F7C797704010E0B08151
                //пакет №13 DD05 S>C
                net.SendAsync(new NP_Packet_0x0281()); //7700DD05F997B53000EEA3724212E2B4845524F4C595643505D7A6764616E7B7875767BED0A0704011E1B1815122F2C292623303D3A3744414E4B4855525F5C596663606D6A7774717E7B0805020F0C191613101D2A2724212E3B3835323F4C49465340AC6A5754566A4B3865727F7C781348DDCAC8F8B99F2
                //пакет №14 DD05 S>C
                net.SendAsync(new NP_Packet_0x00BA()); //0900DD05BFB67E50104170
                //пакет №15 DD05 S>C
                net.SendAsync(new NP_Packet_0x018A()); //1C00DD054863A207D5AF754516E6B9895827F7C897704010E0B0815EC4F4
                //пакет №16 DD05 S>C
                net.SendAsync(new NP_Packet_0x01CC()); //0E00DD05842F7EC797704010E0B08151
                //пакет №17 DD05 S>C
                net.SendAsync(new NP_Packet_0x0030()); //0900DD052CB90D53114270
                //пакет №18 DD05 S>C
                net.SendAsync(new NP_Packet_0x01AF()); //0E00DD054E2DB8C597704010E0B08151
                //пакет №19 DD05 S>C
                net.SendAsync(new NP_Packet_0x02CF()); //2300DD0500E82E825223F4C494643405D5A5754516E6B6865727F7C797704010E0B0815142
                //пакет №19 DD05 S>C
                net.SendAsync(new NP_Packet_0x029C()); //0A00DD05817C5812E0B08151
            }
        }

        public static void Onpacket0212(ClientConnection net, PacketReader reader)
        {
            //reader.Offset += 8; //00 00 00 00 00 00 00 00  Undefined Data
            int number1 = reader.ReadLEInt32();
            int number2 = reader.ReadLEInt32();
            int number3 = reader.ReadLEInt32();
            int number4 = reader.ReadLEInt32();
            int number5 = reader.ReadLEInt32();
            net.SendAsync(new NP_Client0212(number1,number2,number3,number4,number5));
        }

        /// <summary>
        /// Получили клиентский пакет Ping, отвечаем серверным пакетом Pong
        /// </summary>
        /// <param name="net"></param>
        /// <param name="reader"></param>
        public static void OnPacketReceive_Ping(ClientConnection net, PacketReader reader)
        {
            //Ping
            long tm = reader.ReadLEInt64(); //tm
            long when = reader.ReadLEInt64(); //when
            int local = reader.ReadLEInt32(); //local
            net.SendAsync(new NP_Pong(tm, when, local));
        }


        public static void OnPacketReceive_ClientE4FB(ClientConnection net, PacketReader reader)
        {
            var number1 = reader.ReadLEInt32();
            //net.SendAsync(new NP_ChangeState(0));
        }

        public static void OnPacketReceive_Client0D7C(ClientConnection net, PacketReader reader)
        {
            var number1 = reader.ReadLEInt32();
            //var number2 = reader.ReadLEInt32();
            //var number3 = reader.ReadLEInt32();
            //var number4 = reader.ReadLEInt32();
            //var number5 = reader.ReadLEInt32();
            //net.SendAsync(new NP_ChangeState(0));
        }
        public static void OnPacketReceive_ClientE17B(ClientConnection net, PacketReader reader)
        {
            ////пакет для входа в Лобби - продолжение
            //пакет №32 DD05 S>C
            net.SendAsync(new NP_Packet_0x0272());   //0700DD050CBD7B5010
            net.SendAsync(new NP_Packet_0x00EC_2()); //2A00DD05EA6F6B03D3A2724213E3B3835323F4C4946434053B833B2816E6B6865727F7C797704010E0B08151
            net.SendAsync(new NP_Packet_0x008C());   //FE00DD0595F92296663707D7A7775020F0C090613101D1A1724212E2B2835323F3C494643404D5A5754515E6B6865626F7C7976737FED0A0704011E1B1815122F2C292623303D3A3734414E4B4845525F5C596663606D6A7774717E7C0906030FED1A1714111E2B2825222F3C393633304D4A4744415E5B5855526F6C696663707D7A7774010E0B0815121F1C192623202D2A3734313E3B4845424F4C595653505D6A6764616E7B7875727FED0A0704011E1B1815122F2C292623303D3A3744414E4B4855525F5C596663606D6A7774717E7B0805020F0C191613101D2A2724212E3B3835323F4C494643405D5A5754516E6B6865727F7C797704010E0B08151
            net.SendAsync(new NP_Packet_0x014D());   //0F00DD050637C9C697704010E0B0815186
            //список чаров
            net.SendAsync(new NP_Packet_CharList_0x0079()); //0209DD051E05ACB68556F261C495603654B3CB183376E4B591B032F
            //не забыть установить кол-во чаров в ArcheAgeLoginServer :: ArcheAgePackets.cs :: AcWorldList_0X08
            //net.SendAsync(new NP_Packet_CharList_empty_0x0079()); //0800DD05FEA1C9531140
            ///идет клиентский пакет 13000005393DB7A29CAA4C2365F02DB94C5B18BB50
            ///идет клиентский пакет 1300000539297EE205DC192D2A33B7071BC23B38BC
            ///идет клиентский пакет 1300000539B1D74AE4C48857E02BAB7E33AF496A8C
            ///идет клиентский пакет 1300000539211BA0D0AC0DE28974E1158F1BE5BB86
            net.SendAsync(new NP_Packet_0x014F()); //2400DD0564F11F825223F4C495643405D55A754516E634B7D47DF7C797704010E0B081514272
            //net.SendAsync(new NP_Packet_quit_0x00A5());

            //эти пакеты нужны когда есть чары в лобби
            net.SendAsync(new NP_Packet_0x0145());   //1D00DD052777B6070231744517E6BD86214285B4FE1F2E30D1BD8B5DC4F423
            net.SendAsync(new NP_Packet_0x0145_2()); //1D00DD051C70B6070231744514E6BD86214285B4FE1F2E30D2BD8B5DC4F423
            net.SendAsync(new NP_Packet_0x0145_3()); //1D00DD050D71B6074342744517E6BD86214285B4FE1F2E30D1BD8B5DC4F423
            net.SendAsync(new NP_Packet_0x0145_4()); //1D00DD05FA72B6074342744514E6BD86214285B4FE1F2E30D2BD8B5DC4F423
        }
        public static void OnPacketReceive_Client0438(ClientConnection net, PacketReader reader)
        {
            ///клиентский пакет на вход в мир 13000005 3804 2E8CFF98F0282A5A79DE98E9BE80B6
            ///зашифрован - не ловится
        }
        public static void OnPacketReceive_ReloginRequest_0x0088(ClientConnection net, PacketReader reader)
        {
            ///клиентский пакет на релогин Recv: 13 00 00 05 34 0E 6F 39 8E 0A E3 5C E5 B9 85 25 D3 3E B3 8A 74
            net.SendAsync(new NP_Packet_quit_0x01F1()); //good bye
            net.SendAsync(new NP_Packet_0x01E5()); //good bye
        }


        #endregion

        #region Callbacks Implementation

        private static void Handle_AccountInfoReceived(LoginConnection net, PacketReader reader)
        {
            //Set Account Info
            Account account = new Account();
            account.AccountId = reader.ReadInt32();
            account.AccessLevel = reader.ReadByte();
            account.Membership = reader.ReadByte();
            account.Name = reader.ReadDynamicString();
            //account.Password = reader.ReadDynamicString();
            account.Session = reader.ReadInt32();
            account.LastEnteredTime = reader.ReadInt64();
            account.LastIp = reader.ReadDynamicString();

            Console.WriteLine("准备登陆的账号:<"+account.AccountId+">;");
            //检查账户是否在线，若在线则强制断开
            Account m_Authorized = ClientConnection.CurrentAccounts.FirstOrDefault(kv => kv.Value.AccountId == account.AccountId).Value;
            if (m_Authorized!=null)
            {
                //Already
                Account acc = ClientConnection.CurrentAccounts[m_Authorized.Session];
                if (acc.Connection != null)
                {
                    acc.Connection.Dispose(); //Disconenct  
                    Logger.Trace("账户《 " + acc.Name + "》 二次登陆，旧连接被强行断开");
                }
                else
                {
                    ClientConnection.CurrentAccounts.Remove(account.Session);
                }
            }
            else
            {
                Logger.Trace("账户《 {0}》: 授权", account.Name);
                ClientConnection.CurrentAccounts.Add(account.Session, account);
            }
        }

        private static void Handle_GameRegisterResult(LoginConnection con, PacketReader reader)
        {
            bool result = reader.ReadBoolean();
            if (result)
                Logger.Trace("成功安装登陆服务器");
            else
                Logger.Trace("有些问题是在安装登录服务器出现");
            if(result)
               m_CurrentLoginServer = con;
        }

        #endregion

        private static void Register(short opcode, OnPacketReceive<LoginConnection> e)
        {
            m_LHandlers[opcode] = new PacketHandler<LoginConnection>(opcode, e);
            m_Maintained++;
        }

        private static void Register(byte level, ushort opcode, OnPacketReceive<ClientConnection> e)
        {
            if (!levels.ContainsKey(level))
            {
                PacketHandler<ClientConnection>[] handlers = new PacketHandler<ClientConnection>[0xFFFF];
                handlers[opcode] = new PacketHandler<ClientConnection>(opcode, e);
                levels.Add(level, handlers);
            }
            else
            {
                levels[level][opcode] = new PacketHandler<ClientConnection>(opcode, e);
            }
        }

    }
}
