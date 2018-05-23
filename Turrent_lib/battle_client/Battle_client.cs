using System;
using System.Collections.Generic;
using System.IO;

namespace Turrent_lib
{
    public class Battle_client : BattleCore
    {
        public bool clientIsMine;

        private bool serverProcessBattle;

        private Action<MemoryStream, Action<BinaryReader>> clientSendDataCallBack;

        private Action clientRefreshDataCallBack;

        public void Init(Action<MemoryStream, Action<BinaryReader>> _clientSendDataCallBack, Action _clientRefreshDataCallBack)
        {
            clientSendDataCallBack = _clientSendDataCallBack;

            clientRefreshDataCallBack = _clientRefreshDataCallBack;
        }

        public void ClientGetPackage(BinaryReader _br)
        {
            byte tag = _br.ReadByte();

            switch (tag)
            {
                case PackageTag.S2C_UPDATE:

                    ClientUpdate(_br);

                    break;
            }
        }

        private void RefreshData(BinaryReader _br)
        {
            Reset();

            clientIsMine = _br.ReadBoolean();

            serverProcessBattle = _br.ReadBoolean();

            int num = _br.ReadInt32();

            int[] mCards = new int[num];

            num = _br.ReadInt32();

            int[] oCards = new int[num];

            Init(mCards, oCards);

            int nowTick = _br.ReadInt32();

            num = _br.ReadInt32();

            for (int i = 0; i < num; i++)
            {
                int uid = _br.ReadInt32();

                int id = _br.ReadInt32();

                SetCard(uid, id);
            }

            num = _br.ReadInt32();

            for (int i = 0; i < num; i++)
            {
                int tmpTick = _br.ReadInt32();

                for (int m = tick; m < tmpTick; m++)
                {
                    Update();
                }

                int num2 = _br.ReadInt32();

                for (int m = 0; m < num2; m++)
                {
                    bool isMine = _br.ReadBoolean();

                    int uid = _br.ReadInt32();

                    int pos = _br.ReadInt32();

                    AddAction(isMine, uid, pos);
                }
            }

            for (int i = tick; i < nowTick; i++)
            {
                Update();
            }

            clientRefreshDataCallBack();
        }

        private void ClientUpdate(BinaryReader _br)
        {
            int nowTick = _br.ReadInt32();

            if (tick != nowTick)
            {
                throw new Exception("ClientUpdate error   clientTick:" + tick + "   serverTick:" + nowTick);
            }

            int num = _br.ReadInt32();

            if (num > 0)
            {
                for (int i = 0; i < num; i++)
                {
                    bool isMine = _br.ReadBoolean();

                    int uid = _br.ReadInt32();

                    int pos = _br.ReadInt32();

                    AddAction(isMine, uid, pos);
                }

                num = _br.ReadInt32();

                for (int i = 0; i < num; i++)
                {
                    int uid = _br.ReadInt32();

                    int id = _br.ReadInt32();

                    SetCard(uid, id);
                }
            }

            Update();
        }

        public int ClientRequestAddAction(int _uid, int _pos)
        {
            int result = CheckAddSummon(clientIsMine, _uid, _pos);

            if (result == -1)
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    using (BinaryWriter bw = new BinaryWriter(ms))
                    {
                        bw.Write(PackageTag.C2S_DOACTION);

                        bw.Write(_uid);

                        bw.Write(_pos);

                        clientSendDataCallBack(ms, GetAddActionResult);
                    }
                }
            }

            return result;
        }

        private void GetAddActionResult(BinaryReader _br)
        {

        }

        public void ClientRequestRefreshData()
        {
            using (MemoryStream ms = new MemoryStream())
            {
                using (BinaryWriter bw = new BinaryWriter(ms))
                {
                    bw.Write(PackageTag.C2S_REFRESH);

                    clientSendDataCallBack(ms, RefreshData);
                }
            }
        }

        public List<int> GetHandCards()
        {
            return clientIsMine ? mHandCards : oHandCards;
        }

        public Turrent[] GetMyTurrent()
        {
            return clientIsMine ? mTurrent : oTurrent;
        }

        public Turrent[] GetOppTurrent()
        {
            return clientIsMine ? oTurrent : mTurrent;
        }
    }
}
