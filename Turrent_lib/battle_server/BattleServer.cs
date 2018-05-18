using System;
using System.Collections.Generic;
using System.IO;

namespace Turrent_lib
{
    public class BattleServer
    {
        private static readonly Random random = new Random();

        private struct PlayerAction
        {
            public int tick;
            public bool isMine;
            public int key;
            public int value;

            public PlayerAction(int _tick, bool _isMine, int _key, int _value)
            {
                tick = _tick;
                isMine = _isMine;
                key = _key;
                value = _value;
            }
        }

        private class BattleRecordData
        {
            public List<PlayerAction> action = new List<PlayerAction>();

            public int[] mCards;

            public int[] oCards;

            public bool isVsAi;
        }

        private BattleRecordData recordData;

        private bool processBattle;

        private BattleCore battle;

        private int tick;

        private int mCardsShowNum;

        private List<int> mCardsShowList = new List<int>();

        private int oCardsShowNum;

        private List<int> oCardsShowList = new List<int>();

        private Action<bool, bool, MemoryStream> serverSendDataCallBack;

        public BattleServer(bool _processBattle)
        {
            processBattle = _processBattle;

            battle = new BattleCore();
        }

        public void ServerSetCallBack(Action<bool, bool, MemoryStream> _serverSendDataCallBack)
        {
            serverSendDataCallBack = _serverSendDataCallBack;
        }

        public void ServerStart(int _battleInitDataID, IList<int> _mCards, IList<int> _oCards, bool _isVsAi)
        {
            Log.Write("Battle Start!");

            recordData = new BattleRecordData();

            recordData.isVsAi = _isVsAi;

            InitCards(recordData, _mCards, _oCards);

            mCardsShowNum = Math.Min(BattleConst.DEFAULT_HAND_CARDS_NUM, recordData.mCards.Length);

            oCardsShowNum = Math.Min(BattleConst.DEFAULT_HAND_CARDS_NUM, recordData.oCards.Length);

            if (processBattle || recordData.isVsAi)
            {
                battle.Init(recordData.mCards, recordData.oCards);
            }
        }

        public void ServerGetPackage(BinaryReader _br, bool _isMine)
        {
            byte tag = _br.ReadByte();

            switch (tag)
            {
                case PackageTag.C2S_REFRESH:

                    ServerRefreshData(_isMine);

                    break;

                case PackageTag.C2S_DOACTION:

                    ServerDoAction(_isMine, _br);

                    break;

                case PackageTag.C2S_QUIT:

                    ServerQuitBattle(_isMine);

                    break;

                case PackageTag.C2S_RESULT:

                    break;

                default:

                    throw new Exception("Unknow PackageTag:" + tag);
            }
        }

        private void ServerRefreshData(bool _isMine)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                using (BinaryWriter bw = new BinaryWriter(ms))
                {
                    bw.Write(_isMine);

                    bw.Write(processBattle || recordData.isVsAi);

                    bw.Write(recordData.mCards.Length);

                    bw.Write(recordData.oCards.Length);

                    bw.Write(tick);

                    if (_isMine)
                    {
                        bw.Write(mCardsShowNum);

                        for (int i = 0; i < mCardsShowNum; i++)
                        {
                            bw.Write(recordData.mCards[i]);
                        }

                        bw.Write(mCardsShowList.Count);

                        for (int i = 0; i < mCardsShowList.Count; i++)
                        {
                            int uid = mCardsShowList[i];

                            bw.Write(uid);

                            bw.Write(recordData.oCards[uid]);
                        }
                    }
                    else
                    {
                        bw.Write(oCardsShowNum);

                        for (int i = 0; i < oCardsShowNum; i++)
                        {
                            bw.Write(recordData.oCards[i]);
                        }

                        bw.Write(oCardsShowList.Count);

                        for (int i = 0; i < oCardsShowList.Count; i++)
                        {
                            int uid = oCardsShowList[i];

                            bw.Write(uid);

                            bw.Write(recordData.mCards[uid]);
                        }
                    }

                    bw.Write(recordData.action.Count);

                    for (int i = 0; i < recordData.action.Count; i++)
                    {
                        PlayerAction action = recordData.action[i];

                        bw.Write(action.tick);

                        bw.Write(action.isMine);

                        bw.Write(action.key);

                        bw.Write(action.value);
                    }

                    serverSendDataCallBack(_isMine, false, ms);
                }
            }
        }













        private static void InitCards(BattleRecordData _recordData, IList<int> _mCards, IList<int> _oCards)
        {
            _recordData.mCards = new int[Math.Min(_mCards.Count, BattleConst.DECK_CARD_NUM)];

            int[] tmpArr = new int[_mCards.Count];

            for (int i = 0; i < _mCards.Count; i++)
            {
                tmpArr[i] = _mCards[i];
            }

            BattlePublicTools.Shuffle(tmpArr, random.Next);

            Array.Copy(tmpArr, _recordData.mCards, _recordData.mCards.Length);

            _recordData.oCards = new int[Math.Min(_oCards.Count, BattleConst.DECK_CARD_NUM)];

            tmpArr = new int[_oCards.Count];

            for (int i = 0; i < _oCards.Count; i++)
            {
                tmpArr[i] = _oCards[i];
            }

            BattlePublicTools.Shuffle(tmpArr, random.Next);

            Array.Copy(tmpArr, _recordData.oCards, _recordData.oCards.Length);
        }
    }
}
