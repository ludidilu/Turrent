using System;
using System.Collections.Generic;
using System.IO;

namespace Turrent_lib
{
    public class Battle_server
    {
        private static readonly Random random = new Random();

        private struct PlayerAction
        {
            public bool isMine;

            public int key;

            public int value;

            public PlayerAction(bool _isMine, int _key, int _value)
            {
                isMine = _isMine;

                key = _key;

                value = _value;
            }

            public void Write(BinaryWriter _bw)
            {
                _bw.Write(isMine);

                _bw.Write(key);

                _bw.Write(value);
            }

            public void Read(BinaryReader _br)
            {
                isMine = _br.ReadBoolean();

                key = _br.ReadInt32();

                value = _br.ReadInt32();
            }
        }

        private class BattleRecordData
        {
            public Dictionary<int, List<PlayerAction>> action = new Dictionary<int, List<PlayerAction>>();

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

        public Battle_server(bool _processBattle)
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
                        bw.Write(mCardsShowNum + mCardsShowList.Count);

                        for (int i = 0; i < mCardsShowNum; i++)
                        {
                            bw.Write(i);

                            bw.Write(recordData.mCards[i]);
                        }

                        for (int i = 0; i < mCardsShowList.Count; i++)
                        {
                            int uid = mCardsShowList[i];

                            bw.Write(uid);

                            bw.Write(recordData.oCards[uid]);
                        }
                    }
                    else
                    {
                        bw.Write(oCardsShowNum + oCardsShowList.Count);

                        for (int i = 0; i < oCardsShowNum; i++)
                        {
                            bw.Write(i + BattleConst.DECK_CARD_NUM);

                            bw.Write(recordData.oCards[i]);
                        }

                        for (int i = 0; i < oCardsShowList.Count; i++)
                        {
                            int uid = oCardsShowList[i];

                            bw.Write(uid);

                            bw.Write(recordData.mCards[uid]);
                        }
                    }

                    bw.Write(recordData.action.Count);

                    IEnumerator<KeyValuePair<int, List<PlayerAction>>> ee = recordData.action.GetEnumerator();

                    while (ee.MoveNext())
                    {
                        bw.Write(ee.Current.Key);

                        bw.Write(ee.Current.Value.Count);

                        for (int i = 0; i < ee.Current.Value.Count; i++)
                        {
                            PlayerAction action = ee.Current.Value[i];

                            action.Write(bw);
                        }
                    }

                    serverSendDataCallBack(_isMine, false, ms);
                }
            }
        }

        private void ServerDoAction(bool _isMine, BinaryReader _br)
        {
            int uid = _br.ReadInt32();

            int pos = _br.ReadInt32();

            if (_isMine && uid >= BattleConst.DECK_CARD_NUM)
            {
                throw new Exception("ServerDoAction error0");
            }
            else if (!_isMine && uid < BattleConst.DECK_CARD_NUM)
            {
                throw new Exception("ServerDoAction error1");
            }

            List<PlayerAction> list;

            if (!recordData.action.TryGetValue(tick, out list))
            {
                list = new List<PlayerAction>();

                recordData.action.Add(tick, list);
            }

            list.Add(new PlayerAction(_isMine, uid, pos));

            if (processBattle || recordData.isVsAi)
            {
                battle.AddAction(_isMine, uid, pos);
            }
        }

        private void ServerQuitBattle(bool _isMine)
        {

        }

        internal void ServerUpdate()
        {
            List<PlayerAction> list;

            if (recordData.action.TryGetValue(tick, out list))
            {
                using (MemoryStream mMs = new MemoryStream(), oMs = new MemoryStream())
                {
                    using (BinaryWriter mBw = new BinaryWriter(mMs), oBw = new BinaryWriter(oMs))
                    {
                        mBw.Write(tick);

                        oBw.Write(tick);

                        mBw.Write(list.Count);

                        oBw.Write(list.Count);

                        List<int> mList = new List<int>();

                        List<int> oList = new List<int>();

                        for (int i = 0; i < list.Count; i++)
                        {
                            PlayerAction action = list[i];

                            action.Write(mBw);

                            action.Write(oBw);

                            if (action.isMine)
                            {
                                oList.Add(action.key);

                                oCardsShowList.Add(action.key);

                                if (recordData.mCards.Length > mCardsShowNum)
                                {
                                    mList.Add(mCardsShowNum);

                                    mCardsShowNum++;
                                }
                            }
                            else
                            {
                                mList.Add(action.key);

                                mCardsShowList.Add(action.key);

                                if (recordData.oCards.Length > oCardsShowNum)
                                {
                                    oList.Add(oCardsShowNum);

                                    oCardsShowNum++;
                                }
                            }
                        }

                        mBw.Write(mList.Count);

                        for (int i = 0; i < mList.Count; i++)
                        {
                            int uid = mList[i];

                            mBw.Write(uid);

                            mBw.Write(recordData.mCards[uid]);
                        }

                        oBw.Write(oList.Count);

                        for (int i = 0; i < oList.Count; i++)
                        {
                            int uid = oList[i];

                            oBw.Write(uid + BattleConst.DECK_CARD_NUM);

                            oBw.Write(recordData.oCards[uid]);
                        }

                        serverSendDataCallBack(true, true, mMs);

                        serverSendDataCallBack(false, true, oMs);
                    }
                }
            }
            else
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    using (BinaryWriter bw = new BinaryWriter(ms))
                    {
                        bw.Write(tick);

                        bw.Write(0);

                        serverSendDataCallBack(true, true, ms);

                        serverSendDataCallBack(false, true, ms);
                    }
                }
            }

            tick++;

            if (processBattle || recordData.isVsAi)
            {
                battle.Update();
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
