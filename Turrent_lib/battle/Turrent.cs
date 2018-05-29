using System.Collections.Generic;

namespace Turrent_lib
{
    enum TurrentState
    {
        CD,
        FREE,
    }

    public class Turrent
    {
        public Unit parent;

        public ITurrentSDS sds;

        public int pos { private set; get; }

        private TurrentState state;

        internal int time { private set; get; }

        private BattleCore battleCore;

        public int bornTime { private set; get; }

        internal void Init(BattleCore _battleCore, Unit _parent, ITurrentSDS _sds, int _pos, int _time)
        {
            battleCore = _battleCore;

            parent = _parent;

            sds = _sds;

            pos = _pos;

            state = TurrentState.CD;

            bornTime = _time;

            time = bornTime + sds.GetCd();
        }

        internal bool Update(out BattleAttackVO _vo)
        {
            if (state == TurrentState.CD)
            {
                state = TurrentState.FREE;
            }

            bool b = Attack(out _vo);

            if (b)
            {
                time += sds.GetAttackGap();
            }

            return b;
        }

        private bool Attack(out BattleAttackVO _vo)
        {
            List<int> result = BattlePublicTools.GetTurrentAttackTargetList(battleCore, parent.isMine, sds, pos);

            if (result != null)
            {
                Turrent[] oppTurrent = parent.isMine ? battleCore.oTurrent : battleCore.mTurrent;

                List<KeyValuePair<int, int>> damageDataList = new List<KeyValuePair<int, int>>();

                for (int i = 0; i < result.Count; i++)
                {
                    int targetPos = result[i];

                    if (targetPos < 0)
                    {
                        int damage = battleCore.BaseBeDamage(this);

                        damageDataList.Add(new KeyValuePair<int, int>(-1, damage));
                    }
                    else
                    {
                        Turrent targetTurrent = oppTurrent[targetPos];

                        int damage = targetTurrent.BeDamage(this);

                        damageDataList.Add(new KeyValuePair<int, int>(targetPos, damage));
                    }
                }

                _vo = new BattleAttackVO(parent.isMine, pos, damageDataList);

                return true;
            }
            else
            {
                _vo = new BattleAttackVO();

                return false;
            }
        }

        private int BeDamage(Turrent _turrent)
        {
            return parent.BeDamaged(_turrent);
        }

        public string GetData()
        {
            string str = string.Empty;

            str += parent.GetData();

            str += "pos:" + pos + ";";

            str += "time:" + time + ";";

            str += "bornTime:" + bornTime + ";";

            return str;
        }
    }
}
