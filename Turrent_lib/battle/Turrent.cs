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

        internal void Update()
        {
            if (state == TurrentState.CD)
            {
                state = TurrentState.FREE;
            }

            Attack();

            time += sds.GetAttackGap();
        }

        private void Attack()
        {
            int x = pos % BattleConst.MAP_WIDTH;

            int oppX = BattleConst.MAP_WIDTH - x;

            Turrent[] oppTurrent = parent.isMine ? battleCore.oTurrent : battleCore.mTurrent;

            for (int i = 0; i < sds.GetAttackTargetPos().Length; i++)
            {
                int targetPos = oppX + sds.GetAttackTargetPos()[i];

                if (targetPos < 0)
                {
                    battleCore.BaseBeDamage(this);
                }
                else
                {
                    if (oppTurrent[targetPos] != null)
                    {
                        int[] arr = sds.GetAttackDamagePos()[i];

                        for (int m = 0; m < arr.Length; m++)
                        {
                            int damagePos = oppX + arr[m];

                            Turrent damageTurrent = oppTurrent[damagePos];

                            if (damageTurrent != null)
                            {
                                damageTurrent.parent.BeDamage(this);
                            }
                        }

                        break;
                    }
                }
            }
        }
    }
}
