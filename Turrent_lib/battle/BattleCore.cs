using System;
using System.Collections.Generic;

namespace Turrent_lib
{
    public class BattleCore
    {
        public static Func<int, IUnitSDS> getUnitData;

        public static Func<int, ITurrentSDS> getTurrentData;

        public static void Init(Func<int, IUnitSDS> _getUnitData, Func<int, ITurrentSDS> _getTurrentData)
        {
            getUnitData = _getUnitData;

            getTurrentData = _getTurrentData;
        }

        internal Turrent[] mTurrent = new Turrent[BattleConst.MAP_WIDTH * BattleConst.MAP_HEIGHT];

        internal Turrent[] oTurrent = new Turrent[BattleConst.MAP_WIDTH * BattleConst.MAP_HEIGHT];

        internal int mBase;

        internal int oBase;

        internal int mMoney;

        internal int oMoney;

        internal List<int> mHandCards = new List<int>();

        internal List<int> oHandCards = new List<int>();

        internal Queue<int> mCards = new Queue<int>();

        internal Queue<int> oCards = new Queue<int>();

        private int[] cardsArr;

        private int time;

        private List<KeyValuePair<int, int>> mActionList = new List<KeyValuePair<int, int>>();

        private List<KeyValuePair<int, int>> oActionList = new List<KeyValuePair<int, int>>();

        internal void Init(int[] _mCards, int[] _oCards)
        {
            cardsArr = new int[BattleConst.DECK_CARD_NUM * 2];

            mMoney = oMoney = BattleConst.DEFAULT_MONEY;

            for (int i = 0; i < BattleConst.DECK_CARD_NUM && i < _mCards.Length; i++)
            {
                SetCard(i, _mCards[i]);

                if (i < BattleConst.DEFAULT_HAND_CARDS_NUM)
                {
                    mHandCards.Add(i);
                }
                else
                {
                    mCards.Enqueue(i);
                }
            }

            for (int i = 0; i < BattleConst.DECK_CARD_NUM && i < _oCards.Length; i++)
            {
                int index = BattleConst.DECK_CARD_NUM + i;

                SetCard(index, _oCards[i]);

                if (i < BattleConst.DEFAULT_HAND_CARDS_NUM)
                {
                    oHandCards.Add(index);
                }
                else
                {
                    oCards.Enqueue(index);
                }
            }
        }

        private int CheckAddSummon(bool _isMine, int _uid, int _pos)
        {
            List<int> handCards;

            int money;

            Turrent[] turrent;

            if (_isMine)
            {
                handCards = mHandCards;

                money = mMoney;

                turrent = mTurrent;
            }
            else
            {
                handCards = oHandCards;

                money = oMoney;

                turrent = oTurrent;
            }

            int handCardsIndex = handCards.IndexOf(_uid);

            if (handCardsIndex != -1)
            {
                int unitID = GetCard(_uid);

                IUnitSDS sds = getUnitData(unitID);

                int row = _pos % BattleConst.MAP_WIDTH;

                if (Array.IndexOf(sds.GetRow(), row) != -1)
                {
                    int x = _pos % BattleConst.MAP_WIDTH;

                    int y = _pos / BattleConst.MAP_WIDTH;

                    if (Array.IndexOf(sds.GetRow(), y) == -1)
                    {
                        return 5;
                    }

                    for (int i = 0; i < sds.GetPos().Length; i++)
                    {
                        int pos = sds.GetPos()[i];

                        int offsetX = pos % BattleConst.MAP_WIDTH;

                        if (x + offsetX < BattleConst.MAP_WIDTH)
                        {
                            if (turrent[_pos + pos] != null)
                            {
                                return 6;
                            }
                        }
                        else
                        {
                            return 4;
                        }
                    }
                }
                else
                {
                    return 3;
                }

                if (sds.GetCost() > money)
                {
                    return 1;
                }
                else
                {
                    return -1;
                }
            }
            else
            {
                return 7;
            }
        }

        private void AddSummon(bool _isMine, int _uid, int _pos)
        {
            int unitID = GetCard(_uid);

            IUnitSDS sds = getUnitData(unitID);

            if (_isMine)
            {
                mMoney -= sds.GetCost();

                mHandCards.Remove(_uid);
            }
            else
            {
                oMoney -= sds.GetCost();

                oHandCards.Remove(_uid);
            }

            AddUnit(_isMine, sds, _pos);
        }

        private void AddUnit(bool _isMine, IUnitSDS _sds, int _pos)
        {
            Turrent[] turrentPos = _isMine ? mTurrent : oTurrent;

            Unit unit = new Unit();

            unit.Init(this, _isMine, _sds, _pos);

            for (int i = 0; i < _sds.GetPos().Length; i++)
            {
                int pos = _sds.GetPos()[i];

                ITurrentSDS sds = _sds.GetTurrent()[i];

                Turrent turrent = new Turrent();

                turrent.Init(this, unit, sds, pos, time);

                turrentPos[pos] = turrent;
            }
        }

        private int GetCard(int _uid)
        {
            return cardsArr[_uid];
        }

        private void SetCard(int _uid, int _id)
        {
            cardsArr[_uid] = _id;
        }

        internal void Update(int _deltaTime)
        {
            time += _deltaTime;

            UpdateTurrent();

            UpdateAction();
        }

        private void UpdateTurrent()
        {
            List<Turrent> list = new List<Turrent>();

            for (int i = 0; i < mTurrent.Length; i++)
            {
                Turrent turrent = mTurrent[i];

                if (turrent != null && turrent.sds.GetAttackGap() > 0)
                {
                    list.Add(turrent);
                }
            }

            for (int i = 0; i < oTurrent.Length; i++)
            {
                Turrent turrent = oTurrent[i];

                if (turrent != null && turrent.sds.GetAttackGap() > 0)
                {
                    list.Add(turrent);
                }
            }

            if (list.Count > 0)
            {
                while (true)
                {
                    list.Sort(SortTurrentList);

                    Turrent turrent = list[0];

                    list.RemoveAt(0);

                    if (time >= turrent.time)
                    {
                        turrent.Update(time);
                    }
                    else
                    {
                        break;
                    }

                    list.Add(turrent);
                }
            }
        }

        private void UpdateAction()
        {
            List<int> delList = null;

            for (int i = 0; i < mActionList.Count; i++)
            {
                KeyValuePair<int, int> pair = mActionList[i];

                int result = CheckAddSummon(true, pair.Key, pair.Value);

                if (result < 0)
                {
                    AddSummon(true, pair.Key, pair.Value);

                    if (delList == null)
                    {
                        delList = new List<int>();
                    }

                    delList.Add(i);
                }
                else if (result == 1)
                {
                    break;
                }
                else
                {
                    throw new Exception("m summon error");
                }
            }

            if (delList != null)
            {
                int offset = 0;

                for (int i = 0; i < delList.Count; i++)
                {
                    mActionList.RemoveAt(delList[i] + offset);

                    offset--;
                }

                delList.Clear();
            }

            for (int i = 0; i < oActionList.Count; i++)
            {
                KeyValuePair<int, int> pair = oActionList[i];

                int result = CheckAddSummon(false, pair.Key, pair.Value);

                if (result < 0)
                {
                    AddSummon(false, pair.Key, pair.Value);

                    if (delList == null)
                    {
                        delList = new List<int>();
                    }

                    delList.Add(i);
                }
                else if (result == 1)
                {
                    break;
                }
                else
                {
                    throw new Exception("o summon error");
                }
            }

            if (delList != null)
            {
                int offset = 0;

                for (int i = 0; i < delList.Count; i++)
                {
                    oActionList.RemoveAt(delList[i] + offset);

                    offset--;
                }
            }
        }

        internal void BaseBeDamage(Turrent _turrent)
        {
            if (_turrent.parent.isMine)
            {
                oBase -= _turrent.sds.GetAttackDamage();
            }
            else
            {
                mBase -= _turrent.sds.GetAttackDamage();
            }
        }

        private static int SortTurrentList(Turrent _t0, Turrent _t1)
        {
            if (_t0.time > _t1.time)
            {
                return 1;
            }
            else if (_t0.time < _t1.time)
            {
                return -1;
            }
            else
            {
                return 0;
            }
        }
    }
}
