using System.Collections.Generic;
using System.Collections;

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

        internal void Init(BattleCore _battleCore, Unit _parent, ITurrentSDS _sds, int _pos, int _time)
        {
            battleCore = _battleCore;

            parent = _parent;

            sds = _sds;

            pos = _pos;

            state = TurrentState.CD;

            time = _time + sds.GetCd();
        }

        internal IEnumerator Update()
        {
            if (state == TurrentState.CD)
            {
                state = TurrentState.FREE;

                if (parent.sds.GetIsSkill())
                {
                    parent.Die();
                }
            }

            yield return Attack();

            time += sds.GetAttackGap();
        }

        private BattleAttackVO Attack()
        {
            int x = pos % BattleConst.MAP_WIDTH;

            int oppX = BattleConst.MAP_WIDTH - 1 - x;

            Turrent[] oppTurrent = parent.isMine ? battleCore.oTurrent : battleCore.mTurrent;

            for (int i = 0; i < sds.GetAttackTargetPos().Length; i++)
            {
                KeyValuePair<int, int> targetPosFix = sds.GetAttackTargetPos()[i];

                int targetX = oppX + targetPosFix.Key;

                if (targetX >= BattleConst.MAP_WIDTH || targetX < 0)
                {
                    continue;
                }

                int targetY = targetPosFix.Value;

                int targetPos = targetY * BattleConst.MAP_WIDTH + targetX;

                if (oppTurrent[targetPos] != null)
                {
                    KeyValuePair<int, int>[] arr = sds.GetAttackDamagePos()[i];

                    List<KeyValuePair<int, int>> damageDataList = new List<KeyValuePair<int, int>>();

                    for (int m = 0; m < arr.Length; m++)
                    {
                        KeyValuePair<int, int> damagePosFix = arr[m];

                        int damageX = oppX + damagePosFix.Key;

                        if (damageX >= BattleConst.MAP_WIDTH || damageX < 0)
                        {
                            continue;
                        }

                        int damageY = damagePosFix.Value;

                        int damagePos = damageY * BattleConst.MAP_WIDTH + damageX;

                        Turrent damageTurrent = oppTurrent[damagePos];

                        if (damageTurrent != null)
                        {
                            int damage = damageTurrent.parent.BeDamage(this);

                            damageDataList.Add(new KeyValuePair<int, int>(damagePos, damage));
                        }
                    }

                    return new BattleAttackVO(parent.isMine, pos, damageDataList);
                }
            }

            int baseDamage = battleCore.BaseBeDamage(this);

            return new BattleAttackVO(parent.isMine, pos, new List<KeyValuePair<int, int>>() { new KeyValuePair<int, int>(-1, baseDamage) });
        }
    }
}
