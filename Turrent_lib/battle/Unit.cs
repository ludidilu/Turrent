using System.Collections.Generic;

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

        public int dieTime { private set; get; }

        internal List<Turrent> turrentList = new List<Turrent>();

        public void Init(BattleCore _battleCore, bool _isMine, IUnitSDS _sds, int _uid, int _pos, int _time)
        {
            battleCore = _battleCore;

            isMine = _isMine;

            sds = _sds;

            uid = _uid;

            pos = _pos;

            hp = sds.GetHp();

            if (sds.GetLiveTime() > 0)
            {
                dieTime = _time + sds.GetLiveTime();
            }

            Turrent[] turrentPos = isMine ? battleCore.mTurrent : battleCore.oTurrent;

            for (int i = 0; i < sds.GetPos().Length; i++)
            {
                int posFix = sds.GetPos()[i];

                ITurrentSDS turrentSDS = sds.GetTurrent()[i];

                Turrent turrent = new Turrent();

                turrent.Init(battleCore, this, turrentSDS, pos + posFix, _time);

                turrentPos[pos + posFix] = turrent;

                turrentList.Add(turrent);
            }

            for (int i = 0; i < sds.GetAuras().Length; i++)
            {
                Aura.Init(battleCore, this, sds.GetAuras()[i], Aura.AuraRegisterType.AURA, _time);
            }
        }

        internal int BePhysicDamaged(Unit _unit, int _damage, int _time)
        {
            battleCore.eventListener.DispatchEvent(BattleConst.FIX_BE_PHYSIC_DAMAGE, ref _damage, this, _unit);

            if (_damage < 1)
            {
                _damage = 1;
            }

            hp -= _damage;

            battleCore.eventListener.DispatchEvent(BattleConst.BE_PHYSIC_DAMAGE, this, _unit, _time);

            return -_damage;
        }

        internal int BeMagicDamaged(Unit _unit, int _damage, int _time)
        {
            battleCore.eventListener.DispatchEvent(BattleConst.FIX_BE_MAGIC_DAMAGE, ref _damage, this, _unit);

            if (_damage < 1)
            {
                _damage = 1;
            }

            hp -= _damage;

            battleCore.eventListener.DispatchEvent(BattleConst.BE_MAGIC_DAMAGE, this, _unit, _time);

            return -_damage;
        }

        internal void Die(int _time)
        {
            battleCore.eventListener.DispatchEvent<Unit, Unit, int>(BattleConst.DIE, this, null, _time);
        }

        internal string GetData()
        {
            string str = string.Empty;

            str += isMine + ";";

            str += hp + ";";

            str += dieTime + ";";

            return str;
        }
    }
}
