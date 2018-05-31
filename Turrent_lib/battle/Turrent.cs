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

        internal int dieTime { private set; get; }

        private BattleCore battleCore;

        private Dictionary<int, int> lastTargetDic;

        internal void Init(BattleCore _battleCore, Unit _parent, ITurrentSDS _sds, int _pos, int _time)
        {
            battleCore = _battleCore;

            parent = _parent;

            sds = _sds;

            pos = _pos;

            state = TurrentState.CD;

            time = _time + sds.GetCd();

            if (sds.GetLiveTime() > 0)
            {
                dieTime = _time + sds.GetLiveTime();
            }

            if (sds.GetAttackDamageAdd() > 0)
            {
                lastTargetDic = new Dictionary<int, int>();
            }
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
            Dictionary<int, int> recDic = null;

            if (sds.GetAttackDamageAdd() > 0)
            {
                recDic = lastTargetDic;

                lastTargetDic = new Dictionary<int, int>();
            }

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
                        if (sds.GetAttackDamageAdd() > 0)
                        {
                            if (!lastTargetDic.ContainsKey(-1))
                            {
                                int lastTargetTime;

                                if (recDic.TryGetValue(-1, out lastTargetTime))
                                {
                                    lastTargetDic.Add(-1, lastTargetTime);
                                }
                                else
                                {
                                    lastTargetDic.Add(-1, time);
                                }
                            }
                        }

                        int damage = DamageBase();

                        damageDataList.Add(new KeyValuePair<int, int>(-1, damage));
                    }
                    else
                    {
                        Turrent targetTurrent = oppTurrent[targetPos];

                        if (sds.GetAttackDamageAdd() > 0)
                        {
                            if (!lastTargetDic.ContainsKey(targetTurrent.parent.uid))
                            {
                                int lastTargetTime;

                                if (recDic.TryGetValue(targetTurrent.parent.uid, out lastTargetTime))
                                {
                                    lastTargetDic.Add(targetTurrent.parent.uid, lastTargetTime);
                                }
                                else
                                {
                                    lastTargetDic.Add(targetTurrent.parent.uid, time);
                                }
                            }
                        }

                        int damage = DamageTurrent(targetTurrent);

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

        private int DamageBase()
        {
            int damage = sds.GetAttackDamage();

            damage += GetDamageAdd(-1);

            return battleCore.BaseBeDamage(this, damage);
        }

        private int DamageTurrent(Turrent _turrent)
        {
            int damage = sds.GetAttackDamage();

            damage += GetDamageAdd(_turrent.parent.uid);

            return _turrent.BeDamage(this, damage);
        }

        private int GetDamageAdd(int _uid)
        {
            if (sds.GetAttackDamageAdd() > 0)
            {
                int damageAdd = (time - lastTargetDic[_uid]) / sds.GetAttackDamageAddGap() * sds.GetAttackDamageAdd();

                if (damageAdd > sds.GetAttackDamageAddMax())
                {
                    damageAdd = sds.GetAttackDamageAddMax();
                }

                return damageAdd;
            }
            else
            {
                return 0;
            }
        }

        private int BeDamage(Turrent _turrent, int _damage)
        {
            return parent.BeDamaged(_turrent, _damage);
        }

        public string GetData()
        {
            string str = string.Empty;

            str += parent.GetData();

            str += "pos:" + pos + ";";

            str += "time:" + time + ";";

            str += "dieTime:" + dieTime + ";";

            return str;
        }
    }
}
