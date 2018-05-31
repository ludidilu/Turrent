namespace Turrent_lib
{
    public class Unit
    {
        public bool isMine;

        public IUnitSDS sds;

        public int hp { private set; get; }

        private BattleCore battleCore;

        public int uid { private set; get; }

        public int pos { private set; get; }

        public void Init(BattleCore _battleCore, bool _isMine, IUnitSDS _sds, int _uid, int _pos, int _time)
        {
            battleCore = _battleCore;

            isMine = _isMine;

            sds = _sds;

            uid = _uid;

            pos = _pos;

            hp = sds.GetHp();

            Turrent[] turrentPos = isMine ? battleCore.mTurrent : battleCore.oTurrent;

            for (int i = 0; i < sds.GetPos().Length; i++)
            {
                int posFix = sds.GetPos()[i];

                ITurrentSDS turrentSDS = sds.GetTurrent()[i];

                Turrent turrent = new Turrent();

                turrent.Init(battleCore, this, turrentSDS, pos + posFix, _time);

                turrentPos[pos + posFix] = turrent;
            }
        }

        internal int BeDamaged(Turrent _turrent, int _damage)
        {
            hp -= _damage;

            return -_damage;
        }

        internal string GetData()
        {
            string str = string.Empty;

            str += isMine + ";";

            str += hp + ";";

            return str;
        }
    }
}
