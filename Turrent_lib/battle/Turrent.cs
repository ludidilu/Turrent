using System.Collections.Generic;
using System;

namespace Turrent_lib
{
    public class Turrent
    {
        public Unit parent;

        public ITurrentSDS sds;

        public int pos { private set; get; }

        internal int time { private set; get; }

        private BattleCore battleCore;

        private Dictionary<KeyValuePair<int, int>, int> lastTargetDic;

        internal void Init(BattleCore _battleCore, Unit _parent, ITurrentSDS _sds, int _pos, int _time)
        {
            battleCore = _battleCore;

            parent = _parent;

            sds = _sds;

            pos = _pos;

            time = _time + _parent.sds.GetCd();

            if (sds.GetAttackDamageAdd() > 0)
            {
                lastTargetDic = new Dictionary<KeyValuePair<int, int>, int>();
            }
        }

        internal bool Update(out BattleAttackVO _vo)
        {
            if (parent.state == UnitState.CD)
            {
                parent.Ready(time);
            }

            bool b = DoAction(out _vo);

            if (b)
            {
                time += sds.GetAttackGap();
            }

            return b;
        }

        private bool DoAction(out BattleAttackVO _vo)
        {
            Dictionary<KeyValuePair<int, int>, int> recDic = null;

            if (sds.GetAttackDamageAdd() > 0)
            {
                recDic = lastTargetDic;

                lastTargetDic = new Dictionary<KeyValuePair<int, int>, int>();
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
                            if (!lastTargetDic.ContainsKey(new KeyValuePair<int, int>(-1, -1)))
                            {
                                int lastTargetTime;

                                if (recDic.TryGetValue(new KeyValuePair<int, int>(-1, -1), out lastTargetTime))
                                {
                                    lastTargetDic.Add(new KeyValuePair<int, int>(-1, -1), lastTargetTime);
                                }
                                else
                                {
                                    lastTargetDic.Add(new KeyValuePair<int, int>(-1, -1), time);
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
                            if (!lastTargetDic.ContainsKey(new KeyValuePair<int, int>(targetTurrent.parent.uid, targetTurrent.pos)))
                            {
                                int lastTargetTime;

                                if (recDic.TryGetValue(new KeyValuePair<int, int>(targetTurrent.parent.uid, targetTurrent.pos), out lastTargetTime))
                                {
                                    lastTargetDic.Add(new KeyValuePair<int, int>(targetTurrent.parent.uid, targetTurrent.pos), lastTargetTime);
                                }
                                else
                                {
                                    lastTargetDic.Add(new KeyValuePair<int, int>(targetTurrent.parent.uid, targetTurrent.pos), time);
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

            damage += GetDamageAdd(-1, -1);

            if (sds.GetAttackDamageType() == DamageType.PHYSIC)
            {
                battleCore.eventListener.DispatchEvent<int, Unit, Unit>(BattleConst.FIX_DO_PHYSIC_DAMAGE, ref damage, parent, null);

                battleCore.eventListener.DispatchEvent<Unit, Unit>(BattleConst.DO_PHYSIC_DAMAGE_BASE, parent, null);
            }
            else if (sds.GetAttackDamageType() == DamageType.MAGIC)
            {
                battleCore.eventListener.DispatchEvent<int, Unit, Unit>(BattleConst.FIX_DO_MAGIC_DAMAGE, ref damage, parent, null);

                battleCore.eventListener.DispatchEvent<Unit, Unit>(BattleConst.DO_MAGIC_DAMAGE_BASE, parent, null);
            }

            if (damage < 1)
            {
                damage = 1;
            }

            return battleCore.BaseBeDamage(parent, damage);
        }

        private int DamageTurrent(Turrent _turrent)
        {
            int damage = sds.GetAttackDamage();

            damage += GetDamageAdd(_turrent.parent.uid, _turrent.pos);

            switch (sds.GetAttackDamageType())
            {
                case DamageType.PHYSIC:

                    battleCore.eventListener.DispatchEvent(BattleConst.FIX_DO_PHYSIC_DAMAGE, ref damage, parent, _turrent.parent);

                    battleCore.eventListener.DispatchEvent(BattleConst.DO_PHYSIC_DAMAGE, parent, _turrent.parent);

                    return _turrent.parent.BePhysicDamaged(parent, damage, time);

                case DamageType.MAGIC:

                    battleCore.eventListener.DispatchEvent(BattleConst.FIX_DO_MAGIC_DAMAGE, ref damage, parent, _turrent.parent);

                    battleCore.eventListener.DispatchEvent(BattleConst.DO_MAGIC_DAMAGE, parent, _turrent.parent);

                    return _turrent.parent.BeMagicDamaged(parent, damage, time);

                default:

                    throw new Exception("unknown attackDamageType:" + sds.GetAttackDamageType());
            }

        }

        private int GetDamageAdd(int _uid, int _pos)
        {
            if (sds.GetAttackDamageAdd() > 0)
            {
                int damageAdd = (time - lastTargetDic[new KeyValuePair<int, int>(_uid, _pos)]) / sds.GetAttackDamageAddGap() * sds.GetAttackDamageAdd();

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

        public string GetData()
        {
            string str = string.Empty;

            str += parent.GetData();

            str += "pos:" + pos + ";";

            str += "time:" + time + ";";

            return str;
        }
    }
}
