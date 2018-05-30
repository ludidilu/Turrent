using System;
using System.Collections;
using System.Collections.Generic;
using tuple;

namespace Turrent_lib
{
    public enum BattleResult
    {
        DRAW,
        M_WIN,
        O_WIN,
        NOT_OVER
    }

    public class BattleCore
    {
        public static Func<int, IUnitSDS> GetUnitData;

        public static Func<int, ITurrentSDS> GetTurrentData;

        public static void Init<T, U>(Dictionary<int, T> _unitDic, Dictionary<int, U> _turrentDic) where T : IUnitSDS where U : ITurrentSDS
        {
            GetUnitData = delegate (int _id)
            {
                return _unitDic[_id];
            };

            GetTurrentData = delegate (int _id)
            {
                return _turrentDic[_id];
            };
        }

        internal Turrent[] mTurrent = new Turrent[BattleConst.MAP_WIDTH * BattleConst.MAP_HEIGHT];

        internal Turrent[] oTurrent = new Turrent[BattleConst.MAP_WIDTH * BattleConst.MAP_HEIGHT];

        public int mBase { private set; get; }

        public int oBase { private set; get; }

        public int mMoney { private set; get; }

        public int oMoney { private set; get; }

        private int mMoneyTime;

        private int oMoneyTime;

        internal List<int> mHandCards = new List<int>();

        internal List<int> oHandCards = new List<int>();

        internal Queue<int> mCards = new Queue<int>();

        internal Queue<int> oCards = new Queue<int>();

        private int[] mCardsArr = new int[BattleConst.DECK_CARD_NUM];

        private int[] oCardsArr = new int[BattleConst.DECK_CARD_NUM];

        protected int tick;

        private int maxTime;

        private List<Tuple<bool, int, int>> actionList = new List<Tuple<bool, int, int>>();

        internal void Init(int[] _mCards, int[] _oCards, int _mBase, int _oBase, int _maxTime)
        {
            Reset();

            mMoney = oMoney = BattleConst.DEFAULT_MONEY;

            mBase = _mBase;

            oBase = _oBase;

            maxTime = _maxTime;

            for (int i = 0; i < BattleConst.DECK_CARD_NUM && i < _mCards.Length; i++)
            {
                SetCard(true, i, _mCards[i]);

                if (i < BattleConst.HAND_CARDS_NUM)
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
                SetCard(false, i, _oCards[i]);

                if (i < BattleConst.HAND_CARDS_NUM)
                {
                    oHandCards.Add(i);
                }
                else
                {
                    oCards.Enqueue(i);
                }
            }
        }

        public int CheckAddSummon(bool _isMine, int _uid, int _pos)
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
                int unitID = GetCard(_isMine, _uid);

                IUnitSDS sds = GetUnitData(unitID);

                if (sds.GetCost() > money)
                {
                    return 1;
                }

                int row = _pos / BattleConst.MAP_WIDTH;

                if (Array.IndexOf(sds.GetRow(), row) != -1)
                {
                    int x = _pos % BattleConst.MAP_WIDTH;

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

                return -1;
            }
            else
            {
                return 7;
            }
        }

        private BattleSummonVO AddSummon(bool _isMine, int _uid, int _pos)
        {
            int time = tick * BattleConst.TICK_TIME;

            int unitID = GetCard(_isMine, _uid);

            IUnitSDS sds = GetUnitData(unitID);

            if (_isMine)
            {
                if (mMoney == BattleConst.MAX_MONEY)
                {
                    mMoneyTime = time + BattleConst.RECOVER_MONEY_TIME;
                }

                mMoney -= sds.GetCost();

                mHandCards.Remove(_uid);

                mCards.Enqueue(_uid);

                mHandCards.Add(mCards.Dequeue());
            }
            else
            {
                if (oMoney == BattleConst.MAX_MONEY)
                {
                    oMoneyTime = time + BattleConst.RECOVER_MONEY_TIME;
                }

                oMoney -= sds.GetCost();

                oHandCards.Remove(_uid);

                oCards.Enqueue(_uid);

                oHandCards.Add(oCards.Dequeue());
            }

            AddUnit(_isMine, sds, _pos);

            return new BattleSummonVO(_isMine, _uid, _pos);
        }

        private void AddUnit(bool _isMine, IUnitSDS _sds, int _pos)
        {
            int time = tick * BattleConst.TICK_TIME;

            Turrent[] turrentPos = _isMine ? mTurrent : oTurrent;

            Unit unit = new Unit();

            unit.Init(this, _isMine, _sds);

            for (int i = 0; i < _sds.GetPos().Length; i++)
            {
                int pos = _sds.GetPos()[i];

                ITurrentSDS sds = _sds.GetTurrent()[i];

                Turrent turrent = new Turrent();

                turrent.Init(this, unit, sds, _pos + pos, time);

                turrentPos[_pos + pos] = turrent;
            }
        }

        public int GetCard(bool _isMine, int _uid)
        {
            if (_isMine)
            {
                return mCardsArr[_uid];
            }
            else
            {
                return oCardsArr[_uid];
            }
        }

        protected void SetCard(bool _isMine, int _uid, int _id)
        {
            if (_isMine)
            {
                mCardsArr[_uid] = _id;
            }
            else
            {
                oCardsArr[_uid] = _id;
            }
        }

        internal IEnumerator Update()
        {
            tick++;

            yield return UpdateAction();

            yield return UpdateRecoverMoney();

            yield return UpdateTurrent();
        }

        private IEnumerator UpdateTurrent()
        {
            int time = tick * BattleConst.TICK_TIME;

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

            int lastProcessTime = -1;

            BattleResult result = BattleResult.NOT_OVER;

            while (list.Count > 0)
            {
                list.Sort(SortTurrentList);

                Turrent turrent = list[0];

                if (time >= turrent.time)
                {
                    if (lastProcessTime == -1)
                    {
                        lastProcessTime = turrent.time;

                        BattleAttackVO vo;

                        bool b = turrent.Update(out vo);

                        if (b)
                        {
                            yield return vo;
                        }
                        else
                        {
                            list.RemoveAt(0);
                        }
                    }
                    else if (turrent.time > lastProcessTime)
                    {
                        yield return RemoveDieUnit(turrent.time, list);

                        result = GetBattleResult(turrent.time);

                        if (result != BattleResult.NOT_OVER)
                        {
                            break;
                        }

                        lastProcessTime = -1;
                    }
                    else
                    {
                        BattleAttackVO vo;

                        bool b = turrent.Update(out vo);

                        if (b)
                        {
                            yield return vo;
                        }
                        else
                        {
                            list.RemoveAt(0);
                        }
                    }
                }
                else
                {
                    break;
                }
            }

            if (result == BattleResult.NOT_OVER)
            {
                yield return RemoveDieUnit(time, null);

                result = GetBattleResult(time);
            }

            yield return new BattleResultVO(result);
        }

        private IEnumerator RemoveDieUnit(int _time, List<Turrent> _list)
        {
            for (int i = 0; i < mTurrent.Length; i++)
            {
                Turrent t = mTurrent[i];

                if (t != null)
                {
                    if (t.parent.hp < 1 || (t.dieTime > 0 && _time >= t.dieTime))
                    {
                        if (_list != null)
                        {
                            _list.Remove(t);
                        }

                        mTurrent[i] = null;

                        yield return new BattleDeadVO(true, i);
                    }
                }
            }

            for (int i = 0; i < oTurrent.Length; i++)
            {
                Turrent t = oTurrent[i];

                if (t != null)
                {
                    if (t.parent.hp < 1 || (t.dieTime > 0 && _time >= t.dieTime))
                    {
                        if (_list != null)
                        {
                            _list.Remove(t);
                        }

                        oTurrent[i] = null;

                        yield return new BattleDeadVO(true, i);
                    }
                }
            }
        }

        private BattleResult GetBattleResult(int _time)
        {
            if (mBase <= 0 && oBase <= 0)
            {
                return BattleResult.DRAW;
            }
            else if (mBase <= 0)
            {
                return BattleResult.O_WIN;
            }
            else if (oBase <= 0)
            {
                return BattleResult.M_WIN;
            }
            else
            {
                if (_time >= maxTime)
                {
                    return BattleResult.DRAW;
                }
                else
                {
                    return BattleResult.NOT_OVER;
                }
            }
        }

        private IEnumerator UpdateAction()
        {
            if (actionList.Count > 0)
            {
                for (int i = 0; i < actionList.Count; i++)
                {
                    Tuple<bool, int, int> t = actionList[i];

                    int result = CheckAddSummon(t.first, t.second, t.third);

                    if (result < 0)
                    {
                        yield return AddSummon(t.first, t.second, t.third);
                    }
                    else
                    {
                        throw new Exception("summon error   isMine:" + t.first + "  uid:" + t.second + "  pos:" + t.third + "  result:" + result);
                    }
                }

                actionList.Clear();
            }
        }

        private IEnumerator UpdateRecoverMoney()
        {
            int time = tick * BattleConst.TICK_TIME;

            while (mMoney < BattleConst.MAX_MONEY && time >= mMoneyTime)
            {
                mMoney++;

                mMoneyTime += BattleConst.RECOVER_MONEY_TIME;

                yield return new BattleRecoverMoneyVO(true);
            }

            while (oMoney < BattleConst.MAX_MONEY && time >= oMoneyTime)
            {
                oMoney++;

                oMoneyTime += BattleConst.RECOVER_MONEY_TIME;

                yield return new BattleRecoverMoneyVO(false);
            }
        }

        internal int BaseBeDamage(Turrent _turrent)
        {
            if (_turrent.parent.isMine)
            {
                oBase -= _turrent.sds.GetAttackDamage();
            }
            else
            {
                mBase -= _turrent.sds.GetAttackDamage();
            }

            return -_turrent.sds.GetAttackDamage();
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

        internal void AddAction(bool _isMine, int _uid, int _pos)
        {
            actionList.Add(new Tuple<bool, int, int>(_isMine, _uid, _pos));
        }

        protected void Reset()
        {
            mHandCards.Clear();

            oHandCards.Clear();

            mCards.Clear();

            oCards.Clear();

            actionList.Clear();

            for (int i = 0; i < BattleConst.DECK_CARD_NUM; i++)
            {
                mCardsArr[i] = 0;

                oCardsArr[i] = 0;
            }

            for (int i = 0; i < mTurrent.Length; i++)
            {
                mTurrent[i] = null;

                oTurrent[i] = null;
            }

            tick = 0;

            mMoneyTime = 0;

            oMoneyTime = 0;
        }

        public string GetData()
        {
            string str = string.Empty;

            str += tick + "{";

            for (int i = 0; i < mTurrent.Length; i++)
            {
                Turrent t = mTurrent[i];

                if (t != null)
                {
                    str += i + ":";

                    str += t.GetData();

                    str += "|";
                }
            }

            for (int i = 0; i < oTurrent.Length; i++)
            {
                Turrent t = oTurrent[i];

                if (t != null)
                {
                    str += i + ":";

                    str += t.GetData();

                    str += "|";
                }
            }

            str += "}";

            return str;
        }
    }
}
