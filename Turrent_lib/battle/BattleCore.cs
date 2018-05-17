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

        private Dictionary<int, int> actionDic = new Dictionary<int, int>();

        internal int AddSummon(bool _isMine, int _uid, int _pos)
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

                if (sds.GetCost() > money)
                {
                    return 2;
                }

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

                if (_isMine)
                {
                    mMoney -= sds.GetCost();
                }
                else
                {
                    oMoney -= sds.GetCost();
                }

                handCards.RemoveAt(handCardsIndex);

                AddUnit(_isMine, sds, _pos);

                return -1;
            }
            else
            {
                return 7;
            }
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

                turrent.Init(this, unit, sds, pos);

                turrentPos[pos] = turrent;
            }
        }

        private int GetCard(int _uid)
        {
            return cardsArr[_uid];
        }
    }
}
