using System;
using System.Collections.Generic;
using System.IO;

namespace Turrent_lib
{
    public class Battle_client : BattleCore
    {
        public bool clientIsMine;

        private bool serverProcessBattle;

        public void RefreshData(BinaryReader _br)
        {
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
        }

        public void ClientUpdate(BinaryReader _br)
        {
            int nowTick = _br.ReadInt32();

            if (tick != nowTick)
            {
                throw new Exception("ClientUpdate error   clientTick:" + tick + "   serverTick:" + nowTick);
            }

            int num = _br.ReadInt32();

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

            Update();
        }
    }
}
