namespace Turrent_lib
{
    public class Unit
    {
        public bool isMine;

        public IUnitSDS sds;

        public int hp { private set; get; }

        private BattleCore battleCore;

        public void Init(BattleCore _battleCore, bool _isMine, IUnitSDS _sds)
        {
            battleCore = _battleCore;

            isMine = _isMine;

            sds = _sds;

            hp = sds.GetHp();
        }

        internal int BeDamaged(Turrent _turrent)
        {
            hp -= _turrent.sds.GetAttackDamage();

            return -_turrent.sds.GetAttackDamage();
        }
    }
}
